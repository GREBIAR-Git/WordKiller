using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.Properties;

namespace WordKiller.XAMLHelper;

public class GridHelpers
{
    static void UpdateTable(Grid grid)
    {
        grid.Children.Clear();
        TableData tableData = GetSelected(grid);
        if (tableData != null)
        {
            for (int i = 0; i < GetRowCount(grid); i++)
            {
                for (int f = 0; f < GetColumnCount(grid); f++)
                {
                    TextBox textBox = new()
                    {
                        Text = tableData.DataTable[i, f],
                        TextWrapping = TextWrapping.Wrap,
                        AcceptsReturn = true,
                        Style = Application.Current.FindResource("TextBoxTable") as Style
                    };
                    textBox.Language = XmlLanguage.GetLanguage("ru-RU");
                    textBox.SpellCheck.IsEnabled = Settings.Default.SyntaxChecking;
                    textBox.TextChanged += Cell_TextChanged;
                    grid.Children.Add(textBox);
                    Grid.SetColumn(textBox, f);
                    Grid.SetRow(textBox, i);
                }
            }
        }
    }

    static void Cell_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        int row = Grid.GetRow(textBox);
        int column = Grid.GetColumn(textBox);
        GetSelected(textBox.Parent).SetCell(row, column, textBox.Text);
    }

    #region Selected Property

    public static readonly DependencyProperty SelectedProperty =
        DependencyProperty.RegisterAttached(
            "Selected", typeof(TableData), typeof(GridHelpers),
            new(null, SelectedChanged));

    public static TableData GetSelected(DependencyObject obj)
    {
        return (TableData)obj.GetValue(SelectedProperty);
    }

    public static void SetSelected(DependencyObject obj, TableData value)
    {
        obj.SetValue(SelectedProperty, value);
    }

    public static void SelectedChanged(
        DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is Grid grid)
        {
            UpdateTable(grid);
        }
    }

    #endregion

    #region RowCount Property

    public static readonly DependencyProperty RowCountProperty =
        DependencyProperty.RegisterAttached(
            "RowCount", typeof(int), typeof(GridHelpers),
            new(-1, RowCountChanged));

    public static int GetRowCount(DependencyObject obj)
    {
        return (int)obj.GetValue(RowCountProperty);
    }

    public static void SetRowCount(DependencyObject obj, int value)
    {
        obj.SetValue(RowCountProperty, value);
    }

    public static void RowCountChanged(
        DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is not Grid grid || (int)e.NewValue < 0)
            return;

        grid.RowDefinitions.Clear();
        for (int i = 0; i < (int)e.NewValue; i++)
        {
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });
        }

        UpdateTable(grid);
    }

    #endregion

    #region ColumnCount Property

    public static readonly DependencyProperty ColumnCountProperty =
        DependencyProperty.RegisterAttached(
            "ColumnCount", typeof(int), typeof(GridHelpers),
            new(-1, ColumnCountChanged));

    public static int GetColumnCount(DependencyObject obj)
    {
        return (int)obj.GetValue(ColumnCountProperty);
    }

    public static void SetColumnCount(DependencyObject obj, int value)
    {
        obj.SetValue(ColumnCountProperty, value);
    }

    public static void ColumnCountChanged(
        DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (!(obj is Grid) || (int)e.NewValue < 0)
            return;

        Grid grid = (Grid)obj;

        grid.ColumnDefinitions.Clear();
        for (int i = 0; i < (int)e.NewValue; i++)
        {
            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
        }

        UpdateTable(grid);
    }

    #endregion
}