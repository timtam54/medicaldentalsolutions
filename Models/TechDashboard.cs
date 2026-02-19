using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class TechDashboard
    {
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayName("From Date")]
        //public DateTime FromDate { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayName("To Date")]

        public List<CustomerCount> RepairSummary { get; set; }
        public List<CustomerCount> ServiceSummary { get; set; }

    }

    public class CustomerCount
    {

        public string CustArr { get; set; }
        public string MapAddress { get; set; }
        public string CustomerID { get; set; }
        public string Customer { get;set;}
        //public int Cnt { get;set;}
        public string JobNo { get; set; }
        public int BranchID { get; set; }
        public int EngineerID { get; set; }

        public int ID { get; set; }

        public DateTime? ScheduledDate { get; set; }
    }

    public class IDBranch
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
    }

}