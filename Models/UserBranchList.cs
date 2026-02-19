using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class UserBranchList
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public Boolean Selected { get; set; }
        public Boolean Audit { get; set; }

        public string UserName { get; set; }
        public bool? Admin { get; set; }
        //  public string PrefixCode { get; set; }
    }
}