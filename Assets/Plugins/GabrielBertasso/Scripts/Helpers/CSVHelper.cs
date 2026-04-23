using System.Collections.Generic;
using System.IO;

namespace GabrielBertasso.Helpers
{
    public static class CSVHelper
    {
        public static string[,] ConvertToArray(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File not found! Path: \"" + path + "\".");
                return null;
            }

            string[] rows = File.ReadAllLines(path);
            int rowCount = rows.Length;

            List<string[]> columnsPerRow = new List<string[]>();
            int columnCount = 0;
            foreach (string row in rows)
            {
                string[] columns = row.Split(",");
                columnsPerRow.Add(columns);

                if (columns.Length > columnCount)
                {
                    columnCount = columns.Length;
                }
            }

            string[,] array = new string[columnCount, rowCount];
            for (int c = 0; c < columnCount; c++)
            {
                for (int r = 0; r < rowCount; r++)
                {
                    string[] columns = columnsPerRow[r];
                    array[c, r] = columns.Length > c ? columns[c] : string.Empty;
                }
            }

            return array;
        }

        public static bool FindCellWithIds(string[,] array, string columnId, string rowId, out string cell)
        {
            cell = string.Empty;

            if (!FindColumnIndexWithId(array, columnId, out int columnIndex))
            {
                return false;
            }

            if (!FindRowIndexWithId(array, rowId, out int rowIndex))
            {
                return false;
            }

            cell = array[columnIndex, rowIndex];
            return true;
        }

        public static bool FindColumnIndexWithId(string[,] array, string id, out int index)
        {
            for (int r = 0; r < array.GetLength(1); r++)
            {
                for (int c = 0; c < array.GetLength(0); c++)
                {
                    string content = array[c, r];
                    if (content == id)
                    {
                        index = c;
                        return true;
                    }
                }
            }

            index = 0;
            return false;
        }

        public static bool FindRowIndexWithId(string[,] array, string id, out int index)
        {
            for (int c = 0; c < array.GetLength(0); c++)
            {
                for (int r = 0; r < array.GetLength(1); r++)
                {
                    string content = array[c, r];
                    if (content == id)
                    {
                        index = r;
                        return true;
                    }
                }
            }

            index = 0;
            return false;
        }
    }
}