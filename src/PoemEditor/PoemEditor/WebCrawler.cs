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
            spider.Stop();
          
            //NewWords = spider.Breaker.NewWordDiscovery.Keys.ToList();
            foreach(var w in spider.Words.Keys)
            {
                WriteLine(w);
            }
            //this.Dispose();
            //this.Hide();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUrl.Text))
            {
                return;
            }
            update = new Timer();
            update.Interval = 5000;
            toolStripProgressBarCount.Alignment = ToolStripItemAlignment.Right;
            toolStripProgressBarCount.Style = ProgressBarStyle.Marquee;
            spider = new Crawler(option.Location.DBFolder);
            spider.OnWrite += new Crawler.WriteLogger((k, line) => {
                WriteLine(line);
            });
            spider.Start(textBoxUrl.Text);
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
          
        }

        public void WriteLine(string line)
        {
            this.UIThread(() =>
            {
                textBoxConsole.Text = Environment.NewLine + line + textBoxConsole.Text;
            });
        }
    }
}
