﻿using System;
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
        const string path = "HKEY_CURRENT_USER\\Software\\mbm\\PersonalDictionar";

        #region Реализация SingleInstance / Конструктор / Деструктор

        private Settings() { Init(); }

        ~Settings() { Commit(); }

        static Settings sets;
        static Settings() { sets = new Settings(); }
        public static Settings Get() { return sets; }

        #endregion

        private void Init()
        {
            if (keys == null)
                keys = new Dictionary<string, string>();
            else keys.Clear();

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\mbm\\PersonalDictionar", true);

            if (key == null)
                return;

            string[] names = key.GetValueNames();

            names.ToList().ForEach(delegate (string s)
            {
                try { keys.Add(s, (string)key.GetValue(s)); }
                catch { }
            });
        }

        public void Commit()
        {
            keys.Keys.ToList().ForEach(delegate (string s)
            {
                Registry.SetValue(path, s, keys[s]);
            });
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
                if (keys.Keys.Contains(str)) { keys[str] = value; }
                else keys.Add(str, value);
            }
        }
    }
}