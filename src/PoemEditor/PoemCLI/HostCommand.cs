using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoemCLI
{
    public class HostCommand : DynamicObject
    {
        public static string RootDirectory { get; set; }
        static HostCommand() {
            RootDirectory = Environment.CurrentDirectory;
        }
        public string CurrentDirectory { get; set; }
        public string Target { get; set; }
        public string Parameters { get; set; }

        /*
        public string OutputToFile { get; set; }
        public string ErrorToFile { get; set; }
        public string InputToFile { get; set; }
        */

        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
        public CommandStatus ExecuteStatus { get; set; }
        public StorageMode UseStorage { get; set; }


        public string ConsoleOutput { get; set; }
        public string ErrorOutput { get; set; }

        public HostCommand(string file) :this(file,"") {
      
        }
        public HostCommand(string file, string args)
        {
            Target = file;
            if (Path.IsPathRooted(file))
            {
                CurrentDirectory = new FileInfo(file).DirectoryName;
            }
            else
            {
                CurrentDirectory = RootDirectory;
            }
            Parameters = args;
            UseStorage = StorageMode.Memory;
            ExecuteStatus = CommandStatus.NotStarted;
            ConsoleOutput = "";
            
        }
    }
    public enum CommandStatus
    {
        NotStarted = 0,
        Started = 1,
        Paused = 2,
        Error = 3,
        Stopped = 4,
        Exited = 5
    }
    public enum StorageMode
    {
        Memory = 0,
        DiskFile = 1,
        Stream = 2,
        Network =3
    }
}
