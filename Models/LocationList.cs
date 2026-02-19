using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class LocationList
    {
        public string LocationID { get; set; }
        public string Location { get; set; }
    }

    public class CLocationList
    {
        public string CustomerCode { get; set; }
        public Int32 LocationID { get; set; }
        public string Location { get; set; }
    }
}