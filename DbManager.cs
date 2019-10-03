using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using WebParserTestApp2.Model;

namespace WebParserTestApp2
{
    class DbManager
    {
        public async Task<bool> SearchDbRecord(string searchStr)
        {
            using (var context = new AppInfoContext())
            {
                var temp = await context.MainTable
                    .FirstOrDefaultAsync(x => x.SearchQuery == searchStr);

                return (temp != null) ? true : false;
            }
        }

        public async Task UpdateDb(string searchStr, List<AppInfo> appStoreData,
                                                     List<AppInfo> googleData)
        {
            List<AppStoreTable> appStoreDbData = new List<AppStoreTable>();
            List<GooglePlayTable> googleDbData = new List<GooglePlayTable>();

            MainTable mainTable = new MainTable();
            mainTable.SearchQuery = searchStr;

            foreach (AppInfo app in appStoreData)
            {
                /* Конвертирую список скриншотов в единую строку 
                для хранения в бд в виде текста */
                string tmpStr = String.Join(',', app.Screenshots.ToArray());

                AppStoreTable dbApp = new AppStoreTable();
                dbApp.Name = app.Name;
                dbApp.IconLink = app.IconLink;
                dbApp.Link = app.Link;
                dbApp.Rating = app.Rating;
                dbApp.ScreenshotsSum = tmpStr;
                dbApp.SearchQuery = app.SearchQuery;

                appStoreDbData.Add(dbApp);
            }

            foreach (AppInfo app in googleData)
            {
                /* Конвертирую список скриншотов в единую строку 
                для хранения в бд в виде текста */
                string tmpStr = String.Join(',', app.Screenshots.ToArray());

                GooglePlayTable dbApp = new GooglePlayTable();
                dbApp.Name = app.Name;
                dbApp.IconLink = app.IconLink;
                dbApp.Link = app.Link;
                dbApp.Rating = app.Rating;
                dbApp.SearchQuery = app.SearchQuery;

                dbApp.ScreenshotsSum = tmpStr;

                googleDbData.Add(dbApp);
            }

            using (var db = new AppInfoContext())
            {
                db.MainTable.Add(mainTable);

                foreach (GooglePlayTable app in googleDbData)
                    await db.GooglePlayTable.AddAsync(app);

                foreach (AppStoreTable app in appStoreDbData)
                    await db.AppStoreTable.AddAsync(app);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        public async Task<List<AppInfo>> GetGoogleDbData(string searchStr)
        {
            using (var context = new AppInfoContext())
            {
                List<GooglePlayTable> dbResultList = context.GooglePlayTable
                                .AsEnumerable()
                                .Where(x => x.SearchQuery == searchStr)
                                .ToList();

                List<AppInfo> appDbList = new List<AppInfo>();
                foreach (GooglePlayTable dbTable in dbResultList)
                {
                    AppInfo app = new AppInfo();
                    app.Name = dbTable.Name;
                    app.Link = dbTable.Link;
                    app.IconLink = dbTable.IconLink;
                    app.Rating = dbTable.Rating;
                    app.SearchQuery = dbTable.SearchQuery;
                    app.Screenshots = dbTable.ScreenshotsSum.Split(',').ToList();

                    appDbList.Add(app);
                }

                return appDbList;
            }
        }

        public async Task<List<AppInfo>> GetAppStoreDbData(string searchStr)
        {
            using (var context = new AppInfoContext())
            {
                List<AppStoreTable> dbResultList = context.AppStoreTable
                                .AsEnumerable()
                                .Where(x => x.SearchQuery == searchStr)
                                .ToList();

                List<AppInfo> appDbList = new List<AppInfo>();
                foreach (AppStoreTable dbTable in dbResultList)
                {
                    AppInfo app = new AppInfo();
                    app.Name = dbTable.Name;
                    app.Link = dbTable.Link;
                    app.IconLink = dbTable.IconLink;
                    app.Rating = dbTable.Rating;
                    app.SearchQuery = dbTable.SearchQuery;
                    app.Screenshots = dbTable.ScreenshotsSum.Split(',').ToList();

                    appDbList.Add(app);
                }

                return appDbList;
            }
        }
    }
}
