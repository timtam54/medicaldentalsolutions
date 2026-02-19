using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipmentSearch
    {
        public string SerachMDS { get; set; }
        public bool? DisplayEquip { get; set; }
        public string Customerid { get; set; }
        public string Equiptype { get; set; }
        public string ModelID { get; set; }
        public string Department { get; set; }
        public string Locationid { get; set; }
        public int Branchid { get; set; }
        public List<BranchList> BranchList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<CustomerSite> CustomerSiteList { get; set; }
        public List<EquipmentTypeList> EquipTypeList { get; set; }        
        public List<LocationList> LocationList { get; set; }
        public List<EquipmentSearchList> EquipmentSearchList { get; set; }
        public bool? ServiceMDS { get; set; }
        public List<int> Cnt { get; set; }
        public int SelCnt { get; set; }
    }

    public class EquipmentSearchNew
    {
        public string EquipType { get; set; }
        public string SearchMDS { get; set; }
        public bool DisplayEquip { get; set; }
        public string Customerid { get; set; }
        //public string Equiptype { get; set; }
        //public string ModelID { get; set; }
        //public string Department { get; set; }
        //public string Locationid { get; set; }
        public int Branchid { get; set; }

//        public List<CustomerList> CustomerList { get; set; }

        public bool ServiceMDS { get; set; }
    }
    public class AddEquipment
    {
        public string message { get; set; }
        public string Customerid { get; set; }
        public string Equiptype { get; set; }
        public string DeptID { get; set; }
        public string LocationID { get; set; }
        public string VendorUID { get; set; }
        public int? ServiceAreaUID { get; set; }
        public string EquipTypeCode { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<MonthList> MonthList1 { get; set; }
        public List<MonthList> MonthList2 { get; set; }
        public List<MonthList> YearList { get; set; }
        public List<CustomerSite> CustomerSiteList { get; set; }
        public List<EquipmentTypeList> EquipTypeList { get; set; }
        public List<LocationList> LocationList { get; set; }
        public List<VendorList> VendorList { get; set; }
        public List<ServiceAreaList> ServiceAreaList { get; set; }
        public List<ManufacturerEquipType> ManufacturerEquiptype { get; set; }
        public string CustomerCodeEuip { get; set; }
        public int EquipID { get; set; }
        public string GeneralNotes { get; set; }
        public string EquipModel { get; set; }
        public string ModelUID { get; set; }
        public string Manufacturer { get; set; }
        public string SerialNo { get; set; }
        public string MDSQNItem { get; set; }
        public string AlternativeItem { get; set; }
        public bool? MDSQNSell { get; set; }
        public string ManualLibrary { get; set; }
        public Decimal? Cost { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string InitialCondition { get; set; }
        public bool? MDSQNService { get; set; }
        public int ServicePerYear { get; set; }
        public int MinorService { get; set; }
        public int ServicePerMonth { get; set; }
        public int ServicePerMonth1 { get; set; }
        public bool? TickItem { get; set; }
        public bool? Obselete { get; set; }
        public bool? CustomerRequired { get; set; }
        public bool? BERDisposal { get; set; }
        public bool? LongerParts { get; set; }
        public DateTime? DateRequired { get; set; }
        public string RetirementComment { get; set; }
        public bool OverDue { get; set; }
        public int Duemonth { get; set; }
        public int DueYear { get; set; }
        public string DueServiceType { get; set; }
        public int EquipIDEdit { get; set; }
        public bool PopUp { get; set; }
        public int Branchid { get; set; }
        public List<BranchList> BranchList { get; set; }
    }
}