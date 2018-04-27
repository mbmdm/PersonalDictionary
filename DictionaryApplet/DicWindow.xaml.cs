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
using System.Collections.ObjectModel;

namespace PersonalDictionary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DicWindow : Window, IApplet
    {
        public ObservableCollection<Word> Words { get; set; }
        public ObservableCollection<Dictionary> Dictionaries { get; set; }

        public DicWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Closing += DicWindow_Closing;
            this.Words = new ObservableCollection<Word>();
            this.Dictionaries = new ObservableCollection<Dictionary>();

            ICollectionView view = CollectionViewSource.GetDefaultView(this.Words);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Add", new DateToDataGridConverter()));
            view.SortDescriptions.Add(new SortDescription("Add", ListSortDirection.Descending));

            dataGrid.ItemsSource = view;

            ActivateOnlyDictionaryWords_Click(this, null);
            AfterDDUpdate();
        }

        #region IApplet

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

        #region Events

        private void CurrentDictionaryChange_Click(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dictionary dic = e.NewValue as Dictionary;
            if (dic != null)
            {
                DB.GetInstance().CurrentDictionaty = dic;
                ActivateOnlyDictionaryWords_Click(this, null);
            }

            if (DB.GetInstance().CurrentDictionaty.COST)
            {
                this.plus_btn01.IsEnabled = false;
                this.min_btn02.IsEnabled = false;
                notShowDic_cb.IsEnabled = false;
            }

            else
            {
                notShowDic_cb.IsEnabled = true;
                if (this.notShowDic_cb.IsChecked == true)
                {
                    this.plus_btn01.IsEnabled = true;
                    this.min_btn02.IsEnabled = true;
                }
                else
                {
                    this.plus_btn01.IsEnabled = false;
                    this.min_btn02.IsEnabled = false;
                }
            }
        }

        /// <summary>Показыват только слова выбранного словаря</summary>
        private void ActivateOnlyDictionaryWords_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView view = dataGrid.ItemsSource as ICollectionView;

            if (view == null) return;

            var targetWords = DB.GetInstance().CurrentDictionaty.Words;

            if (!notShowDic_cb.IsChecked == true)
            {
                var test = view.SourceCollection;

                view.Filter = str => ((str as Word).En.ToLower().Contains(en_tb.Text.ToLower()) &&
                                       (str as Word).Ru.ToLower().Contains(ru_tb.Text.ToLower()) &&
                                       targetWords.Contains(str as Word));

                this.plus_btn01.IsEnabled = false;
                this.min_btn02.IsEnabled = false;
            }
            else
            {                
                view.Filter = str => ((str as Word).En.ToLower().Contains(en_tb.Text.ToLower()) &&
                                      (str as Word).Ru.ToLower().Contains(ru_tb.Text.ToLower()) &&
                                      !targetWords.Contains(str as Word));

                this.plus_btn01.IsEnabled = true;
                this.min_btn02.IsEnabled = true;
            }

            //(dataGrid.ItemsSource as ICollectionView).Refresh();
        }

        void DicWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

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

            en_tb.Text = string.Empty;
            ru_tb.Text = string.Empty;

            DB db = DB.GetInstance();

            DB.GetInstance().Push(info);

            DB.GetInstance().Commit();

            AfterDDUpdate();
            
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

            AfterDDUpdate();
        }

        private void edit_word_Click(object sender, RoutedEventArgs e)
        {
            string test = this.dataGrid.SelectedItems.Count.ToString();

            MessageBox.Show("Не реализовано: " + test, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void WordFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource is ICollectionView)
            {
                (dataGrid.ItemsSource as ICollectionView).Refresh();
            }

            if (this.en_tb.Text != string.Empty) this.plus_btn02.IsEnabled = true;
            else this.plus_btn02.IsEnabled = false;
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

        #endregion

        #region Инкапсуляция

        void AfterDDUpdate()
        {
            this.Words.Clear();
            DB.GetInstance().Words.ForEach(w => this.Words.Add(w));

            this.Dictionaries.Clear();
            DB.GetInstance().Dictionaties.ForEach(d => this.Dictionaries.Add(d));

            this.current_dic_cb.SelectedItem = DB.GetInstance().CurrentDictionaty;

            (this.dataGrid.ItemsSource as ICollectionView).Refresh();
        }

        #endregion

        private void SettingsFullWordProgressShow_Click(object sender, RoutedEventArgs e)
        {
            SettingsFullWordProgress dialog = new SettingsFullWordProgress();
            dialog.ShowDialog();
            this.dataGrid.Items.Refresh();
        }

        /// <summary>Добавляет выбранное(ые) слово(а) в словарь</summary>
        private void add_word_to_dic_Click(object sender, RoutedEventArgs e)
        {
            if (this.dataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите слова для добавления в словарь", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DictionaryInfo dicinfo;
            dicinfo.Dictionary = DB.GetInstance().CurrentDictionaty;
            dicinfo.WordsNew = new List<Word>();

            foreach (var item in this.dataGrid.SelectedItems)
            {
                Word w = item as Word;
                if (w!= null) dicinfo.WordsNew.Add(w);
            }

            var test1 = DB.GetInstance().Words;
            var test2 = dataGrid.Items[0];
            DB.GetInstance().Push(dicinfo);
            DB.GetInstance().Commit();

            AfterDDUpdate();

            //ActivateOnlyDictionaryWords_Click(this, null);
        }

        /// <summary>Удаляет выбранное(ые) слово(а) из словаря</summary>
        private void del_word_from_dic_Click(object sender, RoutedEventArgs e)
        {
            if (this.dataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите слова для удаления из словаря", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DictionaryInfo dicinfo;
            dicinfo.Dictionary = DB.GetInstance().CurrentDictionaty;
            dicinfo.WordsExclude = new List<Word>();

            foreach (var item in this.dataGrid.SelectedItems)
            {
                Word w = item as Word;
                if (w != null) dicinfo.WordsExclude.Add(w);
            }

            DB.GetInstance().Push(dicinfo);
            DB.GetInstance().Commit();

            AfterDDUpdate();
        }

        private void RibbonApplicationMenu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DataGrid_SelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (this.dataGrid.SelectedItems.Count == 0)
            {
                this.min_btn02.IsEnabled = false;
                this.edit_btn01.IsEnabled = false;
            }

            else
            {
                this.min_btn02.IsEnabled = true;
                this.edit_btn01.IsEnabled = true;
            }
        }
    }


}
