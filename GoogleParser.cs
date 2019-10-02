using HtmlAgilityPack;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebParserTestApp2
{
    class GoogleParser<T> where T : class
    {
        private static string googleSearchReq = "https://play.google.com/store/search?q=";

        public ObservableCollection<AppInfo> googleApps = new ObservableCollection<AppInfo>();

        static readonly HttpClient client = new HttpClient();

        public async Task<ObservableCollection<AppInfo>> GetGoogleData(string searchText)
        {
            googleApps.Clear();

            string str = googleSearchReq + searchText + "&c=apps";

            var response = await client.GetByteArrayAsync(str);
            string source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);

            source = WebUtility.HtmlDecode(source);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(source);

            List<HtmlNode> nodes = document.DocumentNode.Descendants()
                .Where(
                x => (x.Name == "div" &&
                      x.Attributes["class"] != null &&
                      x.Attributes["class"].Value.Contains("FjwTrf mpg5gc")))
                .Take(3).ToList();


            foreach (HtmlNode node in nodes)
            {
                AppInfo app = new AppInfo();

                // Записываем по какому поисковому запросу получено приложение
                app.SearchQuery = searchText;

                // Получаем ссылку на страницу приложения
                app.Link = node.Descendants("a").ToList()[0].GetAttributeValue("href", null);

                // Получаем список ссылок на скриншоты
                app.Screenshots = await GetGoogleAppScreenshots(app.Link);

                // Получаем название приложения
                app.Name = node.Descendants("div")
                    .Where(
                    x => (x.Name == "div" &&
                          x.Attributes["class"] != null &&
                          x.Attributes["class"].Value.Contains("WsMG1c nnK0zc")))
                    .ToList()[0].GetAttributeValue("title", null);

                // Получаем ссылку на иконку
                app.IconLink = node.Descendants("img").ToList()[0].GetAttributeValue("data-src", null);

                try
                {
                    // Извлекаем рейтинг приложения из атрибута aria-label
                    app.Rating = node.Descendants("div")
                        .Where(
                        x => (x.Name == "div" &&
                              x.Attributes["role"] != null &&
                              x.Attributes["role"].Value.Contains("img")))
                        .ToList()[0].GetAttributeValue("aria-label", null);

                    app.Rating = app.Rating.Split(" ").ToArray()[1];
                }
                catch
                {
                    app.Rating = "Unknown";
                }

                googleApps.Add(app);
            }

            return googleApps;
        }

        private async Task<List<string>> GetGoogleAppScreenshots(string appPageLink)
        {
            appPageLink = "https://play.google.com" + appPageLink;

            using (HttpClient scrClient = new HttpClient())
            {
                var response = await scrClient.GetByteArrayAsync(appPageLink);
                string source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);

                source = WebUtility.HtmlDecode(source);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(source);

                List<HtmlNode> nodes = document.DocumentNode.Descendants()
                    .Where(
                    x => (x.Name == "button" &&
                          x.Attributes["class"] != null &&
                          x.Attributes["class"].Value.Contains("Q4vdJd")))
                    .ToList();

                List<string> screenshots = new List<string>();

                foreach (HtmlNode node in nodes)
                {
                    string screenshotLink = node.Descendants("img").ToList()[0].GetAttributeValue("data-src", null);
                    if (screenshotLink == null)
                    { //  Прим: (Для GooglePlay) Ссылки на скриншот иногда хранятся в атрибуте src
                        screenshotLink = node.Descendants("img").ToList()[0].GetAttributeValue("src", null);
                    }
                    screenshots.Add(screenshotLink);
                }

                return screenshots;
            }
        }
    }
}
