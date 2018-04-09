﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;

namespace PersonalDictionary
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private поля класса

        System.Drawing.Font font;
        System.Windows.Forms.Padding padding;
        Dictionary<string, IApplet> applets;
        private System.Windows.Forms.NotifyIcon notify;

        #endregion

        #region Override

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            { DB.GetInstance(); }
            catch (Exception ex)
            {
                string message = "Ошибка открытия XML файла с данными.\n";
                message += ex.Message ;

                if (ex.InnerException != null) { message += '\n' + ex.InnerException.Message; }

                MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
                return;
            }

            base.OnStartup(e);

            InitBase();
            InitNotify();

            AttachApplets();
            RegisterApplets();

            applets.Values.ToArray()[0].Run();
        }

        #endregion

        #region Инкапсуляция

        /// <summary>Инициализация полей класса App</summary>
        private void InitBase()
        {
            font = new System.Drawing.Font("Arial", 12);
            padding = new System.Windows.Forms.Padding(5);
            applets = new Dictionary<string, IApplet>();
            //applets.Add(typeof(DicWindow).FullName, new DicWindow());
            
        }

        /// <summary>Инициализация NotifyIcon</summary>
        private void InitNotify()
        {
            notify = new System.Windows.Forms.NotifyIcon();
            notify.Icon = new System.Drawing.Icon("notify.ico");
            notify.Visible = true;
            notify.Text = "Personla Dictionary";

            System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Font = font;

            System.Windows.Forms.ToolStripMenuItem exit_menu = new System.Windows.Forms.ToolStripMenuItem();
            exit_menu.Text = "Выход";
            exit_menu.Font = font;
            exit_menu.Margin = padding;
            exit_menu.Click += delegate(object sender, EventArgs e) { this.Shutdown(); };
            menu.Items.Add(exit_menu);

            notify.ContextMenuStrip = menu;
        }

        private void AttachApplets()
        {
            string path = Environment.CurrentDirectory;
            path += "\\applets";

            string[] dlls = System.IO.Directory.GetFiles(path, "*.dll");

            foreach (var dll_ka in dlls)
            {
                var asm = Assembly.LoadFrom(dll_ka);

                var types = asm.GetTypes().Where((t, obj) => t.IsClass).ToList();

                foreach (var item in types)
                {
                    IApplet obj = null;
                    try { obj = asm.CreateInstance(item.FullName) as IApplet; }
                    catch { }
                    if (obj != null) applets.Add(obj.GetType().FullName, obj);
                }
            }
        }

        private void RegisterApplets()
        {
            var apps = applets.Values.ToList();

           apps.Sort(delegate(IApplet x, IApplet y)
                {
                    if (x.Position() >= y.Position()) return -1;
                    else return 1;
                });

           foreach (var item in apps)
           {
               System.Windows.Forms.ToolStripMenuItem element = new System.Windows.Forms.ToolStripMenuItem();

               
               element.Text = item.DisplayName();
               element.Font = font;
               element.Margin = padding;
               element.Click += delegate(object sender, EventArgs e) {
                   try
                   {
                       item.Run();
                   }
                   catch (Exception ex)
                   { 
                       MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                       MessageBox.Show("Для разработчиков: Для окна необходимо обрабатывать событие closing, где отменять закрытие с помощью CancelEventArgs. Вместо закрытия диалог должен быть скрыт с помощью Hide().");
                   }
               };


               if (item.IsMainDialog())
                   notify.DoubleClick += delegate(object sender, EventArgs e) { item.Run(); };

               notify.ContextMenuStrip.Items.Insert(0, element);
           }

        }

        #endregion

        #region Test

        void test()
        {
            string[] words = File.ReadAllLines("List.txt", System.Text.Encoding.Default);
            List<WordModifiedCreateInfo> array = new List<WordModifiedCreateInfo>();

            foreach (var item in words)
            {
                var word = item.Split(new char[] { '=' });

                WordModifiedCreateInfo createInfo = new WordModifiedCreateInfo();
                createInfo.En = word[0].Trim();
                createInfo.Ru = word[1].Trim();
                array.Add(createInfo);
            }

            DB db = DB.GetInstance();


            db.Push(array);
            db.Commit();
        }

        #endregion
    }
}
