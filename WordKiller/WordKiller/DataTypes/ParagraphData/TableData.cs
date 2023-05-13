using System;
using WordKiller.Scripts;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class TableData
{
    public string[,] DataTable { get; set; }

    public void SetCell(int row, int column, string data)
    {
        DataTable[row, column] = data;
        SaveHelper.NeedSave = true;
    }
    int rows;
    public int Rows
    {
        get => rows;
        set
        {
            rows = value;
            SaveHelper.NeedSave = true;
        }
    }

    int columns;
    public int Columns
    {
        get => columns;
        set
        {
            columns = value;
            SaveHelper.NeedSave = true;
        }
    }

    public TableData(int row = 1, int column = 1)
    {
        Rows = row;
        Columns = column;
        DataTable = new string[Properties.Settings.Default.MaxRowAndColumn, Properties.Settings.Default.MaxRowAndColumn];
    }
}
