﻿using System;
using System.Globalization;
using System.Windows.Data;
using WordKiller.Scripts;

namespace WordKiller.Converters;

public class DocumentTypeLanguage : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return UIHelper.FindResourse(value.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value as string;
    }
}