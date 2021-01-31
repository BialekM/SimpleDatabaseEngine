using NUnit.Framework;
using SimpleDatabaseEngine;
using System.Collections.Generic;

namespace SimpleDatabaseEngineTests
{
    public class DataTableTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateCorrectTable()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            Assert.AreEqual(4, dataTable.Columns.Count);
            Assert.AreEqual("Pesel", dataTable.PrimaryKeyColumnName);
            Assert.AreEqual(0, dataTable.Rows.Count);
        }

        [Test]
        public void InsertCorrectRowToTable()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            var isInsertCorrectly = dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25" });
            
            Assert.AreEqual(true, isInsertCorrectly);
            Assert.AreEqual(1, dataTable.Rows.Count);
            Assert.AreEqual("Michal", dataTable.Rows["95011911554"][0]);
            Assert.AreEqual("Student", dataTable.Rows["95011911554"][1]);
            Assert.AreEqual("25", dataTable.Rows["95011911554"][2]);
        }

        [Test]
        public void InsertRowWithWrongNumberOfPArameters()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            var isInsertCorrectly = dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25", "error" });
            Assert.AreEqual(false, isInsertCorrectly);
        }

        [Test]
        public void TryInsertRowWithDuplicateKey()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25" });
            var isInsertCorrectly=  dataTable.TryInsertRow("95011911554", new List<string>() { "Pioter", "Kurla", "23" });
            Assert.AreEqual(false, isInsertCorrectly);
        }

        [Test]
        public void DeleteExistingRowFromTable()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25" });
            var isCorrectlyRemoved = dataTable.DeleteRow("95011911554");

            Assert.AreEqual(true, isCorrectlyRemoved);
            Assert.AreEqual(0, dataTable.Rows.Count);
        }

        [Test]
        public void DeleteNotExistingRowFromTable()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            var isCorrectlyRemoved = dataTable.DeleteRow("NotExistingKey");

            Assert.AreEqual(false, isCorrectlyRemoved);
        }

        [Test]
        public void ModifyRowWithCorrectParameters()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25" });
            var isCorrectlyModified = dataTable.TryModifyRow("95011911554", "Age", "20");

            Assert.AreEqual(true, isCorrectlyModified);
            Assert.AreEqual("20", dataTable.Rows["95011911554"][2]);
        }

        [Test]
        public void TryModifyRowWithIncorrectColumnName()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25" });
            var isCorrectlyModified = dataTable.TryModifyRow("95011911554", "IncorrectColumnName", "20");

            Assert.AreEqual(false, isCorrectlyModified);
        }

        [Test]
        public void UpdatetPrimaryKey()
        {
            DataTable dataTable = new DataTable("People", new List<string>() { "Pesel", "Name", "Surname", "Age" });
            dataTable.TryInsertRow("95011911554", new List<string>() { "Michal", "Student", "25" });
            var isCorrectlyModified = dataTable.TryModifyRow("95011911554", "Pesel", "123123198237192378");

            Assert.AreEqual(true, isCorrectlyModified);
            List<string> emptyList = new List<string>();
            Assert.AreEqual(false, dataTable.Rows.TryGetValue("95011911554", out emptyList));
            Assert.AreEqual("Michal", dataTable.Rows["123123198237192378"][0]);
            Assert.AreEqual("Student", dataTable.Rows["123123198237192378"][1]);
            Assert.AreEqual("25", dataTable.Rows["123123198237192378"][2]);
        }

    }
}