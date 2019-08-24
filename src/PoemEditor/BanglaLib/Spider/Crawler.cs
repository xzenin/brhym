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

        BlockingCollection<string> PENDING;
        ConcurrentDictionary<string, FetchModel> DB = new ConcurrentDictionary<string, FetchModel>();


        ConcurrentDictionary<string, FetchModel> words = new ConcurrentDictionary<string, FetchModel>();
        public delegate void WriteLogger(object o, StatusArgs args);

        public event WriteLogger OnWrite;

        Timer urlFetchTimer;
        Timer dbSaveTimer;
        bool living = true;
        int workInProgress = 0;
        int MAX_THREAD = 10;
        DateTime pstartTime = DateTime.Now;

        object dblock = new object();

        string repoFolder;
        Dictionary<char, LikeDictionary> likeDictionary;
        WordBreaker breaker;

        public int WorkInProgress { get => workInProgress; set => workInProgress = value; }
        public WordBreaker Breaker { get => breaker; set => breaker = value; }
        public ConcurrentDictionary<string, FetchModel> Words { get => words; set => words = value; }

        public Crawler(string baseFolder)
        {
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

        public void Start(string url)
        {
            pstartTime = DateTime.Now;
            PENDING = new BlockingCollection<string>(MAX_THREAD);

            FetchModel model = new FetchModel() { Url = url };
            DB[url.ToLower()] = model;


            urlFetchTimer = new Timer(1000);
            urlFetchTimer.Elapsed += Timer_Elapsed;
            dbSaveTimer = new Timer(5000);
            dbSaveTimer.Elapsed += DbSaveTimer_Elapsed;
            urlFetchTimer.Start();
            dbSaveTimer.Start();
            OnWrite(this, new StatusArgs("Started the job"));
        }
        public void Stop()
        {
            living = false;
            urlFetchTimer.Stop();
            urlFetchTimer = null;
            dbSaveTimer.Stop();
            dbSaveTimer = null;
            OnWrite(this, new StatusArgs("Stopped the job"));
        }

        private void DbSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                
                string file = Path.Combine(repoFolder, "_temp_db.jdxn");
                DB.Save(file);
                OnWrite(this, new StatusArgs("DB File saved "));
            }
            catch (Exception ex)
            {
                OnWrite(this, new StatusArgs("Started the job") {  Status = ExecutionSatus.Error });
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WorkInProgress++;
            if (living)
            {
                OnWrite(this, new StatusArgs("Running Producer/Consumer") );
                //Consume
                ConsumeOne();
                //Produce
                ProduceMore();
            }
        }

        private void ConsumeOne()
        {
            Task.Run(() =>
            {
                if (!PENDING.IsCompleted)
                {
                    var url = PENDING.Take();
                    OnWrite(this, new StatusArgs("Job Found - Working: " + url) {  Status = ExecutionSatus.JobFound});
                    WorkOnUrl("" + url);
                }
            });
        }
        private void ProduceMore()
        {
            Task.Run(() =>
            {

                lock (dblock)
                {
                    var values = DB.Values;
                    FetchModel model = values.Where(x => !x.IsStarted()).FirstOrDefault();
                    if (model != null)
                    {                      
                        bool added = PENDING.TryAdd(model.Url);
                        model.Added = DateTime.Now;
                        DB[model.Url.ToLower()] = model;
                        OnWrite(this, new StatusArgs("Adding new Job: " + model.Url) { Status = ExecutionSatus.NewJobEnqued });

                    }
                }

            });
        }
        private void WorkOnUrl(string url)
        {
            try
            {
                FetchModel model = DB[url.ToLower()];
                Download(model);
            }
            catch (Exception mxc) { 
            }
        }
        private FetchModel Download(FetchModel model)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                model.Html = client.DownloadString(model.Url);
                model.Downloaded = DateTime.Now;
                OnWrite(this, new StatusArgs("Downloaded : " + model.Url) { Status = ExecutionSatus.Downloaded });
                   HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(model.Html);
                Task.Run(() =>
                {
                    try
                    {
                        string root = model.Domain;
                        OnWrite(this, new StatusArgs("Working on link" ));
                     

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

                        WriteLine(string.Join(",", linkModels.Select(x=>x.Url).ToArray()));

                        foreach (var m in linkModels)
                        {
                            try
                            {
                                string key = m.Url.ToLower();
                                if (key.StartsWith(m.Domain))
                                {
                                    if (!DB.ContainsKey(key))
                                    {
                                        DB[key] = m;
                                        OnWrite(this, new StatusArgs("New Link found." + key) { Status = ExecutionSatus.NewUrlAdded });
                                    }
                                    else
                                    {
                                        WriteLine("Already added..." + m.Url);
                                    }
                                }
                                else
                                {
                                    WriteLine("Skipping..." + m.Url);
                                }
                            }
                            catch (Exception mdx) {
                                OnWrite(this, new StatusArgs("Error in adding links." + mdx.Message) { Status = ExecutionSatus.Error });
                            }
                        }
                    }
                    catch (Exception ecx)
                    {
                        OnWrite(this, new StatusArgs("Error in parsing links." + ecx.Message) { Status = ExecutionSatus.Error });
                    }
                });
                Task.Run(() =>
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
                            OnWrite(Breaker.NewWordDiscovery.Values, new StatusArgs(w) { Status = ExecutionSatus.NewWord });
                        }
                        catch
                        {

                        }
                    }
                
                    WriteLine("Completed model:" + model.Url);
                    model.Parsed = DateTime.Now;
                    DB[model.Url.ToLower()] = model;

                });
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
