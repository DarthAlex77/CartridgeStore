using System.Collections.Generic;
using System.Linq;
using CartridgeStore.Models;
using DevExpress.Mvvm;
using AppContext = CartridgeStore.Models.AppContext;

namespace CartridgeStore.ViewModels
{
    public class UnitsViewModel : ViewModelBase
    {
        public UnitsViewModel()
        {
            ApplyUnitsCommand       =  new DelegateCommand(ApplyUnits);
            Units                   =  new List<Unit>();
            AppContext.DbSetChanged += Refresh;
            Refresh();
        }

        public bool CloseWindowTrigger
        {
            get { return GetProperty(() => CloseWindowTrigger); }
            set { SetProperty(() => CloseWindowTrigger, value); }
        }

        public DelegateCommand ApplyUnitsCommand { get; set; }
        public List<Unit>      Units             { get; set; }

        private void ApplyUnits()
        {
            using (AppContext db = new AppContext())
            {
                if (Units.Count > db.Units.Count())
                {
                    db.Units.AddRange(Units.Except(db.Units.ToList(), new UnitComparer()).ToList());
                }
                else if (Units.Count < db.Units.Count())
                {
                    db.Units.RemoveRange(db.Units.ToList().Except(Units, new UnitComparer()).ToList());
                }

                db.SaveChanges();
            }

            CloseWindowTrigger = true;
        }

        private void Refresh()
        {
            using (AppContext db = new AppContext())
            {
                Units.Clear();
                Units.AddRange(db.Units);
            }
        }
    }
}