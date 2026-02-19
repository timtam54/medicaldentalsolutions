using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServiceWorks
    {
       public List<string> Engineers { get; set; }
        public List<ServiceWorkTime> ServiceWorkTimes { get; set; }
    }

    public class ServiceEngLock
    {
        public int? EngineerID { get; set; }
        public string Engineer { get; set; }
        public int ServiceID { get; set; }
        public int BranchID { get; set; }
        public int Index { get; set; }
    }

    public class ServiceWorkTime
    {
        public bool? Exist { get; set; }
        public bool? Repair { get; set; }

        [Required]
        public string ChargeTypecode { get; set; }
        public int? ServiceJobID { get; set; }
        public int? EngineerID { get; set; }
        public int? BranchID { get; set; }

//        [DataType(DataType.Time)]
  //      [DisplayFormat(DataFormatString = "{0:HH-mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Time")]

        public decimal? StartTime2 { get; set; }

  //      [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? StartTime { get; set; }

//        public DateTime? StartTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date")]
        public DateTime? EndTime { get; set; }
    //    [DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End Time")]
        public decimal? EndTime2 { get; set; }
        public string ChargeType { get; set; }
        public string Engineer { get; set; }
    }
}