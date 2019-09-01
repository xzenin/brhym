using BanglaLib;
using BanglaLib.Lib;
using BanglaLib.Lib.Model;
using PoemEditor.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Threading;
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
        WordBreaker breaker;

        WordManager wordManager;

        public MainModel ViewModel
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                NotifyPropertyChanged("ViewModel");
            }
        }
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
        private void UpdateModelView(Action code)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, code);
            });
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
            breaker = new WordBreaker(option.Location.DBFolder);
            wordManager = new WordManager(breaker);
        }
        private void WriteLine(string line)
        {
            txtConsole.Text = line + Environment.NewLine + txtConsole.Text;
        }

        private void TxtNotepad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Count > 0)
            {
                dirty = true;
            }
        }

        private void CollectWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Word = "Starting..";
                overallProgress.IsIndeterminate = true;
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += new DoWorkEventHandler((s1, e1) =>
                {

                    string wordFile = System.IO.Path.Combine(option.Location.DBFolder, "Bangla.txt");
                    breaker.InitializeFolder();
                    breaker.OnWrite += new WordBreaker.WriteLogger((s4, e4) =>
                    {
                        if (e4.Status == BanglaLib.Spider.ExecutionSatus.NewWord)
                        {
                            txtConsole.Dispatcher.Invoke(
                                        new UpdateTextCallback(this.WriteLine),
                                        new object[] { e4.Line });
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
                worker.ProgressChanged += new ProgressChangedEventHandler((s2, e2) =>
                {
                    WriteLine("String working.");
                });
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((s3, e3) =>
                {
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
        private void Window_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private TextRange GetCurrentTextRange(RichTextBox notepad)
        {
            TextPointer start = notepad.CaretPosition;
            TextPointer end = notepad.CaretPosition;

            string stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);
            string stringAfterCaret = start.GetTextInRun(LogicalDirection.Forward);
            int countToMoveLeft = 0;
            int countToMoveRight = 0;

            for (int i = stringBeforeCaret.Length - 1; i >= 0; --i)
            {
                if (!char.IsWhiteSpace(stringBeforeCaret[i]))
                    ++countToMoveLeft;
                else break;
            }

            for (int i = 0; i < stringAfterCaret.Length; ++i)
            {
                if (!char.IsWhiteSpace(stringAfterCaret[i]))
                    ++countToMoveRight;
                else break;
            }

            start = start.GetPositionAtOffset(-countToMoveLeft);
            end = end.GetPositionAtOffset(countToMoveRight);
            TextRange r = new TextRange(start, end);
            return r;
        }

        private string GetCurrentWordWithSuggested(RichTextBox notepad, string phrase)
        {
            TextRange range = GetCurrentTextRange(notepad);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
            range.Select(range.Start, range.End);
            if (string.IsNullOrEmpty(range.Text))
            {
                return string.Empty;
            }
            range.Text = phrase;

            return phrase;
        }

        private string GetCurrentWord(RichTextBox notepad)
        {
            TextRange range = GetCurrentTextRange(notepad);
            string text = range.Text;
            ViewModel.Position = txtNotepad.CaretPosition.GetOffsetToPosition(txtNotepad.CaretPosition).ToString();
            return text;
        }


        private void TxtNotepad_KeyUp(object sender, KeyEventArgs e)
        {
           
            //if (e.Key == Key.F4)
            //{
                UpdateModelView(() =>
                {
                    ViewModel.Status = "Loading EndsWith";
                    var listStartWith = wordManager.GetWordByPrefix(ViewModel.Word);
                    ViewModel.StartsWith.Clear();
                    listStartWith.ForEach(x =>
                    {
                        ViewModel.StartsWith.Add(x);
                    });
                    ViewModel.Status = "Loading EndsWith";
                    var listEndsWith = wordManager.GetWordBySuffix(ViewModel.Word);
                    ViewModel.EndsWith.Clear();
                    listEndsWith.ForEach(x =>
                    {
                        ViewModel.EndsWith.Add(x);
                    });
                });
            //}
            //else
            //{
                UpdateModelView(() =>
                {
                    ViewModel.Word = GetCurrentWord(txtNotepad);
                    ViewModel.Status = "Loading Suggestion";
                    var listSuggestion = wordManager.GetWordSuggestion(ViewModel.Word);
                    ViewModel.Suggestions.Clear();
                    listSuggestion.ForEach(x =>
                    {
                        ViewModel.Suggestions.Add(x);
                    });
                });
            //}
           
        }

        private void TxtNotepad_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateModelView(() =>
            {
                ViewModel.Word = GetCurrentWord(txtNotepad);
                ViewModel.Status = "Loading Suggestion";
                var listSuggestion = wordManager.GetWordSuggestion(ViewModel.Word);
                ViewModel.Suggestions.Clear();
                listSuggestion.ForEach(x =>
                {
                    ViewModel.Suggestions.Add(x);
                });
            });
        }

        private void CrawlSite_Click(object sender, RoutedEventArgs e)
        {
            CrawlWindow window = new CrawlWindow();
            window.ShowDialog();
        }

        private void ListSuggestion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReplaceText(ViewModel.SelectedSuggestion);
        }
        private void StartsWith_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReplaceText(ViewModel.SelectedStartsWith);
           
        }
        private void EndsWith_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReplaceText(ViewModel.SelectedEndsWith);
        }
        private void ListSuggestion_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReplaceText(ViewModel.SelectedSuggestion);
            }
        }
        private void ListStartWith_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReplaceText(ViewModel.SelectedStartsWith);               
            }
        }
        private void ListEndsWith_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ReplaceText(ViewModel.SelectedEndsWith);
            }
        }
        private void ReplaceText(string phrase)
        {
            UpdateModelView(() =>
            {
                ViewModel.Word = GetCurrentWord(txtNotepad);
                ViewModel.Position = txtNotepad.CaretPosition.GetOffsetToPosition(txtNotepad.CaretPosition).ToString();
                ViewModel.Status = "Replacing " + ViewModel.Word + " with " + phrase;
                if (!string.IsNullOrEmpty(phrase))
                {
                    GetCurrentWordWithSuggested(txtNotepad, phrase);
                }
            });

        }
    }
}
