using BanglaLib.Lib.Model;
using BanglaLib.Spider;
using PoemEditor.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoemEditor
{
    public partial class WebCrawler : Form
    {
        Crawler spider;
        Option option = new Option();
        Timer update;
        List<string> newWords;

        public List<string> NewWords { get => newWords; set => newWords = value; }

        public WebCrawler(Option moption)
        {
            option = moption;
            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            toolStripProgressBarCount.Style = ProgressBarStyle.Blocks;
            backgroundWorker1.CancelAsync();
            if (spider != null)
            {
                spider.Stop();
            }
         
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUrl.Text))
            {
                return;
            }
            backgroundWorker1.DoWork += new DoWorkEventHandler((s1, e1) => {
                RunJob(textBoxUrl.Text);
            });
            backgroundWorker1.RunWorkerAsync();
        }
        private void RunJob(string url) { 

            update = new Timer();
            update.Interval = 5000;
            //toolStripProgressBarCount.Alignment = ToolStripItemAlignment.Right;
            //toolStripProgressBarCount.Style = ProgressBarStyle.Marquee;
            spider = new Crawler(option.Location.DocumentFolder);
            spider.OnWrite += new Crawler.WriteLogger((k, arg) =>
            {
                Task.Run(() =>
                {
                    switch (arg.Status)
                    {
                        case ExecutionSatus.NewWord:
                            NewWord(arg.Line);
                            break;
                        case ExecutionSatus.NewJobEnqued:
                            AddNewJob(arg.Line);
                            break;
                        case ExecutionSatus.NewUrlAdded:
                            AddNewUrl(arg.Line);
                            break;
                        case ExecutionSatus.Error:
                            DebugMessage(arg.Line);
                            break;
                        default:
                            DebugMessage(arg.Line);
                            break;
                    }

                });
            });
            spider.Start(url);

            /*
            update.Tick += new EventHandler((c, r) => {
                
                toolStripStatusLabelCount.Text = "" + spider.WorkInProgress;
                NewWords.Clear();
                NewWords.AddRange(spider.Words.Keys);
                WriteLine("==========================");
                foreach (var w in spider.Words.Keys)
                {
                    WriteLine(w);
                }
                WriteLine("==========================");
            });
            */
        }

        public void NewWord(string line)
        {
            textBoxConsole.UIThread(() =>
            {
                textBoxConsole.Text = Environment.NewLine + line + textBoxConsole.Text;
            });
        }
        public void DebugMessage(string line)
        {
            textBoxDebug.UIThread(() =>
            {
                textBoxDebug.Text = Environment.NewLine + line + textBoxDebug.Text;
            });
        }
        public void AddNewUrl(string line)
        {
            listViewAll.UIThread(() =>
            {
                listViewAll.Items.Add(line);
            });
        }
        public void AddNewJob(string line)
        {
            listViewRunning.UIThread(() =>
            {
                listViewRunning.Items.Add(line);
            });
        }
        public void UpdateProgressBar(string  value)
        {
            if (textBoxConsole.InvokeRequired)
            {
                textBoxConsole.Invoke(new MethodInvoker(() => textBoxConsole.Text = value));
            }
            else
            {
                textBoxConsole.Text = value;
            }
        }
    }
}
