using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class VendorForm
    {
        public int VendorCode { get; set; }
        public int Code { get; set; }
        public string CompanyName { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public DateTime? ContractDate { get; set; }
        public string ContractDetails { get; set; }
        public string Notes { get; set; }
        public bool PopUp { get; set; }
    }
}