using System.Windows;
using System.Windows.Controls;
using WordKiller.DataTypes;

namespace WordKiller.Scripts.ForUI;

static class TitleElements
{
    static ExtendedUIElement[] defaultElements;

    static Grid titlePanel;
    public static void SaveTitleUIElements(Grid panel)
    {
        defaultElements = new ExtendedUIElement[panel.Children.Count];
        for (int i = 0; i < defaultElements.Length; i++)
        {
            defaultElements[i] = new ExtendedUIElement();
        }
        for (int i = 0; i < panel.Children.Count; i++)
        {
            defaultElements[i].Element = panel.Children[i];
            defaultElements[i].Row = Grid.GetRow(panel.Children[i]);
            defaultElements[i].Column = Grid.GetColumn(panel.Children[i]);
        }
        titlePanel = panel;
    }

    public static void ShowTitleElems(string str)
    {
        titlePanel.Children.Clear();
        PushbackControls(titlePanel);
        ShowAllChildControls(titlePanel);
        ResetAllChildColumnSpans(titlePanel);
        SplitCell(str, out int[] rows, out int[] columns);
        UIElement[] titleSave = CopyControls(titlePanel, rows, columns);
        titlePanel.Children.Clear();
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
        PushbackControls(titleSave, titlePanel, rows, columns);
        for (int i = 0; i < titlePanel.Children.Count; i++)
        {
            if (columns[i] == 3 && RowElemCounter(rows, rows[i]) <= 4)
            {
                Grid.SetColumnSpan(titlePanel.Children[i], 3);
            }
            else if (columns[i] == 1 && RowElemCounter(rows, rows[i]) <= 2)
            {
                Grid.SetColumnSpan(titlePanel.Children[i], 5);
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

    public static void UpdateProfessorComboBox(ComboBox professorComboBox, ComboBox facultyComboBox)
    {
        string str = string.Empty;
        professorComboBox.Items.Clear();
        if (facultyComboBox.SelectedIndex == 0)
        {
            str = "Амелина О.В.!Артёмов А.В.!Валухов В.А.!Волков В.Н.!Гордиенко А.П.!Демидов А.В.!Загородних Н.А.!Захарова О.В.!Конюхова О.В.!Корнаева Е.П.!Королева А.К.!Короткий А.В.!Коськин А.В.!Кравцова Э.А.!Лукьянов П.В.!Лунёв Р.А.!Лыськов О.Э.!Машкова А.Л.!Митин А.А.!Новиков С.В.!Новикова Е.В.!Олькина Е.В.!Преснецова В.Ю.!Раков В.И.!Рыженков Д.В.!Савина О.А.!Санников Д.П.!Сезонов Д.С.!Соков О.А.!Стычук А.А.!Терентьев С.В.!Ужаринский А.Ю.!Фроленкова Л.Ю.!Фролов А.И.!Фролова В.А.!Чижов А.В.!Шатеев Р.В.";
        }
        else if (facultyComboBox.SelectedIndex == 1)
        {
            str = "Бондарева Л.А.!Дрёмин В.В.!Дунаев А.В.!Жидков А.В.!Козлов И.О.!Козлова Л.Д.!Марков В.В.!Матюхин С.И.!Незнанов А.И.!Подмастерьев К.В.!Секаева Ж.А.!Селихов А.В.!Углова Н.В.!Шуплецов В.В.!Яковенко М.В.";
        }
        else if (facultyComboBox.SelectedIndex == 2)
        {
            str = "Белевская Ю.А.!Ерёменко В.Т.!Мишин Д.С.!Пеньков Н.Г!Савва Ю.Б.!Фисун А.П.";
        }
        else if (facultyComboBox.SelectedIndex == 3)
        {
            str = "Батуров Д.П.!Гордон В.А.!Кирсанова О.В.!Матюхин С.И.!Потураева Т.В.!Ромашин С.Н.!Семёнова Г.А.!Фроленкова Л.Ю.!Якушина С.И.";
        }
        else if (facultyComboBox.SelectedIndex == 4)
        {
            str = "Аксёнов К.В.!Багров В.В.!Батенков А.А.!Варгашкин В.Я.!Власова М.А.!Воронина О.А.!Донцов В.М.!Косчинский С.Л.!Лобанова В.А.!Лобода О.А.!Майоров М.В.!Мишин В.В.!Муравьёв А.А.!Плащенков Д.А.!Рязанцев П.Н.!Селихов А.В.!Суздальцев А.И.!Тугарев А.С.!Тютякин А.В.!Филина А.В.";
        }
        else if (facultyComboBox.SelectedIndex == 5)
        {
            str = "Аксёнов К.В.!Гладышев А.В.!Качанов А.Н.!Коренков Д.А.!Королева Т.Г.!Петров Г.Н.!Филина А.В.!Чернышов В.А.";
        }
        foreach (string s in str.Split('!'))
        {
            professorComboBox.Items.Add(s);
        }
    }
}
