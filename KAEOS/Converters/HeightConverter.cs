using System;
using System.Globalization;
using System.Windows.Data;

namespace KAEOS.Converters
{
    public class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var margin = double.Parse(parameter.ToString());
            var height = (double)value;

            return height + margin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
