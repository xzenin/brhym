using BanglaLib.Lib.Model;
using BanglaLib.Spider;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanglaLib
{
    public class LineStore
    {
        FileRepository repository;
        int wordLowerBound = 2432;
        int wordHigherBound = 2560;
        int fileCount = 0;
        int batchSize = 100;
        int batchCount = 0;
        public object lck = new object();
        Dictionary<string, FetchModel> lineDictionaries;
        ConcurrentDictionary<string, LikeWord> newWordDiscovery = new ConcurrentDictionary<string, LikeWord>();
        public int FileCount { get => fileCount; set => fileCount = value; }
        public ConcurrentDictionary<string, LikeWord> NewWordDiscovery { get => newWordDiscovery; set => newWordDiscovery = value; }

        public delegate void WriteLogger(object o, ExecutionStatusArgs args);

        public event WriteLogger OnWrite;

        public LineStore(string basePath)
        {
            repository = new FileRepository(basePath);
            OnWrite = new WriteLogger((s, e) => {
                Debug.WriteLine(e.Line);
            });
        }
        public bool IsBangla(char ch) {
            return ch >= wordLowerBound && ch < wordHigherBound;
        }
        //must be trimmed
        public bool IsBangla(string word)
        {
            var bangla = word.ToCharArray().Count(c => IsBangla(c));
            return bangla == word.Length;
        }
        public Dictionary<string, FetchModel> ReadWordFromRepository()
        {
            lineDictionaries = new Dictionary<string, FetchModel>();
            var nextDirectory = repository.EnsureDirectoryExist("_spider");
            DirectoryInfo dr = new DirectoryInfo(nextDirectory.RootDirectory);
            if (dr.GetFiles("*.txt").Length > 1)
            {
                foreach (var file in dr.GetFiles("*.txt"))
                {
                    var dks = nextDirectory.Deserialize<FetchModel>(file.FullName);
                    OnWrite(this, new ExecutionStatusArgs("Read File : " + file.FullName));
                    lineDictionaries[dks.Key] = dks;
                }

            }else
            {
                OnWrite(this, new ExecutionStatusArgs("No file found in source"));
               return InitializeFolder();
            }
            return lineDictionaries;
        }
        public Dictionary<string, FetchModel> InitializeFolder()
        {
            lineDictionaries = new Dictionary<string, FetchModel>();
            var nextDirectory = repository.EnsureDirectoryExist("_spider");
            OnWrite(this, new ExecutionStatusArgs("Intialized the folder " + nextDirectory.RootDirectory));
            return lineDictionaries;
        }
        public void WriteBack()
        {
            var nextDirectory = repository.EnsureDirectoryExist("_spider");
            foreach (var kv in lineDictionaries)
            {
                string fd = "keyword_" + kv.Value.ID + ".txt";
                nextDirectory.WriteContent(fd, kv.Value.Html);
            }
            OnWrite(this, new ExecutionStatusArgs("Written back to folder " + nextDirectory.RootDirectory));
        }
        public void WriteBack(FetchModel model)
        {
            var nextDirectory = repository.EnsureDirectoryExist("_spider");
                string fd = "keyword_" + model.ID + ".txt";
                nextDirectory.AppendContent(fd, model.Html);
            OnWrite(this, new ExecutionStatusArgs("Written back to file " + nextDirectory.RootDirectory));
        }

    }

}
