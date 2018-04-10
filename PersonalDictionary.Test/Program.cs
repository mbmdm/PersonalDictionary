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


        }

        /// <summary>Простое добавление слов</summary>
        static void test1()
        {
            Console.Write("{0, -35} | ", "Test 1 - простое добавление слов ");
            SaveResourceFile("test1_resoult", "test1_resoult.xml");
            SaveResourceFile("source_empty_dic", "dic.xml");

            DB db = DB.GetInstance();

            WordModifiedCreateInfo info1 = new WordModifiedCreateInfo();
            info1.En = "testing...";
            info1.Ru = "апробирование";

            db.Push(info1);
            db.Commit();

            int wrongIndex;

            if (!CompareFiles("dic.xml", "test1_resoult.xml", out wrongIndex))
                Console.Write("false (pos " + wrongIndex + ")");
            else Console.Write("true");
        }

        static bool CompareFiles(string f1, string f2, out int wrongIndex)
        {
            FileStream fs1 = new FileStream(f1, FileMode.Open);
            FileStream fs2 = new FileStream(f2, FileMode.Open);

            wrongIndex = 0;
            string current_word = string.Empty;

            while (true)
            {
                int i1 = fs1.ReadByte();
                int i2 = fs2.ReadByte();
                wrongIndex++;

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
                    return true;

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
                    return false;
                
            }

            wrongIndex = -1;
            return true;
        }

        static void SaveResourceFile(string resource, string filename)
        {
            var res = Properties.Resources.ResourceManager.GetObject(resource);
            StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8);
            sw.WriteLine(res);
            sw.Close();
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
    }
}
