using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EngineerForm
    {
        public List<BranchList> BranchList { get; set; }
        public int Branchid { get; set; }
        public int EngineerId { get; set; }
        public string EngineerName { get; set; }
        public bool PopUp { get; set; }
        public string AdminEmail { get; set; }
        public string UserName { get; set; }
    }
}