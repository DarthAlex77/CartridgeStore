using System.Windows.Input;

namespace CartridgeStore.Views
{
    public partial class UnitsWindow
    {
        public UnitsWindow()
        {
            InitializeComponent();
        }

        private void DeleteFocusedRowOnDelKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && !View.IsEditing)
            {
                View.DeleteRow(View.FocusedRowHandle);
            }
        }
    }
}