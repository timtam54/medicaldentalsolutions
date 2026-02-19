using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipmentSearchList
    {
        public Int32 CustomerID { get; set; }
        public string EquipID { get; set; }
        public string Customer { get; set; }
        public string MDSItemNo { get; set; }
        public string SerialNumber { get; set; }
        public string CustomerSite { get; set; }
        public string Location { get; set; }
        public string EquipDesc { get; set; }
        public string EquipementType { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string VendorName { get; set; }
        public string CurrentlyServicedByMDS { get; set; }
        public string InService { get; set; }
        public DateTime? WarrantyExpirationDate { get; set; }
        public double? TotalCostOfRepairs { get; set; }
        public double? TotalRepairHours { get; set; }
        public decimal? Cost { get; set; }
    }
}