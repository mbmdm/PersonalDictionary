using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDictionary
{
    public class Word : IComparable
    {
        internal int ID { get; set; }
        public string En { get; internal set; }
        public string Ru { get; internal set; }
        public DateTime Add { get; internal set; }
        public DateTime Modified { get; internal set; }

        internal Word() { }

        public int CompareTo(object obj)
        {
            Word word = (Word)obj;

            if (word.ID > ID) return -1;
            else if (word.ID == ID) return 0;
            else return 1;
        }
    }

    public class Dictionary
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public List<Word> Words { get; private set; }

        internal Dictionary() { Words = new List<Word>(); }
    }

    public class AppletProgressInfo
    {
        public Word Word { get; internal set; }
        public int Progress { get; internal set; }
    }

    public struct WordModifiedCreateInfo
    {
        public Word Word { get; set; }
        public string En { get; set; }
        public string Ru { get; set; }
    }

    public struct DictionaryCreateIfon
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public struct CreateModifiedAppletProgressInfo
    {
        public Word Word { get; set; }
        public int Progress { get; set; }
    }

}
