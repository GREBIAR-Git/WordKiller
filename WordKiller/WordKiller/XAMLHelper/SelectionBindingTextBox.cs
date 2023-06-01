using System.Windows;
using System.Windows.Controls;

namespace WordKiller.XAMLHelper;

public class SelectionBindingTextBox : TextBox
{
    public static readonly DependencyProperty BindableSelectionStartProperty =
        DependencyProperty.Register(
        "BindableSelectionStart",
        typeof(int),
        typeof(SelectionBindingTextBox),
        new PropertyMetadata(OnBindableSelectionStartChanged));

    public static readonly DependencyProperty BindableSelectionLengthProperty =
        DependencyProperty.Register(
        "BindableSelectionLength",
        typeof(int),
        typeof(SelectionBindingTextBox),
        new PropertyMetadata(OnBindableSelectionLengthChanged));

    private bool changeFromUI;

    public SelectionBindingTextBox() : base()
    {
        SelectionChanged += OnSelectionChanged;
    }

    public int BindableSelectionStart
    {
        get
        {
            return (int)GetValue(BindableSelectionStartProperty);
        }

        set
        {
            SetValue(BindableSelectionStartProperty, value);
        }
    }

    public int BindableSelectionLength
    {
        get
        {
            return (int)GetValue(BindableSelectionLengthProperty);
        }

        set
        {
            SetValue(BindableSelectionLengthProperty, value);
        }
    }

    private static void OnBindableSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var textBox = dependencyObject as SelectionBindingTextBox;

        if (!textBox.changeFromUI)
        {
            int newValue = (int)args.NewValue;
            textBox.SelectionStart = newValue;
        }
        else
        {
            textBox.changeFromUI = false;
        }
    }

    private static void OnBindableSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var textBox = dependencyObject as SelectionBindingTextBox;

        if (!textBox.changeFromUI)
        {
            int newValue = (int)args.NewValue;
            textBox.SelectionLength = newValue;
        }
        else
        {
            textBox.changeFromUI = false;
        }
    }

    private void OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (BindableSelectionStart != SelectionStart)
        {
            changeFromUI = true;
            BindableSelectionStart = SelectionStart;
        }

        if (BindableSelectionLength != SelectionLength)
        {
            changeFromUI = true;
            BindableSelectionLength = SelectionLength;
        }
    }
}
