using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDS.Models;
using MDS.DB;
using MDS.Extensions;


namespace MDS.Helper
{
    public class Utility
    {
        static List<Branch> BranchCache;// = new List<Branch>();

        static TrackerDataContext auditdb = new TrackerDataContext();
        public static void Audit(string HttpContextUserIdentityName, string _Page,int _RecordKey,HttpRequestBase Request)
        {
            try
            {
                if (BranchCache == null)
                    BranchCache = auditdb.Branches.ToList();
                DateTime dt = DateTime.UtcNow.AddHours(10);
                string _Username = MDS.Controllers.LoginController.AdminTechCustomer(HttpContextUserIdentityName);
                Audit au = new DB.Audit()
                {
                    UserName = _Username,
                    ActivityDteTme = dt,
                    Page = _Page,
                    RecordKey = _RecordKey,
                    IPAddress = Request.UserHostAddress,
                    Browser = Request.Browser.Browser,
                    Branch = BranchCache.Where(i => i.BranchID == MDS.Controllers.LoginController.BranchID(HttpContextUserIdentityName)).FirstOrDefault().BranchName
                };
                auditdb.Audits.InsertOnSubmit(au);
                auditdb.SubmitChanges();
            }
            catch (Exception ex)
            {
                ;
            }
        }

        public static List<CustomerList> GetCustomerList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var CustomerList = db.Customers.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name) && i.CustomerCode != "All").OrderBy(i => i.CompanyName).AsEnumerable().Select(i => new CustomerList
            {
                CompanyName = StringExtension.ToTitleCase(i.CompanyName),
                CustomerCode = i.CustomerCode
            }).ToList();

            CustomerList.Insert(0, new CustomerList { CustomerCode = "", CompanyName = "--All Customers--" });
            return CustomerList;
        }
        public static List<ServiceFunctionList> GetServiceFunctionList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var ServiceFunction = db.ServiceFunctions.OrderBy(i => i.ServiceFunctionDesc).AsEnumerable().Select(i => new ServiceFunctionList
            {
                ServiceFunctionDesc = StringExtension.ToTitleCase(i.ServiceFunctionDesc),
                ServiceFunctionCode = i.ServiceFunctionCode
            }).ToList();

            ServiceFunction.Insert(0, new ServiceFunctionList { ServiceFunctionCode = "", ServiceFunctionDesc = "--All Service Function--" });
            return ServiceFunction;
        }

        public static List<CustomerList> GetCustomerListByBranchId()
        {
            TrackerDataContext db = new TrackerDataContext();
            var CustomerList = db.Customers.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name) && i.CustomerCode != "All").OrderBy(i => i.CompanyName).AsEnumerable().Select(i => new CustomerList
            {
                CompanyName = StringExtension.ToTitleCase(i.CompanyName),
                CustomerCode = i.CustomerCode
            }).ToList();

            CustomerList.Insert(0, new CustomerList { CustomerCode = "", CompanyName = "--All Customers--" });
            return CustomerList;
        }

        public static List<CustomerList> GetCustomerListByBranchIdSelect()
        {
            TrackerDataContext db = new TrackerDataContext();
            var CustomerList = db.Customers.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name) && i.CustomerCode != "All").OrderBy(i => i.CompanyName).AsEnumerable().Select(i => new CustomerList
            {
                CompanyName = StringExtension.ToTitleCase(i.CompanyName),
                CustomerCode = i.CustomerCode
            }).ToList();

            CustomerList.Insert(0, new CustomerList { CustomerCode = "-1", CompanyName = "--All Customers--" });
            return CustomerList;
        }


        public static List<EquipmentTypeList> GetEquipTypeList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var EquipTypeList = db.EquipTypes.Where(i=>i.BranchID==Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).OrderBy(i => i.Name).AsEnumerable().Select(i => new EquipmentTypeList
            {
                EquipTypeCode = i.EquipTypeCode,
                Name = StringExtension.ToTitleCase(i.Name),
            }).ToList();
            EquipTypeList.Insert(0, new EquipmentTypeList { EquipTypeCode = "", Name = "--All Equip Type--" });
            return EquipTypeList;
        }
        public static List<EquipmentTypeList> GetEquipTypeListByBranchId()
        {
            TrackerDataContext db = new TrackerDataContext();
            var EquipTypeList = db.USP_EuipByBranchId(Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).Select(i => new EquipmentTypeList
            {
                EquipTypeCode = i.EquipUID.ToString(),
                Name = i.EquipDesc
            }).ToList();
            EquipTypeList.Insert(0, new EquipmentTypeList { EquipTypeCode = "", Name = "--All Equip Type--" });
            return EquipTypeList;
        }

        public static List<ConditionList> GetConditionList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var ConditionList = db.Conditions.OrderBy(i => i.ConditionID).AsEnumerable().Select(i => new ConditionList
            {
                ConditionID = i.ConditionID.ToString(),
                ConditionDesc = StringExtension.ToTitleCase(i.ConditionDesc),
            }).ToList();
            ConditionList.Insert(0, new ConditionList { ConditionID = "", ConditionDesc = "--Please Select Condition--" });
            return ConditionList;
        }


        
        public static List<EngineerList> GetEngineerList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var EngineerList = db.Engineers.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).OrderBy(i => i.EngineerName).AsEnumerable().Select(i => new EngineerList
            {
                EngineerID = i.EngineerID.ToString(),
                EngineerName = StringExtension.ToTitleCase(i.EngineerName),
            }).ToList();
            EngineerList.Insert(0, new EngineerList { EngineerID = "", EngineerName = "--Please Select Engineer--" });
            return EngineerList;
        }

        public static List<TypesList> GetTypeList()
        {
            List<TypesList> x = new List<TypesList>();
            MDS.Models.TypesList pp;
            pp = new TypesList();
            pp.Type = "Discount";
            x.Add(pp);

            pp = new TypesList();
            pp.Type = "Group";
            x.Add(pp);
            pp = new TypesList();
            pp.Type = "Inventory Part";
            x.Add(pp);
            pp = new TypesList();
            pp.Type = "Non-inventory Part";
            x.Add(pp);
            pp = new TypesList();
            pp.Type = "Other Charge";
            x.Add(pp);
            pp = new TypesList();
            pp.Type = "Service";
            x.Add(pp);
            pp = new TypesList();
            pp.Type = "Subtotal";
            x.Add(pp);

            return x;
        }

        public static List<TaxCodeList> GetTaxCodeList()
        {
            List<TaxCodeList> x = new List<TaxCodeList>();
            MDS.Models.TaxCodeList pp;
            pp = new TaxCodeList();
            pp.Code = "EXP";
            x.Add(pp);

            pp = new TaxCodeList();
            pp.Code = "EXP";
            x.Add(pp);
             pp = new TaxCodeList();
             pp.Code = "FRE";
            x.Add(pp);
             pp = new TaxCodeList();
             pp.Code = "GST";
            x.Add(pp);
            pp = new TaxCodeList();
            pp.Code = "NCF";
            x.Add(pp);
            pp = new TaxCodeList();
            pp.Code = "NCG";
            x.Add(pp);
            return x;
        }

        public static List<AccountList> GetAccountList()
        {
            List<AccountList> x = new List<AccountList>();
            MDS.Models.AccountList pp;
            pp = new AccountList();
            pp.Name = "Purchases Supplies";
            x.Add(pp);

            pp = new AccountList();
            pp.Name = "Inventory Asset";
            x.Add(pp);
             pp = new AccountList();
            pp.Name = "Travel:Accomodation";
            x.Add(pp);
              pp = new AccountList();
            pp.Name = "Tool Replacements";
            x.Add(pp);
                 pp = new AccountList();
            pp.Name = "Labour Income";
            x.Add(pp);

      pp = new AccountList();
            pp.Name = "Product Sales";
            x.Add(pp);
             pp = new AccountList();
            pp.Name = "Provision for Income Tax";
            x.Add(pp);
                pp = new AccountList();
            pp.Name = "Commission Received";
            x.Add(pp);
              pp = new AccountList();
            pp.Name = "Pre-paid Borrowing Costs";
            x.Add(pp);

             pp = new AccountList();
            pp.Name = "Discount";
            x.Add(pp);
              pp = new AccountList();
            pp.Name = "Reimbursement Income";
            x.Add(pp);
               pp = new AccountList();
            pp.Name = "Uncategorised Income";
            x.Add(pp);
               pp = new AccountList();
            pp.Name = "GST";
            x.Add(pp);
               pp = new AccountList();
            pp.Name = "Motor Vehicle @ Cost Comm Ute";
            x.Add(pp);
             pp = new AccountList();
            pp.Name = "Cost of Goods Sold";
            x.Add(pp);
            pp = new AccountList();
            pp.Name = "Bank Charges";
            x.Add(pp);
            return x;
        }
        public static List<EngineerList> GetEngineerListByBranchId()
        {
            TrackerDataContext db = new TrackerDataContext();
            var EngineerList = db.Engineers.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).OrderBy(i => i.EngineerName).AsEnumerable().Select(i => new EngineerList
            {
                EngineerID = i.EngineerID.ToString(),
                EngineerName = StringExtension.ToTitleCase(i.EngineerName),
            }).ToList();
            EngineerList.Insert(0, new EngineerList { EngineerID = "", EngineerName = "--Please Select Engineer--" });
            return EngineerList;
        }

        public static List<EngineerList> GetEngineerListByBranch(int branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var EngineerList = db.Engineers.OrderBy(i => i.EngineerName).Where(i => i.BranchID == branchid).AsEnumerable().Select(i => new EngineerList
            {
                EngineerID = i.EngineerID.ToString(),
                EngineerName = StringExtension.ToTitleCase(i.EngineerName),
            }).ToList();
            EngineerList.Insert(0, new EngineerList { EngineerID = "", EngineerName = "--Please Select Engineer--" });
            return EngineerList;
        }
        public static List<TravelList> GetTravelList(int branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var TravelList = db.TravelTypes.Where(i => i.BranchID == branchid).OrderBy(i => i.TravelType1).AsEnumerable().Select(i => new TravelList
            {
                TravelTypeCode = i.TravelTypeCode.ToString(),
                TravelType = StringExtension.ToTitleCase(i.TravelType1),
            }).ToList();
            TravelList.Insert(0, new TravelList { TravelTypeCode = "", TravelType = "--Select Travel--" });
            return TravelList;
        }
        
        public static List<ChargeTypeList> GetLabourList(int branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var ChargeTypeList = db.ChargeTypes.Where(i => i.BranchID == branchid).OrderBy(i => i.ChargeType1).AsEnumerable().Select(i => new ChargeTypeList
            {
                ChargeTypeCode = i.ChargeTypecode.ToString(),
                ChargeType = StringExtension.ToTitleCase(i.ChargeType1),
            }).ToList();
            ChargeTypeList.Insert(0, new ChargeTypeList { ChargeTypeCode = "", ChargeType = "--Select Labour--" });
            return ChargeTypeList;
        }
        public static List<BranchList> GetBranchList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var BranchList = db.Branches.AsEnumerable().Select(i => new BranchList
            {
                BranchID = i.BranchID.ToString(),
                BranchName = StringExtension.ToTitleCase(i.BranchName),
            }).ToList();
            BranchList.Insert(0, new BranchList { BranchID = "", BranchName = "--Select Branch--" });
            return BranchList;
        }
        public static List<MDS.Models.CustomerSite> GetCustomerSites(String Customercode)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Customersite = db.CustomerDepartments.Where(i => i.CustomerCode == Customercode && i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).Select(i => new MDS.Models.CustomerSite
            {
                DeptID = i.DeptID.ToString(),
                Department = i.Department,
            }).ToList();
            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID = "-1", Department = "--Select Site--" });
            return Customersite;
        }

        public static List<PartsUsedList> GetPartList(int id,int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var PartsList = db.RepairParts.Where(i => i.RepairOrServiceUID == id && i.BranchID == BranchID).Select(i => new PartsUsedList
                            {
                                PartName = i.PartDesc,
                                Price = i.CostPerUnit.Value,
                                NoOfParts = i.NumberUsed.Value,
                                PartId = i.PartID.HasValue ? i.PartID.Value : 0,
                                PartNumber = i.PartNumber,
                                
                                Stocked_Part = i.PartID.HasValue ? true : false
                            }).ToList();

            for (int i = 0; i < PartsList.Count; i++)
            {
                PartsList[i].Id = i + 1;
            }
            return PartsList;
        }

        public static List<PartsUsedList> GetServicePartList(int id,int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var PartsList = db.ServiceParts.Where(i => i.RepairOrServiceUID == id && i.BranchID == BranchID).Select(i => new PartsUsedList
            {
                PartName = i.PartDesc,
                Price = i.CostPerUnit.Value,
                NoOfParts =(i.NumberUsed==null)?0: i.NumberUsed.Value,// i.NumberUsed.Value,
                PartId = i.PartID.HasValue ? i.PartID.Value : 0,
                PartNumber = i.PartNumber,
                Stocked_Part = i.PartID.HasValue ? true : false
            }).ToList();

            for (int i = 0; i < PartsList.Count; i++)
            {
                PartsList[i].Id = i + 1;
            }
            return PartsList;
        }

        public static string GetBranchCode()
        {
            TrackerDataContext db = new TrackerDataContext();
            return db.Branches.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).FirstOrDefault().PrefixCode;

        }
        public static List<ServiceJobList> GetServiceJob()
        {
            TrackerDataContext db = new TrackerDataContext();
            var ServicejobData = db.ServiceJobs.OrderBy(i => i.JobCode).Select(i => new ServiceJobList
            {
                ServiceJobUID = i.ServiceJobUID.ToString(),
                FullJobCode = (i.BNQLocationCode + "S" + i.JobCode).ToString()
            }).ToList();
            ServicejobData.Insert(0, new ServiceJobList { ServiceJobUID = "", FullJobCode = "--Select Service Job--" });
            return ServicejobData;
        }
        public static List<ServiceJobList> GetServiceJob(string customercode)
        {
            TrackerDataContext db = new TrackerDataContext();
            var ServicejobData = db.ServiceJobs.Where(i => i.CustomerCode == customercode && i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).OrderByDescending(i => i.JobCode).Select(i => new ServiceJobList
            {
                ServiceJobUID = i.ServiceJobUID.ToString(),
                FullJobCode = (i.BNQLocationCode + "S" + i.JobCode).ToString()
            }).ToList();
            ServicejobData.Insert(0, new ServiceJobList { ServiceJobUID = "", FullJobCode = "--Select Service Job--" });
            return ServicejobData;
        }
        public static List<ServicesearchList> ServiceJobDetailbyId(int servicejobid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var ServiceJob = db.GetServiceJobDetailsbyID(servicejobid, Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).Select(i => new ServicesearchList
            {
                ServiceJobUID = i.ServiceJobUID.ToString(),
                Customer = i.Customer,
                JobCode = i.JobCode,
                EngineerName = i.Engineername,
                CustomerSite = i.CustomerSite
            }).ToList();
            return ServiceJob;
        }
        
        //public static List<EquipmentSearchList> EquipmentbDetailbyId(int servicejobid)
        //{
        //    TrackerDataContext db = new TrackerDataContext();
        //    var ServiceJob = db.GetServiceJobDetailsbyID(servicejobid, Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).Select(i => new EquipmentSearchList
        //    {
        //        ServiceJobUID = i.ServiceJobUID.ToString(),
        //        Customer = i.Customer,
        //        JobCode = i.JobCode,
        //        EngineerName = i.Engineername,
        //        CustomerSite = i.CustomerSite
        //    }).ToList();

        //    return ServiceJob;

        //}
        public static List<TestScirpt> GetServiceTestScript()
        {
            TrackerDataContext db = new TrackerDataContext();
            var TestScript = db.EquipTypeTests.OrderBy(i => i.EquipTypeTestID).AsEnumerable().Select(i => new TestScirpt
            {
                Test = StringExtension.ToTitleCase(i.Test),
                TestScriptID = i.EquipTypeTestID.ToString()
            }).ToList();

            TestScript.Insert(0, new TestScirpt { TestScriptID = "", Test = "--All Service Test--" });
            return TestScript;
        }
        public static List<LocationList> GetLocationList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Locationlist = db.CustomerLocations.AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
    //            Locationid = i.CustomerCode
    LocationID = i.LocationID.ToString()

            }).ToList();
            Locationlist.Insert(0, new LocationList { LocationID = "-1", Location = "--Select Location--" });
            return Locationlist;
        }

        public static List<MDS.Models.CustomerSite> GetCustomerSitesList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Customersite = db.CustomerDepartments.AsEnumerable().Select(i => new MDS.Models.CustomerSite
            {
                DeptID = i.DeptID.ToString(),
                Department = i.Department,
            }).ToList();
            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID = "-1", Department = "--Select Site--" });
            return Customersite;
        }

        public static List<LocationList> GetLocationListName()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Locationlist = db.CustomerLocations.AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                LocationID = i.LocationID.ToString()

            }).ToList();
            Locationlist.Insert(0, new LocationList { LocationID = "-1", Location = "--Select Location--" });
            return Locationlist;
        }

        public static List<VendorList> GetVendorList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Vendors = db.Vendors.AsEnumerable().Select(i => new VendorList
            {
                VendorUID = i.VendorUID.ToString(),
                CompanyName = i.CompanyName
            }).ToList();
            Vendors.Insert(0, new VendorList { VendorUID = "", CompanyName = "--Select Vendor--" });
            return Vendors;
        }
        public static List<ServiceAreaList> GetServiceAreaList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var ServiceArea = db.ServiceAreas.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.Current.User.Identity.Name)).AsEnumerable().Select(i => new ServiceAreaList
            {
                ServiceAreaUID = i.ServiceAreaUID.ToString(),
                AreaDescription = i.AreaDescription
            }).ToList();
            ServiceArea.Insert(0, new ServiceAreaList { ServiceAreaUID = "", AreaDescription = "--Select Service Area--" });
            return ServiceArea;
        }
        public static List<ManufactureList> GetManufaturerList()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Manufacture = db.EquipManufacturers.AsEnumerable().Select(i => new ManufactureList
            {
                ManufactureId = i.Manufacturer,
                Manufacture = i.Manufacturer
            }).ToList();
            Manufacture.Insert(0, new ManufactureList { ManufactureId = "", Manufacture = "--Select Manufacturer--" });
            return Manufacture;
        }


        public static List<MonthList> GetMonths()
        {
            var lst = new List<MonthList>();
            lst.Insert(0, new MonthList { Month = "Select", MonthInt = "0" });
            lst.Insert(1, new MonthList { Month = "Jan", MonthInt = "1" });
            lst.Insert(2, new MonthList { Month = "Feb", MonthInt = "2" });
            lst.Insert(3, new MonthList { Month = "Mar", MonthInt = "3" });
            lst.Insert(4, new MonthList { Month = "Apr", MonthInt = "4" });
            lst.Insert(5, new MonthList { Month = "May", MonthInt = "5" });
            lst.Insert(6, new MonthList { Month = "Jun", MonthInt = "6" });
            lst.Insert(7, new MonthList { Month = "Jul", MonthInt = "7" });
            lst.Insert(8, new MonthList { Month = "Aug", MonthInt = "8" });
            lst.Insert(9, new MonthList { Month = "Sep", MonthInt = "9" });
            lst.Insert(10, new MonthList { Month = "Oct", MonthInt = "10" });
            lst.Insert(11, new MonthList { Month = "Nov", MonthInt = "11" });
            lst.Insert(12, new MonthList { Month = "Dec", MonthInt = "12" });
            return lst;
        }

        public static List<MonthList> GetServiceYear()
        {
            var lst = new List<MonthList>();
            lst.Insert(0, new MonthList { Month = "1", MonthInt = "1" });
            lst.Insert(1, new MonthList { Month = "2", MonthInt = "2" });
            return lst;
        }

        public static List<MonthYears> GetMonthyearsData()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Monthyear = db.FutureMonths().AsEnumerable().Select(i => new MonthYears
            {
                Val = i.val.ToString(),
                Description = i.descrip
            }).ToList();
            Monthyear.Insert(0, new MonthYears { Val = "", Description = "--Select Month--" });
            return Monthyear;
        }
    }
}