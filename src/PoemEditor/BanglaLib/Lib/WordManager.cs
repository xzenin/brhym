using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BanglaLib.Lib.Model;

namespace BanglaLib.Lib
{
    public class WordManager : IWordManager
    {
        bool loaded = false;
        public bool Authenticate(string congigJson)
        {
            return true;
        }

        public LikeWord GetWord(string suffix)
        {
            throw new NotImplementedException();
        }

        public List<LikeWord> GetWordByPrefix(string prefix)
        {
            throw new NotImplementedException();
        }

        public List<LikeWord> GetWordBySuffix(string suffix)
        {
            throw new NotImplementedException();
        }

        public List<LikeWord> GetWordSuggestion(string suffix)
        {
            throw new NotImplementedException();
        }

        public void Load(string configJson)
        {
            loaded = true;
        }
    }
}
