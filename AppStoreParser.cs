using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebParserTestApp2
{
    class AppStoreParser<T> where T : class
    {
        private static string appStoreSearchReq = "https://theappstore.org/search.php?search=";

        public List<AppInfo> appStoreApps = new List<AppInfo>();

        static readonly HttpClient client = new HttpClient();

        public async Task<List<AppInfo>> GetAppStoreData(string searchStr)
        {
            appStoreApps.Clear();

            string str = appStoreSearchReq + searchStr + "&platform=software";
            string source = "";
            try
            {
                var response = await client.GetByteArrayAsync(str);
                source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            source = WebUtility.HtmlDecode(source);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(source);

            List<HtmlNode> nodes = document.DocumentNode.Descendants()
                .Where(
                    x => (x.Name == "div" &&
                          x.Attributes["class"] != null &&
                          x.Attributes["class"].Value.Contains("appmain")) /*&&*/
                          /*x.GetAttributeValue("price",null) == "0"*/)
                .Take(3)
                .ToList();

            AppInfo app = new AppInfo();
            // Парсим каждую ноду с приложением
            // TODO параллельно парсить три приложения
            foreach (HtmlNode node in nodes)
            {
                try
                {
                    app = await AppStoreWorker(node, searchStr);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }

                appStoreApps.Add(app);
            }

            return appStoreApps;
        }

        private async Task<AppInfo> AppStoreWorker(HtmlNode node, string searchStr)
        {
            AppInfo app = new AppInfo();

            // Записываем по какому поисковому запросу получено приложение
            app.SearchQuery = searchStr;

            // Получаем ссылку на приложение в официальном AppStore
            app.Link = node.Descendants("a").ToList()[0].GetAttributeValue("href", null);

            if (app.Link.Contains("https://apps.apple.com/"))
            {
                using (HttpClient auxClient = new HttpClient())
                {
                    string appStoreSource = "";
                    try
                    {
                        var appStoreResponse = await auxClient.GetByteArrayAsync(app.Link);
                        appStoreSource = Encoding.GetEncoding("utf-8").GetString(appStoreResponse, 0,
                                                                                        appStoreResponse.Length - 1);
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show($"плохая ссылка {app.Link} на приложение в AppStore");
                    }

                    appStoreSource = WebUtility.HtmlDecode(appStoreSource);
                    HtmlDocument appStoreDoc = new HtmlDocument();
                    appStoreDoc.LoadHtml(appStoreSource);

                    // Получаем ноду иконки и саму иконку
                    HtmlNode iconNode = appStoreDoc.DocumentNode.Descendants()
                        .Where(
                            x => (x.Name == "div" &&
                                  x.Attributes["class"] != null &&
                                  x.Attributes["class"].Value.Contains("product-hero__media l-column small-5 medium-4 " +
                                                                        "large-3 small-valign-top")))
                        .FirstOrDefault();
                    app.IconLink = node.Descendants("img").ToList()[0].GetAttributeValue("src", null);

                    // Получаем название
                    app.Name = appStoreDoc.DocumentNode.Descendants()
                        .Where(
                            x => (x.Name == "h1" &&
                                  x.Attributes["class"] != null &&
                                  x.Attributes["class"].Value.Contains("product-header__title app-header__title")))
                        .FirstOrDefault().InnerText;
                    app.Name = app.Name.Split("\n")[1].Trim();

                    try
                    {
                        // Полуачаем рейтинг
                        app.Rating = appStoreDoc.DocumentNode.Descendants()
                            .Where(
                                x => (x.Name == "span" &&
                                      x.Attributes["class"] != null &&
                                      x.Attributes["class"].Value.Contains("we-customer-ratings__averages__display")))
                            .FirstOrDefault().InnerText;
                    }
                    catch
                    {
                        app.Rating = "Unknown";
                    }

                    // Получаем ноду со скриншотами
                    HtmlNode screenshotsNode = appStoreDoc.DocumentNode.Descendants()
                        .Where(
                            x => (x.Name == "div" &&
                                  x.Attributes["class"] != null &&
                                  x.Attributes["class"].Value.Contains("we-screenshot-viewer ember-view")))
                        .FirstOrDefault();

                    // Получаем скриншоты
                    app.Screenshots = GetAppStoreScreenshots(screenshotsNode);

                    return app;
                }
            }
            else
            {
                return new AppInfo
                {
                    Name = "Не нашлось:(",
                    Link = "nope",
                    Rating = "nope",
                    Screenshots = new List<string> { "nope" },
                    IconLink = "nope",
                    SearchQuery = searchStr
                };
            }
        }

        private List<string> GetAppStoreScreenshots(HtmlNode node)
        {
            List<HtmlNode> imageNodes = node.Descendants()
                        .Where(
                            x => (x.Name == "img" &&
                                  x.Attributes["class"] != null &&
                                  x.Attributes["class"].Value.Contains("we-artwork__image")))
                         .ToList();

            List<string> screenshots = new List<string>();
            foreach (HtmlNode imgNode in imageNodes)
            {
                screenshots.Add(imgNode.GetAttributeValue("src", null));
            }

            return screenshots;
        }
    }
}
