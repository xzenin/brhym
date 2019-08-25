using BanglaLib;
using BanglaLib.Lib.Model;
using PoemEditor.Config;
using System;
using System.Collections.Generic;
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
using WordPicker.Config;

namespace WordPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
       
        public MainModel Model { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            option.FileName = defaultFile;
            synchronizationContext = SynchronizationContext.Current;
            Model = new MainModel();
            Model.Status = DateTime.Now.ToString();
            this.statusbar.DataContext = Model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            //Model.Status = "Configuration read.";
            //Model.Position = (sequence++).ToString();
            Model.Word = "NothingSelected";

            //Model.Word = DateTime.Now.ToString();
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
            txtConsole.Text += Environment.NewLine + line;
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
            if (!running)
            {
                running = true;
                Task.Run(() =>
                {
                    while (running)
                    {
                        Model.Status = "Configuration:";
                        Model.Position = (sequence++).ToString();
                        Model.Word = DateTime.Now.ToString();
                        statusbar.UpdateUITextBox();
                        Thread.Sleep(2000);
                    }
                });
            }
            else
            {
                running = false;
            }
            /*
            try
            {
                //Cursor.Current = Cursors.WaitCursor;
                WordBreaker breaker = new WordBreaker(option.Location.DBFolder);
                string wordFile = System.IO.Path.Combine(option.Location.DBFolder, "Bangla.txt");
                breaker.InitializeFolder();
                breaker.BreakFile(wordFile);
                breaker.WriteBack();
                WriteLine("Completed word breaking process.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //Cursor.Current = Cursors.Default;
            }
            */

        }

        private void TxtNotepad_KeyUp(object sender, KeyEventArgs e)
        {
            TextRange textRange = new TextRange(txtNotepad.Document.ContentStart, txtNotepad.Document.ContentEnd);
            MessageBox.Show(textRange.Text);
            /*
            if (e.Key == Key.F4)
            {
                string word = Model.Word;
                if (!bangla.Any())
                {
                    WordBreaker breaker = new WordBreaker(option.Location.DBFolder);
                    bangla = breaker.ReadWordFromRepository();
                }
                Suggest(word);
                return;
            }

            var posInLine = txtNotepad.SelectionStart - richTextBoxEditor.GetFirstCharIndexOfCurrentLine();
            toolStripStatusCursorPosition.Text = "" + posInLine;
            string guessWord = richTextBoxEditor.Text;
            int start = richTextBoxEditor.SelectionStart;
            int previousSpace = 0;
            for (int i = start - 1; i >= 0; i--)
            {
                char ch = richTextBoxEditor.Text[i];
                previousSpace = i;
                if (char.IsWhiteSpace(ch))
                {
                    break;
                }
            }
            int nextSpace = start;
            for (int i = start; i < richTextBoxEditor.Text.Length;)
            {
                char ch = richTextBoxEditor.Text[i];
                nextSpace = ++i;
                if (char.IsWhiteSpace(ch))
                {
                    break;
                }

            }
            if (previousSpace >= nextSpace)
            {
                guessWord = "";
                WriteLine("Space here.");
            }
            else
            {
                int totalchar = nextSpace - previousSpace;
                guessWord = richTextBoxEditor.Text.Substring(previousSpace, totalchar);
            }
            guessWord = guessWord.Trim();
            toolStripStatusCurrentWord.Text = guessWord;
            textBoxTheWord.Text = guessWord;
            */
        }
        private List<LikeWord> StartsWith(string word)
        {
            List<LikeWord> likes = new List<LikeWord>();
            char firstChar = word[0];
            var dic = bangla[firstChar];
            if (dic == null)
            {
                likes.Add(
                    new LikeWord()
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
    }
}
