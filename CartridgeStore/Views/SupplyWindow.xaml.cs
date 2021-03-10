using System.Windows.Input;
using DevExpress.Xpf.Grid;

namespace CartridgeStore.Views
{
    public partial class SupplyWindow
    {
        public SupplyWindow()
        {
            InitializeComponent();
        }

        private void OnInvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        private void DeleteFocusedRowOnDelKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && !View.IsEditing)
            {
                View.DeleteRow(View.FocusedRowHandle);
            }
        }

        private void Grid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key > Key.D0 && e.Key < Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                if (View.FocusedRowHandle != DataControlBase.NewItemRowHandle)
                {
                    Grid.CurrentColumn    = Grid.Columns.GetColumnByFieldName("BarCode");
                    View.FocusedRowHandle = DataControlBase.NewItemRowHandle;
                }
            }
        }

        private void View_OnShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            e.Cancel = View.FocusedRowHandle != DataControlBase.NewItemRowHandle;
        }
    }
}