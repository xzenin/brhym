using BanglaLib;
using BanglaLib.Lib.Model;
using PoemEditor.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WordPicker.GUI;
using WordPicker.Model;

namespace WordPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        string defaultFile = "_default.json";
        Option option = new Option();
        long sequence = 10000000;
        private readonly SynchronizationContext synchronizationContext = null;
        private DateTime previousTime = DateTime.Now;
        Dictionary<char, LikeDictionary> bangla = new Dictionary<char, LikeDictionary>();

        bool dirty = false;
        string currentFileInOpening;
        bool running = false;
        public delegate void UpdateTextCallback(string message);
        MainModel model;
        public MainModel ViewModel { get {
                return model;
            } set {
                model = value;
                NotifyPropertyChanged("ViewModel");
            } }
        public MainWindow()
        {
            InitializeComponent();
            option.FileName = defaultFile;
            synchronizationContext = SynchronizationContext.Current;
            ViewModel = new MainModel();
            ViewModel.Init();
            ViewModel.Status = DateTime.Now.ToString();
            ViewModel.Word = DateTime.Now.ToString();
            this.UI.DataContext = model;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void UpdateModelView(object model)
        {
           
            /*
            Application.Current.Dispatcher.Invoke(() => {
               // this.statusbar.DataContext = model;
                this.statusbar.UpdateUIElement();
               // this.rightPanel.DataContext = model;
                this.rightPanel.UpdateUIElement();
            });
            */
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
        private void LoadConfig()
        {
            ApplicationOption app = new ApplicationOption();
            var opt = app.CreateOption(defaultFile);
            app.WriteOption(defaultFile, opt);
            option = app.ReadOption(option.FileName);
        }
        private void WriteLine(string line)
        {
            txtConsole.Text = line + Environment.NewLine + txtConsole.Text;
        }

        private void TxtNotepad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Count>0)
            {
                dirty = true;
            }
        }

        private  void CollectWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Word = "Starting..";
                overallProgress.IsIndeterminate = true;
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += new DoWorkEventHandler( (s1,e1)=> {
                    WordBreaker breaker = new WordBreaker(option.Location.DBFolder);
                    string wordFile = System.IO.Path.Combine(option.Location.DBFolder, "Bangla.txt");
                    breaker.InitializeFolder();
                    breaker.OnWrite += new WordBreaker.WriteLogger((s4, e4) => {
                        if (e4.Status == BanglaLib.Spider.ExecutionSatus.NewWord)
                        {
                            txtConsole.Dispatcher.Invoke(
                                        new UpdateTextCallback(this.WriteLine),   
                                        new object[] {e4.Line } );
                            ViewModel.Word = e4.Line;
                        }
                        else
                        {
                            model.Word = e4.Line;
                            //UpdateModelView(ViewModel);
                        }
                    });
                    breaker.BreakFile(wordFile);
                    breaker.WriteBack();
                   
                });
                worker.ProgressChanged += new ProgressChangedEventHandler((s2, e2) => {
                    WriteLine("String working.");
                });
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((s3, e3) => {
                    WriteLine("Completed word breaking process.");
                });
                worker.RunWorkerAsync(10000);

              
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                overallProgress.IsIndeterminate = false;
            }            

        }


        private List<LikeWord> StartsWith(string word)
        {
            List<LikeWord> likes = new List<LikeWord>();
            char firstChar = word[0];
            var dic = bangla[firstChar];
            if (dic == null)
            {
                likes.Add(
                    new LikeWord(word)
                    {
                        Word = word,
                        ID = 0,
                        On = DateTime.Now,
                        POS = "",
                        Source = "temp"
                    });
            }
            else
            {
                likes = dic.WordList.Where(x => x.Word.StartsWith(word)).ToList();
            }
            return likes;
        }

        private void Window_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private string GetCurrentWord(RichTextBox notepad)
        {
            TextPointer start = notepad.CaretPosition;
            TextPointer end = notepad.CaretPosition;
            
            string stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);
            string stringAfterCaret = start.GetTextInRun(LogicalDirection.Forward);
            int countToMoveLeft = 0;
            int countToMoveRight = 0;

            for (int i = stringBeforeCaret.Length - 1; i >= 0; --i)
            {
                if (char.IsLetter(stringBeforeCaret[i]))
                    ++countToMoveLeft;
                else break;
            }


            for (int i = 0; i < stringAfterCaret.Length; ++i)
            {
                if (char.IsLetter(stringAfterCaret[i]))
                    ++countToMoveRight;
                else break; 
            }

            start = start.GetPositionAtOffset(-countToMoveLeft); 
            end = end.GetPositionAtOffset(countToMoveRight); 
            TextRange r = new TextRange(start, end);
            string text = r.Text;
            return text;
        }

      
        private void TxtNotepad_KeyUp(object sender, KeyEventArgs e)
        {
            ViewModel.Word = GetCurrentWord(txtNotepad);
            UpdateModelView(ViewModel);
        }

        private void TxtNotepad_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Word = GetCurrentWord(txtNotepad);
            UpdateModelView(ViewModel);
        }

        private void CrawlSite_Click(object sender, RoutedEventArgs e)
        {
            CrawlWindow window = new CrawlWindow();
            window.ShowDialog();
        }
    }
}
