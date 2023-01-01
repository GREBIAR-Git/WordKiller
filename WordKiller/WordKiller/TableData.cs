using System.Collections.Generic;
using System.Linq;

namespace WordKiller
{
    internal class TablesData
    {
        public List<TableData> Collection { get; private set; }

        int current;

        public TableData CurrentData
        {
            get => Collection.ElementAt(current);
        }

        public TablesData()
        {
            Collection = new List<TableData>();
            Collection.Add(new TableData());
        }

        public void AddTable()
        {
            current = Collection.Count - 1;
            Collection.Add(new TableData());
        }

        public void SelectTable(int index)
        {
            current = index;
            Collection.ElementAt(index);
        }

        public void UnselectedTable()
        {
            current = Collection.Count - 1;
        }

        public void DeleteTable(int index)
        {
            current = Collection.Count;
            Collection.RemoveAt(index);
        }

        public void SwapTable(int i, int f)
        {
            (Collection[f], Collection[i]) = (Collection[i], Collection[f]);
        }
    }

    class TableData
    {
        public string[,] DataTable { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public TableData(int row = 1, int column = 1)
        {
            Rows = row;
            Columns = column;
            DataTable = new string[Properties.Settings.Default.MaxRowAndColumn, Properties.Settings.Default.MaxRowAndColumn];
        }
    }
}
