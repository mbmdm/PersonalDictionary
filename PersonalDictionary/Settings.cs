using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PersonalDictionary
{
    public class Settings
    {
        Dictionary<string, string> keys;

        #region Реализация SingleInstance / Конструктор / Деструктор

        private Settings() { Init(); }

        ~Settings() { Commit(); }

        static Settings sets;
        static Settings() { sets = new Settings(); }
        public static Settings Get() { return sets; }

        #endregion

        private void Init()
        {
            keys = new Dictionary<string, string>();

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\mbm\\PersonalDictionar", true);

            Registry.SetValue("HKEY_CURRENT_USER\\Software\\mbm\\PersonalDictionar", Guid.NewGuid().ToString(), DateTime.Now.ToLongTimeString());

            if (key == null)
                return;

            string[] names = key.GetValueNames();

            names.ToList().ForEach(delegate (string s)
            {
                try { keys.Add(s, (string)key.GetValue(s)); }
                catch { }
            });
        }

        private void Commit()
        {
            throw new NotImplementedException();
        }

        public string this[string str]
        {
            get
            {
                if (keys.Keys.Contains(str))
                    return keys[str];
                else return null;
            }
            set
            {
                books[index] = value;
            }
        }
    }
}
