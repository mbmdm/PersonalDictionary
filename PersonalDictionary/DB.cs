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

        #region Поля

        public Dictionary currentDictionaty;
        public Dictionary CurrentDictionaty
        {
            get { return currentDictionaty; }
            set
            {
                Settings.Get()["PersonalDictionary.CurrentDictionaty"] = value.Name;
                Settings.Get().Commit();
                currentDictionaty = value;
            }
        }
        public List<Word> Words { get; private set; }
        public List<Dictionary> Dictionaties { get; private set; }
        public List<AppletData> ApplestsData { get; private set; }

        private List<WordInfo> WordsInfo { get; set; }
        private List<WordInfo> WordsInfoDelete { get; set; } // Коллекция слов на удаление
        private List<DictionaryInfo> DictionariesInfo { get; set; }
        private List<DictionaryInfo> DictionariesInfoDelete { get; set; } //Коллекция словарей на удаление
        private List<AppletDataInfo> AppletsDataInfo { get; set; }
        private List<AppletDataInfo> AppletsDataInfoDelete { get; set; }

        XDocument xdoc;

        #endregion

        #region Реализация SingleInstance / Конструктор / Деструктор

        private DB() { Init(); }
        ~DB() { Commit(); }

        static DB db;        
        static DB() { db = new DB();}
        public static DB GetInstance() {return db;}

        #endregion

        #region Интерфейс

        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">Если свойство Word != null, 
        /// то выполняется внесение изменений в существующий объкт Word в базе данных. 
        /// В противном случае выполняется занесение нового слова в базу</param>
        public void Push(WordInfo info) { WordsInfo.Add(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">см. описание к Push(WordInfo info)</param>
        public void Push(List<WordInfo> info) { WordsInfo.AddRange(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">Если свойство Dictionary != null, 
        /// то выполняется внесение изменений в существующий объкт Dictionary в базе данных. 
        /// В противном случае выполняется занесение нового словаря в базу</param>
        public void Push(DictionaryInfo info) { DictionariesInfo.Add(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">см. описание к Push(DictionaryCreateIfon info)</param>
        public void Push(List<DictionaryInfo> info) { DictionariesInfo.AddRange(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">Ни одно из свойсвт не дожлно быть null. В случае если слово ранее тренировано,
        /// будет внесено изменение в существующую запись, в противном случае будет создана новая запись</param>
        public void Push(AppletDataInfo info) { AppletsDataInfo.Add(info); }
        /// <summary>Осуществляет внесение изменений в базу данных. Изменения будут применены после Commit().</summary>
        /// <param name="info">см. описание к Push(AppletDataInfo info)</param>
        public void Push(List<AppletDataInfo> info) { AppletsDataInfo.AddRange(info); }

        /// <summary>Полностью удаляет все данные о слове из базы данных: из глобального словаря, из всех персональных словарей, из всех результатов тренировок.</summary>
        /// <param name="info">Свойство Word аргумета типа WordInfo должно быть не null</param>
        public void Delete(WordInfo info) 
        {
            if (info.Word == null) return;

            WordsInfoDelete.Add(info);
        }

        /// <summary>Полностью удаляет все данные о слове из базы данных: из глобального словаря, из всех персональных словарей, из всех результатов тренировок.</summary>
        /// <param name="info">см. описание к Delete(WordInfo info) </param>
        public void Delete(WordInfo[] info)
        {
            foreach (var item in info)
                this.Delete(item);
        }

        /// <summary>Полностью удаляет все данные о словеваре из базы данных</summary>
        /// <param name="info">Свойство Dictionary аргумета типа DictionaryCreateIfon должно быть не null</param>
        public void Delete(DictionaryInfo info)
        {
            if (info.Dictionary == null) return;

            DictionariesInfoDelete.Add(info);
        }

        /// <summary>Полностью удаляет все данные о словеваре из базы данных</summary>
        /// <param name="info">см. описание к Delete(DictionaryCreateIfon info)</param>
        public void Delete(DictionaryInfo[] info)
        {
            foreach (var item in info)
                this.Delete(item);
        }

        /// <summary>Применяет все накопленные изменения, перезаписывает файл данных, обновляет все свойства объекта DB</summary>
        public void Commit()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement(root_xml_name));

            Commit_Word(doc);
            Commit_Dic(doc);
            Commit_Progress(doc);

            //xdoc.Save(Environment.CurrentDirectory + "\\" + xml_Name);

            doc.Save(Environment.CurrentDirectory + "\\" + xml_Name);

            Init();
        }

        #endregion

        internal void RegisterApplet(AppletData applet)
        {
            var root = xdoc.Element(root_xml_name).Element(xml_traing_progress_applest_name);

            XElement xe = new XElement("applet");
            xe.Add(new XAttribute("uid", applet.AppletID));
            xe.Add(new XAttribute("display", applet.AppletDisplay));
            root.Add(xe);
            xdoc.Save(Environment.CurrentDirectory + "\\" + xml_Name);

            Init();
        }

        #region Инкапсуляция

        [Obsolete("Не использовать. Будет удален. Вместо него Commit_Word(XDocument doc)")]
        private void Commit_Word()
        {
            var create = WordsInfo.Where(i => i.Word == null).ToArray();

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

        private void Commit_Word(XDocument doc)
        {
            doc.Element(root_xml_name).Add(new XElement(xml_global_dictionary_name));
            var root = doc.Element(root_xml_name).Element(xml_global_dictionary_name);

            #region Часть 0. Удаляем из существующих словарей слова, стоящие на удалении

            WordsInfoDelete.ForEach(delegate(WordInfo info)
            {
                Dictionaties.ForEach(delegate (Dictionary d)
                {
                    d.Words.Remove(info.Word);
                });
            });

            #endregion

            #region Часть 1. Вносятся изменения в существующие слова

            WordsInfo.Where(i => (i.Word != null)).ToList().ForEach(delegate(WordInfo info)
            {
                if (info.Ru != string.Empty)
                    info.Word.Ru = info.Ru;

                if (info.En != string.Empty)
                    info.Word.En = info.En;

                info.Word.Modified = DateTime.Now;
            });

            #endregion

            #region Часть 2. Удаляем слова

            WordsInfoDelete.ForEach(delegate (WordInfo info)
            { Words.Remove(info.Word); });

            #endregion

            #region Часть 3. Записываем все в XDocument

            Words.ForEach(delegate (Word w)
            {
                XElement xe = new XElement("word");
                xe.Add(new XAttribute("id", w.ID));
                xe.Add(new XAttribute("en", w.En));
                xe.Add(new XAttribute("ru", w.Ru));
                xe.Add(new XAttribute("date_add", w.Add));
                xe.Add(new XAttribute("date_modified", w.Modified));
                root.Add(xe);
            });

            #endregion

            #region Часть 4. Записываем новыве слова

            int id = (Words.Count == 0) ? 0 : Words[Words.Count - 1].ID;

            WordsInfo.Where(i => (i.Word == null)).ToList().ForEach(delegate (WordInfo info)
            {
                id++;

                XElement xe = new XElement("word");
                xe.Add(new XAttribute("id", id));
                xe.Add(new XAttribute("en", info.En));
                xe.Add(new XAttribute("ru", info.Ru));
                xe.Add(new XAttribute("date_add", DateTime.Now.ToString()));
                xe.Add(new XAttribute("date_modified", DateTime.Now.ToString()));
                root.Add(xe);
            });

            #endregion
        }

        private void Commit_Dic(XDocument doc)
        {
            doc.Element(root_xml_name).Add(new XElement(xml_personal_dictionaries_name));
            var root = doc.Element(root_xml_name).Element(xml_personal_dictionaries_name);

            #region Часть 1. Вносятся изменения в существующие словари

            DictionariesInfo.Where(i => (i.Dictionary != null)).ToList().ForEach(delegate (DictionaryInfo info)
            {
                if (info.Description != null) //Изменения в описание словаря
                    if (info.Description != string.Empty)
                        info.Dictionary.Description = info.Description;

                if (info.Name != null) //Изменение в наименование словаря
                    if (info.Name != string.Empty)
                        info.Dictionary.Name = info.Name;

                if (info.WordsNew != null && info.WordsNew.Count != 0) //Добавляем новые слова
                    info.Dictionary.Words.AddRange(info.WordsNew);

                if (info.WordsExclude != null && info.WordsExclude.Count != 0) //Исключаем слова из словаря
                {
                    info.WordsExclude.ForEach(delegate(Word w)
                    {
                        info.Dictionary.Words.Remove(w);
                    });
                }
            });

            #endregion

            #region Часть 2. Удаляем словари

            DictionariesInfoDelete.ForEach(delegate (DictionaryInfo info)
            { Dictionaties.Remove(info.Dictionary); });

            #endregion

            #region Часть 3. Записываем все в XDocument

            Dictionaties.ForEach(delegate (Dictionary d)
            {
                XElement xe = new XElement("dictionary");
                xe.Add(new XAttribute("name", d.Name));
                xe.Add(new XAttribute("descr", d.Description));

                string words = string.Empty;

                d.Words.ForEach(delegate (Word w)
                { words += "#" + w.ID; });

                xe.Add(new XAttribute("words", words));

                root.Add(xe);
            });

                #endregion
            }

        private void Commit_Progress(XDocument doc)
        {
            doc.Element(root_xml_name).Add(new XElement(xml_traing_progress_applest_name));
            var root = doc.Element(root_xml_name).Element(xml_traing_progress_applest_name);

            #region Часть 1. Вносим изменения в прогресс апплетов

            AppletsDataInfo.ForEach(delegate (AppletDataInfo info)
            {
                if (info.AppletData.WordProgress.Keys.Contains(info.Word))
                    info.AppletData.WordProgress[info.Word] = info.Progress;
                else info.AppletData.WordProgress.Add(info.Word, info.Progress);
            });

            #endregion

            #region Часть 2. Записываем все в XDocument

            ApplestsData.ForEach(delegate (AppletData data)
            {
                XElement xe = new XElement("applet");
                xe.Add(new XAttribute("uid", data.AppletID));
                xe.Add(new XAttribute("display", data.AppletDisplay));
                data.WordProgress.Keys.ToList().ForEach(delegate (Word w)
                {
                    XElement temp = new XElement("appletdata");
                    temp.Add(new XAttribute("ref", w.ID));
                    temp.Add(new XAttribute("progress", data.WordProgress[w]));
                    xe.Add(temp);
                });

                root.Add(xe);
            });

            #endregion
        }

        internal void Init()
        { 
            if (Words == null) Words = new List<Word>();
            else Words.Clear();

            if (Dictionaties == null) Dictionaties = new List<Dictionary>();
            else Dictionaties.Clear();

            if (ApplestsData == null) ApplestsData = new List<AppletData>();
            else ApplestsData.Clear();

            if (WordsInfo == null) WordsInfo = new List<WordInfo>();
            else WordsInfo.Clear();

            if (WordsInfoDelete == null) WordsInfoDelete = new List<WordInfo>();
            else WordsInfoDelete.Clear();

            if (DictionariesInfo == null) DictionariesInfo = new List<DictionaryInfo>();
            else DictionariesInfo.Clear();

            if (DictionariesInfoDelete == null) DictionariesInfoDelete = new List<DictionaryInfo>();
            else DictionariesInfoDelete.Clear();

            if (AppletsDataInfo == null) AppletsDataInfo = new List<AppletDataInfo>();
            else AppletsDataInfo.Clear();

            if (AppletsDataInfoDelete == null) AppletsDataInfoDelete = new List<AppletDataInfo>();
            else AppletsDataInfoDelete.Clear();

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

            string currentDic = Settings.Get()["PersonalDictionary.CurrentDictionaty"];

            if (this.CurrentDictionaty == null && this.Dictionaties.Count != 0)
                CurrentDictionaty = Dictionaties[0];
            else if (this.CurrentDictionaty != null && !this.Dictionaties.Contains(CurrentDictionaty))
                CurrentDictionaty = Dictionaties[0];

            if (currentDic != null)
            {
                var findDic = Dictionaties.Where(d => d.Name == currentDic).ToArray();
                if (findDic.Length == 1) CurrentDictionaty = findDic[0];
            }
        }

        private void Init_Progress()
        {
            var root = xdoc.Element(root_xml_name).Element(xml_traing_progress_applest_name);

            foreach (var e in root.Elements())
            {
                AppletData app_info = new AppletData();
                {
                    app_info.AppletID = e.Attribute("uid").Value.ToString();
                    app_info.AppletDisplay = e.Attribute("display").Value.ToString();
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

                ApplestsData.Add(app_info);
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
