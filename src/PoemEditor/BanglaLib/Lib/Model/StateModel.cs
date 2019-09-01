using BanglaLib.Spider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BanglaLib.Lib.Model
{
    public class StateModel : INotifyPropertyChanged
    {
        public static int MAX_THREAD = 2;

        BlockingCollection<FetchModel> inProgressModels;
        ConcurrentDictionary<string, FetchModel> pageDatabase;
        ConcurrentDictionary<string, string> words;

        static object dblock = new object();

        Dictionary<char, LikeDictionary> likeDictionary;

        string rootFolder;

        public StateModel()
        {
            words = new ConcurrentDictionary<string, string>();
            InProgressModels = new BlockingCollection<FetchModel>(MAX_THREAD);
            PageDatabase = new ConcurrentDictionary<string, FetchModel>();
        }

        public ConcurrentDictionary<string, string> Words
        {
            get => words;

            set
            {
                if (words != value)
                {
                    words = value;
                    OnPropertyChanged("Words");
                }

            }
        }

        public BlockingCollection<FetchModel> InProgressModels
        {
            get => inProgressModels;

            set
            {
                if (inProgressModels != value)
                {
                    inProgressModels = value;
                    OnPropertyChanged("InProgressModels");
                }

            }

        }
        public ConcurrentDictionary<string, FetchModel> PageDatabase
        {
            get => pageDatabase;
            set
            {
                if (pageDatabase != value)
                {
                    pageDatabase = value;
                    OnPropertyChanged("PageDatabase");
                }

            }
        }
        public Dictionary<char, LikeDictionary> LikeDictionary
        {
            get => likeDictionary;

            set
            {
                if (likeDictionary != value)
                {
                    likeDictionary = value;
                    OnPropertyChanged("LikeDictionary");
                }

            }
        }

        public string RootFolder { get => rootFolder; set => rootFolder = value; }

        /*

        private FetchModel AddUrlToDB(FetchModel model)
        {
            if (!pageDatabase.ContainsKey(model.Key))
            {
                pageDatabase[model.Key] = model;
                OnWrite(model, new StatusArgs()
                {
                    Line = "New Url Added->" + model.Key,
                    Model = model,
                    Status = ExecutionSatus.NewUrlAdded
                });
                return model;
            }
            return null;
        }
        private FetchModel AddUrlToDB(string url)
        {
            FetchModel model = new FetchModel() { Url = url };
            return AddUrlToDB(model);
        }
        private FetchModel UpdateModelInDB(FetchModel model)
        {
            lock (dblock)
            {
                if (pageDatabase.ContainsKey(model.Key))
                {
                    pageDatabase[model.Key] = model;
                }
            }
            return model;
        }
        private FetchModel UpdateDownlaoded(FetchModel model)
        {
            model.Downloaded = DateTime.Now;
            model.Status = 3;
            UpdateModelInDB(model);
            return model;
        }
        private FetchModel UpdateCompleted(FetchModel model)
        {
            model.Done = DateTime.Now;
            model.Status = 5;
            UpdateModelInDB(model);
            return model;
        }
        private FetchModel UpdateQueued(FetchModel model)
        {
            model.Added = DateTime.Now;
            model.Status = 2;
            UpdateModelInDB(model);
            return model;
        }
        private FetchModel GetModelFromDB(FetchModel model)
        {
            if (pageDatabase.ContainsKey(model.Key))
            {
                return pageDatabase[model.Key];
            }
            return null;
        }
        */

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
