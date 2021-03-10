using System.Configuration;
using System.Data.SqlClient;
using CartridgeStore.Helpers;
using CartridgeStore.Models;
using CartridgeStore.Properties;
using DevExpress.Mvvm;

namespace CartridgeStore.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            ApplySettingsCommand = new DelegateCommand(ApplySettings);
            UseLocalDB           = Checkers.IsLocalDBInstalled() && Settings.Default.UseLocalDb;
            EncryptAppConfig     = Settings.Default.EncryptAppConfig;
            Initialize();
        }

        private void Initialize()
        {
            SqlConnectionStringBuilder builder;
            if (UseLocalDB)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
                builder = new SqlConnectionStringBuilder(connectionString);
            }
            else
            {
                string connectionString = ConfigurationManager.ConnectionStrings["CartridgeStoreDB"].ConnectionString;
                builder = new SqlConnectionStringBuilder(connectionString);
            }

            IntegratedSecurity = builder.IntegratedSecurity;
            DataSource         = builder.DataSource;
            InitialCatalog     = builder.InitialCatalog;
            UserID             = builder.UserID;
            Password           = builder.Password;
        }

        private void ApplySettings()
        {
            bool settingApplied          = false;
            bool configProtectionChanged = false;

            #region ApplySettings

            if (UseLocalDB)
            {
                Settings.Default.UseLocalDb       = UseLocalDB;
                Settings.Default.EncryptAppConfig = EncryptAppConfig;
                Settings.Default.Save();
                settingApplied = true;
            }
            else
            {
                Settings.Default.UseLocalDb       = UseLocalDB;
                Settings.Default.EncryptAppConfig = EncryptAppConfig;
                Settings.Default.Save();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                        DataSource         = DataSource,
                        InitialCatalog     = InitialCatalog,
                        IntegratedSecurity = IntegratedSecurity,
                        Password           = Password,
                        UserID             = UserID
                };
                if (Checkers.IsServerConnected(builder.ToString()))
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.ConnectionStrings.ConnectionStrings["CartridgeStoreDB"].ConnectionString = builder.ToString();
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                    settingApplied = true;
                }
            }

            #endregion

            #region Change Protection for App.config

            if (EncryptAppConfig)
            {
                configProtectionChanged = AppConfigProtectionHelper.ProtectConfiguration();
            }
            else
            {
                configProtectionChanged = AppConfigProtectionHelper.UnProtectConfiguration();
            }

            #endregion

            CloseWindowTrigger = settingApplied && configProtectionChanged;
            if (CloseWindowTrigger)
            {
                AppContext.OnDbSetChanged();
            }
        }

        #region Properties

        public DelegateCommand ApplySettingsCommand { get; set; }

        public bool EncryptAppConfig
        {
            get { return GetProperty(() => EncryptAppConfig); }
            set { SetProperty(() => EncryptAppConfig, value); }
        }

        public bool IntegratedSecurity
        {
            get { return GetProperty(() => IntegratedSecurity); }
            set { SetProperty(() => IntegratedSecurity, value); }
        }

        public string DataSource
        {
            get { return GetProperty(() => DataSource); }
            set { SetProperty(() => DataSource, value); }
        }

        public string InitialCatalog
        {
            get { return GetProperty(() => InitialCatalog); }
            set { SetProperty(() => InitialCatalog, value); }
        }

        public string UserID
        {
            get { return GetProperty(() => UserID); }
            set { SetProperty(() => UserID, value); }
        }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value); }
        }

        public bool UseLocalDB
        {
            get { return GetProperty(() => UseLocalDB); }
            set { SetProperty(() => UseLocalDB, value, Initialize); }
        }

        public bool CloseWindowTrigger
        {
            get { return GetProperty(() => CloseWindowTrigger); }
            set { SetProperty(() => CloseWindowTrigger, value); }
        }

        #endregion
    }
}