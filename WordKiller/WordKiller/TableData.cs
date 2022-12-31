using System.Collections.Generic;
using System.Linq;

namespace WordKiller
{
    internal class TablesData
    {
        public List<TableData> collection { get; private set; }

        int current;

        public TableData CurrentData
        {
            get => collection.ElementAt(current);
        }

        public void InitTable()
        {
            collection = new List<TableData>();
            collection.Add(new TableData());
        }

        public void AddTable()
        {
            current = collection.Count - 1;
            collection.Add(new TableData());
        }

        public void SelectTable(int index)
        {
            current = index;
            collection.ElementAt(index);
        }

        public void UnselectedTable()
        {
            current = collection.Count - 1;
        }

        public void DeleteTable(int index)
        {
            current = collection.Count;
            collection.RemoveAt(index);
        }

        public void SwapTable(int i, int f)
        {
            (collection[f], collection[i]) = (collection[i], collection[f]);
        }
    }

    class TableData
    {
        public string[,] DataTable { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public TableData(int row = 1, int column = 1)
        {
            Rows = 1;
            Columns = 1;
            DataTable = new string[Properties.Settings.Default.MaxRowAndColumn, Properties.Settings.Default.MaxRowAndColumn];
        }
    }
}
