using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoemCLI
{
    public class CommandExecutor
    {
        string baseFolder;
        public CommandExecutor(string folder)
        {
            baseFolder = folder;
        }

        public virtual HostCommand CreateCommand(string exe, string args)
        {
            var command = new HostCommand(exe, args);
            command.CurrentDirectory = baseFolder;
            return command;
        }

        public virtual HostCommand ExecuteCommand(HostCommand command)
        {
            try
            {
                command.Started = DateTime.Now;
                command.ExecuteStatus = CommandStatus.Started;

                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(command.Target, command.Parameters);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = command.CurrentDirectory;

                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler((s1, e1) => {
                    
                    command.ExecuteStatus = CommandStatus.Exited;
                });
                //* Set your output and error (asynchronous) handlers
                process.OutputDataReceived += new DataReceivedEventHandler( (s2,e2)=> {
                    command.ConsoleOutput += Environment.NewLine + e2.Data;
                } );
                process.ErrorDataReceived += new DataReceivedEventHandler((s3, e3) => {
                    command.ErrorOutput += Environment.NewLine + e3.Data;
                    command.ExecuteStatus = CommandStatus.Error;
                });
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                command.ErrorOutput += ex.Message;
                
               command.ExecuteStatus = CommandStatus.Error;
            }
            finally
            {
                command.Ended = DateTime.Now;
            }
            return command;
        }
    }
}
