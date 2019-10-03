using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WebParserTestApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GoogleParser<List<AppInfo>> googleParser;
        AppStoreParser<List<AppInfo>> appStoreParser;
        DbManager dbManager;
        Window waitWindow;

        public MainWindow()
        {
            InitializeComponent();

            waitWindow = new Window();
            //waitWindow.Name = "Wait for it...";
            waitWindow.FontSize = 30;
            waitWindow.Title = "Wait for it...";
            waitWindow.Content = "Окно закроется автоматически после получения данных парсинга...\n Пока так -_-";

            dbManager = new DbManager();

            googleParser = new GoogleParser<List<AppInfo>>();
            appStoreParser = new AppStoreParser<List<AppInfo>>();

            GooglePlayGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            AppStoreGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            GooglePlayGrid.Items.Clear();
            AppStoreGrid.Items.Clear();


            string searchStr = SearchBar.Text;

            Task<bool> dbTask = Task.Run(() => dbManager.SearchDbRecord(searchStr));
            await Task.WhenAll(dbTask);

            if (dbTask.Result == false) // Такго запроса нет в базе
            {
                waitWindow.Show();

                Task<List<AppInfo>> googleTask = googleParser.GetGoogleData(searchStr);
                Task<List<AppInfo>> appleTask = appStoreParser.GetAppStoreData(searchStr);

                await Task.WhenAll(googleTask, appleTask);

                // save received data to db
                await dbManager.UpdateDb(searchStr, appleTask.Result, googleTask.Result);

                foreach (AppInfo app in googleTask.Result)
                    GooglePlayGrid.Items.Add(app);

                foreach (AppInfo app in appleTask.Result)
                    AppStoreGrid.Items.Add(app);

                waitWindow.Hide();

            }
            else // Инфа по запросу уже есть в базе - берем данные из нее
            {
                Task<List<AppInfo>> googleDbTask = dbManager.GetGoogleDbData(searchStr);
                Task<List<AppInfo>> appStoreDbTask = dbManager.GetAppStoreDbData(searchStr);

                await Task.WhenAll(googleDbTask, appStoreDbTask);

                foreach (AppInfo app in googleDbTask.Result)
                    GooglePlayGrid.Items.Add(app);

                foreach (AppInfo app in appStoreDbTask.Result)
                    AppStoreGrid.Items.Add(app);
            }


        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            GooglePlayGrid.Items.Clear();
            AppStoreGrid.Items.Clear();
        }

        private void List_Keyup(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                var item = sender as ListView;
                Clipboard.SetText(item.SelectedItem.ToString());
            }
            
        }
    }
}
