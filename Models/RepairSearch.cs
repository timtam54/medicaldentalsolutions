using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class RepairSearch
    {
        public string Resolved { get; set; }        
        public string Complete { get; set; }
        public string ServiceJobID { get; set; }
        public string SelectedEquipment { get; set; }
        public string RepairJob { get; set; }
        public string CustomerOrderNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("In From Date")]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("In To Date")]
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
        public string Equiptype { get; set; }
        public string Department { get; set; }
        public string EngineerID { get; set; }
        public string Locationid { get; set; }
        public int Branchid { get; set; }
        public List<BranchList> BranchList { get; set; }
        public List<RepairSearchList> Repairs { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<CustomerSite> CustomerSiteList { get; set; }
        public List<EquipmentTypeList> EquipTypeList { get; set; }
        public List<EngineerList> EngineerList { get; set; }
        public List<LocationList> LocationList { get; set; }
        public string ServiceData { get; set; }
        public string EquipmentData { get; set; }
        public string EquipmentClear { get; set; }
        public string ServiceClear { get; set; }
        public bool DateInFilter { get; set; }
        public List<int> Cnt { get; set; }
        public int SelCnt { get; set; }

        public bool DateOutFilter { get; set; }
    }
    [Serializable]
    public class AddRepaire
    {

//        public List<MDS.DB.Photo> Photos { get; set; }
        public string Equiptype { get; set; }
        public string EngineerID { get; set; }
        public string ConditionID { get; set; }

        public string TravelTypeCode { get; set; }
        public string ChargeTypeCode { get; set; }
        public string Customerid { get; set; }
        public string JobCode { get; set; }

        public bool CustomerSignature { get; set; }
        public string PartName { get; set; }
        public Int16? NoOfParts { get; set; }
        public decimal?  Cost { get; set; }
        public string FaultDetail { get; set; }
        public string WorkDone { get; set; }

        public List<EquipmentTypeList> EquipTypeList { get; set; }
        public List<EngineerList> EngineerList { get; set; }
        public List<ConditionList> ConditionList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<TravelList> TravelList { get; set; }
        public List<ItemSearch> ItemList { get; set; }
       // public List<PartsUsedList> Parts { get; set; }
        public List<ChargeTypeList> ChargeType { get; set; }

        public string EquipItem { get; set; }
        public int EquipId { get; set; }
        public int RepairId { get; set; }
        public string Customer { get; set; }
        public string CustomerEquipCode { get; set; }
        public DateTime? DateInitalCall { get; set; }
        public DateTime? TimeInitalCall { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Equipment Repair Date")]
        public DateTime? DateEquipRepair { get; set; }
        public DateTime? TimeEquipRepair { get; set; }
        public DateTime? TimeOutEquipRepair { get; set; }

        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public string Notes { get; set; }
        //public string LastNotes { get; set; }  
        public bool Approve { get; set; }
        public bool? orderNumber { get; set; }
        public string ApprovalContact { get; set; }
        public string Accessories { get; set; }
        public string FaultRepair { get; set; }
        public string AcctuallyWorkDone { get; set; }
        public decimal EarthResistant { get; set; }
        public decimal Insulation { get; set; }
        public decimal LeakageCurrent { get; set; }
        public bool VerbalApproval { get; set; }
        public string OrderNo { get; set; }
        public string ApprovalReceived { get; set; }
        public string LabourId { get; set; }       
        public string LabourRate { get; set; }
        public string TravelRate { get; set; }
        public decimal? PromotionalExpenses { get; set; }
        public DateTime? RepairDate { get; set; }
        public bool? AllSpecified { get; set; }
        public bool? ItemRepair { get; set; }
        public bool? ResultRepair { get; set; }
        public bool HasItem { get; set; }
        public bool HasJob { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Completed Date")]
        public DateTime? JobCompletionDate { get; set; }

        [DisplayName("Completed Time")]
        public decimal? JobCompletionTime { get; set; }
        public string Invoice { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Invoice Date")]
        public DateTime? InvoiceDate { get; set; }
        public decimal? ChargesInvoice { get; set; }
        public string PersonName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Receipt Date")]
        public DateTime? ReceiptDate { get; set; }
        public DateTime? ReceiptTime { get; set; }
        public float? LabourHours { get; set; }
        public float? TravelHours { get; set; }
        public bool? NoCharge { get; set; }
        public bool? Charge { get; set; }
        public bool? ChargePartsOnly { get; set; }
        public bool? SafetyTestDone { get; set; }
        public bool? RetirementReportPrinted { get; set; }
        public bool InvoiceThruServiceJob { get; set; }
        public string SelectedEquipment { get; set; }
        public string EquipmentData { get; set; }
        public string AssociateServiceJob { get; set; }
        public int ServiceJobID { get; set; }
        public bool PopUp { get; set; }
        public string TechnicalContact { get; set; }

        public List<ChargeTypeHour> ChargeTypeHours { get; set; }
    }
}