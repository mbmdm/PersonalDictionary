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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersonalDictionary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DicWindow : Window, IApplet
    {
        DB db = DB.GetInstance();

        #region IApplet

        public DicWindow()
        {
            InitializeComponent();
            this.Closing += DicWindow_Closing;
            this.dataGrid.ItemsSource = DB.GetInstance().Words;
        }

        void DicWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Run() { this.ShowDialog(); }

        public string DisplayName() { return "Словарь"; }

        public int Position() { return int.MinValue; }

        public Type GetType() { return typeof(DicWindow); }

        public bool IsMainDialog() { return true; }

        #endregion

        private void add_word_Click(object sender, RoutedEventArgs e)
        {
            WordInfo info = new WordInfo();
            info.En = en_tb.Text.Trim();
            info.Ru = ru_tb.Text.Trim();

            if (info.En == string.Empty || info.Ru == string.Empty)
            {
                MessageBox.Show("Введите новое слово", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DB db = DB.GetInstance();

            DB.GetInstance().Push(info);

            DB.GetInstance().Commit();

            this.dataGrid.Items.Refresh();
        }

        private void del_word_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.dataGrid.SelectedItems)
            {
                WordInfo info;
                info.Word = item as Word;
                DB.GetInstance().Delete(info);
            }

            DB.GetInstance().Commit();

            this.dataGrid.Items.Refresh();
        }

        private void edit_word_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализовано", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
