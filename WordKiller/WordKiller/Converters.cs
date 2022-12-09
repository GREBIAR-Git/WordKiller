using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WordKiller
{
    class AdaptiveFontSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var textBox = (TextBox)values[0];
            var dpiX = 96.0 * VisualTreeHelper.GetDpi(textBox).DpiScaleX; // .NET 4.6.2+
            var formattedText =
                new FormattedText(
                    textBox.Text,
                    CultureInfo.InvariantCulture,
                    textBox.FlowDirection,
                    new Typeface(
                        textBox.FontFamily,
                        textBox.FontStyle,
                        textBox.FontWeight,
                        textBox.FontStretch),
                    textBox.FontSize,
                    textBox.Foreground,
                    dpiX);
            var fontSize = textBox.FontSize * textBox.ViewportWidth / formattedText.Width;
            if (parameter == null) return fontSize;
            var maxSize = (double)parameter;
            return Math.Min(fontSize, maxSize);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }


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


