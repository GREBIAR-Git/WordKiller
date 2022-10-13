using System.Windows;
using System.Windows.Controls;

namespace WordKiller;

static class TitleElements
{
    static DefaultUIElement[] defaultElements;
    public static void SaveTitleUIElements(Grid panel)
    {
        defaultElements = new DefaultUIElement[panel.Children.Count];
        for (int i = 0; i < defaultElements.Length; i++)
        {
            defaultElements[i] = new DefaultUIElement();
        }
        for (int i = 0; i < panel.Children.Count; i++)
        {
            defaultElements[i].Element = panel.Children[i];
            defaultElements[i].Row = Grid.GetRow(panel.Children[i]);
            defaultElements[i].Column = Grid.GetColumn(panel.Children[i]);
        }
    }

    public static void ShowTitleElems(Grid panel, string str)
    {
        panel.Children.Clear();
        PushbackControls(panel);
        ShowAllChildControls(panel);
        ResetAllChildColumnSpans(panel);
        SplitCell(str, out int[] rows, out int[] columns);
        UIElement[] titleSave = CopyControls(panel, rows, columns);
        panel.Children.Clear();
        for (int i = 0; i < titleSave.Length; i++)
        {
            if (columns[i] >= 4 && RowElemCounter(rows, rows[i]) <= 4)
            {
                columns[i] -= 2;
            }
            if (columns[i] >= 2 && RowElemCounter(rows, rows[i]) <= 2)
            {
                columns[i] -= 2;
            }
        }
        PushbackControls(titleSave, panel, rows, columns);
        for (int i = 0; i < panel.Children.Count; i++)
        {
            if (columns[i] == 3 && RowElemCounter(rows, rows[i]) <= 4)
            {
                Grid.SetColumnSpan(panel.Children[i], 3);
            }
            else if (columns[i] == 1 && RowElemCounter(rows, rows[i]) <= 2)
            {
                Grid.SetColumnSpan(panel.Children[i], 5);
            }
        }
    }

    static void PushbackControls(UIElement[] controls, Grid panel, int[] rows, int[] colums)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            panel.Children.Add(controls[i]);
            Grid.SetRow(controls[i], rows[i]);
            Grid.SetColumn(controls[i], colums[i]);
        }
    }

    static void PushbackControls(Grid panel)
    {
        for (int i = 0; i < defaultElements.Length; i++)
        {
            panel.Children.Add(defaultElements[i].Element);
            Grid.SetRow(defaultElements[i].Element, defaultElements[i].Row);
            Grid.SetColumn(defaultElements[i].Element, defaultElements[i].Column);
        }
    }

    static void ShowAllChildControls(Grid panel)
    {
        foreach (UIElement children in panel.Children)
        {
            children.Visibility = Visibility.Visible;
        }
    }

    static void ResetAllChildColumnSpans(Grid panel)
    {
        foreach (UIElement children in panel.Children)
        {
            Grid.SetColumnSpan(children, 1);
        }
    }

    static void SplitCell(string str, out int[] rows, out int[] columns)
    {
        string[] cells;
        if (str != string.Empty)
        {
            cells = str.Split(' ');
        }
        else
        {
            cells = System.Array.Empty<string>();
        }
        columns = new int[cells.Length];
        rows = new int[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            string[] cell = cells[i].Split('.');
            if (cell.Length > 1)
            {
                columns[i] = int.Parse(cell[0]);
                rows[i] = int.Parse(cell[1]);
            }
        }
    }

    static int RowElemCounter(int[] rows, int row)
    {
        int counter = 0;
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] == row)
            {
                counter++;
            }
        }
        return counter;
    }


    static T[] ArrayPushBack<T>(T[] array, T element)
    {
        T[] newArray = new T[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        newArray[^1] = element;
        return newArray;
    }

    static int CheckControlPosition(Grid tableLayoutPanel, int controlIndex, int[] rows, int[] columns)
    {
        if (rows.Length == columns.Length)
        {
            int ctrlToCheckR = Grid.GetRow(tableLayoutPanel.Children[controlIndex]);
            int ctrlToCheckC = Grid.GetColumn(tableLayoutPanel.Children[controlIndex]);
            for (int i = 0; i < rows.Length; i++)
            {
                if (ctrlToCheckR == rows[i] && ctrlToCheckC == columns[i])
                {
                    return i;
                }
            }
        }
        return -1;
    }

    static UIElement[] CopyControls(Grid tableLayoutPanel, int[] rows, int[] columns)
    {
        UIElement[] newArray = System.Array.Empty<UIElement>();
        for (int i = 0; i < tableLayoutPanel.Children.Count; i++)
        {
            if (rows.Length == columns.Length)
            {
                int cellIndex = CheckControlPosition(tableLayoutPanel, i, rows, columns);
                if (cellIndex != -1)
                {
                    newArray = ArrayPushBack(newArray, tableLayoutPanel.Children[i]);

                    int tmpColumn = columns[newArray.Length - 1];
                    int tmpRow = rows[newArray.Length - 1];
                    columns[newArray.Length - 1] = columns[cellIndex];
                    rows[newArray.Length - 1] = rows[cellIndex];
                    columns[cellIndex] = tmpColumn;
                    rows[cellIndex] = tmpRow;
                }
            }
            else
            {
                break;
            }
        }
        return newArray;
    }
}
