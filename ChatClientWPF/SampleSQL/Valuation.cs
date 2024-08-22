//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClientWPF.SampleSQL
{
    internal class Valuation
    {
       // [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
       // [Indexed]
        public int StockId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
       // [Ignore]
        public string IgnoreField { get; set; }
    }
}
