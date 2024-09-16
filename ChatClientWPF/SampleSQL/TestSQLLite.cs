using CommonLibraryStandart.Other;
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


            //var db = new SQLiteConnection(databasePath);
            //db = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);
            //db.CreateTable<Stock>();
            //db.CreateTable<Valuation>();
        }

        public void AddStock(string symbol)
        {
            var stock = new Stock()
            {
                Symbol = symbol
            };
            //var keyNum =  db.Insert(stock);
            //Console.WriteLine("{0} == {1}", stock.Symbol, stock.Id);
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
