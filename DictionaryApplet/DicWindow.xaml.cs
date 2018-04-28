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
            DB.GetInstance().Updated += OnDBUpdate;
            this.DataContext = this;
            this.Closing += DicWindow_Closing;
            this.Words = new ObservableCollection<Word>();
            this.Dictionaries = new ObservableCollection<Dictionary>();

            ICollectionView view = CollectionViewSource.GetDefaultView(this.Words);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Add", new DateToDataGridConverter()));
            view.SortDescriptions.Add(new SortDescription("Add", ListSortDirection.Descending));

            dataGrid.ItemsSource = view;

            SetDataGridFilters();
            OnDBUpdate();
            UIComponentsEnableInit();
        }
        ~DicWindow() { DB.GetInstance().Updated -= OnDBUpdate; }

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

        /// <summary>После изменения выбора текущего словаря</summary>
        private void CurrentDictionaryChange_Click(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dictionary dic = e.NewValue as Dictionary;

            if (dic == null) return;

            DB.GetInstance().CurrentDictionaty = dic;

            SetDataGridFilters();
            UIComponentsEnableInit();
        }

        /// <summary>При вкл/выкл checkbox "Не в словаре"</summary>
        private void ShowNotInDicWords_Click(object sender, RoutedEventArgs e)
        {
            SetDataGridFilters();
            UIComponentsEnableInit();
        }

        /// <summary>Добавляет слово в БД</summary>
        private void Add_word_Click(object sender, RoutedEventArgs e)
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
        }

        /// <summary>Удаляет слово из БД</summary>
        private void Del_word_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.dataGrid.SelectedItems)
            {
                WordInfo info;
                info.Word = item as Word;
                DB.GetInstance().Delete(info);
            }

            DB.GetInstance().Commit();
        }

        /// <summary>Изменяет слово в БД</summary>
        private void Edit_word_Click(object sender, RoutedEventArgs e)
        {
            #region Выбрано одного слово в DataGrid

            if (dataGrid.SelectedItems.Count == 1)
            {
                Word w = this.dataGrid.SelectedItem as Word;
                if (w == null) return;

                ChangeWordWindow dialog = new ChangeWordWindow();
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.Word = w;
                dialog.ShowDialog();

                var info = dialog.ChangedWordInfo;

                if (dialog.Resoult == System.Windows.Forms.DialogResult.OK)
                {
                    DB.GetInstance().Push(info);
                    DB.GetInstance().Commit();
                }
            }

            #endregion
        }

        /// <summary>UI: при вводе текста в поля для англ. и рус. слов</summary>
        private void WordFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource is ICollectionView) (dataGrid.ItemsSource as ICollectionView).Refresh();

            UIComponentsEnableInit();
        }

        /// <summary>Предотвращает закрытие окна на прав. верхн. крест (происходит скрытие)</summary>
        void DicWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
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

        /// <summary>Показывает диалговое окно, в котором можно установить апрелы, прогресс слов в которых будет учитываться при расчете общего прогресса слова</summary>
        private void CalcWordProgressSettingsDialog_Click(object sender, RoutedEventArgs e)
        {
            SettingsFullWordProgress dialog = new SettingsFullWordProgress();

            dialog.ShowDialog();

            if (dataGrid.ItemsSource is ICollectionView) (dataGrid.ItemsSource as ICollectionView).Refresh();
        }

        /// <summary>Добавляет выбранное(ые) слово(а) в словарь</summary>
        private void Add_word_to_dic_Click(object sender, RoutedEventArgs e)
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
                if (w != null) dicinfo.WordsNew.Add(w);
            }

            DB.GetInstance().Push(dicinfo);
            DB.GetInstance().Commit();
        }

        /// <summary>Удаляет выбранное(ые) слово(а) из словаря</summary>
        private void Del_word_from_dic_Click(object sender, RoutedEventArgs e)
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
        }

        /// <summary>Завершает работу всего приложения</summary>
        private void ShutDownAppication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>Проиходит при изменении выбора в DataGrid</summary>
        private void DataGrid_SelectedChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            UIComponentsEnableInit();
        }

        /// <summary>Двойной клик по элементу в DataGrid</summary>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit_word_Click(this, null);
        }

        #endregion

        #region Инкапсуляция

        /// <summary>Перезаписывает все ObservableCollection коллекции. Выполнять после DB.Commit()</summary>
        void OnDBUpdate()
        {
            this.Words.Clear();
            DB.GetInstance().Words.ForEach(w => this.Words.Add(w));

            this.Dictionaries.Clear();
            DB.GetInstance().Dictionaties.ForEach(d => this.Dictionaries.Add(d));

            this.current_dic_cb.SelectedItem = DB.GetInstance().CurrentDictionaty;

            (this.dataGrid.ItemsSource as ICollectionView).Refresh();
        }

        /// <summary>Устанавливает фильтры для DataGrid, отображающего коллекцию слов</summary>
        void SetDataGridFilters()
        {
            ICollectionView view = dataGrid.ItemsSource as ICollectionView;

            if (view == null) return;

            var targetWords = DB.GetInstance().CurrentDictionaty.Words;

            if (!this.group3_DisplayNotDicWords_cb.IsChecked == true)
            {
                var test = view.SourceCollection;

                view.Filter = str => ((str as Word).En.ToLower().Contains(en_tb.Text.ToLower()) &&
                                       (str as Word).Ru.ToLower().Contains(ru_tb.Text.ToLower()) &&
                                       targetWords.Contains(str as Word));
            }
            else
            {
                view.Filter = str => ((str as Word).En.ToLower().Contains(en_tb.Text.ToLower()) &&
                                      (str as Word).Ru.ToLower().Contains(ru_tb.Text.ToLower()) &&
                                      !targetWords.Contains(str as Word));
            }
        }

        /// <summary>Согласует свойства IsEnabled в различных элементов диалогового окна</summary>
        void UIComponentsEnableInit()
        {
            //При вводе текста в поле для англ. слова
            if (en_tb.Text != string.Empty) this.group1_plus_btn.IsEnabled = true;
            else this.group1_plus_btn.IsEnabled = false;

            //При выборе элементов в DataGrid
            if (this.dataGrid.SelectedItems.Count == 0)
            {
                this.group1_min_btn.IsEnabled = false;
                this.group1_edit_btn.IsEnabled = false;
                this.group3_min_btn.IsEnabled = false;
                this.group3_plus_btn.IsEnabled = false;
            }
            else
            {
                this.group1_min_btn.IsEnabled = true;
                this.group1_edit_btn.IsEnabled = true;

                if (this.group3_DisplayNotDicWords_cb.IsChecked == true) //Зависимость от вкл/выкл опции "Не в словаре"
                {
                    this.group3_plus_btn.IsEnabled = true;
                    this.group3_min_btn.IsEnabled = false; 
                }
                else
                {
                    this.group3_plus_btn.IsEnabled = false;
                    this.group3_min_btn.IsEnabled = true;
                }
            }

            // Если словарь системный мы не можем не добавлять в него, не удалять
            if (DB.GetInstance().CurrentDictionaty.COST)
            {
                this.group3_min_btn.IsEnabled = false;
                this.group3_plus_btn.IsEnabled = false;
                this.group3_DisplayNotDicWords_cb.IsEnabled = false;
            }
            else this.group3_DisplayNotDicWords_cb.IsEnabled = true;

        }

        #endregion


    }


}
