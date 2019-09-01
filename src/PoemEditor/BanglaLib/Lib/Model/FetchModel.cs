using System;
using System.Collections.Generic;
using System.Text;

namespace BanglaLib.Spider
{
    public class FetchModel
    {
        public string Url { get; set; }
        public DateTime Added { get; set; }
        public DateTime Done { get; set; }
        public string Html { get; set; }

        public string ID { get; set; }
        public ExecutionSatus Status { get; set; }

        public FetchModel(string url=null)
        {
            Url = url;
            Added = DateTime.Now;
            Done = DateTime.MinValue;
            Status = 0;
            ID = Guid.NewGuid().ToString().Replace("-", "");            
        }

        public FetchModel Clone()
        {
            FetchModel model = new FetchModel();
            model.Status = this.Status;
            model.Url = this.Url;
            model.Html = this.Html;
            model.Added = this.Added;
            model.Done = this.Done;
            model.ID = this.ID;
            return model;
        }

        public bool IsStarted()
        {
            bool done = (int)Status >= 1;
            return done;
        }
        public bool IsComplete()
        {
            bool done =  (int)Status >= 5 ;
            return done;
        }
        public bool IsDownloaded()
        {
            bool done = (int)Status >=2 ;
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
                    case ExecutionSatus.NewLink:
                        stx = "Started";
                        break;
                    case ExecutionSatus.NewJob:
                        stx = "Queued";
                        break;
                    case ExecutionSatus.DownloadedLink:
                        stx = "Downloaded";
                        break;
                    case ExecutionSatus.ParsedHtml:
                        stx = "Parsed";
                        break;
                    case ExecutionSatus.LinkDone:
                        stx = "Done";
                        break;
                    default:
                        stx = "Message";
                        break;
                }
                return stx;
            }
        }
    }
}
