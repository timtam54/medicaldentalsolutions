using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ConditionList
    {
        public string ConditionID { get; set; }
        public string ConditionDesc { get; set; }
    }

    public class Condition
    {
        public int ConditionID { get; set; }
        public string ConditionDesc { get; set; }
    }
}