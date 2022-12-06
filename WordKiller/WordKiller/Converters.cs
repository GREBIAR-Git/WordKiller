using System;
using System.Windows.Data;

namespace WordKiller
{
    [ValueConversion(typeof(double), typeof(string))]
    public class FontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return (int)(double.Parse(parameter.ToString()) / 16 * (double.Parse(value.ToString()) + 10));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}


