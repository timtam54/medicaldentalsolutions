using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class CustomerForm
    {
        public string CustomerCode { get; set; }
        public string Code { get; set; }
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
        public DateTime? ContractRenewalDate { get; set; }
        public string Notes { get; set; }
        public bool StopSupply { get; set; }
        public DateTime? StopDate { get; set; }
        public string StoppedBy { get; set; }
        public List<BranchList> BranchList { get; set; }
        public int Branchid { get; set; }
        public string OnLinePassword { get; set; }
    }
}