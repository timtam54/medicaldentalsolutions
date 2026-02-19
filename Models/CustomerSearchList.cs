using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class CustomerSearchList
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string ContractDate { get; set; }
        public string RenewalDate { get; set; }
    }
}