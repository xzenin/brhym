using BanglaLib.Spider;
using PoemEditor.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WordPicker.Model;

namespace WordPicker.GUI
{
    /// <summary>
    /// Interaction logic for CrawlWindow.xaml
    /// </summary>
    public partial class CrawlWindow : Window
    {
        CrawlModel model = new CrawlModel();
        Crawler spider;
        Option option = new Option();
        public delegate void UpdateTextCallback(string message);
        Timer timer;
        public CrawlWindow()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            model = new CrawlModel();
            model.Init();
            model.Url = "http://www.tagoreweb.in/";
            this.UI.DataContext = model;
        }

        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            spider = new Crawler(option.Location.DocumentFolder);
            spider.OnWrite += new Crawler.WriteLogger((k, arg) =>
            {
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(
                          DispatcherPriority.Background,
                          new Action(() =>
                          {
                              switch (arg.Status)
                              {
                                  case ExecutionSatus.NewWord:
                                      UpdateNotePad(arg.Line);
                                      break;
                                  case ExecutionSatus.NewJobEnqued:
                                      DebugMessage(arg.Line);
                                      model.AddToProgress(arg.Model);
                                      break;
                                  case ExecutionSatus.NewUrlAdded:
                                      DebugMessage(arg.Line);
                                      if (arg.Model.IsComplete())
                                      {
                                          model.AddToDone(arg.Model);
                                      }
                                      else
                                      {
                                          model.AddToDB(arg.Model);
                                      }
                                      break;
                                  case ExecutionSatus.Error:
                                      DebugMessage(arg.Line);
                                      break;
                                  default:
                                      DebugMessage(arg.Line);
                                      break;
                              }

                          }));


                });
            });
            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler((t, l) =>
            {
                UpdateUrlList();
            });
            //BackgroundWorker worker = new BackgroundWorker();
            //worker.DoWork += new DoWorkEventHandler((s2, e2) => {
            spider.Start(txtUrl.Text);
            //});
            //worker.RunWorkerAsync(txtUrl.Text);
        }

        private void UpdateUrlList()
        {

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            spider.Stop();
        }
        private void WriteLine(string line)
        {
            txtConsole.Text = line + Environment.NewLine + txtConsole.Text;
        }
        private void WriteNodePad(string line)
        {
            txtNotepad.AppendText(line + '\r');
        }

        public void UpdateNotePad(string line)
        {
            txtNotepad.Dispatcher.Invoke(
                                       new UpdateTextCallback(this.WriteNodePad),
                                       new object[] { line });
        }
        public void DebugMessage(string line)
        {
            txtConsole.Dispatcher.Invoke(
                                     new UpdateTextCallback(this.WriteLine),
                                     new object[] { line });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (spider != null)
            {
                spider.Stop();
            }
        }
    }
}
