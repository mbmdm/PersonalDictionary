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

        internal Word() { }

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
            return String.Compare(dic.Name, this.Name);
        }
    }

    public class AppletProgressInfo
    {
        public string AppletID { get; internal set; }
        public Dictionary<Word, int> WordProgress { get; internal set; }
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
        public Dictionary Dictionary { get; set; }
        public List<Word> WordsNew { get; set; }
        public List<Word> WordsExclude { get; set; }
    }

    public struct CreateModifiedAppletProgressInfo
    {
        public Word Word { get; set; }
        public int Progress { get; set; }
        public AppletProgressInfo AppletProgressInfo { get; set; }
    }

}
