using BanglaLib.Lib.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using BanglaLib.Config;

namespace BanglaLib.Spider
{
    public class Crawler
    {
        static Crawler()
        {
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;
        }

        public delegate void WriteLogger(object o, ExecutionStatusArgs args);

        public event WriteLogger OnWrite;
        public event WriteLogger OnNewLine;
        public event WriteLogger OnNewWord;
        public event WriteLogger OnNewLink;

        public event WriteLogger OnJobQueue;
        public event WriteLogger OnJobDequeue;

        public event WriteLogger OnStart;
        public event WriteLogger OnStop;


        Timer urlFetchTimer;
        Timer dbSaveTimer;
        bool living = true;
        int workInProgress = 0;

        object dblock = new object();

        BlockingCollection<Task> workingTask = new BlockingCollection<Task>();
        WordBreaker breaker;
        LineStore store;
        public int WorkInProgress { get => workInProgress; set => workInProgress = value; }
        public WordBreaker Breaker { get => breaker; set => breaker = value; }
        public BlockingCollection<Task> WorkingTask { get => workingTask; set => workingTask = value; }

        StateModel state;
        string fileTemplate = "_temp_db.jdxn";
        string modelFileName;
        public Crawler(string baseFolder)
        {
            modelFileName = Path.Combine(baseFolder, fileTemplate);
            try
            {
                state = DocumentExtn.Read<StateModel>(modelFileName);
            }
            catch {
                state = new StateModel();
            }

            state.RootFolder = baseFolder;
            state.Save(modelFileName);

            breaker = new WordBreaker(state.RootFolder);
            breaker.InitializeFolder();
            OnWrite += Crawler_OnWrite;
            store = new LineStore(state.RootFolder);
            store.InitializeFolder();
        }

        #region Extended Contructor

        private void Crawler_OnWrite(object o, ExecutionStatusArgs line)
        {
            WriteLine(line.Line);
        }
        private void WriteLine(string line)
        {
            Debug.WriteLine(line);
        }
        public void WriteMessage(FetchModel model, string message, ExecutionSatus status = ExecutionSatus.Message)
        {
            OnWrite(model, new ExecutionStatusArgs(message) { Model = model, Status = status });
        }

        #endregion

        #region Repository Section

        private FetchModel AddUrlToDB(FetchModel model)
        {
            if (!state.PageDatabase.ContainsKey(model.Key))
            {
                state.PageDatabase[model.Key] = model;
                OnWrite(model, new ExecutionStatusArgs()
                {
                    Line = "New Url Added->" + model.Key,
                    Model = model,
                    Status = ExecutionSatus.NewLink
                });
                if (OnNewLink != null)
                {
                    OnNewLink(model, new ExecutionStatusArgs()
                    {
                        Line = "New Url Added->" + model.Key,
                        Model = model,
                        Status = ExecutionSatus.NewLink
                    });
                }
                return model;
            }
            return null;
        }
        private FetchModel AddNewLinkToDB(string url)
        {
            FetchModel model = new FetchModel() { Url = url };
            return AddUrlToDB(model);
        }
        private FetchModel UpdateLinkInDB(FetchModel model)
        {
            lock (dblock)
            {
                if (state.PageDatabase.ContainsKey(model.Key))
                {
                    state.PageDatabase[model.Key] = model;
                }
            }
            return model;
        }
        private FetchModel UpdateDownlaoded(FetchModel model)
        {
            // model.Downloaded = DateTime.Now;
            model.Status = ExecutionSatus.DownloadedLink;
            UpdateLinkInDB(model);
            return model;
        }
        private FetchModel UpdateCompleted(FetchModel model)
        {
            model.Done = DateTime.Now;
            model.Status = ExecutionSatus.LinkDone;
            UpdateLinkInDB(model);
            return model;
        }
        private FetchModel UpdateQueued(FetchModel model)
        {
            model.Added = DateTime.Now;
            model.Status = ExecutionSatus.NewJob;
            UpdateLinkInDB(model);
            return model;
        }
        private FetchModel GetLinkFromDB(FetchModel model)
        {
            if (state.PageDatabase.ContainsKey(model.Key))
            {
                return state.PageDatabase[model.Key];
            }
            return null;
        }

        private bool IsQueueFullOrEmpty()
        {
            return state.InProgressModels.IsCompleted;
        }
        private FetchModel GetAnItemFromQueue()
        {
            var model = state.InProgressModels.Take();
            if(OnJobDequeue!=null)
            {
                OnJobDequeue(model, new ExecutionStatusArgs("Transfering to handler")
                {
                    Model = model,
                    Status = ExecutionSatus.RemovedJob
                });
            }
            return model;
        }
        private FetchModel GetNextLinkFromDB()
        {
            lock (dblock)
            {
                var values = state.PageDatabase.Values;
                //Get next new item
                FetchModel model = values.Where(x => x.Status < ExecutionSatus.NewJob).FirstOrDefault();
                return model;
            }
        }
        private bool AddJobToPending(FetchModel model)
        {
            var copy = model.Clone();
            //Dont queue the same item...
            bool added = state.InProgressModels.TryAdd(copy);
            if (added)
            {
                if (OnJobQueue != null)
                {
                    OnJobQueue(copy, new ExecutionStatusArgs("Adding new Job: " + model.Url)
                    {
                        Status = ExecutionSatus.NewJob,
                        Model = model
                    });
                }
            }
            return added;
        }

        private string AddWord(FetchModel model, string word)
        {
            state.Words[word] = "d";
            if (OnNewWord != null)
            {
                OnNewWord(model, new ExecutionStatusArgs("New Word Added")
                {
                    Line = word,
                    Model = model,
                    Status = ExecutionSatus.NewWord
                }); ;
            }
            return word;
        }
        private string AddLine(FetchModel model, string line)
        {
            if (OnNewLine != null)
            {
                OnNewLine(model, new ExecutionStatusArgs("New Line Added")
                {
                    Line = line,
                    Model = model,
                    Status = ExecutionSatus.NewLine
                }); ;
            }
            return line;
        }

        #endregion 

        public void Start(string url)
        {
            AddNewLinkToDB(url);

            urlFetchTimer = new Timer(1000);
            urlFetchTimer.Elapsed += Timer_Elapsed;
            dbSaveTimer = new Timer(10000);
            dbSaveTimer.Elapsed += DbSaveTimer_Elapsed;
            urlFetchTimer.Start();
            dbSaveTimer.Start();
            OnWrite(this, new ExecutionStatusArgs("Started the job") { Status = ExecutionSatus.StartedApp });
            if (OnStart != null)
            {
                OnStart(this, new ExecutionStatusArgs("Started the job") { Status = ExecutionSatus.StartedApp });
            }
        }
        public void Stop()
        {
            living = false;
            try
            {
                //urlFetchTimer.Stop();
                urlFetchTimer = null;
                //dbSaveTimer.Stop();
                dbSaveTimer = null;
            }
            catch
            {
            }

            OnWrite(this, new ExecutionStatusArgs("Stopped the job") { Status = ExecutionSatus.StoppedApp });
        }

        private void DbSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WorkingTask.Add(Task.Run(() =>
            {
                try
                {
                    state.Save(modelFileName);
                    OnWrite(this, new ExecutionStatusArgs("DB File saved ") { Status = ExecutionSatus.FileSaved });
                }
                catch (Exception ex)
                {
                    OnWrite(this, new ExecutionStatusArgs("Started the job") { Status = ExecutionSatus.Error });
                }
            }));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WorkInProgress++;
            if (living)
            {
                OnWrite(this, new ExecutionStatusArgs("Running Producer/Consumer"));
                //Produce
                ProduceMore();
                //Consume
                ConsumeOne();
            }
        }

        #region Action Begins

        private void ConsumeOne()
        {
            WorkingTask.Add(Task.Run(() =>
            {
                //It will not enter into if queue is empty
                if (!IsQueueFullOrEmpty())
                {
                    var model = GetAnItemFromQueue();
                    WriteMessage(model, "Got a new link from queue: " + model.Key);                 
                    try
                    {
                        //Do not operate on same object...
                        var dbCopy = GetLinkFromDB(model);
                        if (OnJobQueue != null)
                        {
                            OnJobQueue(dbCopy, new ExecutionStatusArgs("Job Found - Working: " + dbCopy.Url)
                            {
                                Status = ExecutionSatus.StartedJob,
                                Model = dbCopy
                            });
                        }
                        Download(dbCopy);
                    }
                    catch (Exception mxc)
                    {
                        OnWrite(this, new ExecutionStatusArgs("Bad URL or could not work. Error: " + mxc.Message) { Status = ExecutionSatus.Error, Model = model });
                    }
                }
            }));
        }
        private void ProduceMore()
        {
            WorkingTask.Add(Task.Run(() =>            {

                FetchModel model = GetNextLinkFromDB();
               
                if (model != null)
                {
                    WriteMessage(model, "Found next link for new job queue:  " + model.Key);
                    bool added = AddJobToPending(model);
                    if (added)
                    {
                        WriteMessage(model, "Added next link for new job queue:  " + model.Key);
                        UpdateQueued(model);
                    }
                }

            }));
        }

        #endregion

        #region Regular Task
        private FetchModel Download(FetchModel model)
        {
            var downloadTask = new List<Task>();
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                model.Html = client.DownloadString(model.Url);
                UpdateDownlaoded(model);
                WriteMessage(model, "Downloaded : " + model.Url, ExecutionSatus.DownloadedLink);
   
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(model.Html);
                downloadTask.Add(Task.Run(() =>
                {
                    try
                    {
                        string root = model.Domain;
                        OnWrite(this, new ExecutionStatusArgs("Working on link"));

                        List<FetchModel> linkModels = new List<FetchModel>();
                        foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            try
                            {

                                // Get the value of the HREF attribute
                                string hrefValue = link.GetAttributeValue("href", string.Empty);
                                hrefValue = GetAbsoluteUrlString(model.Url, hrefValue);
                                if (!string.IsNullOrEmpty(hrefValue))
                                {
                                    FetchModel md = new FetchModel() { Url = hrefValue };
                                    linkModels.Add(md);
                                }
                            }
                            catch (Exception lx)
                            {
                                OnWrite(this, new ExecutionStatusArgs("Error in getting links." + lx.Message) { Status = ExecutionSatus.Error });
                            }
                        }

                        WriteLine(string.Join(",", linkModels.Select(x => x.Url).ToArray()));

                        foreach (var m in linkModels)
                        {
                            try
                            {
                                if (m.Key.StartsWith(m.Domain))
                                {
                                    var copied = AddUrlToDB(m);
                                }
                                else
                                {
                                    WriteLine("Skipping..." + m.Url);
                                }
                            }
                            catch (Exception mdx)
                            {
                                OnWrite(this, new ExecutionStatusArgs("Error in adding links." + mdx.Message) { Status = ExecutionSatus.Error });
                            }
                        }
                    }
                    catch (Exception ecx)
                    {
                        OnWrite(this, new ExecutionStatusArgs("Error in parsing links." + ecx.Message) { Status = ExecutionSatus.Error });
                    }
                }));
                downloadTask.Add(Task.Run(() =>
                {
                    List<string> lines = new List<string>();
                    foreach (HtmlNode node in document.DocumentNode.SelectNodes("//text()"))
                    {
                        try
                        {
                            lines.Add(node.InnerText);
                        }
                        catch
                        {
                        }
                    }

                    //All lines
                    List<string> cleanWords = new List<string>();
                    foreach (var line in lines)
                    {
                        var ws = line.Split(" \n\t".ToCharArray())
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToList();
                        cleanWords.AddRange(ws);
                    }
                    //All words
                    List<string> words = cleanWords
                    .Where(x => Breaker.IsBangla(x))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                    var smodel = model.Clone();
                    smodel.Html = string.Join(" ", words);
                    this.AddLine(smodel, smodel.Html);
                    WriteMessage(smodel, smodel.Html, ExecutionSatus.NewLine);

                    foreach (var w in words)
                    {
                        this.AddWord(model,w);
                        WriteMessage(model,w, ExecutionSatus.NewWord);
                    }

                    WriteLine("Completed model:" + model.Url);
                }));
                Task.WaitAll(downloadTask.ToArray());
                UpdateCompleted(model);
            }
            catch (Exception ex)
            {
                WriteMessage(model, "Error in download links." + ex.Message, ExecutionSatus.Error);
                throw;
            }
            return model;
        }
        static string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(baseUrl), uri);
            return uri.ToString();
        }
        #endregion
    }
}
