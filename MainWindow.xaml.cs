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
        GoogleParser<ObservableCollection<AppInfo>> googleParser;
        AppStoreParser<ObservableCollection<AppInfo>> appStoreParser;

        public MainWindow()
        {
            InitializeComponent();



            googleParser = new GoogleParser<ObservableCollection<AppInfo>>();
            appStoreParser = new AppStoreParser<ObservableCollection<AppInfo>>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            GooglePlayGrid.Items.Clear();
            AppStoreGrid.Items.Clear();

            string searchStr = SearchBar.Text;

            //Task databaseSearch = 


            Task<ObservableCollection<AppInfo>> gTask = googleParser.GetGoogleData(searchStr);
            Task<ObservableCollection<AppInfo>> aTask = appStoreParser.GetAppStoreData(searchStr);

            await Task.WhenAll(gTask, aTask);
            foreach (AppInfo app in gTask.Result)
            {
                GooglePlayGrid.Items.Add(app);
            }

            foreach (AppInfo app in aTask.Result)
            {
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
