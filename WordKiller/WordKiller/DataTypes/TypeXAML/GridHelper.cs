using System.Windows;
using System.Windows.Controls;
using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.DataTypes.TypeXAML;

public class GridHelpers
{
    #region Selected Property

    public static readonly DependencyProperty SelectedProperty =
        DependencyProperty.RegisterAttached(
            "Selected", typeof(TableData), typeof(GridHelpers),
            new PropertyMetadata(null, SelectedChanged));

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
            new PropertyMetadata(-1, RowCountChanged));

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
        if (!(obj is Grid) || (int)e.NewValue < 0)
            return;
        Grid grid = (Grid)obj;

        grid.RowDefinitions.Clear();
        for (int i = 0; i < (int)e.NewValue; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, type: GridUnitType.Star) });
        }
        UpdateTable(grid);
    }

    #endregion

    #region ColumnCount Property

    public static readonly DependencyProperty ColumnCountProperty =
        DependencyProperty.RegisterAttached(
            "ColumnCount", typeof(int), typeof(GridHelpers),
            new PropertyMetadata(-1, ColumnCountChanged));

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
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, type: GridUnitType.Star) });
        }
        UpdateTable(grid);
    }

    #endregion

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
                        Style = Application.Current.FindResource("TextBoxTable") as Style,
                    };
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
}
