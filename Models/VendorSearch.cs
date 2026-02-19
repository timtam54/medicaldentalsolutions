using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class VendorSearch
    {
        public List<VendorSearchList> Vendors { get; set; }
        public string VendorName { get; set; }
        public bool PopUp { get; set; }
    }
}