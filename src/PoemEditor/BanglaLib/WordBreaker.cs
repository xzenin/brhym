using BanglaLib.Lib.Model;
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
    public class WordBreaker
    {
        FileRepository repository;
        int wordLowerBound = 2432;
        int wordHigherBound = 2560;
        int fileCount = 0;
        int batchSize = 100;
        int batchCount = 0;
        public object lck = new object();
        Dictionary<char, LikeDictionary> likeDictionaries;
        public int FileCount { get => fileCount; set => fileCount = value; }
        public WordBreaker(string basePath)
        {
            repository = new FileRepository(basePath);
        }

        public Dictionary<char, LikeDictionary> ReadWordFromRepository()
        {
            Dictionary<char, LikeDictionary> mlikes = new Dictionary<char, LikeDictionary>();
            var nextDirectory = repository.EnsureDirectoryExist("repo");
            DirectoryInfo dr = new DirectoryInfo(nextDirectory.RootDirectory);
            foreach (var file in dr.GetFiles("*.json"))
            {
                var dks = nextDirectory.Deserialize<LikeDictionary>(file.FullName);
                mlikes[dks.Start] = dks;
            }
            return mlikes;
        }

        public int BreakFile(string fileName)
        {
            FileCount = 0;
          
            try
            {
                likeDictionaries = new Dictionary<char, LikeDictionary>();
                var nextDirectory = repository.EnsureDirectoryExist("repo");
                for (var i = wordLowerBound; i < wordHigherBound; i++)
                {
                    string fd = "repo_" + i + ".json";
                    LikeDictionary dictionary = new LikeDictionary();
                    dictionary.Index = (long)i;
                    dictionary.Start = (char)i;
                    dictionary.WordList = new List<LikeWord>();
                    nextDirectory.Serialize(fd, dictionary);
                    likeDictionaries[dictionary.Start] = dictionary;
                }

                using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    bool finished = false;
                    List<string> lines = new List<string>();
                    while (!finished)
                    {
                        string line = reader.ReadLine();

                        Debug.WriteLine(line);
                        //End condition
                        if (line == null)
                        {
                        
                            batchCount++;
                            Debug.WriteLine("Proccing batch count: " + batchCount + " for last batch :" + batchSize);
                            bool batchComplete = ProcessBatch(lines.ToArray());
                            finished = true;
                        }
                        lines.Add(line);
                        if(lines.Count() >= batchSize)
                        {
                            batchCount++;
                            Debug.WriteLine("Starting batch count: " + batchCount + " for batch size :" + batchSize);
                            bool batchComplete = ProcessBatch(lines.ToArray());
                            lines = new List<string>();
                        }
                    }
                }

                foreach (var kv in likeDictionaries)
                {
                    string fd = "repo_" + kv.Value.Index + ".json";
                    nextDirectory.Serialize(fd, kv.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //Cursor.Current = Cursors.Default;
            }
            return FileCount;
        }


        public bool ProcessBatch(string[] documentIds)
        {

            BlockingCollection<string> inputQueue = CreateInputQueue(documentIds);

            BlockingCollection<string> queue = new BlockingCollection<string>(500);

            var consumer = Task.Run(() =>
            {
                foreach (var translatedDocument in queue.GetConsumingEnumerable())
                {
                   ProcessWord( translatedDocument);
                }
            });

            var producers = Enumerable.Range(0, 7)
                .Select(_ => Task.Run(() =>
                {
                    foreach (var documentId in inputQueue.GetConsumingEnumerable())
                    {
                        var document = documentId;
                        queue.Add(document);
                    }
                }))
                .ToArray();

            Task.WaitAll(producers);

            queue.CompleteAdding();

            consumer.Wait();
            return true;
        }
        private BlockingCollection<string> CreateInputQueue(string[] documentIds)
        {
            var inputQueue = new BlockingCollection<string>();

            foreach (var id in documentIds)
                inputQueue.Add(id);

            inputQueue.CompleteAdding();

            return inputQueue;
        }
        public int ProcessWord(string line)
        {
            string[] words = line.Split(' ');
            foreach (var word in words)
            {

                if (!string.IsNullOrWhiteSpace(word))
                {
                    char fchar = word[0];
                    if (fchar < wordLowerBound || fchar > wordHigherBound)
                    {
                        return 0;
                    }
                    else
                    {
                        var w = new LikeWord()
                        {
                            ID = FileCount++,
                            POS = "to",
                            Source = "init",
                            Word = word,
                            On = DateTime.Now

                        };
                        lock (lck)
                        {
                            LikeDictionary dictionary = likeDictionaries[fchar];
                            if (!dictionary.WordList.Where((x) => x.Word == word).Any())
                            {
                                dictionary.WordList.Add(w);
                            }
                        }
                    }
                }
            }
            return 1;
        }
    }

}
