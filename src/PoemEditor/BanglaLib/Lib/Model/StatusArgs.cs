using System;
using System.Collections.Generic;
using System.Text;

namespace BanglaLib.Spider
{
    public class ExecutionStatusArgs :EventArgs
    {
        public ExecutionSatus Status { get; set; }
        public string  Line { get; set; }
        public FetchModel Model { get; set; }

        public ExecutionStatusArgs(string message="Working....") {
            Line = message;
            Status = ExecutionSatus.Message;
        }

    }
    public enum ExecutionSatus
    {
        NewLink =0,
        SavedLinkToDB=1,
        GetNextLinkFromDB =2,
        NewJob = 3,
        RemovedJob =4,
        StartedJob =5,
        DownloadedLink = 6,
        ParsedHtml = 7,
        Error =8,
        NewLine = 9,
        NewWord = 10,
        LinkDone =11,
        FileSaved =12,
        StartedApp = 13,
        StoppedApp = 14,
        Message = 15
    }
}
