using System.Windows.Input;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.XtraEditors.DXErrorProvider;

namespace CartridgeStore.Views
{
    public partial class ReleaseWindow
    {
        public ReleaseWindow()
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

        private void Grid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key > Key.D0 && e.Key < Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                if (Grid.CurrentColumn != Grid.Columns.GetColumnByFieldName("Qty"))
                {
                    if (View.FocusedRowHandle != DataControlBase.NewItemRowHandle)
                    {
                        Grid.CurrentColumn    = Grid.Columns.GetColumnByFieldName("BarCode");
                        View.FocusedRowHandle = DataControlBase.NewItemRowHandle;
                    }
                }
            }
        }

        private void View_OnShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (e.Column == Grid.Columns.GetColumnByFieldName("Qty"))
            {
                e.Cancel = false;
            }
            else if (View.FocusedRowHandle != DataControlBase.NewItemRowHandle)
            {
                e.Cancel = true;
            }
        }

        private void View_OnInvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        private void BaseEdit_OnValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value == null)
            {
                e.IsValid      = false;
                e.ErrorType    = ErrorType.Critical;
                e.ErrorContent = "Подразделение должно быть выбрано";
            }
        }
    }
}