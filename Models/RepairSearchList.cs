using MDS.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class RepairSearchList
    {
        public Int32 RepairUID { get; set; }
        public string JobCode { get; set; }
        public string Customer { get; set; }
        public string Equipment { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string OrderNumber { get; set; }
        public string Engineer { get; set; }
        public Boolean? RepairCompleted { get; set; }
        public decimal? TotalCharge { get; set; }
        public string FaultDetails { get; set; }
        public string WorkDone { get; set; }
        public float? RepairTravelExpenseCost { get; set; }
        public decimal? PartsCost { get; set; }
        
        public DateTime? WarrantyExpirationDate { get; set; }
    }
}