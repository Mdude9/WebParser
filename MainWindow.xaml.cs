using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
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
        

        public MainWindow()
        {
            InitializeComponent();

            dbManager = new DbManager();

            googleParser = new GoogleParser<List<AppInfo>>();
            appStoreParser = new AppStoreParser<List<AppInfo>>();
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

            if (dbTask.Result == false) 
            {
                Task<List<AppInfo>> gTask = googleParser.GetGoogleData(searchStr);
                Task<List<AppInfo>> aTask = appStoreParser.GetAppStoreData(searchStr);

                await Task.WhenAll(gTask, aTask);

                // save received data to db
                await dbManager.UpdateDb(searchStr, gTask.Result, aTask.Result);

                foreach (AppInfo app in gTask.Result)
                    GooglePlayGrid.Items.Add(app);

                foreach (AppInfo app in aTask.Result)
                    AppStoreGrid.Items.Add(app);
            }
            else
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

        private void GoogleList_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
