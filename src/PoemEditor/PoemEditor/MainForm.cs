using Newtonsoft.Json;
using PoemEditor.Config;
using System;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using BanglaLib;
using BanglaLib.Lib.Model;

namespace PoemEditor
{
    public partial class MainEditor : Form
    {
        string defaultFile = "_default.json";
        Option option = new Option();
        long sequence = 10000000;
        private readonly SynchronizationContext synchronizationContext = null;
        private DateTime previousTime = DateTime.Now;
        Dictionary<char, LikeDictionary> bangla = new Dictionary<char, LikeDictionary>();
        public MainEditor()
        {
            InitializeComponent();
            option.FileName = defaultFile;
            synchronizationContext = SynchronizationContext.Current;
        }

        private void MainEditor_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }
        private void LoadConfig()
        {
            ApplicationOption app = new ApplicationOption();
            //var opt = app.CreateOption(defaultFile);
            //app.WriteOption( defaultFile, opt);
            option = app.ReadOption(option.FileName);
        }
        /// <summary>
        /// 0980-09FD
        /// 2432-2558
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                WordBreaker breaker = new WordBreaker(option.Location.DBFolder);
                string wordFile = Path.Combine(option.Location.DBFolder, "Bangla.txt");
                breaker.BreakFile(wordFile);
                WriteLine("Completed word breaking process.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void WriteLine(string line)
        {
            textBoxConsole.Text += Environment.NewLine + line;
        }

        private void RichTextBoxEditor_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void RichTextBoxEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F4)
            {
                string word = toolStripStatusCurrentWord.Text;
                if (!bangla.Any())
                {
                    WordBreaker breaker = new WordBreaker(option.Location.DBFolder);
                    bangla = breaker.ReadWordFromRepository();
                }
                Suggest(word);
                return;
            }

            var posInLine = richTextBoxEditor.SelectionStart - richTextBoxEditor.GetFirstCharIndexOfCurrentLine();
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
                      
        }
        private void Suggest(string guessWord)
        {
            var suggestion = StartsWith(guessWord);

            this.UIThread(() =>
            {
                textBoxTheWord.Text = guessWord;
            });
            this.UIThread(() =>
            {
                comboBoxTheWord.Items.Clear();
                comboBoxTheWord.Items.AddRange(suggestion.Select(x => x.Word).Take(20).ToArray());
            });
            this.UIThread(() => {
                listBoxSuggestion.Items.Clear();
                listBoxSuggestion.Items.AddRange(suggestion.Select(x => x.Word).Take(100).ToArray());
            });
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

        private void CrawlWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebCrawler web = new WebCrawler(option);
            web.ShowDialog();
        }
    }
}
