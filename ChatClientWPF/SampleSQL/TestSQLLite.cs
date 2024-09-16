//using SQLite;
using System;
using System.IO;

namespace ChatClientWPF.SampleSQL
{
    internal class TestSQLLite
    {
        //  private SQLiteConnection db;

        public void TestCreateTable()
        {
            // Get an absolute path to the database file
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");
        }

        public void AddStock(string symbol)
        {
            var stock = new Stock()
            {
                Symbol = symbol
            };
            
            var t = $"{stock.Symbol} == {stock.Id}";
        }

        public void TestQuery()
        {
            // var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

            //foreach (var stock in query)
            //    Console.WriteLine("Stock: " + stock.Symbol);
        }
    }
}
