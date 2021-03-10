using System.Configuration;
using DevExpress.Xpf.Core;

namespace CartridgeStore.Helpers
{
    public static class AppConfigProtectionHelper
    {
        public static bool ProtectConfiguration()
        {
            Configuration        config      = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            const string         provider    = "RsaProtectedConfigurationProvider";
            ConfigurationSection connStrings = config.ConnectionStrings;
            if (connStrings != null)
            {
                if (!connStrings.SectionInformation.IsProtected)
                {
                    if (!connStrings.ElementInformation.IsLocked)
                    {
                        connStrings.SectionInformation.ProtectSection(provider);
                        connStrings.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                        return true;
                    }

                    DXMessageBox.Show($"Can't protect, section {connStrings.SectionInformation.Name} is locked");
                    return false;
                }

                return true;
            }

            DXMessageBox.Show("Can't get the connection strings section");
            return false;
        }

        public static bool UnProtectConfiguration()
        {
            Configuration        config      = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection connStrings = config.ConnectionStrings;
            if (connStrings != null)
            {
                if (connStrings.SectionInformation.IsProtected)
                {
                    if (!connStrings.ElementInformation.IsLocked)
                    {
                        connStrings.SectionInformation.UnprotectSection();
                        connStrings.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                        return true;
                    }

                    DXMessageBox.Show($"Can't unprotect, section {connStrings.SectionInformation.Name} is locked");
                    return false;
                }

                return true;
            }

            DXMessageBox.Show("Can't get the connection strings section");
            return false;
        }
    }
}