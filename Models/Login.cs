using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class MonthList
    {
        public string Month { get; set; }
        public string MonthInt { get; set; }
    }
}