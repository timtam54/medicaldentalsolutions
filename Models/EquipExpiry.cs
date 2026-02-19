using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{

    public class EquipExpiryHeader
    {
        public int BranchID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("From Date")]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("To Date")]

        public DateTime ToDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Programmed Date")]
        public DateTime ProgrammedDate {get;set;}

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Programmed Start Time")]
        public DateTime ProgrammedStartTime { get; set; }


        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Programmed End Time")]
        public DateTime ProgrammedEndTime { get; set; }

        public List<EquipExpiry> Results { get; set; }
    }
    public class EquipExpiry
    {
      public string CustomerCode { get; set; }
        public int EquipID { get; set; }
        public string BNQItemCode { get; set; }
        public string SerialNumber { get; set; }
        
        public DateTime WarrantyExpirationDate { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Customer { get; set; }
        
        public int? ServiceJobID { get; set; }

        public string ServiceJobNo { get; set; }
        public DateTime? ServiceJobProgrammed { get; set; }

        public bool? ServiceJobComplete { get; set; }
    }
}