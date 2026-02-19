using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class CustomerLocationList
    {
        public string CustomerCode { get; set; }
        public List<CLocationList> LocationList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
    }
}