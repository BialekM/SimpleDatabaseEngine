using System;
using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class DataTable
    {
        public string Name { get; set; }
        public List<string> Columns { get; set; }
        public SortedDictionary<string, List<string>> Rows;
        public string PrimaryKeyColumnName;

        public DataTable(string name, List<string> columns)
        {
            Name = name;
            Columns = columns;
            Rows = new SortedDictionary<string, List<string>>();

            if (columns != null && columns.Count > 0)
                PrimaryKeyColumnName = columns[0];
        }

        public bool TryInsertRow(string primaryKeyValue, List<string> values)
        {
            if (Rows.ContainsKey(primaryKeyValue))
            {
                Console.WriteLine($"Key {primaryKeyValue} exists in database/n Please type another key value");
                return false;
            }
            if (values.Count != Columns.Count - 1)
            {
                Console.WriteLine($"Incorrect number of parameters/n Actual {values.Count}, Expected {Columns.Count - 1}");
                return false;
            }
            if (values != null)
            {
                Console.WriteLine($"Row added");
                Rows.Add(primaryKeyValue, values);
                return true;
            }
            return false;
        }

        public bool DeleteRow(string primaryKeyValue)
        {
            var isDeleted = Rows.Remove(primaryKeyValue);

            if (isDeleted)
                Console.WriteLine("Row not exist");
            else
                Console.WriteLine("Row deleted");

            return isDeleted;
        }

        public bool TryModifyRow(string primaryKeyValue, string columnName, string newValue)
        {
            var columnIndex = GetColumnIndex(columnName);
            
            if (columnIndex == -1)
                return false;

            if (columnName.Equals(PrimaryKeyColumnName))
            {
                return UpdatePrimaryKey(primaryKeyValue, newValue);
            }

            if (Rows.TryGetValue(primaryKeyValue, out var rowsValues))
            {
                rowsValues[columnIndex - 1] = newValue;
                Rows[primaryKeyValue] = rowsValues;
                return true;
            }
            return false;
        }

        private int GetColumnIndex(string columnName)
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i] == columnName)
                    return i;
            }
            return -1;
        }

        private bool UpdatePrimaryKey(string oldPrimaryKey, string newPrimaryKey)
        {
            if (Rows.TryGetValue(oldPrimaryKey, out var rowsValues))
            {
                if (TryInsertRow(newPrimaryKey, rowsValues))
                {
                    Rows.Remove(oldPrimaryKey);
                    Console.WriteLine("Row updated");
                    return true;
                }                   
            }
            Console.WriteLine("Can't update row");
            return false;
        }

    }
}
