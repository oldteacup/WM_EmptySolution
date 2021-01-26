using Common.SQLitelib.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common.SQLitelib
{
    public partial class SQLiteDBContext
    {
        public DbSet<ClipboardData> ClipboardData { get; set; }
        public DbSet<RecycleBin> RecycleBin { get; set; }

    }

    public partial class SQLiteDBContext : DbContext
    {
#if UWP
        public static readonly string ConnectionString = Path.Combine(ApplicationData.Current.RoamingFolder.Path, "Clipboard_9E00BF43D4184246BD0C45D35BFA1C29", "database.db");
#else
        public static readonly string ConnectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clipboard_9E00BF43D4184246BD0C45D35BFA1C29", "database.db");
#endif

        public static SQLiteDBContext Instance { get; } = new SQLiteDBContext();

        //迁移
        public async void MigrateAsync()
        {
            await Instance.Database.MigrateAsync();
        }

        public SQLiteDBContext() : base()
        {
            Database.MigrateAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dir = new FileInfo(ConnectionString).Directory;
            if (!dir.Exists)
            {
                dir.Create();
            }
            optionsBuilder.UseSqlite($"Data Source={ConnectionString}").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }


    }
}
