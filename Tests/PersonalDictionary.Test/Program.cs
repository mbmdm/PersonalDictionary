using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PersonalDictionary.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            test1();
            test2();
            test3();
            test4();
            test5();
            test6();
            //test7(); //Нет вывода результатов в консоль
            Console.Read();
        }

        #region Tests DB

        /// <summary>Простое добавление слов</summary>
        static void test1()
        {
            Console.Write("{0, -55} | ", "Test 1 - простое добавление слов ");
            SaveResourceFile("test1_resoult", "test1_resoult.xml");
            SaveResourceFile("source_empty_dic", "dic.xml");

            DB db = DB.GetInstance();

            WordInfo info1 = new WordInfo();
            info1.En = "testing...";
            info1.Ru = "апробирование";

            db.Push(info1);
            db.Commit();

            int wrongIndex;

            if (!CompareFiles2("dic.xml", "test1_resoult.xml", out wrongIndex))
                Console.WriteLine("false (line " + wrongIndex + ")");
            else Console.WriteLine("true");            
        }

        /// <summary>Простое изменение слов</summary>
        static void test2()
        {
            Console.Write("{0, -55} | ", "Test 2 - простое изменение слов ");
            SaveResourceFile("test2_resoult", "test2_resoult.xml");
            SaveResourceFile("source_empty_dic", "dic.xml");

            Testing.Clear();

            DB db = DB.GetInstance();
            
            //Добавляем одно слово

            WordInfo info1 = new WordInfo();
            info1.En = "testing...";
            info1.Ru = "апробирование";

            db.Push(info1);
            db.Commit();

            //Изменяем слово

            WordInfo info2 = new WordInfo();
            info2.Word = db.Words[0];
            info2.Ru = "EnWord2";
            info2.En = "EnWord1";

            db.Push(info2);
            db.Commit();

            int wrongIndex;

            if (!CompareFiles2("dic.xml", "test2_resoult.xml", out wrongIndex))
                Console.WriteLine("false (line " + wrongIndex + ")");
            else Console.WriteLine("true");
        }

        /// <summary>Простое удаление слов</summary>
        static void test3()
        {
            Console.Write("{0, -55} | ", "Test 3 - gростое удаление слов ");
            SaveResourceFile("source_empty_dic", "test3_resoult.xml");
            SaveResourceFile("source_empty_dic", "dic.xml");

            Testing.Clear();

            DB db = DB.GetInstance();

            //Добавляем одно слово

            WordInfo info1 = new WordInfo();
            info1.En = "testing...";
            info1.Ru = "апробирование";

            db.Push(info1);
            db.Commit();

            //Изменяем слово

            WordInfo info3 = new WordInfo();
            info3.Word = db.Words[0];

            db.Delete(info3);
            db.Commit();

            int wrongIndex;

            if (!CompareFiles2("dic.xml", "test3_resoult.xml", out wrongIndex))
                Console.WriteLine("false (line " + wrongIndex + ")");
            else Console.WriteLine("true");
        }

        /// <summary>Удаление слов, включая удаление из словарей</summary>
        static void test4()
        {
            Console.Write("{0, -55} | ", "Test 4 - Удаление слов, включая удаление из словарей ");
            SaveResourceFile("test4_resoult", "test4_resoult.xml");
            SaveResourceFile("source_test4_dic", "dic.xml");

            Testing.Clear();

            DB db = DB.GetInstance();

            //Удаляем слова
            WordInfo info1 = new WordInfo();
            info1.Word = db.Words[1];
            WordInfo info2 = new WordInfo();
            info2.Word = db.Words[2];

            db.Delete(info1);
            db.Delete(info2);
            db.Commit();

            int wrongIndex;

            if (!CompareFiles2("dic.xml", "test4_resoult.xml", out wrongIndex))
                Console.WriteLine("false (line " + wrongIndex + ")");
            else Console.WriteLine("true");
        }

        /// <summary>Ряд тестов над словарями</summary>
        static void test5()
        {
            Console.Write("{0, -55} | ", "Test 5 - ряд тестов над словарями ");
            SaveResourceFile("test5_resoult", "test5_resoult.xml");
            SaveResourceFile("source_test5_dic", "dic.xml");

            Testing.Clear();

            DB db = DB.GetInstance();

            //Изменяем имя словаря
            DictionaryInfo info1 = new DictionaryInfo();
            info1.Dictionary = db.Dictionaties[1];
            info1.Name = info1.Dictionary.Name + "_changed";
            info1.Description = info1.Dictionary.Description + "_changed";
            db.Push(info1);

            //Удаляем слова из словаря
            DictionaryInfo info2 = new DictionaryInfo();
            info2.Dictionary = db.Dictionaties[2];
            info2.WordsExclude = new List<Word>();
            info2.WordsExclude.Add(info2.Dictionary.Words[0]);
            info2.WordsExclude.Add(info2.Dictionary.Words[1]);
            db.Push(info2);

            //Добавляем слова в словарь
            DictionaryInfo info3 = new DictionaryInfo();
            info3.Dictionary = db.Dictionaties[3];
            info3.WordsNew = new List<Word>();
            info3.WordsNew.Add(db.Words[2]);
            db.Push(info3);

            //Добавляем словарь
            DictionaryInfo info4 = new DictionaryInfo();
            info4.Name = "new_dic_5";
            info4.Description = "new_descr_5";
            db.Push(info4);

            //Удаляем словарь
            DictionaryInfo info5 = new DictionaryInfo();
            info5.Dictionary = db.Dictionaties[0];
            db.Delete(info5);

            db.Commit();

            int wrongIndex;

            if (!CompareFiles2("dic.xml", "test5_resoult.xml", out wrongIndex))
                Console.WriteLine("false (line " + wrongIndex + ")");
            else Console.WriteLine("true");
        }

        /// <summary>Ряд тестов AppletsData</summary>
        static void test6()
        {
            Console.Write("{0, -55} | ", "Test 6 - ряд тестов AppletsData ");
            SaveResourceFile("test6_resoult", "test6_resoult.xml");
            SaveResourceFile("source_test6_dic", "dic.xml");

            Testing.Clear();

            DB db = DB.GetInstance();

            //Добавление апплета
            var data = Testing.GetAppletData("TestApplet_1");
            Testing.RegisterApplet(data);

            //Добавление в апплет данных о прогрессе слова
            AppletDataInfo info1 = new AppletDataInfo();
            info1.AppletData = db.ApplestsData[2];
            info1.Word = db.Words[0];
            info1.Progress = 99;
            db.Push(info1);
            AppletDataInfo info2 = new AppletDataInfo();
            info2 = new AppletDataInfo();
            info2.AppletData = db.ApplestsData[2];
            info2.Word = db.Words[1];
            info2.Progress = 33;
            db.Push(info2);

            //Изменяем прогресс слова в апплете и добаляем прогресс о новом слове
            AppletDataInfo info3 = new AppletDataInfo();
            info3.AppletData = db.ApplestsData[0];
            info3.Word = db.Words[1];
            info3.Progress = 100;
            db.Push(info3);
            AppletDataInfo info4 = new AppletDataInfo();
            info4.AppletData = db.ApplestsData[0];
            info4.Word = db.Words[3];
            info4.Progress = 10;
            db.Push(info4);

            db.Commit();

            int wrongIndex;

            if (!CompareFiles2("dic.xml", "test6_resoult.xml", out wrongIndex))
                Console.WriteLine("false (line " + wrongIndex + ")");
            else Console.WriteLine("true");
        }

        #endregion

        #region Test Settings

        static void test7()
        {
            var sets = Settings.Get();

            sets["test123"] = "1111111";
            sets["tset2"] = "test_key";
        }

        #endregion

        #region Инкапсуляция

        [Obsolete]
        static bool CompareFiles(string f1, string f2, out long position)
        {
            position = 0;

            FileStream fs1 = new FileStream(f1, FileMode.Open);
            FileStream fs2 = new FileStream(f2, FileMode.Open);

            string current_word = string.Empty;

            while (true)
            {
                int i1 = fs1.ReadByte();
                int i2 = fs2.ReadByte();

                char ch = (char)i1;
                char ch2 = (char)i2;
                if (ch == ' ')
                    current_word = string.Empty;
                else current_word += ch;

                //Исключаем атрибуты дат модификации и даты добавления
                if (current_word == "date_add=\"" || current_word == "date_modified=\"")
                {
                    char[] separator = new char[] {'"'};

                    MoveFileStream(separator, fs1);
                    MoveFileStream(separator, fs2);
                    current_word = string.Empty;
                    continue;
                }


                if (i1 == -1 && i2 == -1)
                    break;

                else if (i1 == -1 && i2 != -1)
                {
                    SkipChars(fs2, i2);
                    continue;
                }
                else if (i1 != -1 && i2 == -1)
                {
                    SkipChars(fs1, i1);
                    continue;
                }

                if (i1 != i2)
                {
                    position = fs1.Position;
                    fs1.Close(); fs2.Close();
                    return false;                    
                }
                
            }

            position = fs1.Position;
            fs1.Close(); fs2.Close();
            return true;
        }

        static bool CompareFiles2(string f1, string f2, out int wrongLine)
        {
            wrongLine = -1;
            bool flag = true;

            StreamReader sr1 = new StreamReader(f1, Encoding.UTF8);
            StreamReader sr2 = new StreamReader(f2, Encoding.UTF8);

            string str1 = string.Empty;
            string str2 = string.Empty;

            while (true)
            {
                if (sr1.EndOfStream && sr2.EndOfStream)
                    break;

                wrongLine++;

                str1 = sr1.ReadLine(); str1 = RemoveDateAtt(str1);
                str2 = sr2.ReadLine(); str2 = RemoveDateAtt(str2);

                if (str1 != str2)
                { flag = false; break; }
            }

            sr1.Close(); sr2.Close();
            return flag;
        }

        static string RemoveDateAtt(string str)
        {
            string _out = str;

            if (str == null)
                return string.Empty;

            if (str.Contains("date_add="))
            {
                int startIndex = _out.IndexOf("date_add=");
                int endIndex =_out.Substring(startIndex + 10).IndexOf('"')+1 ;
                _out = _out.Remove(startIndex, endIndex + 10);
            }

            if (str.Contains("date_modified="))
            {
                int startIndex = _out.IndexOf("date_modified=");
                int endIndex = _out.Substring(startIndex + 15).IndexOf('"')+1;
                _out = _out.Remove(startIndex, endIndex + 15);
            }

            return _out;


        }

        static void SaveResourceFile(string resource, string filename)
        {
            var res = Properties.Resources.ResourceManager.GetObject(resource);
            StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8);
            sw.WriteLine(res);
            sw.Close();
            sw.Dispose();
        }

        static void MoveFileStream(char[] chars, FileStream stream)
        {
            while (true)
            {
                char ch = (char)stream.ReadByte();

                foreach (var item in chars)
                    if (item == ch) return;
            }
        }

        static void SkipChars(FileStream fs, int ch)
        {
            int[] same = { (int)'\r', 10 };

            char ch_ = (char)ch;
            while (true)
            {
                if (!same.Contains(ch))
                    break;

                int i = fs.ReadByte();

                if (i == -1)
                    return;

                ch = (char)i;
            }

            fs.Position -= 1;
        }

        #endregion
    }
}
