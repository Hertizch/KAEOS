using System;
using System.Globalization;
using System.Windows.Data;

namespace KAEOS.Converters
{
    public class BytesToSuffixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double output;
            double.TryParse(value.ToString(), out output);

            return SizeSuffix(output);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        static string SizeSuffix(double value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (Math.Abs(value) < 0.001) { return "0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return $"{adjustedSize:n1} {SizeSuffixes[mag]}";
        }
    }
}
