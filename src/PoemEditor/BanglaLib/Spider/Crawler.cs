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
        public delegate void WriteLogger(object o, string line);

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
            likeDictionary = Breaker.ReadWordFromRepository();
            OnWrite += Crawler_OnWrite;
        }

        private void Crawler_OnWrite(object o, string line)
        {
            Debug.WriteLine(line);
            Console.WriteLine(line);
        }

        public void WriteLine(string line)
        {
            OnWrite(this, line);
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
        }
        public void Stop()
        {
            living = false;
            urlFetchTimer.Stop();
            urlFetchTimer = null;
            dbSaveTimer.Stop();
            dbSaveTimer = null;
        }

        private void DbSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Let us save your work.");
                var models = DB.Values;
                string file = Path.Combine(repoFolder, "_temp_db.jdxn");
                models.Save(file);
                WriteLine("File saveed.");
            }
            catch(Exception ex)
            {
                WriteLine("File save- failed.");
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WorkInProgress++;
            if (living)
            {
                Debug.WriteLine("Jinda hain");
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
                    //WriteLine("Got an item : " + url);
                    //new instance of url
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
                        //WriteLine("Added to queue, new item." + model.Url);
                    }
                }

            });
        }
        private void WorkOnUrl(string url)
        {
            FetchModel model = DB[url.ToLower()];
            Download(model);
        }
        private FetchModel Download(FetchModel model)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                model.Html = client.DownloadString(model.Url);
                model.Downloaded = DateTime.Now;
                Task.Run(() =>
                {
                    try
                    {
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(model.Html);

                        List<FetchModel> linkModels = new List<FetchModel>();
                        foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            try
                            {

                                // Get the value of the HREF attribute
                                string hrefValue = link.GetAttributeValue("href", string.Empty);
                                if (!string.IsNullOrEmpty(hrefValue))
                                {

                                    /*
                                    if (hrefValue.StartsWith("/"))
                                    {
                                        Debug.WriteLine("Relative URL found :" + hrefValue);
                                        hrefValue = model.Domain + hrefValue;

                                        Debug.WriteLine("Absolute URL found :" + hrefValue);

                                    }
                                    */
                                    if (hrefValue.StartsWith("http"))
                                    {

                                        FetchModel md = new FetchModel() { Url = hrefValue };
                                        linkModels.Add(md);
                                    }
                                    else
                                    {
                                       // WriteLine("Skipping link : " + hrefValue);

                                    }
                                }
                            }
                            catch(Exception lx)
                            {
                               // WriteLine("One link: missing ?");
                            }
                        }
                        foreach (var m in linkModels)
                        {
                            string key = m.Url.ToLower();
                            if (key.StartsWith(m.Domain))
                            {
                                if (!DB.ContainsKey(key))
                                {
                                    DB[key] = m;
                                }
                            }
                        }
                    }
                    catch(Exception ecx)
                    {
                        //WriteLine("Some link could not be parsed." + ecx.Message);
                    }
                });
                Task.Run(() =>
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(model.Html);

                    List<string> lines = new List<string>();
                    foreach (HtmlNode node in document.DocumentNode.SelectNodes("//text()"))
                    {
                        try
                        {
                            Console.WriteLine("text=" + node.InnerText);
                            lines.Add(node.InnerText);
                        }
                        catch {
                        }
                    }

                    //string text = document.Text;
                    List<string> cleanWords = new List<string>();
                    foreach (var line in lines)
                    {
                        var ws = line.Split(" \n\t".ToCharArray())
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToList();
                        cleanWords.AddRange(ws);
                    }
                    List<string> words = cleanWords
                    .Where(x => Breaker.IsBangla(x))
                    .Where(x=> !string.IsNullOrEmpty(x))
                    .ToList();
                    foreach (var w in words) {
                        Words[w] = model;
                    }
                    bool batchComplete = Breaker.ProcessBatch(words.ToArray());
                    WriteLine("Completed model:" + model.Url );
                    model.Parsed = DateTime.Now;
                    DB[model.Url.ToLower()] = model;
                    WriteLine(string.Join(",", words.ToArray()));
                });
            }
            catch (Exception ex)
            {

            }
            return model;
        }
    }
}
