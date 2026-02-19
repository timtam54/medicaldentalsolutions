using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class Diary
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("From Date")]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("To Date")]

        public DateTime ToDate { get; set; }
        public List<DateTime> Days { get; set; }
        public List<Eng> Engineers { get; set; }
        public List<int> Hours { get; set; }
        public List<int> Minutes { get; set; }
        public List<Booking> Bookings { get; set; }

    }

    public class Eng
    {
        public int EngineerID { get; set; }
        public string EngineerName { get; set; }

    }

    public class Booking
    {
        public int ID { get; set; }
        public string RepairOrService { get; set; }
        public bool Complete { get; set; }

        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }

        public int? EngineerID { get; set; }
        public int? EquipUID { get; set; }
        public string CustomerCode { get; set; }
        public int BranchID { get; set; }

        public int? Code { get; set; }
    }
}