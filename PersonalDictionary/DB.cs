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
        #region const

        const string xml_Name = "dic.xml";
        const string root_xml_name = "library";
        const string xml_global_dictionary_name = "GlobalDictionary";
        const string xml_personal_dictionaries_name = "PersonalDictionaries";
        const string xml_traing_progress_applest_name = "TraingProgressApplest";

        #endregion

        public List<Word> Words { get; private set; }
        public List<Dictionary> Dictionaties { get; private set; }
        public List<AppletProgressInfo> TraingProgressApplest { get; private set; }

        private List<WordModifiedCreateInfo> WordModifiedCreateInfos { get; set; }
        private List<DictionaryCreateIfon> DictionaryCreateIfons { get; set; }
        private List<CreateModifiedAppletProgressInfo> CreateModifiedAppletProgressInfos { get; set; }

        XDocument xdoc;

        private DB() { Init();}
        ~DB() {Commit();}

        #region Реализация SingleInstance

        static DB db;        
        static DB() { db = new DB();}
        public static DB GetInstance() {return db;}

        #endregion

        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">Если свойство Word != null, 
        /// то выполняется внесение изменений в существующий объкт Word в базе данных. 
        /// В противном случае выполняется занесение нового слова в базу</param>
        public void Push(WordModifiedCreateInfo info) { WordModifiedCreateInfos.Add(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">см. описание к Push(WordModifiedCreateInfo info)</param>
        public void Push(List<WordModifiedCreateInfo> info) { WordModifiedCreateInfos.AddRange(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">Если свойство Dictionary != null, 
        /// то выполняется внесение изменений в существующий объкт Dictionary в базе данных. 
        /// В противном случае выполняется занесение нового словаря в базу</param>
        public void Push(DictionaryCreateIfon info) { DictionaryCreateIfons.Add(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">см. описание к Push(DictionaryCreateIfon info)</param>
        public void Push(List<DictionaryCreateIfon> info) { DictionaryCreateIfons.AddRange(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">Ни одно из свойсвт не дожлно быть null. В случае если слово ранее тренировано,
        /// будет внесено изменение в существующую запись, в противном случае будет создана новая запись</param>
        public void Push(CreateModifiedAppletProgressInfo info) { CreateModifiedAppletProgressInfos.Add(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">см. описание к Push(CreateModifiedAppletProgressInfo info)</param>
        public void Push(List<CreateModifiedAppletProgressInfo> info) { CreateModifiedAppletProgressInfos.AddRange(info); }

        /// <summary>Полностью удаляет все данные о слове из базы данных: из глобального словаря, из всех персональных словарей, из всех результатов тренировок.</summary>
        /// <param name="info">Свойство Word аргумета типа WordModifiedCreateInfo должно быть не null</param>
        public void Delete(WordModifiedCreateInfo info) 
        {
            if (info.Word == null) return;

            Words.Remove(info.Word);
        }

        /// <summary>Полностью удаляет все данные о слове из базы данных: из глобального словаря, из всех персональных словарей, из всех результатов тренировок.</summary>
        /// <param name="info">см. описание к Delete(WordModifiedCreateInfo info) </param>
        public void Delete(WordModifiedCreateInfo[] info)
        {
            foreach (var item in info)
                this.Delete(item);
        }


        /// <summary>Применяет все накопленные изменения, перезаписывает файл данных, обновляет все свойства объекта DB</summary>
        public void Commit()
        {
            Commit_Word();
            Commit_Dic();
            Commit_Progress();

            xdoc.Save(Environment.CurrentDirectory + "\\" + xml_Name);

            Init();
        }

        #region Инкапсуляция

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
            
            if (Words == null) Words = new List<Word>();
            else Words.Clear();

            if (Dictionaties == null) Dictionaties = new List<Dictionary>();
            else Dictionaties.Clear();

            if (TraingProgressApplest == null) TraingProgressApplest = new List<AppletProgressInfo>();
            else TraingProgressApplest.Clear();

            if (WordModifiedCreateInfos == null) WordModifiedCreateInfos = new List<WordModifiedCreateInfo>();
            else WordModifiedCreateInfos.Clear();

            xdoc = XDocument.Load(Environment.CurrentDirectory + "\\" + xml_Name);

            Init_Word();
            Init_Dic();
            Init_Progress();
        }

        private void Init_Word()
        {
            var root = xdoc.Element(root_xml_name).Element(xml_global_dictionary_name);

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
            var root = xdoc.Element(root_xml_name).Element(xml_personal_dictionaries_name);

            foreach (var e in root.Elements())
            {
                Dictionary dic = new Dictionary();
                {
                    dic.Name = e.Attribute("name").Value.ToString();
                    dic.Description = e.Attribute("descr").Value.ToString();
                    string[] ids = e.Attribute("words").Value.ToString().Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    dic.Words = new List<Word>();
                    dic.Words.AddRange(GetWord(ids));
                };

                Dictionaties.Add(dic);
            }

            Dictionaties.Sort();
        }

        private void Init_Progress()
        {
            var root = xdoc.Element(root_xml_name).Element(xml_traing_progress_applest_name);

            foreach (var e in root.Elements())
            {
                AppletProgressInfo app_info = new AppletProgressInfo();
                {
                    app_info.AppletID = e.Attribute("appletUID").Value.ToString();
                    app_info.WordProgress = new Dictionary<Word, int>();

                    foreach (var e_ in e.Elements())
                    {
                        int id = int.Parse(e_.Attribute("ref").Value.ToString());
                        Word w = GetWord(id);
                        int progress = int.Parse(e_.Attribute("progress").Value.ToString());

                        if (w != null) app_info.WordProgress.Add(w, progress);
                        else throw new Exception("Some exception in PersonalDictionary.DB.Init_Progress(). Check invariant of xml data");
                    }
                };

                TraingProgressApplest.Add(app_info);
            }
        }

        Word GetWord(int id)
        {
            Word _out = null;
            var q = Words.Where(i => i.ID == id).ToArray();

            if (q.Length > 1)
                throw new Exception("Some exception in PersonalDictionary.DB.GetWord(int id). Check invariant of xml data");
            else if (q.Length == 0)
                throw new Exception("Some exception in PersonalDictionary.DB.GetWord(int id). Check invariant of xml data");

            _out = q[0];

            return _out;
        }

        Word[] GetWord(int[] ids)
        {
            List<Word> _out = new List<Word>();

            foreach (var id in ids)
                _out.Add(GetWord(id));

            return _out.ToArray();
        }

        Word[] GetWord(string[] ids)
        {
            List<Word> _out = new List<Word>();

            foreach (var id in ids)
                _out.Add(GetWord(int.Parse(id)));

            return _out.ToArray();
        }

        #endregion
    }
}
