using System.Windows;
using System.Windows.Controls;

namespace WordKiller.XAMLHelper;

public static class TextBoxHelper
{
    public static readonly DependencyProperty SelectedTextProperty =
        DependencyProperty.RegisterAttached(
            "SelectedText",
            typeof(string),
            typeof(TextBoxHelper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                SelectedTextChanged));

    public static string GetSelectedText(DependencyObject obj)
    {
        return (string)obj.GetValue(SelectedTextProperty);
    }

    public static void SetSelectedText(DependencyObject obj, string value)
    {
        obj.SetValue(SelectedTextProperty, value);
    }

    static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is TextBox tb)
        {
            if (e.OldValue == null && e.NewValue != null)
            {
                tb.SelectionChanged += tb_SelectionChanged;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                tb.SelectionChanged -= tb_SelectionChanged;
            }

            if (e.NewValue is string newValue && newValue != tb.SelectedText)
            {
                tb.SelectedText = newValue;
            }
        }
    }

    static void tb_SelectionChanged(object sender, RoutedEventArgs e)
    {
        TextBox tb = sender as TextBox;
        if (tb != null)
        {
            SetSelectedText(tb, tb.SelectedText);
        }
    }
}