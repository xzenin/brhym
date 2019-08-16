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
        public string Html { get; set; }

        public FetchModel()
        {
            Added = DateTime.MinValue;
            Downloaded = DateTime.MinValue;
            Parsed = DateTime.MinValue;
        }

        public bool IsStarted()
        {
            bool done = (Added != DateTime.MinValue);
            return done;
        }
        public bool IsComplete()
        {
            bool done = (Downloaded != DateTime.MinValue) && (Parsed != DateTime.MinValue);
            return done;
        }
        public bool IsDownloaded()
        {
            bool done = (Downloaded != DateTime.MinValue);
            return done;
        }
        public string Domain {
            get {
                Uri uri = new Uri(Url);
                string domain = uri.GetLeftPart(UriPartial.Authority);
                return domain.ToLower();
            }
        }
    }
}
