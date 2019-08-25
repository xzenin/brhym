using System;
using System.Collections.Generic;
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

        List<string> suggestions = new List<string>();
        List<string> startsWith = new List<string>();
        List<string> endsWith = new List<string>();

        public MainModel()
        {

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
        public List<string> Suggestions
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
        public List<string> StartsWith
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
        public List<string> EndsWith
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
