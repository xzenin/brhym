using BanglaLib.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WordPicker.Config
{
    public class MainModel : INotifyPropertyChanged
    {

        string word;
        string text;
        string postion;
        string status;
        FlowDocument document = new FlowDocument();

        IList<LikeWord> suggestions = new ObservableCollection<LikeWord>();
        IList<LikeWord> startsWith = new ObservableCollection<LikeWord>();
        IList<LikeWord> endsWith = new ObservableCollection<LikeWord>();

        string selectedSuggestion;
        string selectedStartWidth;
        string selectedEndsWidth;

        public MainModel()
        {

        }

        public void Init()
        {
            Suggestions.Add(new LikeWord("One" + DateTime.Now.Ticks));
            Suggestions.Add(new LikeWord("Two" + DateTime.Now.Ticks));
            Suggestions.Add(new LikeWord("Three" + DateTime.Now.Ticks));

            StartsWith.Add(new LikeWord("Hundred1" + DateTime.Now.Ticks));
            StartsWith.Add(new LikeWord("Hundred2" + DateTime.Now.Ticks));
            StartsWith.Add(new LikeWord("Hundred3" + DateTime.Now.Ticks));

            EndsWith.Add(new LikeWord("Sunday" + DateTime.Now.Ticks));
            EndsWith.Add(new LikeWord("Monday" + DateTime.Now.Ticks));
            EndsWith.Add(new LikeWord("Tuesday" + DateTime.Now.Ticks));
        }

        public string WriteLine(string line)
        {
            text += Environment.NewLine + line;
            return text;
        }
        public string Write(string line)
        {
            text += line;
            return text;
        }

        public string Word
        {
            get => word;
            set
            {
                if (word != value)
                {
                    word = value;
                    OnPropertyChanged("Word");
                }
            }
        }
        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }
        public string Position
        {
            get => postion;
            set
            {
                if (postion != value)
                {
                    postion = value;
                    OnPropertyChanged("Position");
                }
            }
        }
        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        public IList<LikeWord> Suggestions
        {
            get => suggestions;
            set
            {
                if (suggestions != value)
                {
                    suggestions = value;
                    OnPropertyChanged("Suggestions");
                }
            }
        }
        public IList<LikeWord> StartsWith
        {
            get => startsWith;
            set
            {
                if (startsWith != value)
                {
                    startsWith = value;
                    OnPropertyChanged("StartsWith");
                }
            }
        }
        public IList<LikeWord> EndsWith
        {
            get => endsWith;
            set
            {
                if (endsWith != value)
                {
                    endsWith = value;
                    OnPropertyChanged("EndsWith");
                }
            }
        }

        public FlowDocument Document
        {
            get => document;
            set
            {
                if (document != value)
                {
                    document = value;
                    OnPropertyChanged("Document");
                }
            }

        }

        public string SelectedSuggestion
        {
            get => selectedSuggestion; set
            {
                if (selectedSuggestion != value)
                {
                    selectedSuggestion = value;
                    OnPropertyChanged("SelectedSuggestion");
                }
            }
        }
        public string SelectedStartWidth
        {
            get => selectedStartWidth; set
            {
                if (selectedStartWidth != value)
                {
                    selectedStartWidth = value;
                    OnPropertyChanged("SelectedStartWidth");
                }
            }
        }
        public string SelectedEndsWidth
        {
            get => selectedEndsWidth; set
            {
                if (selectedEndsWidth != value)
                {
                    selectedEndsWidth = value;
                    OnPropertyChanged("SelectedEndsWidth");
                }
            }
        }
        

        public void AddLine(string line)
        {
            document.Blocks.Add(new Paragraph(new Run(line)));
            OnPropertyChanged("Document");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
