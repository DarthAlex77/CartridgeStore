using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CartridgeStore.Helpers;
using DevExpress.XtraEditors.DXErrorProvider;

namespace CartridgeStore.Models
{
    public class Cartridge : IDXDataErrorInfo, IEquatable<Cartridge>
    {
        public Cartridge() { }

        public Cartridge(string manufacturer, string model, string barCode, int qty)
        {
            Manufacturer = manufacturer;
            Model        = model;
            BarCode      = barCode;
            Qty          = qty;
        }

        [Display(AutoGenerateField = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CartridgeId { get; set; }

        [Display(Name = "Штрих код")]
        public string BarCode { get; set; }

        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }

        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Display(Name = "Количество")]
        public int Qty { get; set; }

        public void GetPropertyError(string propertyName, ErrorInfo info)
        {
            switch (propertyName)
            {
                case "BarCode":
                {
                    if (!BarCodeHelper.IsValidBarCode(BarCode))
                    {
                        info.ErrorType = ErrorType.Information;
                        info.ErrorText = "Не стандартный штрих код";
                    }

                    if (string.IsNullOrWhiteSpace(BarCode))
                    {
                        info.ErrorType = ErrorType.Critical;
                        info.ErrorText = "Поле Штрих код обязательно для заполнения";
                    }

                    break;
                }
                case "Manufacturer":
                {
                    if (string.IsNullOrWhiteSpace(Manufacturer))
                    {
                        info.ErrorType = ErrorType.Critical;
                        info.ErrorText = "Поле Производитель обязательно для заполнения";
                    }

                    break;
                }
                case "Model":
                {
                    if (string.IsNullOrWhiteSpace(Model))
                    {
                        info.ErrorType = ErrorType.Critical;
                        info.ErrorText = "Поле Модель обязательно для заполнения";
                    }

                    break;
                }
                case "Qty":
                {
                    if (Qty<1)
                    {
                        info.ErrorType = ErrorType.Critical;
                        info.ErrorText = "Количество не должно быть меньше 1";
                    }
                    break;
                }
            }
        }

        public void GetError(ErrorInfo info) { }

        public bool Equals(Cartridge other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(BarCode, other.BarCode, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Manufacturer, other.Manufacturer, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Model, other.Model, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsInitialized()
        {
            return !string.IsNullOrWhiteSpace(Manufacturer) && !string.IsNullOrWhiteSpace(Model) && !string.IsNullOrWhiteSpace(BarCode) && Qty > 0;
        }
    }

    public class ExcludedCartridge
    {
        public ExcludedCartridge(string barCode, string manufacturer, string model, int qty)
        {
            BarCode      = barCode;
            Manufacturer = manufacturer;
            Model        = model;
            Qty          = qty;
        }

        public ExcludedCartridge() { }

        [Key]
        [Display(AutoGenerateField = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CartridgeId { get; set; }

        [Display(Name = "Штрих код")]
        public string BarCode { get; set; }

        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }

        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Display(Name = "Количество")]
        public int Qty { get; set; }
    }
}