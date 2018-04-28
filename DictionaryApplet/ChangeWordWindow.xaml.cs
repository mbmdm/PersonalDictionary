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
    /// Логика взаимодействия для ChangeWordWindow.xaml
    /// </summary>
    public partial class ChangeWordWindow : Window
    {
        public Word Word { get; set; }
        public List<Word> Words { get; set; }
        public WordInfo ChangedWordInfo { get; private set; } 

        public System.Windows.Forms.DialogResult Resoult { get; private set; }

        public bool ClearProgress { get; private set; }

        public ChangeWordWindow()
        {
            InitializeComponent();
            this.IsVisibleChanged += ChangeWordWindow_IsVisibleChanged;
            this.Resoult = System.Windows.Forms.DialogResult.No;
        }

        private void ChangeWordWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility != Visibility.Visible) return;
            else if (this.Word == null && this.Words == null) return;
            else if (this.Words != null) this.ch_box.IsEnabled = true;
            else if (this.Word != null)
            {
                this.ch_box.IsEnabled = true;
                this.en_tb.IsEnabled = true;
                this.ru_tb.IsEnabled = true;
                this.en_tb.Text = Word.En;
                this.ru_tb.Text = Word.Ru;
            }
        }

        private void Ok_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Resoult = System.Windows.Forms.DialogResult.OK;

            if (ch_box.IsChecked == true) ClearProgress = true;
            else ClearProgress = false;

            WordInfo info = new WordInfo();

            info.Word = this.Word;
            if (en_tb.Text != string.Empty) info.En = en_tb.Text;
            if (ru_tb.Text != string.Empty) info.Ru = ru_tb.Text;

            this.ChangedWordInfo = info;

            Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Resoult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
