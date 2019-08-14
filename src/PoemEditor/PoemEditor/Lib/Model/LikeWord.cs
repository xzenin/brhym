using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoemEditor.Lib.Model
{
    public class LikeWord
    {
        public long ID { get; set; }
        public string Word { get; set; }
        public string POS { get; set; }
        public string Source { get; set; }
        public DateTime On { get; set; }
    }
    public class LikeDictionary {
        public long Index { get; set; }
        public char Start { get; set; }
        public List<LikeWord> WordList { get; set; }

        public LikeDictionary() {
            WordList = new List<LikeWord>();
        }
    }
}
