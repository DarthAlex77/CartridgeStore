using System.Windows;
using CartridgeStore.Helpers;
using DevExpress.Xpf.Core;

namespace CartridgeStore.Views
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void UseLocalDb_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!Checkers.IsLocalDBInstalled())
            {
                UseLocalDb.IsChecked = false;
                DXMessageBox.Show("MS SQL LocalDB не установлена");
            }
        }

        private void ShowUnits_OnClick(object sender, RoutedEventArgs e)
        {
            new UnitsWindow().ShowDialog();
        }
    }
}