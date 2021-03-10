using CartridgeStore.Views;
using DevExpress.Xpf.Bars;

namespace CartridgeStore
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DeliveryButton_OnItemClick(object sender, ItemClickEventArgs e)
        {
            new ReleaseWindow().ShowDialog();
        }

        private void SupplyButton_OnItemClick(object sender, ItemClickEventArgs e)
        {
            new SupplyWindow().ShowDialog();
        }

        private void SettingsButton_OnItemClick(object sender, ItemClickEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }
    }
}