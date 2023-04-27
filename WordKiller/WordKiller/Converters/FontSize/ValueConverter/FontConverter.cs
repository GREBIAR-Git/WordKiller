using System;
using System.Globalization;
using System.Windows.Data;

namespace WordKiller.Converters.ValueConverter;

[ValueConversion(typeof(double), typeof(string))]
public class FontConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        if (value != null)
        {
            return ScalingFontSize.Scale(parameter.ToString(), double.Parse(value.ToString()));
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        return value;
    }
}
