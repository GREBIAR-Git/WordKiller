using System;
using WordKiller.Properties;
using WordKiller.Scripts.File;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class TableData
{
    int columns;
    int rows;

    public TableData(int row = 1, int column = 1)
    {
        Rows = row;
        Columns = column;
        DataTable = new string[Settings.Default.MaxRowAndColumn, Settings.Default.MaxRowAndColumn];
    }

    public string[,] DataTable { get; set; }

    public int Rows
    {
        get => rows;
        set
        {
            rows = value;
            SaveHelper.NeedSave = true;
        }
    }

    public int Columns
    {
        get => columns;
        set
        {
            columns = value;
            SaveHelper.NeedSave = true;
        }
    }

    public void SetCell(int row, int column, string data)
    {
        DataTable[row, column] = data;
        SaveHelper.NeedSave = true;
    }
}