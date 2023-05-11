using System;
using System.Windows.Data;
using WordKiller.Scripts.ForUI;

namespace WordKiller.Converters
{
    public class DocumentTypeLanguage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return UIHelper.FindResourse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value as string;
        }
    }
}
