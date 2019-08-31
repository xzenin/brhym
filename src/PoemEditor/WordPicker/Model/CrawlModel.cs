using BanglaLib.Lib.Model;
using BanglaLib.Spider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WordPicker.Model
{
    public class CrawlModel : INotifyPropertyChanged
    {

        string url;
        string text;
        string postion;
        string status;

        IList<FetchModel> allPages = new ObservableCollection<FetchModel>();
        IList<FetchModel> workingPages = new ObservableCollection<FetchModel>();
        IList<FetchModel> donePages = new ObservableCollection<FetchModel>();

        string selectedSuggestion;
        string selectedStartWidth;
        string selectedEndsWidth;

        public CrawlModel()
        {

        }

        public void Init()
        {
            /*
            UrlInDB.Add(new FetchModel("One" + DateTime.Now.Ticks));
            UrlInDB.Add(new FetchModel("Two" + DateTime.Now.Ticks));
            UrlInDB.Add(new FetchModel("Three" + DateTime.Now.Ticks));

            UrlInProgress.Add(new FetchModel("Hundred1" + DateTime.Now.Ticks));
            UrlInProgress.Add(new FetchModel("Hundred2" + DateTime.Now.Ticks));
            UrlInProgress.Add(new FetchModel("Hundred3" + DateTime.Now.Ticks));

            UrlDone.Add(new FetchModel("Sunday" + DateTime.Now.Ticks));
            UrlDone.Add(new FetchModel("Monday" + DateTime.Now.Ticks));
            UrlDone.Add(new FetchModel("Tuesday" + DateTime.Now.Ticks));
            */
        }

        public FetchModel AddToDB(FetchModel model)
        {
            UrlInDB.Add(model);
            // OnPropertyChanged("UrlInDB");
            return model;
        }
        public FetchModel AddToProgress(FetchModel model)
        {
            UrlInProgress.Add(model);
            // OnPropertyChanged("UrlInDB");
            return model;
        }

        public FetchModel AddToDone(FetchModel model)
        {
            UrlDone.Add(model);
            // OnPropertyChanged("UrlInDB");
            return model;
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

        public string Url
        {
            get => url;
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged("Url");
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
        public IList<FetchModel> UrlInDB
        {
            get => allPages;
            set
            {
                if (allPages != value)
                {
                    allPages = value;
                    OnPropertyChanged("UrlInDB");
                }
            }
        }
        public IList<FetchModel> UrlInProgress
        {
            get => workingPages;
            set
            {
                if (workingPages != value)
                {
                    workingPages = value;
                    OnPropertyChanged("UrlInProgress");
                }
            }
        }
        public IList<FetchModel> UrlDone
        {
            get => donePages;
            set
            {
                if (donePages != value)
                {
                    donePages = value;
                    OnPropertyChanged("UrlDone");
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
