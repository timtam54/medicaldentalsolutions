using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class BranchForm
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string PrefixCode { get; set; }
        public string Password { get; set; }
        public string PasswordROTech { get; set; }
    }
}