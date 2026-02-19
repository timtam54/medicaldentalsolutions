using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class UserDetailBranches
    {
        public bool New { get; set; }
        public string Email { get; set; }
        //public string AdminUserTech { get; set; }
        public bool? Admin { get; set; }
        public string Password { get; set; }

        public List<UserBranchList> branches { get; set; }

        public List<int> SelectedBranch { get; set; }
        public List<int> AuditBranch { get; set; }
    }
}