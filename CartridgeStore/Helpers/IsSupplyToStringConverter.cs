using System;
using System.Globalization;
using System.Windows.Data;

namespace CartridgeStore.Helpers
{
    public class IsSupplyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(bool))
            {
                return value;
            }

            return (bool) value ? "Поставка" : "Выдача";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}