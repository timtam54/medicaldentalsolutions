using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class PartsForm
    {
        //public List<TestScirpt> Tests { get; set; }
        public string Type { get; set; }
        public string Item { get; set; }
        public decimal? Price { get; set; }
        public string Descrip { get; set; }
        public int ID { get; set; }
        public decimal? Cost { get; set; }
        public bool Active { get; set; }
        public string TaxCode { get; set; }
        public string Account { get; set; }
      
        public List<TypesList> TypeList { get; set; }
        public List<AccountList> AccountList { get; set; }
        public List<TaxCodeList> TaxCodeList { get; set; }
    }
}