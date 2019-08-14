using Newtonsoft.Json;
using PoemEditor.Config;
using PoemEditor.Lib.Model;
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

namespace PoemEditor
{
    public partial class MainEditor : Form
    {
        string defaultFile = "_default.json";
        Option option = new Option();
        long sequence = 10000000;
        private readonly SynchronizationContext synchronizationContext = null;
        private DateTime previousTime = DateTime.Now;
        List<LikeDictionary> bangla = new List<LikeDictionary>();
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
                string fullpath = Path.Combine(option.Location.DBFolder, "repo");
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
                for (var i = 2432; i < 2560; i++)
                {
                    string fd = "repo_" + i + ".json";
                    string fullpathFd = Path.Combine(option.Location.DBFolder, "repo", fd);
                    LikeDictionary dictionary = new LikeDictionary();
                    dictionary.Index = (long)i;
                    dictionary.Start = (char)i;
                    dictionary.WordList = new List<LikeWord>();
                    File.WriteAllText(fullpathFd, JsonConvert.SerializeObject(dictionary));
                }
                string doctext = Path.Combine(option.Location.DBFolder, "Bangla.txt");
                using (StreamReader reader = new StreamReader(doctext, Encoding.UTF8))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();

                        Debug.WriteLine(line);
                        if (line == null)
                        {
                            break;
                        }
                        string[] words = line.Split(' ');
                        foreach (var word in words)
                        {
                            if (!string.IsNullOrWhiteSpace(word))
                            {
                                char fchar = word[0];
                                if (fchar < 2432 || fchar > 2560)
                                {
                                    continue;
                                }
                                else
                                {
                                    string fd = "repo_" + (int)fchar + ".json";
                                    string fullpathFd = Path.Combine(option.Location.DBFolder, "repo", fd);

                                    LikeDictionary dictionary = new LikeDictionary();
                                    if (File.Exists(fullpathFd))
                                    {
                                        string content = File.ReadAllText(fullpathFd);
                                        dictionary = JsonConvert.DeserializeObject<LikeDictionary>(content);
                                    }
                                    else
                                    {
                                        dictionary.Index = (long)fchar;
                                        dictionary.Start = fchar;
                                        dictionary.WordList = new List<LikeWord>();
                                    }
                                    if (dictionary.WordList.Where((x) => x.Word == word).Any() == false)
                                    {
                                        dictionary.WordList.Add(new LikeWord()
                                        {
                                            ID = sequence++,
                                            POS = "to",
                                            Source = "init",
                                            Word = word,
                                            On = DateTime.Now

                                        });
                                    }
                                    string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
                                    File.WriteAllText(fullpathFd, json);
                                }
                            }
                        }
                    }
                }
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
            var k = SuggestWord(guessWord);
            k.Wait(1000);
        }

        private async Task SuggestWord(string word)
        {
            await Task.Run(() =>
            {
                UpdateUI(word);
            });
        }
        public void UpdateUI(string word)
        {
            if (string.IsNullOrEmpty(word)) return;
            var timeNow = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;

            synchronizationContext.Post(new SendOrPostCallback(o =>
            {
                Text = @"Word " + word;
                if (!bangla.Any())
                {
                    ReadDictionary();
                }
                var suggestion = StartsWith(word);
                listBoxSuggestion.Items.Clear();
                listBoxSuggestion.Items.AddRange(suggestion.Select(x => x.Word).Take(100).ToArray());
                textBoxTheWord.Text = word;
                comboBoxTheWord.Items.Clear();
                comboBoxTheWord.Items.AddRange(suggestion.Select(x => x.Word).Take(20).ToArray());

            }), word);

            previousTime = timeNow;
        }
        private List<LikeWord> StartsWith(string word)
        {
            List<LikeWord> likes = new List<LikeWord>();
            char firstChar = word[0];
            var dic = bangla.Where(x => x.Start == firstChar).FirstOrDefault();
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
        private void ReadDictionary()
        {

            try
            {
                //bangla.Clear();
                //Cursor.Current = Cursors.WaitCursor;
                string fullpath = Path.Combine(option.Location.DBFolder, "repo");
                DirectoryInfo dr = new DirectoryInfo(fullpath);
                foreach (var file in dr.GetFiles("*.json"))
                {
                    var content = File.ReadAllText(file.FullName);
                    LikeDictionary dictionary = JsonConvert.DeserializeObject<LikeDictionary>(content);
                    bangla.Add(dictionary);
                }
            }
            catch (Exception cx)
            {

            }
        }
    }
}
