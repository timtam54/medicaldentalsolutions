using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class PartsSearch
    {
        public Int32 ID { get; set; }
        public string Type { get; set; }
        public string Item { get; set; }
        public string Descrip { get; set; }
        public decimal? Price { get; set; }
        public List<PartsSearch> Parts { get; set; }
        public string  Account { get; set; }
        public bool Active { get; set; }
    }

    public class PartSearch
    {
        public Int32 ID { get; set; }
        public string ItemNo { get; set; }
        public string Descrip { get; set; }

        public decimal Price { get; set; }
    }

}