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

        BlockingCollection<FetchModel> inProgressModels;
        ConcurrentDictionary<string, FetchModel> pageDatabase;
        ConcurrentDictionary<string, FetchModel> words;


        public delegate void WriteLogger(object o, StatusArgs args);
        public event WriteLogger OnWrite;

        Timer urlFetchTimer;
        Timer dbSaveTimer;
        bool living = true;
        int workInProgress = 0;
        int MAX_THREAD = 2;
        DateTime pstartTime = DateTime.Now;

        object dblock = new object();

        string repoFolder;
        Dictionary<char, LikeDictionary> likeDictionary;
        BlockingCollection<Task> workingTask = new BlockingCollection<Task>();
        WordBreaker breaker;

        public int WorkInProgress { get => workInProgress; set => workInProgress = value; }
        public WordBreaker Breaker { get => breaker; set => breaker = value; }
        public ConcurrentDictionary<string, FetchModel> Words { get => words; set => words = value; }
        public BlockingCollection<Task> WorkingTask { get => workingTask; set => workingTask = value; }


        public Crawler(string baseFolder)
        {
            words = new ConcurrentDictionary<string, FetchModel>();
            inProgressModels = new BlockingCollection<FetchModel>(MAX_THREAD);
            pageDatabase = new ConcurrentDictionary<string, FetchModel>();
            repoFolder = baseFolder;
            Breaker = new WordBreaker(repoFolder);
            breaker.InitializeFolder();
            OnWrite += Crawler_OnWrite;
        }

        private void Crawler_OnWrite(object o, StatusArgs line)
        {
            WriteLine(line.Line);
        }
        private void WriteLine(string line)
        {
            Debug.WriteLine(line);
        }

        private FetchModel AddUrlToDB(FetchModel model)
        {
            if (!pageDatabase.ContainsKey(model.Key))
            {
                pageDatabase[model.Key] = model;
                OnWrite(model, new StatusArgs()
                {
                    Line = "New Url Added->" + model.Key,
                    Model = model,
                    Status = ExecutionSatus.NewUrlAdded
                });
                return model;
            }
            return null;
        }
        private FetchModel AddUrlToDB(string url)
        {
            FetchModel model = new FetchModel() { Url = url };
            return AddUrlToDB(model);
        }
        private FetchModel UpdateModelInDB(FetchModel model)
        {
            lock (dblock)
            {
                if (pageDatabase.ContainsKey(model.Key))
                {
                    pageDatabase[model.Key] = model;
                }
            }
            return model;
        }
        private FetchModel UpdateDownlaoded(FetchModel model)
        {
            model.Downloaded = DateTime.Now;
            model.Status = 3;
            UpdateModelInDB(model);
            return model;
        }
        private FetchModel UpdateCompleted(FetchModel model)
        {
            model.Done = DateTime.Now;
            model.Status = 5;
            UpdateModelInDB(model);
            return model;
        }
        private FetchModel UpdateQueued(FetchModel model)
        {
            model.Added  = DateTime.Now;
            model.Status = 2;
            UpdateModelInDB(model);
            return model;
        }
        private FetchModel GetModelFromDB(FetchModel model)
        {
            if (pageDatabase.ContainsKey(model.Key))
            {
                return pageDatabase[model.Key];
            }
            return null;
        }

        public void Start(string url)
        {
            pstartTime = DateTime.Now;

            AddUrlToDB(url);

            urlFetchTimer = new Timer(1000);
            urlFetchTimer.Elapsed += Timer_Elapsed;
            dbSaveTimer = new Timer(10000);
            dbSaveTimer.Elapsed += DbSaveTimer_Elapsed;
            urlFetchTimer.Start();
            dbSaveTimer.Start();
            OnWrite(this, new StatusArgs("Started the job"));
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
                WorkingTask.ToList().ForEach(t =>
                {
                    // t.Dispose();
                });
            }
            catch
            {
            }
            OnWrite(this, new StatusArgs("Stopped the job"));
        }

        private void DbSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WorkingTask.Add(Task.Run(() =>
            {
                try
                {
                    string file = Path.Combine(repoFolder, "_temp_db.jdxn");
                    pageDatabase.Save(file);
                    OnWrite(this, new StatusArgs("DB File saved "));
                    WorkingTask.Where(x => x.IsCompleted).ToList().ForEach(x =>
                    {
                        // x.Dispose();
                    });
                }
                catch (Exception ex)
                {
                    OnWrite(this, new StatusArgs("Started the job") { Status = ExecutionSatus.Error });
                }
            }));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WorkInProgress++;
            if (living)
            {
                OnWrite(this, new StatusArgs("Running Producer/Consumer"));
                //Produce
                ProduceMore();
                //Consume
                ConsumeOne();
            }
        }

        private void ConsumeOne()
        {
            WorkingTask.Add(Task.Run(() =>
            {
                if (!inProgressModels.IsCompleted)
                {
                    var model = inProgressModels.Take();
                    OnWrite(this, new StatusArgs("Job Found - Working: " + model.Url)
                    {
                        Status = ExecutionSatus.JobFound,
                        Model = model
                    });
                    WorkOnUrl(model);
                }
            }));
        }
        private void ProduceMore()
        {
            WorkingTask.Add(Task.Run(() =>
            {

                lock (dblock)
                {
                    var values = pageDatabase.Values;
                    //Get next new item
                    FetchModel model = values.Where(x => x.Status<=0 ).FirstOrDefault();
                    if (model != null)
                    {
                        bool added = inProgressModels.TryAdd(model.Clone());
                        if (added)
                        {
                            UpdateCompleted(model);
                            OnWrite(this, new StatusArgs("Adding new Job: " + model.Url)
                            {
                                Status = ExecutionSatus.NewJobEnqued,
                                Model = model
                            });
                        }
                    }
                }

            }));
        }
        private void WorkOnUrl(FetchModel model)
        {
            try
            {
                var dbCopy = GetModelFromDB(model);
                Download(dbCopy);
            }
            catch (Exception mxc)
            {
                OnWrite(this, new StatusArgs("Bad URL or could not work. Error: " + mxc.Message) { Status = ExecutionSatus.Error, Model = model });
            }
        }
        private FetchModel Download(FetchModel model)
        {
            var downloadTask = new List<Task>();
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                model.Html = client.DownloadString(model.Url);
                UpdateDownlaoded(model);

                OnWrite(this, new StatusArgs("Downloaded : " + model.Url) { Status = ExecutionSatus.Downloaded });

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(model.Html);
                downloadTask.Add(Task.Run(() =>
                {
                    try
                    {
                        string root = model.Domain;
                        OnWrite(this, new StatusArgs("Working on link"));


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
                                OnWrite(this, new StatusArgs("Error in getting links." + lx.Message) { Status = ExecutionSatus.Error });
                            }
                        }

                        WriteLine(string.Join(",", linkModels.Select(x => x.Url).ToArray()));

                        foreach (var m in linkModels)
                        {
                            try
                            {
                                string key = m.Url.ToLower();
                                if (key.StartsWith(m.Domain))
                                {
                                    var copied = AddUrlToDB(m);
                                    if (copied != null)
                                    {
                                        pageDatabase[key] = m;
                                        OnWrite(this, new StatusArgs("New Link found." + key)
                                        {
                                            Status = ExecutionSatus.NewUrlAdded,
                                            Model = copied
                                        });
                                    }
                                }
                                else
                                {
                                    WriteLine("Skipping..." + m.Url);
                                }
                            }
                            catch (Exception mdx)
                            {
                                OnWrite(this, new StatusArgs("Error in adding links." + mdx.Message) { Status = ExecutionSatus.Error });
                            }
                        }
                        model.Parsed = DateTime.Now;
                        UpdateModelInDB(model);
                    }
                    catch (Exception ecx)
                    {
                        OnWrite(this, new StatusArgs("Error in parsing links." + ecx.Message) { Status = ExecutionSatus.Error });
                    }
                }));
                downloadTask.Add(Task.Run(() =>
                {
                    //HtmlDocument document = new HtmlDocument();
                    //document.LoadHtml(model.Html);

                    List<string> lines = new List<string>();
                    foreach (HtmlNode node in document.DocumentNode.SelectNodes("//text()"))
                    {
                        try
                        {
                            OnWrite(this, new StatusArgs("text=" + node.InnerText));
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

                    foreach (var w in words)
                    {
                        Words[w] = model;
                        try
                        {
                            Breaker.ProcessWord(w);
                            OnWrite(Breaker.NewWordDiscovery.Values, new StatusArgs(w)
                            {
                                Status = ExecutionSatus.NewWord,
                                Model = model
                            });
                        }
                        catch
                        {

                        }
                    }

                    WriteLine("Completed model:" + model.Url);
                    UpdateCompleted(model);
                }));
                Task.WaitAll(downloadTask.ToArray());

            }
            catch (Exception ex)
            {
                OnWrite(this, new StatusArgs("Error in download links." + ex.Message) { Status = ExecutionSatus.Error });
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
    }
}
