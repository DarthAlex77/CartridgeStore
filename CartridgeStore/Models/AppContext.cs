using System;
using System.Configuration;
using System.Data.Entity;
using CartridgeStore.Properties;
using DevExpress.Xpf.Core;

namespace CartridgeStore.Models
{
    public class AppContext : DbContext
    {
        public AppContext() : base(ConnectionString())
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AppContext>());
        }

        public DbSet<Cartridge> Cartridges { get; set; }
        public DbSet<Story>     Stories    { get; set; }
        public DbSet<Unit>      Units      { get; set; }

        public static event Action DbSetChanged;

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception e)
            {
                DXMessageBox.Show(e.GetBaseException().Message);
            }
            finally
            {
                OnDbSetChanged();
            }

            return 0;
        }

        private static string ConnectionString()
        {
            if (Settings.Default.UseLocalDb)
            {
                return ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
            }

            return ConfigurationManager.ConnectionStrings["CartridgeStoreDB"].ConnectionString;
        }

        public static void OnDbSetChanged()
        {
            DbSetChanged?.Invoke();
        }
    }
}