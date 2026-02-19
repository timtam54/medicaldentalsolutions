using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
   
    public class ServiceWorkOderSearch
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Customerid { get; set; }
        public string EngineerID { get; set; }
        public string Department { get; set; }
        public string ServiceJobID { get; set; }
        public int Branchid { get; set; }
        public string Equiptype { get; set; }
        public List<EquipmentTypeList> EquipTypeList { get; set; }
        public List<BranchList> BranchList { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<EngineerList> EngineerList { get; set; }
        public List<ServiceworkorderList> ServiceWorkOrder { get; set; }
        public List<CustomerSite> CustomerSiteList { get; set; }
        public bool DataEntryIncompeteOnly { get; set; }
        public DateTime ServicesAfterDate { get; set; }
        public string Location { get; set; }
       // public int EquipUID { get; set; }
        public string SelectedEquipment { get; set; }
        public string EquipmentData { get; set; }
        public string ServiceData { get; set; }
        public bool RepairJob { get; set; }
        public string EquipmentClear { get; set; }
        public string ServiceClear { get; set; }
        public string RepairClear { get; set; }
        public bool ServicesAfterDateFilter { get; set; }
        public List<int> Cnt { get; set; }
        public int SelCnt { get; set; }
    }
    public class AddServiceWorkOrder
    {
        public string ConditionID { get; set; }
        public List<ConditionList> ConditionList { get; set; }

        public string Equiptype { get; set; }           
            public string EngineerID { get; set; }
            public string ServiceJobUID { get; set; }
            public string EngineerName { get; set; }
            public string Department { get; set; }            
            public string Locationid { get; set; }
            public string ServiceFunctionCode { get; set; }
            public string CustomerCode { get; set; }    
            public List<EquipmentTypeList> EquipTypeList { get; set; }           
            public List<EngineerList> EngineerList { get; set; }
            public List<CustomerSite> CustomerSiteList { get; set; }           
            public List<LocationList> LocationList { get; set; }
            public List<ServiceJobList> ServiceJobList { get; set; }
            public List<ServiceFunctionList> ServiceFunctionList { get; set; }

            public string EquipItem { get; set; }
            public Int32 EquipId { get; set; }
            public Int32 SelectedEquipId { get; set; }
            public int ServiceId { get; set; }
            public string JobCode { get; set; }
            public DateTime? ActualServiceDate { get; set; }
            public String BookingNotes { get; set; }
            public String ServiceNotes { get; set; }
            public string WorkDone { get; set; }

            public string CustomerEquipCode { get; set; }
            public bool? MajorService { get; set; }
            [Required]
            public Int16 Month { get; set; }
            [Required]
            public Int16 Year { get; set; }
            public Int32 ServiceReferenceNo { get; set; }
            public decimal EarthResistance { get; set; }
            public bool? SafetyTest { get; set; }
            public decimal InsulationResistance { get; set; }
            public decimal LeakageCurrent { get; set; }
            public bool? ServiceConculsion { get; set; }
            public bool? Compeleterecord { get; set; }
            public Int32 RepairID { get; set; }
            public List<PartsUsedList> Parts { get; set; }
            public string SelectedEquipment { get; set; }
            public bool PopUp { get; set; }       
  
    }
    public class ServiceTestScript
    {
        public string SerialNo { get; set; }
        public string CustEquipCode { get; set; }
        public string ItemCode { get; set; }
        public string TypeCode { get; set; }
        public string EquipmentTypeCode { get; set; }
        public string ServiceJobCode { get; set; }
        public int TestScriptID { get; set; }
        public List<TestScirpt> TestScriptData { get; set; }
        public List<CLSTestScript> TS { get; set; }
        public bool CheckedScript { get; set; }
        public string Comment { get; set; }
        public bool PassFail { get; set; }
        public string Parts { get; set; }
        public string Remarks { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public string TSLabel { get; set; }
        public string TSSN { get; set; }
        public int ServiceTestScriptID { get; set; }
        public int ServiceUID { get; set; }
    }

    public class CLSTestScript {
        public int ServiceTestScriptID { get; set; }      
        public int TestScriptID { get; set; }
        public string TestScriptText { get; set; }
        public string CheckedScriptText { get; set; }
        public bool CheckedScript { get; set; }
        public string Comment { get; set; }
        public int ServiceUID { get; set; }
        public int BranchID { get; set; }
    }
}