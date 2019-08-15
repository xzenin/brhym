using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoemEditor.Config
{
    public class ApplicationOption
    {
        JSDocument docer = new JSDocument();
        public Option ReadOption(string file)
        {
            file = Path.Combine(new Option().Location.AppFolder, file);
            var item = docer.Read<Option>(file);
            return item;
        }
        public Option WriteOption(string file, Option option)
        {
            if(!Path.IsPathRooted(file))
            {
                file = Path.Combine(option.Location.AppFolder, file);
            }
            option.Save(file);            
            return option;
        }
        public Option CreateOption(string file)
        {

            Option opt = new Option();
            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(opt.Location.AppFolder, file);
            }
            opt.FileName = file;
            return opt;
        }
    }
    public class Option
    {
        public string FileName { get; set; }
        public DateTime Updated { get; set; }
        public ApplicationLocation Location { get; set; }

        public Option()
        {
            Location = new ApplicationLocation();
            Updated = DateTime.Now;
        }
    }
    public class ApplicationLocation {
        public string AppFolder { get; set; }
        public string DBFolder { get; set; }
        public string DocumentFolder { get; set; }
        public string WordFile { get; set; }
        public static string wordFileName = "Bangla_words.doc";
        public ApplicationLocation()
        {
            AppFolder = Environment.CurrentDirectory;// Assembly.GetExecutingAssembly().Location;
            DBFolder = Path.Combine(AppFolder, "_db");
            if (Directory.Exists(DBFolder) == false) {
                Directory.CreateDirectory(DBFolder);
            }
            WordFile = Path.Combine(DBFolder, wordFileName);
            DocumentFolder = Path.Combine(DBFolder, "_document");
            if (Directory.Exists(DocumentFolder) == false)
            {
                Directory.CreateDirectory(DocumentFolder);
            }
        }
    }
}
