using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class CustomerDepartmentList
    {
        
        public string CustomerCode { get; set; }
        public List<CCustomerSite> DepartmentList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
    }
}