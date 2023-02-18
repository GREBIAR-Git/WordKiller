using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WordKiller.Converters.MultiValueConverter
{
    public class AdaptiveFontSizeTextBlock : IMultiValueConverter
    {
        // values[0] - TextBlock; values[1] - Text/Content; values[2] - currentFontSize;
        // values[3] - ActualWidth; values[4] - ActualHeight
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TextBlock control = (TextBlock)values[0];

            if (control == null)
            {
                return double.NaN;
            }
            var dpiX = 96.0 * VisualTreeHelper.GetDpi(control).DpiScaleX;
            FormattedText formattedText;
            try
            {
                formattedText =
                    new FormattedText(
                        values[1].ToString(),
                        CultureInfo.InvariantCulture,
                        control.FlowDirection,
                        new Typeface(
                            control.FontFamily,
                            control.FontStyle,
                            control.FontWeight,
                            control.FontStretch),
                        control.FontSize,
                        control.Foreground,
                        dpiX);
            }
            catch
            {
                return 0;
            }

            double fontSize = control.FontSize * control.ActualWidth / formattedText.Width;

            if (values[2] == null) return ScalingFontSize.Scale(parameter.ToString(), fontSize);
            var maxSize = double.Parse(values[2].ToString());
            return ScalingFontSize.Scale(parameter.ToString(), Math.Min(fontSize, maxSize));
        }



        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
