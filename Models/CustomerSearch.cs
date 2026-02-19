using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class CustomerSearch
    {
        public List<CustomerSearchList> Customers { get; set; }
        public List<BranchList> BranchList { get; set; }
        public string CompanyName { get; set; }
        public string CustomerCode { get; set; }
        public string CompanyNameStart { get; set; }
        public bool ContractOverDue { get; set; }
        public int Branchid { get; set; }
    }
}