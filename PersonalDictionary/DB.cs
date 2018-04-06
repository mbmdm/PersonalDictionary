using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace PersonalDictionary
{
    public class DB
    {
        public List<Word> Words { get; private set; }
        public List<Dictionary> Dictionaties { get; private set; }
        public List<AppletProgressInfo> TraingProgressApplest { get; private set; }

        private List<WordModifiedCreateInfo> WordModifiedCreateInfos { get; set; }
        private List<DictionaryCreateIfon> DictionaryCreateIfons { get; set; }
        private List<CreateModifiedAppletProgressInfo> CreateModifiedAppletProgressInfos { get; set; }

        XDocument xdoc;

        private DB() { Init(); }

        #region Реализация SingleInstance

        static DB db;        
        static DB() { db = new DB();}
        public static DB GetInstance() {return db;}

        #endregion

        public void Commit()
        {
            Commit_Word();
            Commit_Dic();
            Commit_Progress();

            xdoc.Save(Environment.CurrentDirectory + "\\dic.xml");

            Init();
        }

        private void Commit_Word()
        {
            var create = WordModifiedCreateInfos.Where(i => i.Word == null).ToArray();

            int id = Words[Words.Count - 1].ID;

            foreach (var item in create)
            {
                id++;

                var root = xdoc.Element("library").Element("GlobalDictionary");
                XElement xe = new XElement("word");
                xe.Add(new XAttribute("id", id));
                xe.Add(new XAttribute("en", item.En));
                xe.Add(new XAttribute("ru", item.Ru));
                xe.Add(new XAttribute("date_add", DateTime.Now.ToString()));
                xe.Add(new XAttribute("date_modified", DateTime.Now.ToString()));
                root.Add(xe);
            }
        }

        private void Commit_Dic()
        { 

        
        }

        private void Commit_Progress()
        {


        }

        private void Init()
        {
            Words = new List<Word>();
            Dictionaties = new List<Dictionary>();
            TraingProgressApplest = new List<AppletProgressInfo>();

            WordModifiedCreateInfos = new List<WordModifiedCreateInfo>();

            xdoc = XDocument.Load(Environment.CurrentDirectory + "\\dic.xml");

            Init_Word();
            Init_Dic();
            Init_Progress();
        }

        private void Init_Word()
        {
            var root = xdoc.Element("library").Element("GlobalDictionary");

            foreach (var e in root.Elements())
            {
                Word w = new Word();
                {
                    w.En = e.Attribute("en").Value.ToString();
                    w.Ru = e.Attribute("ru").Value.ToString();
                    w.ID = int.Parse(e.Attribute("id").Value.ToString());
                    w.Modified = DateTime.Parse(e.Attribute("date_modified").Value.ToString());
                    w.Add = DateTime.Parse(e.Attribute("date_add").Value.ToString());
                };

                Words.Add(w);
            }

            Words.Sort();
        }

        private void Init_Dic()
        {


        }

        private void Init_Progress()
        {


        }

        public void create_edit(WordModifiedCreateInfo info) { WordModifiedCreateInfos.Add(info); }
        public void create_edit(List<WordModifiedCreateInfo> info) { WordModifiedCreateInfos.AddRange(info); }
        public void create_edit(DictionaryCreateIfon info) { DictionaryCreateIfons.Add(info); }
        public void create_edit(List<DictionaryCreateIfon> info) { DictionaryCreateIfons.AddRange(info); }
        public void create_edit(CreateModifiedAppletProgressInfo info) { CreateModifiedAppletProgressInfos.Add(info); }
        public void create_edit(List<CreateModifiedAppletProgressInfo> info) { CreateModifiedAppletProgressInfos.AddRange(info); }

        public void delete(WordModifiedCreateInfo info) 
        {
            if (info.Word == null) return;



        }
    }
}
