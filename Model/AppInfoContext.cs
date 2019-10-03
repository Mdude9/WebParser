using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebParserTestApp2.Model
{
    public class AppInfoContext : DbContext
    {
        public DbSet<MainTable> MainTable { get; set; }
        public DbSet<GooglePlayTable> GooglePlayTable { get; set; }
        public DbSet<AppStoreTable> AppStoreTable { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=AppsInfo.db");

    }

    // В sqlite хранится другая модель, т.к. List<string> не поддерживается
    public class GooglePlayTable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string IconLink { get; set; }
        public string Rating { get; set; }
        public string SearchQuery { get; set; }
        public string ScreenshotsSum { get; set; }
    }

    // В sqlite хранится другая модель, т.к. List<string> не поддерживается
    public class AppStoreTable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string IconLink { get; set; }
        public string Rating { get; set; }
        public string SearchQuery { get; set; }
        public string ScreenshotsSum { get; set; }
    }

    public class MainTable
    {
        [Key]
        public int Id { get; set; }
        public string SearchQuery { get; set; }

    }

}
