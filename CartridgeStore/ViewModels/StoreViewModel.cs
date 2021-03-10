using System.Threading;
using CartridgeStore.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;

namespace CartridgeStore.ViewModels
{
    public class StoreViewModel : ViewModelBase
    {
        public StoreViewModel()
        {
            Cartridges = new DXObservableCollection<Cartridge>();
            Refresh();
            AppContext.DbSetChanged += Refresh;
        }

        public DXObservableCollection<Cartridge> Cartridges { get; set; }

        private void Refresh()
        {
            using (AppContext db = new AppContext())
            {
                Cartridges.Clear();
                Thread.Sleep(50);
                Cartridges.AddRange(db.Cartridges);
            }
        }
    }
}