using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class LabourChargeList
    {
        //public string CustomerCode { get; set; }
        public List<LabourCharge> LabourCharges { get; set; }
      
    }

    public class LabourCharge
    {
        public string ChargeTypeCode { get; set; }
        public string ChargeType { get; set; }
        public decimal? ChargeRate { get; set; }

    }
}