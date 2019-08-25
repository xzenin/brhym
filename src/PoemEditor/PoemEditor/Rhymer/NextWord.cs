using NetMQ;
using NetMQ.Sockets;
using PoemCLI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoemEditor.Rhymer
{
    public partial class NextWord : Form
    {
        bool running = false;
        public NextWord()
        {
            InitializeComponent();
        }

        private void ButtonServer_Click(object sender, EventArgs e)
        {
            running = true;
            Task.Run(() =>
            {
                using (var server = new ResponseSocket())
                {
                    server.Bind("tcp://*:5555");
                    this.UIThread(() =>
                    {
                        richTextBoxRecive.Text = "Started application.";
                    });

                    while (running)
                    {
                        string msg = server.ReceiveFrameString();
                        this.UIThread(() =>
                        {
                            richTextBoxRecive.Text += string.Format("From Client: {0}", msg);
                        });

                        server.SendFrame("World");
                    }
                }
            });
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                string path = @"E:\Lab\git\brhym\poetry";
                string target = @"E:\Program Files (x86)\Microsoft Visual Studio\Shared\Python37_64\python.exe";
                string restOfLine = @"suniltest.py";
                this.UIThread(() =>
                {
                    richTextBoxSend.Text = "Sending message ";
                });
              
                CommandExecutor pshell = new CommandExecutor(path);
                HostCommand command = pshell.CreateCommand(target, restOfLine);
                var output = pshell.ExecuteCommand(command);

                this.UIThread(() =>
                {
                    if (output.ExecuteStatus == CommandStatus.Error)
                    {
                        richTextBoxSend.Text += output.ErrorOutput;
                    }
                    else
                    {
                        richTextBoxSend.Text += output.ConsoleOutput;
                    }
                });
            });
        }

        private void ButtonRecieve_Click(object sender, EventArgs e)
        {
            using (var server = new ResponseSocket())
            {
                server.Bind("tcp://*:5555");

                string msg = server.ReceiveFrameString();
                richTextBoxRecive.Text += string.Format("From Client: {0}", msg);
                //server.SendFrame("World");

            }
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            running = false;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
