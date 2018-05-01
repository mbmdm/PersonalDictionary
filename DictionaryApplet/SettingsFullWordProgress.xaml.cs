using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PersonalDictionary
{
    /// <summary>
    /// Логика взаимодействия для SettingsFullWordProgress.xaml
    /// </summary>
    public partial class SettingsFullWordProgress : Window
    {
        string property_name;

        public SettingsFullWordProgress()
        {
            property_name = Settings.Applets_to_calc_full_progress;

            InitializeComponent();

            Init();
        }

        void Init()
        {
            string val = Settings.Get()[property_name];

            for (int i = 0; i < DB.GetInstance().ApplestsData.Count; i++)
            {
                var ch_box = new System.Windows.Controls.CheckBox();
                ch_box.Content = DB.GetInstance().ApplestsData[i].AppletDisplay;
                ch_box.Tag = DB.GetInstance().ApplestsData[i];

                if (val != null && val.Contains(DB.GetInstance().ApplestsData[i].AppletID))
                    ch_box.IsChecked = true;


                if (i ==0 || i%2==0)
                    this.panel1.Children.Add(ch_box);
                else this.panel2.Children.Add(ch_box);
            }
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_btn_Click(object sender, RoutedEventArgs e)
        {
            Settings sets = Settings.Get();
            string val = string.Empty;

            List<CheckBox> boxes = new List<CheckBox>();

            foreach (var item in panel1.Children)
                boxes.Add(item as CheckBox);
            foreach (var item in panel2.Children)
                boxes.Add(item as CheckBox);


            foreach (var item in boxes)
            {
                if (item == null) continue;

                CheckBox ch = item as CheckBox;
                AppletData appData = ch.Tag as AppletData;

                if (ch.IsChecked == true) val+="#" + appData.AppletID;
            }
            sets[property_name] = val;
            sets.Commit();

            Close();
        }
    }
}
