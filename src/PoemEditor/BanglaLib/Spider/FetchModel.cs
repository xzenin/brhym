using System;
using System.Collections.Generic;
using System.Text;

namespace BanglaLib.Spider
{
    public class FetchModel
    {
        public string Url { get; set; }
        public DateTime Added { get; set; }
        public DateTime Downloaded { get; set; }
        public DateTime Parsed { get; set; }
        public DateTime Done { get; set; }
        public string Html { get; set; }

        public int Status { get; set; }

        public FetchModel(string url=null)
        {
            Url = url;
            Added = DateTime.Now;
            Downloaded = DateTime.MinValue;
            Parsed = DateTime.MinValue;
            Done = DateTime.MinValue;
            Status = 0;
        }

        public FetchModel Clone()
        {
            FetchModel model = new FetchModel();
            model.Parsed = this.Parsed;
            model.Status = this.Status;
            model.Url = this.Url;
            model.Html = this.Html;
            model.Added = this.Added;
            model.Downloaded = this.Downloaded;
            model.Done = this.Done;
            return model;

        }

        public bool IsStarted()
        {
            bool done = Status >= 1;
            return done;
        }
        public bool IsComplete()
        {
            bool done =  Status >= 5 ;
            return done;
        }
        public bool IsDownloaded()
        {
            bool done = Status >=2 ;
            return done;
        }
        public string Domain {
            get {
                Uri uri = new Uri(Url);
                string domain = uri.GetLeftPart(UriPartial.Authority);
                return domain.ToLower();
            }
        }
        public string Key
        {
            get
            {

                return Url.ToLower();
            }
        }
        public string StatusText
        {
            get
            {
                string stx = "Added";
                switch(Status)
                {
                    case 1:
                        stx = "Started";
                        break;
                    case 2:
                        stx = "Qued";
                        break;
                    case 3:
                        stx = "Downloaded";
                        break;
                    case 4:
                        stx = "Parsed";
                        break;
                    case 5:
                        stx = "Done";
                        break;
                    default:
                        stx = "New";
                        break;
                }
                return stx;
            }
        }
    }
}
