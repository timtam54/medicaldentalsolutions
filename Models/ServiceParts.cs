using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServiceParts
    {
        public Int32 ServiceID { get; set; }
        public Int32 BranchID { get; set; }

        public List<DB.ServicePart> ServicePartsList { get; set; }
    }

}