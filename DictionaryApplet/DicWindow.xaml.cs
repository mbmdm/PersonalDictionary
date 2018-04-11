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
using System.ComponentModel;
using System.Globalization;

namespace PersonalDictionary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DicWindow : Window, IApplet
    {
        public List<Word> Words { get; set; }

        #region IApplet

        public DicWindow()
        {
            InitializeComponent();
            this.Closing += DicWindow_Closing;
            this.Loaded += MainWindow_Loaded;

            this.Words = DB.GetInstance().Words;
        }

        void DicWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;            
        }

        public void Run() 
        {
            if (this.Visibility != Visibility.Visible)
                this.ShowDialog();
            else Activate();
        }

        public string DisplayName() { return "Словарь"; }

        public int Position() { return int.MinValue; }

        public Type ToType() { return typeof(DicWindow); }

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

            //this.dataGrid.Items.Refresh();
            (dataGrid.ItemsSource as ICollectionView).Refresh();
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

            (dataGrid.ItemsSource as ICollectionView).Refresh();
        }

        private void edit_word_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Не реализовано", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void en_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource is ICollectionView)
            {
                (dataGrid.ItemsSource as ICollectionView).Refresh();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(DB.GetInstance().Words);
            view.Filter = str => ((str as Word).En.ToLower().Contains(en_tb.Text.ToLower()) &&
                                  (str as Word).Ru.ToLower().Contains(ru_tb.Text.ToLower()));

            dataGrid.ItemsSource = view;
            view.GroupDescriptions.Add(new PropertyGroupDescription("Add", new TempConverter()));

            

            view.SortDescriptions.Add(new SortDescription("Add", ListSortDirection.Descending));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                return;

            Button b = sender as Button;
            

            if (((b.Content as Image).Name == "off"))
            {
                Uri uri = new Uri("img/marker1.jpg", UriKind.Relative);
                (b.Content as Image).Source = new BitmapImage(uri);
                (b.Content as Image).Name = "on";
            }
            else
            {
                Uri uri = new Uri("img/marker3.jpg", UriKind.Relative);
                (b.Content as Image).Source = new BitmapImage(uri);
                (b.Content as Image).Name = "off";
            }
        }
    }

    class TempConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                string[] variants = { "Сегодня", "Вчера", "Месяц назад", "Год назад", "Давно" };

                DateTime date = new DateTime(((DateTime)value).Year, ((DateTime)value).Month, ((DateTime)value).Day);
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                if (date == now)
                    return variants[0];
                else if ((now - date).Days == 1)
                    return variants[1];
                else if ((now - date).Days < 30)
                    return variants[2];
                else if ((now - date).Days < 362)
                    return variants[3];
                else if ((now - date).Days >= 362)
                    return variants[4];

                return "sort error";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
