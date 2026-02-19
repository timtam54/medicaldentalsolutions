using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class BranchSearch
    {
        public string BranchName { get; set; }
        public List<BranchList> Branches { get; set; }
    }
}