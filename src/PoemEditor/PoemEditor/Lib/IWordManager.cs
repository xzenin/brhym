using PoemEditor.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoemEditor.Lib
{
    public interface IWordManager
    {
        void Load(string configJson);
        bool Authenticate(string congigJson);
        List<LikeWord> GetWordByPrefix(string prefix);
        List<LikeWord> GetWordBySuffix(string suffix);
        LikeWord GetWord(string suffix);
        List<LikeWord> GetWordSuggestion(string suffix);
    }
}
