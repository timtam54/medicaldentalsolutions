using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServicesearchList
    {
        public string Branch { get; set; }
        public string ServiceJobUID { get; set; }
        public string JobCode { get; set; }
        public string Customer { get; set; }
        public string CustomerSite { get; set; }        
        public DateTime? DateProgrammed { get; set; }
        public DateTime? DateStart { get; set; }
        public string EngineerName { get; set; }
        public int EquipmentID { get; set; }
        public string EquipmentSerialNumber { get; set; }
        public string EquipmentModel { get; set; }

        public string EquipmentType { get; set; }
        public int ServiceWOID { get; set; }

        public int? BranchID { get; set; }

    }
}