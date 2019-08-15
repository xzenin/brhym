using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BanglaLib
{
    public class FileRepository
    {
        public string RootDirectory { get; set; }

        public FileRepository(string baseDirectory)
        {
            RootDirectory = baseDirectory;
        }

        public FileRepository EnsureDirectoryExist(string directory) {
            if (!Path.IsPathRooted(directory))
            {
                directory = Path.Combine(RootDirectory, directory);
            }
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            return new FileRepository(directory);
        }
        public string GetFullFileName(string fileName) {
            string fullName = Path.IsPathRooted(fileName) ? fileName : Path.Combine(RootDirectory, fileName);
            return fullName;
        }

        public string ReadContent(string fileName)
        {
            string fullName = GetFullFileName(fileName);
            string content = File.ReadAllText(fullName);
            return content;
        }
        public string WriteContent(string fileName, string content)
        {
            string fullName = GetFullFileName(fileName);
            File.WriteAllText(fullName, content);
            return content;
        }
        public T Deserialize<T>(string fileName)
        {
            var json = ReadContent(fileName);
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
        public int Serialize(string fileName, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            WriteContent(fileName, json);
            return json.Length;
        }
    }
}
