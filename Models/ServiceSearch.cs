using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServiceSearch
    {
        public string SelectedEquipment { get; set; }
        public string EquipmentData { get; set; }

       // public string EquipItem { get; set; }
       // public int EquipId { get; set; }

        public string ServiceWork { get; set; }
        public string Invoice { get; set; }

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
        [DisplayName("Out From Date")]

        public DateTime OutFromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Out To Date")]

        public DateTime OutToDate { get; set; }
        public string Customerid { get; set; }
        public string EngineerID { get; set; }
        public string Department { get; set; }
        public int Branchid { get; set; }
        public string Locationid { get; set; }
        public string ServiceJob { get; set; }
        public string CustomerOrderNo { get; set; }
        public List<BranchList> BranchList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<EngineerList> EngineerList { get; set; }
        public List<ServicesearchList> Service { get; set; }
        public List<CustomerSite> CustomerSiteList { get; set; }
        public List<LocationList> LocationList { get; set; }
        public bool DateInFilter { get; set; }
        public bool DateOutFilter { get; set; }

        public List<int> Cnt { get; set; }
        public int SelCnt { get; set; }

    }

    public class ChargeTypeHour
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double Hours { get; set; }
        public string ChargeType { get; set; }
        public decimal ChargeRate { get; set; }

        public double Total { get; set; }

    }
    public class AddService
    {
        public List<ChargeTypeHour> ChargeTypeHours { get; set; }
        public string Customerid { get; set; }

        public List<int?> Engineers { get; set; }
     
        public string DeptID { get; set; }
        public string Branchid { get; set; }
        public string LocationID { get; set; }
        public string Location { get; set; }
        public string TravelTypeCode { get; set; }
        public string ChargeTypeCode { get; set; }
        public bool PopUp { get; set; }

        public List<BranchList> BranchList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<EngineerList> EngineerList { get; set; }        
        public List<CustomerSite> CustomerSiteList { get; set; }
        public List<TravelList> TravelList { get; set; }
        public List<ChargeTypeList> ChargeType { get; set; }
        public List<LocationList> LocationList { get; set; }

        public bool CustomerSignatureX { get; set; }
        public int ServiceId { get; set; }
        public string JobCode { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Programmed")]
        [Required]
        public DateTime? ProgrammedDate { get; set; }

        [Required]
        [DisplayName("Programmed Time Start")]
        public decimal? ProgrammedTime { get; set; }
  //      [DataType(DataType.Time)]
//        [DisplayFormat(DataFormatString = "{0:HH-mm}", ApplyFormatInEditMode = true)]
        [Required]
        [DisplayName("Programmed Time End")]
        public decimal? ProgrammedEnd { get; set; }
        public String  BookingNotes { get; set; }
        public String TechnicalContact { get; set; }
        public string ApprovalContact { get; set; }
        public string CustomerEquipCode { get; set; }
        public bool? IsaApprove { get; set; }
        public bool? Isorder { get; set; }
        public string SpecialNotes { get; set; }
        public bool? VerbalApproval { get; set; }
        public string OrderNo { get; set; }
        public string IsApprovalReceived { get; set; }
//        public DateTime? ServiceStartDate { get; set; }
  //      public DateTime? ServiceStartTime { get; set; }
        //public DateTime? ServiceCompleteDate { get; set; }
        //public DateTime? ServiceCompleteTime { get; set; }
        public decimal? LabourHours { get; set; }
        public decimal? TravelHours { get; set; }
        public string LabourRate { get; set; }
        public string TravelRate { get; set; }
        public bool DontChangeCust { get; set; }
        public decimal? PropotionalExpenses { get; set; }
        public bool ServiceComplete { get; set; }
        public bool ServiceComplete_Outstanding { get; set; }
        public bool ServiceComplete_BERED { get; set; }
        public bool HasJobInvoice { get; set; }
        public string Invoice { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Invoice Date")]

        public DateTime? InvoiceDate { get; set; }
        public decimal? Charges { get; set; }
        public string CustomerSignature { get; set; }
        public decimal? CostOfServicePartsForServiceJob { get; set; }
        public decimal? CostOfTotalRepairsForServiceJob { get; set; }
      
    }
}