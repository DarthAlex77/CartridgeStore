using System.Data.Entity;
using System.Threading;
using CartridgeStore.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;

namespace CartridgeStore.ViewModels
{
    public class StoryViewModel : ViewModelBase
    {
        public StoryViewModel()
        {
            Stories = new DXObservableCollection<Story>();
            Refresh();
            AppContext.DbSetChanged += Refresh;
        }

        public DXObservableCollection<Story> Stories { get; set; }

        private void Refresh()
        {
            using (AppContext db = new AppContext())
            {
                Stories.Clear();
                Thread.Sleep(50);
                Stories.AddRange(db.Stories.Include(x => x.Cartridges).Include(x => x.Unit));
            }
        }
    }
}