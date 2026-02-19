using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ClientReportingSearch
    {
        public bool PopUp { get; set; }
        public List<LoginTableVM> LoginList { get; set; }
    }
    public class LoginTableVM
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Customer{ get; set; }
        public Customer Customerobj { get; set; }
        public string Branch { get; set; }
        public bool PopUp { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public bool IsUpdate { get; set; }
        public string OldUserName { get; set; }
    }
}