using System;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class TableData
{
    public string[,] DataTable { get; set; }

    public void SetCell(int row, int column, string data)
    {
        DataTable[row, column] = data;
    }

    public int Rows { get; set; }

    public int Columns { get; set; }

    public TableData(int row = 1, int column = 1)
    {
        Rows = row;
        Columns = column;
        DataTable = new string[Properties.Settings.Default.MaxRowAndColumn, Properties.Settings.Default.MaxRowAndColumn];
    }
}
