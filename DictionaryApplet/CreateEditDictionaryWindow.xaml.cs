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
    /// Логика взаимодействия для CreateEditDictionaryWindow.xaml
    /// </summary>
    public partial class CreateEditDictionaryWindow : Window
    {
        public System.Windows.Forms.DialogResult Resoult { get; private set; }
        public Dictionary Dictionary { get; set; }
        public DictionaryInfo DictionaryInfo { get; private set; }

        public CreateEditDictionaryWindow()
        {
            InitializeComponent();

            this.IsVisibleChanged += CreateEditDictionaryWindow_IsVisibleChanged;


        }

        private void CreateEditDictionaryWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Dictionary != null)
            {
                this.descr_tb.Text = Dictionary.Description;
                this.name_tb.Text = Dictionary.Name;
            }
        }

        private void Ok_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Resoult = System.Windows.Forms.DialogResult.OK;

            DictionaryInfo info;
            info.Dictionary = Dictionary;

            info.Name = name_tb.Text;
            info.Description = descr_tb.Text;

            if (info.Description == null) info.Description = string.Empty;

            this.DictionaryInfo = info;

            Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Resoult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
