﻿using System.Windows;

namespace WordKiller.XAMLHelper;

public static class FocusHelper
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool), typeof(FocusHelper),
            new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

    public static bool GetIsFocused(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsFocusedProperty);
    }

    public static void SetIsFocused(DependencyObject obj, bool value)
    {
        obj.SetValue(IsFocusedProperty, value);
    }

    static void OnIsFocusedPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var uie = (UIElement)d;
        if ((bool)e.NewValue)
        {
            uie.Focus();
        }
    }
}