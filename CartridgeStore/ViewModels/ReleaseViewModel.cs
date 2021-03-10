using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CartridgeStore.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.XtraEditors.DXErrorProvider;
using AppContext = CartridgeStore.Models.AppContext;

namespace CartridgeStore.ViewModels
{
    public class ReleaseViewModel : ViewModelBase
    {
        public ReleaseViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick     += Timer_Tick;
            timer.Interval =  TimeSpan.FromSeconds(1);
            timer.Start();
            Units                   = new List<Unit>();
            ReleasingCartridges     = new DXObservableCollection<Cartridge>();
            ReleaseCartridgeCommand = new DelegateCommand(ReleaseCartridges, CanReleaseCartridges);
            StopTimerCommand        = new DelegateCommand<RoutedEventArgs>(_ => timer.Stop(), timer.IsEnabled);
            AutoFillCommand         = new DelegateCommand<CellValueChangedEventArgs>(AutoFill);
            ValidateRowCommand      = new DelegateCommand<GridRowValidationEventArgs>(ValidateRow);
            CloseWindowCommand      = new DelegateCommand<CancelEventArgs>(CloseWindow);
            Refresh();
            AppContext.DbSetChanged += Refresh;
        }


        public DateTime DeliveryDateTime
        {
            get { return GetProperty(() => DeliveryDateTime); }
            set { SetProperty(() => DeliveryDateTime, value); }
        }

        public Unit SelectedUnit
        {
            get { return GetProperty(() => SelectedUnit); }
            set { SetProperty(() => SelectedUnit, value); }
        }

        public bool CloseWindowTrigger
        {
            get { return GetProperty(() => CloseWindowTrigger); }
            set { SetProperty(() => CloseWindowTrigger, value); }
        }

        public bool                                        IsValid                 { get; set; }
        public DelegateCommand                             ReleaseCartridgeCommand { get; set; }
        public DelegateCommand<CellValueChangedEventArgs>  AutoFillCommand         { get; set; }
        public DelegateCommand<GridRowValidationEventArgs> ValidateRowCommand      { get; set; }
        public DelegateCommand<CancelEventArgs>            CloseWindowCommand      { get; set; }
        public DelegateCommand<RoutedEventArgs>            StopTimerCommand        { get; set; }
        public DXObservableCollection<Cartridge>           ReleasingCartridges     { get; set; }
        public List<Unit>                                  Units                   { get; set; }

        private bool CanReleaseCartridges()
        {
            return SelectedUnit != null && ReleasingCartridges.Any() && IsValid;
        }

        private void ReleaseCartridges()
        {
            using (AppContext db = new AppContext())
            {
                foreach (Cartridge cartridge in ReleasingCartridges)
                {
                    Cartridge tmp = db.Cartridges.SingleOrDefault(x => x.BarCode == cartridge.BarCode);
                    if (tmp != null)
                    {
                        tmp.Qty -= cartridge.Qty;
                    }
                    else
                    {
                        throw new InvalidOperationException("Картридж не найден");
                    }
                }

                SelectedUnit = db.Units.Find(SelectedUnit.UnitId);
                db.Stories.Add(new Story(DeliveryDateTime, false, SelectedUnit, ReleasingCartridges.ToList()));
                db.SaveChanges();
                ReleasingCartridges.Clear();
                CloseWindowTrigger = true;
            }
        }

        private static void AutoFill(CellValueChangedEventArgs e)
        {
            if (e.Source.Grid.IsValidRowHandle(DataControlBase.NewItemRowHandle))
            {
                e.Source.Grid.View.FocusedRowHandle = e.Source.Grid.VisibleRowCount;
                e.Source.CommitEditing();
            }
        }

        private void Refresh()
        {
            using (AppContext db = new AppContext())
            {
                Units.Clear();
                Units.AddRange(db.Units);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DeliveryDateTime = DateTime.Now;
        }

        private void ValidateRow(GridRowValidationEventArgs e)
        {
            IsValid   = false;
            e.IsValid = false;
            if (e.Row != null)
            {
                Cartridge row = (Cartridge) e.Row;
                using (AppContext db = new AppContext())
                {
                    Cartridge c = db.Cartridges.SingleOrDefault(x => x.BarCode == row.BarCode);
                    if (c == null)
                    {
                        e.IsValid      = false;
                        e.ErrorContent = "Картридж не найден";
                        e.ErrorType    = ErrorType.Critical;
                    }
                    else if (row.Qty > c.Qty)
                    {
                        e.IsValid      = false;
                        e.ErrorContent = "Картриджей больше чем есть";
                        e.ErrorType    = ErrorType.Critical;
                    }
                    else
                    {
                        if (e.RowHandle == DataControlBase.NewItemRowHandle)
                        {
                            ReleasingCartridges.Add(new Cartridge(c.Manufacturer, c.Model, c.BarCode, 1));
                            ReleasingCartridges.Remove(row);
                            e.IsValid = true;
                            IsValid   = true;
                        }
                        else
                        {
                            if (row.Qty <= 0)
                            {
                                e.IsValid      = false;
                                e.ErrorType    = ErrorType.Critical;
                                e.ErrorContent = "Картриджей не должно быть меньше 1";
                            }
                            else
                            {
                                e.IsValid = true;
                                IsValid   = true;
                            }
                        }
                    }
                }

                if (e.IsValid && ReleasingCartridges.Count > 1)
                {
                    List<Cartridge> temp = ReleasingCartridges.GroupBy(x => x.BarCode).Select(x => new Cartridge
                    {
                            BarCode      = x.Key,
                            CartridgeId  = x.First().CartridgeId,
                            Manufacturer = x.First().Manufacturer,
                            Model        = x.First().Model,
                            Qty          = x.Sum(y => y.Qty)
                    }).ToList();
                    ReleasingCartridges.Clear();
                    Thread.Sleep(50);
                    ReleasingCartridges.AddRange(temp);
                }
            }
        }

        private void CloseWindow(CancelEventArgs e)
        {
            if (ReleasingCartridges.Any())
            {
                e.Cancel = DXMessageBox.Show("Вы хотите закрыть это окно без сохранения ?", "Закрыть ?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes;
            }
        }
    }
}