using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServiceJobSearchList
    {
        //public Int32 RepairUID { get; set; }
        
        public string Customer { get; set; }
        public string DueOn { get; set; }
        public string EquipmentType { get; set; }
        public string CustomerSite { get; set; }
        public string Location { get; set; }
        public string Manufacturer { get; set; }
        public Int32 EquipId { get; set; }
        public Int32 LastServiceUID { get; set; }
        public short? NextServiceYear { get; set; }
        public short? NextServiceMonth { get; set; }
        public string DateServiced { get; set; }
        public string MDSItemCode { get; set; }
        public string SerialNo { get; set; }
        public string FutureService { get; set; }
        
    }
}