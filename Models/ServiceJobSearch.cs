using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServiceJobSearch
    {
        public string OverDue { get; set; }       
      
        public string Customerid { get; set; }

        public string Department { get; set; }      
        public string Locationid { get; set; }
        public string MonthVal { get; set; }
        public int Branchid { get; set; }
        public List<BranchList> BranchList { get; set; }
        public List<MonthYears> monthdataList { get; set; }
        public List<ServiceJobSearchList> Service { get; set; }
        public List<CustomerList> CustomerList { get; set; }
        public List<CustomerSite> CustomerSiteList { get; set; }
      
        public List<LocationList> LocationList { get; set; }
        public string SelectedEquipment { get; set; }
        public string EquipmentData { get; set; }
        public string EquipmentClear { get; set; }
        public DateTime progDate { get; set; }
        public string progEngineer { get; set; }

        public List<int> Cnt { get; set; }
        public int? SelCnt { get; set; }

    }

    public class DateEngineer
    {
        public DateTime progDate { get; set; }
        public List<EngineerList> EngineerList { get; set; }
        public string EngineerID { get; set; }


    }
}