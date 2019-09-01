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
        WordBreaker breaker;
        Dictionary<char, LikeDictionary> banglaCharIndexDictionary = new Dictionary<char, LikeDictionary>();
        Dictionary<string, string> banglaCompleteDictionary = new Dictionary<string, string>();

        public WordManager(string baseLocation)
        {
            breaker = new WordBreaker(baseLocation);
        }
        public WordManager(WordBreaker _breaker)
        {
            breaker = _breaker;
        }

        public Dictionary<char, LikeDictionary> BanglaCharIndexedDictionary
        {
            get
            {
                if (!banglaCharIndexDictionary.Any())
                {
                    banglaCharIndexDictionary = breaker.ReadWordFromRepository();
                }
                return banglaCharIndexDictionary;
            }
            set => banglaCharIndexDictionary = value;
        }

        public Dictionary<string, string> BanglaCompleteDictionary
        {
            get
            {
                if (banglaCompleteDictionary.Count <= 0)
                {
                    var charDisctionaries = BanglaCharIndexedDictionary.Values;
                    foreach (var d in charDisctionaries)
                    {
                        d.WordList.ForEach(w =>
                        {
                            banglaCompleteDictionary[w.Word] = w.POS;
                        });
                    }
                }
                return banglaCompleteDictionary;
            }
            set => banglaCompleteDictionary = value;
        }

        public bool Authenticate(string congigJson)
        {
            return true;
        }

        public LikeWord GetWord(string suffix)
        {
            throw new NotImplementedException();
        }

        public List<LikeWord> GetWordByPrefix(string word)
        {
            if (!breaker.IsBangla(word))
            {
                return DefaultValue(word);
            }
            List<LikeWord> likes = new List<LikeWord>();
            char firstChar = word.First();
            var dic = BanglaCharIndexedDictionary[firstChar];
            if (dic == null)
            {
                return DefaultValue(word);
            }
            else
            {
                likes = dic.WordList.Where(x => x.Word.StartsWith(word)).ToList();
            }
            if (!likes.Any())
            {
                return DefaultValue(word);
            }
            return likes;
        }

        public List<LikeWord> GetWordBySuffix(string word)
        {
            if (!breaker.IsBangla(word))
            {
                return DefaultValue(word);
            }
            List<LikeWord> likes = (from p in BanglaCompleteDictionary.Keys
                                    where p.EndsWith(word)
                                    select new LikeWord(p)
                                   ).ToList();
            if (!likes.Any())
            {
                return DefaultValue(word);
            }
            return likes;
        }

        public List<LikeWord> DefaultValue(string word)
        {
            List<LikeWord> likes = new List<LikeWord>();
            likes.Add(
                   new LikeWord(word)
                   {
                       Word = word,
                       ID = 0,
                       On = DateTime.Now,
                       POS = "",
                       Source = "temp"
                   });
            return likes;
        }

        public List<LikeWord> GetWordSuggestion(string word)
        {
            if (!breaker.IsBangla(word))
            {
                return DefaultValue(word);
            }
            List<LikeWord> likes = new List<LikeWord>();
            char firstChar = word.First();
            var dic = BanglaCharIndexedDictionary[firstChar];
            if (dic == null)
            {
                return DefaultValue(word);
            }
            else
            {
                likes = dic.WordList.Where(x => x.Word.StartsWith(word)).ToList();
            }
            if (!likes.Any())
            {
                return DefaultValue(word);
            }
            return likes;
        }

        public void Load(string configJson)
        {
            loaded = true;
        }
    }
}
