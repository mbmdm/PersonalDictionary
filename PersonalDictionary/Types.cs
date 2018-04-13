using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDictionary
{
    public class Word : IComparable, ICloneable
    {
        internal int ID { get; set; }
        public string En { get; internal set; }
        public string Ru { get; internal set; }
        public DateTime Add { get; internal set; }
        public DateTime Modified { get; internal set; }
        public string Test { get; set; }
        static Random r;

        static Word() { r = new Random(); }

        internal Word() { Test = "test_" + (Word.r.Next(0,10)%2).ToString(); }

        public int CompareTo(object obj)
        {
            Word word = (Word)obj;

            if (word.ID > ID) return -1;
            else if (word.ID == ID) return 0;
            else return 1;
        }

        public override string ToString()
        {
            return String.Format("{0}, [{1}]", En, Ru);
        }

        public object Clone()
        {
            Word _out = new Word();
            _out.Add = Add;
            _out.En = En;
            _out.ID = ID;
            _out.Ru = Ru;
            _out.Modified = Modified;

            return _out as object;
        }
    }

    public class Dictionary : IComparable
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public List<Word> Words { get; internal set; }

        internal Dictionary() { Words = new List<Word>(); }

        public int CompareTo(object obj)
        {
            Dictionary dic = (Dictionary)obj;
            return String.Compare(this.Name, dic.Name);
        }
    }

    public class AppletData
    {
        public string AppletID { get; internal set; }
        public string AppletDisplay { get; internal set; }
        public Dictionary<Word, int> WordProgress { get; internal set; }

        internal AppletData() { WordProgress = new Dictionary<Word, int>();  }

    }

    public struct WordInfo
    {
        public Word Word { get; set; }
        public string En { get; set; }
        public string Ru { get; set; }
    }

    public struct DictionaryInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary Dictionary { get; set; }
        public List<Word> WordsNew { get; set; }
        public List<Word> WordsExclude { get; set; }
    }

    public struct AppletDataInfo
    {
        public Word Word { get; set; }
        public int Progress { get; set; }
        public AppletData AppletData { get; set; }
    }

}
