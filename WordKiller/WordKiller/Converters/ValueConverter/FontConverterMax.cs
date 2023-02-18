using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WordKiller.Converters.ValueConverter
{
    internal class FontConverterMax : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value != null)
            {
                double scale = ScalingFontSize.Scale("14", double.Parse(value.ToString()));
                double max = double.Parse(parameter.ToString());
                if (scale > max)
                {
                    return max;
                }
                else
                {
                    return scale;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return value;
        }
    }
}
