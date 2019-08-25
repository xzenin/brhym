using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoemCLI
{
    class Program
    {
        public static List<string> ALLCOMMAND = new List<string>();
        public static int CursorPosition = 0;
        static void Main(string[] args)
        {
            CommandExecutor pshell = new CommandExecutor( Environment.CurrentDirectory );
            while (true)
            {
               
                Console.Write(":>");
                string line = Console.ReadLine();
             
                if (line.ToLower() == "exit")
                {
                    break;
                }
                else
                {
                    string [] arguments = line.Split(' ');
                    if (arguments.Length > 1)
                    {
                        string target = arguments[0];
                        string restOfLine = line.Substring(target.Length).Trim();
                        HostCommand command = pshell.CreateCommand(target, restOfLine);
                        var output = pshell.ExecuteCommand(command);
                        Console.WriteLine($"Status: {output.ExecuteStatus} Time Started: {output.Started} Time Ended: {output.Ended}");
                        if(output.ExecuteStatus != CommandStatus.Error)
                        {
                            Console.WriteLine(output.ConsoleOutput);
                        }
                        else
                        {
                            Console.WriteLine($"Some Error : { output.ErrorOutput } ");
                        }
                    }
                }
            }
        }       
    
    }
}
