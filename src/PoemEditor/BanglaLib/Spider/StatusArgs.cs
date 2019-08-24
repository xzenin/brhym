using System;
using System.Collections.Generic;
using System.Text;

namespace BanglaLib.Spider
{
    public class StatusArgs :EventArgs
    {
        public ExecutionSatus Status { get; set; }
        public string  Line { get; set; }

        public StatusArgs(string message="Working....") {
            Line = message;
            Status = ExecutionSatus.Message;
        }

    }
    public enum ExecutionSatus
    {
        JobFound=0,
        JobStarted =1,
        Downloaded = 2,
        Parsed = 3,
        Error =4,
        Message = 5,
        NewWord = 6,
        NewUrlAdded=7,
        NewJobEnqued = 8,
        FileSaved =9
    }
}
