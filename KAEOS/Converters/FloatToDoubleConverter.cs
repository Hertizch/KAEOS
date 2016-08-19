using System;
using System.Globalization;
using System.Windows.Data;

namespace KAEOS.Converters
{
    public class FloatToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            double output;
            return double.TryParse(value.ToString(), out output) ? Math.Truncate(output) : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
