using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using CartridgeStore.Helpers;
using CartridgeStore.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using AppContext = CartridgeStore.Models.AppContext;

namespace CartridgeStore.ViewModels
{
    public class SupplyViewModel : ViewModelBase
    {
        public SupplyViewModel()
        {
            Cartridges = new List<Cartridge>();
            Refresh();
            AppContext.DbSetChanged += Refresh;
            IsAutoEnterEnabled      =  true;
            IsSingleModeEnabled     =  true;
            NewCartridges           =  new DXObservableCollection<Cartridge>();
            ValidateRowCommand      =  new DelegateCommand<GridRowValidationEventArgs>(ValidateRow);
            AutoFillCommand         =  new DelegateCommand<CellValueChangedEventArgs>(AutoFill);
            AddCartridgesCommand    =  new DelegateCommand(AddCartridges, CanAddCartridges);
            CloseWindowCommand      =  new DelegateCommand<CancelEventArgs>(CloseWindow);
        }

        public bool IsSingleModeEnabled
        {
            get { return GetProperty(() => IsSingleModeEnabled); }
            set { SetProperty(() => IsSingleModeEnabled, value, IsSingleModeEnabledChanged); }
        }

        public bool IsAutoEnterEnabled
        {
            get { return GetProperty(() => IsAutoEnterEnabled); }
            set { SetProperty(() => IsAutoEnterEnabled, value, IsAutoEnterEnabledChanged); }
        }

        public DelegateCommand<GridRowValidationEventArgs> ValidateRowCommand   { get; set; }
        public DelegateCommand<CancelEventArgs>            CloseWindowCommand   { get; set; }
        public DelegateCommand<CellValueChangedEventArgs>  AutoFillCommand      { get; set; }
        public DelegateCommand                             AddCartridgesCommand { get; set; }
        public List<Cartridge>                             Cartridges           { get; set; }
        public DXObservableCollection<Cartridge>           NewCartridges        { get; set; }

        public bool IsValid
        {
            get { return GetProperty(() => IsValid); }
            set { SetProperty(() => IsValid, value); }
        }

        public bool CloseWindowTrigger
        {
            get { return GetProperty(() => CloseWindowTrigger); }
            set { SetProperty(() => CloseWindowTrigger, value); }
        }

        private bool CanAddCartridges()
        {
            return NewCartridges.Any() && IsValid;
        }

        private void Refresh()
        {
            using (AppContext db = new AppContext())
            {
                Cartridges.Clear();
                Cartridges.AddRange(db.Cartridges);
            }
        }

        private void ValidateRow(GridRowValidationEventArgs e)
        {
            if (e.Row != null)
            {
                IsValid = e.IsValid;
                bool      validBarCode = true;
                bool      validOthers  = true;
                Cartridge row          = (Cartridge) e.Row;
                if (!row.IsInitialized())
                {
                    validOthers    = false;
                }

                if (validOthers && !BarCodeHelper.IsValidBarCode(row.BarCode))
                {
                    validBarCode = DXMessageBox.Show("Штрих код не стандартный, принять принудительно?", "Принять", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
                }

                e.IsValid = validBarCode && validOthers;
                IsValid   = e.IsValid;
                if (e.IsValid && NewCartridges.Count > 1)
                {
                    List<Cartridge> temp = NewCartridges.GroupBy(x => x.BarCode).Select(x => new Cartridge
                    {
                            BarCode      = x.Key,
                            CartridgeId  = x.First().CartridgeId,
                            Manufacturer = x.First().Manufacturer,
                            Model        = x.First().Model,
                            Qty          = x.Sum(y => y.Qty)
                    }).ToList();
                    NewCartridges.Clear();
                    Thread.Sleep(50);
                    NewCartridges.AddRange(temp);
                }
            }
        }

        private void AutoFill(CellValueChangedEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.RowHandle == DataControlBase.NewItemRowHandle && e.Column.FieldName == "BarCode")
                {
                    if (IsSingleModeEnabled)
                    {
                        e.Source.Grid.SetCellValue(e.RowHandle, "Qty", 1);
                    }

                    string    barcode = e.Value.ToString();
                    Cartridge x       = (Cartridge) e.Row;
                    foreach (Cartridge nc in NewCartridges)
                    {
                        if (!ReferenceEquals(x, nc) && x.BarCode.Equals(nc.BarCode))
                        {
                            e.Source.Grid.SetCellValue(e.RowHandle, "Manufacturer", nc.Manufacturer);
                            e.Source.Grid.SetCellValue(e.RowHandle, "Model", nc.Model);
                            return;
                        }
                    }

                    foreach (Cartridge c in Cartridges)
                    {
                        if (barcode.Equals(c.BarCode) && Cartridges.Any())
                        {
                            e.Source.Grid.SetCellValue(e.RowHandle, "Manufacturer", c.Manufacturer);
                            e.Source.Grid.SetCellValue(e.RowHandle, "Model", c.Model);
                            return;
                        }
                    }
                }

                if (IsAutoEnterEnabled)
                {
                    if (e.Source.Grid.IsValidRowHandle(DataControlBase.NewItemRowHandle))
                    {
                        e.Source.Grid.View.FocusedRowHandle = e.Source.Grid.VisibleRowCount;
                        e.Source.CommitEditing();
                    }
                }
            }
        }

        private void AddCartridges()
        {
            using (AppContext db = new AppContext())
            {
                foreach (Cartridge newCartridge in NewCartridges)
                {
                    if (!db.Cartridges.Any())
                    {
                        db.Cartridges.Add(newCartridge);
                    }
                    else
                    {
                        Cartridge tmp = db.Cartridges.SingleOrDefault(x => x.BarCode == newCartridge.BarCode);
                        if (tmp != null)
                        {
                            tmp.Qty += newCartridge.Qty;
                        }
                        else
                        {
                            db.Cartridges.Add(newCartridge);
                        }
                    }
                }

                db.Stories.Add(new Story(DateTime.Now, true, null, NewCartridges.ToList()));
                db.SaveChanges();
                NewCartridges.Clear();
                CloseWindowTrigger = true;
            }
        }

        private void CloseWindow(CancelEventArgs e)
        {
            if (NewCartridges.Any())
            {
                e.Cancel = DXMessageBox.Show("Вы хотите закрыть это окно без сохранения ?", "Закрыть ?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes;
            }
        }

        private void IsSingleModeEnabledChanged()
        {
            IsAutoEnterEnabled = IsSingleModeEnabled;
        }

        private void IsAutoEnterEnabledChanged()
        {
            if (!IsSingleModeEnabled)
            {
                IsAutoEnterEnabled = false;
            }
        }
    }
}