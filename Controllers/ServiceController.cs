using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDS.DB;
using MDS.Models;
using MDS.Helper;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web.Security;

namespace MDS.Controllers
{
    public class ServiceController : BaseController
    {
        private int CurrentBranchID()
        {
            Object oo = HttpContext.User.Identity.Name;
            return LoginController.BranchID(oo.ToString());
            //System.
        }

        //
        // GET: /Service/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All ServiceJob Request", 0, Request);

            var model = new ServiceSearch();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.SelCnt = 100;
            

            model.Invoice = "E";

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                Session["S_Cust"] = Cust;
                model.ServiceWork = "C";
            }
            else
            {
                if (Session["S_ServiceWorkComplete"] != null)
                {
                    model.ServiceWork = Session["S_ServiceWorkComplete"].ToString();
                }
                else
                {
                    model.ServiceWork = "I"; //model.ServiceWork = "";
                }

            }
            if (Session["S_Cust"] != null)
            {
                model.Customerid = Session["S_Cust"].ToString();
            }
            else
            {
                model.Customerid = "";
            }

            if (Session["S_Custdep"] != null)
            {
                model.Department = Session["S_Custdep"].ToString();
            }
            else
            {
                model.Department = "";
            }


            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            if (model.Locationid == "--Location--")
                model.Locationid = "";
            if (model.Department == "--Select Customer Site--")
                model.Department = "";


            var servicedata = db.ServiceJobSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), model.Customerid, model.Department, -1, model.Branchid, model.ServiceWork, "E", model.Locationid, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false,false,model.SelCnt).Select(i => new ServicesearchList
            {
                ServiceJobUID = i.ServiceJobUID.ToString(),
                Customer = i.Customer,
                JobCode = i.JobCode,
                DateStart = i.DateStart.HasValue ? i.DateStart.Value : (DateTime?)null,
                DateProgrammed = i.DateProgrammed.HasValue ? i.DateProgrammed.Value : (DateTime?)null,
                EngineerName = i.Engineername,
                CustomerSite = i.CustomerSite
            }).ToList();
            model.Service = servicedata;
            model.FromDate = DateTime.Now.AddMonths(-1);
            model.ToDate = DateTime.Now;

            model.OutFromDate = DateTime.Now.AddMonths(-1);
            model.OutToDate = DateTime.Now;

            return View(model);
        }
        [Authorize]
        public JsonResult GetEquipmentTypeSite(String Customerid, Int32 Branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
     
            
            var Customersite = db.EquipTypeModels.Where(i => i.BranchID == Branchid && i.CustomerCode==Customerid ).AsEnumerable().Select(i => 
             new EquipType
            {
                EquipTypeCode = i.EquipTypeCode,
                Name = i.Name
            }).ToList();
           Customersite.Insert(0, new EquipType { EquipTypeCode = "", Name = "--Select Equip Type--" });
            return Json(Customersite.AsEnumerable().ToList(), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetCustomerSite(String Customerid,string Branchid, string userFilter)
        {
            TrackerDataContext db = new TrackerDataContext();
            List<CustomerSite> Customersite;
            if (userFilter == null) userFilter = "";
            {
                Customersite = db.CustomerDepartments.Where(i => (i.Department.StartsWith(userFilter) || userFilter=="") && (i.CustomerCode == Customerid || Customerid == "-1") && (i.BranchID == CurrentBranchID())).OrderBy(i => i.Department).AsEnumerable().Select(i => new MDS.Models.CustomerSite
                {
                    Department = i.Department,
                    DeptID = i.DeptID.ToString()
                }).ToList();
            }
//            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID = "-1", Department = "--Select Customer Site--" });
            return Json(Customersite, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public JsonResult GetCustomerSiteX(String Customerid,string Branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            List<CustomerSite> Customersite;
            if ((Branchid == null) || (Branchid == ""))
                Branchid = CurrentBranchID().ToString();
            if (Customerid == "-1")
            {
                Customersite = db.CustomerDepartments.Where(i => i.BranchID ==Convert.ToInt32( Branchid)).OrderBy(i=>i.Department).AsEnumerable().Select(i => new MDS.Models.CustomerSite
                {
                    Department = i.Department,
                    DeptID = i.DeptID.ToString()
                }).ToList();
            }
            else
            {
                Customersite = db.CustomerDepartments.Where(i => i.CustomerCode == Customerid && i.BranchID ==Convert.ToInt32( Branchid)).OrderBy(i => i.Department).AsEnumerable().Select(i => new MDS.Models.CustomerSite
                {
                    Department = i.Department,
                    DeptID = i.DeptID.ToString()
                }).ToList();
            }
            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID ="", Department = "--Select Customer Site--" });
            return Json(Customersite, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult GetLocationList(String Customerid, string userFilter)
        {
            return GetLocationListX(Customerid, CurrentBranchID());
        }
        [Authorize]
        public JsonResult GetLocationListX(String Customerid, Int32 BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Locationlist = db.CustomerLocations.Where(i => i.CustomerCode == Customerid && i.BranchID == BranchID).OrderBy(i => i.Location).AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                 LocationID = i.LocationID.ToString()
            }).ToList();
            Locationlist.Insert(0, new LocationList { LocationID = "-1", Location = "--Select Location--" });
            return Json(Locationlist, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult GetCustomerLocation(String Customerid, Int32 Branchid, string userFilter)
        {
            if (userFilter == null) userFilter = "";
             TrackerDataContext db = new TrackerDataContext();
            var CustomerLocation = db.CustomerLocations.Where(i => (i.LocationID.ToString().Contains(userFilter) || i.Location.Contains(userFilter) || userFilter=="") && (i.CustomerCode == Customerid || Customerid=="-1") && (i.Location!="") && i.BranchID == Branchid).OrderBy(i=>i.Location).AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                LocationID = i.LocationID.ToString()
            }).ToList();
//            CustomerLocation.Insert(0, new LocationList { LocationID = "-1", Location = "--Location--" });
            return Json(CustomerLocation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerLocationBlank(String Customerid, Int32 Branchid)
        {
            //if (userFilter == null) userFilte//r = "";
            TrackerDataContext db = new TrackerDataContext();
            var CustomerLocation = db.CustomerLocations.Where(i =>  (i.CustomerCode == Customerid || Customerid == "-1") && (i.Location != "") && i.BranchID == Branchid).OrderBy(i => i.Location).AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                LocationID = i.LocationID.ToString()
            }).ToList();
            CustomerLocation.Insert(0, new LocationList { LocationID = "-1", Location = "--Location--" });
            return Json(CustomerLocation, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(ServiceSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search ServiceJob Request", 0, Request);

            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                Session["S_Cust"] = Cust;
                model.Customerid = Cust;
                model.ServiceWork = "C";
            }
            else
            {
                if (model.Customerid == null)
                {
                    model.Customerid = "";
                }
                if (model.ServiceWork == null)
                {
                    model.ServiceWork = "";
                }
            }
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }

            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }
            if (model.FromDate.ToShortDateString() == "1/01/0001")
            {
                model.FromDate = DateTime.Today;
            }
            if (model.ToDate.ToShortDateString() == "1/01/0001")
            {
                model.ToDate = DateTime.Today;
            }
            if (model.OutFromDate.ToShortDateString() == "1/01/0001")
            {
                model.OutFromDate = DateTime.Today;
            }
            if (model.OutToDate.ToShortDateString() == "1/01/0001")
            {
                model.OutToDate = DateTime.Today;
            }
            if (model.ServiceJob == null)
            {
                model.ServiceJob = "";
            }
            if (model.CustomerOrderNo == null)
            {
                model.CustomerOrderNo = "";
            }
            
            if (model.Invoice == null)
            {
                model.Invoice = "";
            }
            var Customer = model.Customerid;
            if (Customer == "-1")
                Customer = "";
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var FromDate = model.FromDate;
            var ToDate = model.ToDate;
            var OutDateFrom = model.OutFromDate;
            var OutDateIn = model.OutToDate;
            var Location = model.Locationid;
            var ServiceJob = model.ServiceJob;
            
            var CustomerOrderNo = model.CustomerOrderNo;
            var ServiceWork = model.ServiceWork;

            var Invoice = model.Invoice;
            Session["S_Cust"] = Customer;
            Session["S_Custdep"] = CustDepartment;
            Session["S_ServiceWorkComplete"]=model.ServiceWork;
            TrackerDataContext db = new TrackerDataContext();

            if (Location == "--Location--")
                Location = "";
            if (CustDepartment == "--Select Customer Site--")
                CustDepartment = "";

            var servicedata = db.ServiceJobSearch(FromDate, ToDate, Customer, CustDepartment, Convert.ToInt16(Engineerid), model.Branchid, ServiceWork, Invoice, Location, ServiceJob, CustomerOrderNo, OutDateFrom, OutDateIn, model.DateInFilter, model.DateOutFilter,false,model.SelCnt).Select(i => new ServicesearchList
            {
                ServiceJobUID = i.ServiceJobUID.ToString(),
                Customer = i.Customer,
                JobCode = i.JobCode,
                DateStart = i.DateStart.HasValue ? i.DateStart.Value : (DateTime?)null,
                DateProgrammed = i.DateProgrammed.HasValue ? i.DateProgrammed.Value : (DateTime?)null,
                EngineerName = i.Engineername,
                CustomerSite = i.CustomerSite

            }).ToList();
            model.Service = servicedata;

            model.FromDate = DateTime.Now.AddMonths(-1);
            model.ToDate = DateTime.Now;

            model.OutFromDate = DateTime.Now.AddMonths(-1);
            model.OutToDate = DateTime.Now;
            return View(model);
        }

        

       
        [Authorize]
        public ActionResult GetRepairForService(Int32 ServiceJobID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get Repair for ServiceJob Request", 0, Request);

            var model = new RepairSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationList();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceJobID = ServiceJobID.ToString();

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 120;
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if (model.SelCnt == 0) model.SelCnt = 100;

            var s = db.RepairsSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), "", "", LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToChar("E"), Convert.ToChar("E"), ServiceJobID, -1, "", "", -1, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false,model.SelCnt).Select(i => new RepairSearchList
            {
                Customer = i.Customer,
                RepairUID = i.RepairUID,
                DateIn = i.DateIn.HasValue ? i.DateIn.Value : (DateTime?)null,
                DateOut = i.DateOut.HasValue ? i.DateOut.Value : (DateTime?)null,
                Equipment = i.EquipmentType,
                OrderNumber = i.OrderNumber,
                JobCode = i.JObCode,
                Engineer=i.EngineerName,
                    RepairCompleted=i.RepairCompleted,
                TotalCharge =Convert.ToDecimal( i.RepairTravelExpenseCost)+i.PartsCost,
                FaultDetails = i.FaultDetails,
                WorkDone = i.WorkDone,
                RepairTravelExpenseCost = i.RepairTravelExpenseCost,
                PartsCost = i.PartsCost,
                WarrantyExpirationDate = i.WarrantyExpirationDate


            }).ToList();
            model.Repairs = s;

            var sj = Utility.ServiceJobDetailbyId(ServiceJobID);
            if (sj.Count > 0)
            {
                model.ServiceData = sj[0].JobCode.ToString() + "," + sj[0].Customer + "," + sj[0].CustomerSite;
            }
            else
            {
                model.ServiceData = "";
            }
            ViewBag.PopUpRepair = ServiceJobID;
            Session["ServiceJobID"] = ServiceJobID;
            return PartialView(model);
        }
        [Authorize]
        public ActionResult GetWOForService(Int32 ServiceJobID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get Work Order for ServiceJob Request", 0, Request);

            var model = new ServiceWorkOderSearch();
            model.ServiceJobID = ServiceJobID.ToString();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
             model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if (model.SelCnt == 0) model.SelCnt = 100;

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }
            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.ServicesAfterDate == null)
            {
                model.ServicesAfterDate = Convert.ToDateTime("2000/1/1");
            }

            if (model.Location == null)
            {
                model.Location = "";
            }

            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;
            var Location = model.Location;

            TrackerDataContext db = new TrackerDataContext();

            if (Location == "--Location--")
                Location = "";
            if (CustDepartment == "--Select Customer Site--")
                CustDepartment = "";

            var serviceworkorder = db.ServiceWorkOrderSearchNew(Equiptype, Convert.ToInt32(ServiceJobID), Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, false, model.ServicesAfterDate, Location, -1, model.ServicesAfterDateFilter, model.SelCnt).Select(i => new ServiceworkorderList
            {
                CompanyName = i.CompanyName,
                EngineerName = i.EngineerName,
                Equipment = i.EquipDesc,
                DateServiced = i.DateServiced.HasValue ? i.DateServiced.Value : (DateTime?)null,
                ServiceArea = i.ServiceArea,
                Location = i.Location,
                WorkDone = i.WorkDone,
                ServiceId = i.ServiceUID,
                NotesForNextService = i.NotesForNextService,
                NotesForThisService = i.NotesForThisService,
                PartsCost = i.PartsCost,
                WarrantyExpirationDate = i.WarrantyExpirationDate
            }).ToList();
            model.ServiceWorkOrder = serviceworkorder;

            var sj = Utility.ServiceJobDetailbyId(ServiceJobID);
            if (sj.Count > 0)
            {
                model.ServiceData = sj[0].JobCode.ToString() + "," + sj[0].Customer + "," + sj[0].CustomerSite;
            }
            else
            {
                model.ServiceData = "";
            }
            ViewBag.PopUpService = ServiceJobID;
            Session["ServiceJobID"] = ServiceJobID;
            return PartialView(model);
        }

        [Authorize]
        public ActionResult GetWOForServicenew(Int32 Id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get Work Order for ServiceJob Request", 0, Request);

            var model = new ServiceWorkOderSearch();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }
            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.ServicesAfterDate == null)
            {
                model.ServicesAfterDate = Convert.ToDateTime("2000/1/1");
            }
            else if (model.ServicesAfterDate.Year==1)
                model.ServicesAfterDate = Convert.ToDateTime("2000/1/1");
        
            if (model.Location == null)
            {
                model.Location = "";
            }

            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;
            model.ServiceJobID = Id.ToString();
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if (Session["SelCnt_WOService"] != null)
            {
                model.SelCnt =Convert.ToInt32(Session["SelCnt_WOService"]);
            }

            if (model.SelCnt==0) model.SelCnt = 400;
            TrackerDataContext db = new TrackerDataContext();

            var Location = model.Location;
            if (Location == "--Location--")
                Location = "";
            if (CustDepartment == "--Select Customer Site--")
                CustDepartment = "";

            var serviceworkorder = db.ServiceWorkOrderSearchNew(Equiptype, Convert.ToInt32(Id), Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, false, model.ServicesAfterDate, Location, -1, model.ServicesAfterDateFilter, model.SelCnt).Select(i => new ServiceworkorderList
            {
                CompanyName = i.CompanyName,
                EngineerName = i.EngineerName,
                Equipment = i.EquipDesc,
                DateServiced = i.DateServiced.HasValue ? i.DateServiced.Value : (DateTime?)null,
                ServiceArea = i.ServiceArea,
                Location=i.Location,
                WorkDone = i.WorkDone,
                ServiceId = i.ServiceUID,
                NotesForNextService = i.NotesForNextService,
                NotesForThisService = i.NotesForThisService,
                PartsCost = i.PartsCost,
                WarrantyExpirationDate = i.WarrantyExpirationDate
            }).ToList();
            model.ServiceWorkOrder = serviceworkorder;

            var sj = Utility.ServiceJobDetailbyId(Id);
            if (sj.Count > 0)
            {
                model.ServiceData = sj[0].JobCode.ToString() + "," + sj[0].Customer + "," + sj[0].CustomerSite;
            }
            else
            {
                model.ServiceData = "";
            }
            ViewBag.PopUpService = Id;
            Session["ServiceJobID"] = Id;
            return PartialView("~/Views/Service/GetWOForService.cshtml",model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult GetWOForService(ServiceWorkOderSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get Work Order for ServiceJob Submit", 0, Request);

            model.EquipTypeList = Utility.GetEquipTypeList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }
            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.ServicesAfterDate == null)
            {
                model.ServicesAfterDate = Convert.ToDateTime("2000/1/1");
            }

            if (model.Location == null)
            {
                model.Location = "";
            }

            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;

            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if (model.SelCnt == 0) model.SelCnt = 400;

            Session["SelCnt_WOService"] = model.SelCnt;


            TrackerDataContext db = new TrackerDataContext();
            var Location=model.Location;
            if (Location == "--Location--")
                Location = "";
            if (CustDepartment == "--Select Customer Site--")
                CustDepartment = "";

            var serviceworkorder = db.ServiceWorkOrderSearchNew(Equiptype, Convert.ToInt32(model.ServiceJobID), Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, false, model.ServicesAfterDate, Location, -1, model.ServicesAfterDateFilter, model.SelCnt).Select(i => new ServiceworkorderList
            {
                CompanyName = i.CompanyName,
                EngineerName = i.EngineerName,
                Equipment = i.EquipDesc,
                Location=i.Location,
                DateServiced = i.DateServiced.HasValue ? i.DateServiced.Value : (DateTime?)null,
                ServiceArea = i.ServiceArea,
                WorkDone = i.WorkDone,
                ServiceId = i.ServiceUID,
                NotesForNextService = i.NotesForNextService,
                NotesForThisService = i.NotesForThisService,
                PartsCost = i.PartsCost,
                WarrantyExpirationDate = i.WarrantyExpirationDate
            }).ToList();
            model.ServiceWorkOrder = serviceworkorder;

            var sj = Utility.ServiceJobDetailbyId(Convert.ToInt32(model.ServiceJobID));
            if (sj.Count > 0)
            {
                model.ServiceData = sj[0].JobCode.ToString() + "," + sj[0].Customer + "," + sj[0].CustomerSite;
            }
            else
            {
                model.ServiceData = "";
            }
            ViewBag.PopUpService = model.ServiceJobID;
            Session["ServiceJobID"] = model.ServiceJobID;
            return PartialView(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchRepairData(RepairSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search Repair for ServiceJob Submit", 0, Request);

            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.LocationList = Utility.GetLocationList();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if (model.SelCnt == 0) model.SelCnt = 100;

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            if (model.FromDate.ToShortDateString() == "1/01/0001")
            {
                model.FromDate = DateTime.Today;
            }
            if (model.ToDate.ToShortDateString() == "1/01/0001")
            {
                model.ToDate = DateTime.Today;
            }
            if (model.OutFromDate.ToShortDateString() == "1/01/0001")
            {
                model.OutFromDate = DateTime.Today;
            }
            if (model.OutToDate.ToShortDateString() == "1/01/0001")
            {
                model.OutToDate = DateTime.Today;
            }

            if (model.Customerid != null)
            {
                var Customer = model.Customerid;
                var Equip = model.Equiptype;
                DateTime? FromDate = model.FromDate;
                DateTime? ToDate = model.ToDate;
                var Location = model.Locationid;
                var OutFromDate = model.OutFromDate;
                var OutToDate = model.OutToDate;
                var Customersite = model.Department;
                var Engineer = Convert.ToInt32(model.EngineerID);
                var Branch = model.Branchid;
                var CustomerOrderNo = model.CustomerOrderNo;
                var RepairJob = model.RepairJob;
                var Repair = model.Resolved;
                var Handover = model.Complete;
                var ServiceJobID = Convert.ToInt32(model.ServiceJobID);
                var EquipID = Convert.ToInt32(model.SelectedEquipment);
                TrackerDataContext db = new TrackerDataContext();
                db.CommandTimeout = 120;
                if (Location == "--Location--")
                    Location = "";
                if (Customersite == "--Select Customer Site--")
                    Customersite = "";
                var s = db.RepairsSearch(FromDate, ToDate, Customer, Equip, LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToChar(Repair), Convert.ToChar(Handover), ServiceJobID, EquipID, Customersite, Location, Engineer, CustomerOrderNo, RepairJob, OutFromDate, OutToDate, model.DateInFilter, model.DateOutFilter,model.SelCnt).Select(i => new RepairSearchList
                {
                    Customer = i.Customer,
                    RepairUID = i.RepairUID,
                    DateIn = i.DateIn.HasValue ? i.DateIn.Value : (DateTime?)null,
                    DateOut = i.DateOut.HasValue ? i.DateOut.Value : (DateTime?)null,
                    Equipment = i.EquipmentType,
                    OrderNumber = i.OrderNumber,
                    JobCode = i.JObCode,
                    Engineer = i.EngineerName,
                    RepairCompleted = i.RepairCompleted ,
                    TotalCharge =Convert.ToDecimal( i.RepairTravelExpenseCost)+i.PartsCost,
                    FaultDetails = i.FaultDetails,
                    WorkDone = i.WorkDone,
                    RepairTravelExpenseCost = i.RepairTravelExpenseCost,
                    PartsCost = i.PartsCost,
                    WarrantyExpirationDate = i.WarrantyExpirationDate

                }).ToList();
                model.Repairs = s;
            }

            return View("GetRepairForService", model);
        }
        [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add ServiceJob Request", 0, Request);
            Session["bytespdf"] = null;

            var model = new AddService();
            model.TravelList = Utility.GetTravelList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.CustomerList = Utility.GetCustomerList();
            model.ChargeType = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.LabourRate = "$0.00";
            model.TravelRate = "$0.00";
            model.TravelHours = 0;
            model.LabourHours = 0;
            model.CustomerSignatureX = false;
            TrackerDataContext db = new TrackerDataContext();
            int BranchID=LoginController.BranchID(HttpContext.User.Identity.Name);
            var JobNo = db.ServiceJobs.Where(i => i.BranchID == BranchID).Max(i => i.JobCode) + 1;
            if (JobNo == null)
                JobNo = 1;
            model.JobCode = Utility.GetBranchCode() + "S" + JobNo;
            Session["NewServiceID"] = 0;
            model.Engineers = new List<int?>();
            model.Engineers.Add(0);
            model.Engineers.Add(0);
            model.Engineers.Add(0);
            ViewBag.EngineerIDX = db.Engineers.Where(ee => ee.BranchID == BranchID).ToList();
            ViewBag.ProgrammedEndX = FillHourMinDDL().ToList();
            ViewBag.ProgrammedTimeX = FillHourMinDDL().ToList();

            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id, int? BranchID)
        {
            var data = HttpContext.User.Identity.Name;
            ATC atc = MDS.Controllers.LoginController.IsAdmin(data);

            if (BranchID != null)
            {
                if (BranchID != LoginController.BranchID(User.Identity.Name))
                //return RedirectToAction("Select", "Login", new { UserName = MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name), BranchID = BranchID, Admin = "A" });
                {
                    if (atc== ATC.Tech)
                        FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name) + "," + "T", true);
                    else
                        FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name) + "," + "A", true);
                    return RedirectToAction("Edit", new { id = id, BranchID = BranchID });
                }
            }
            if (atc == ATC.Tech)
                return RedirectToAction("Service", "Tech", new { ServiceJobUID = id, BranchID = BranchID });
            Utility.Audit(HttpContext.User.Identity.Name, "Edit ServiceJob Request", id, Request);
            var model = new AddService();
            model = GetEditService(id);
            Session["NewServiceID"] = id;
            Session["bytespdf"] = null;
            model.PopUp = false;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUp(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit ServiceJob Request", id, Request);

            var model = new AddService();
            model = GetEditService(id);
            Session["NewServiceID"] = id;
            Session["bytespdf"] = null;

            model.PopUp = true;
            ViewBag.Popup = 1;
            return View("Add", model);
        }

        private void setNewServiceJobIDFromSession(AddService model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add ServiceJob Request", 0, Request);

            if (model.ServiceId == 0)
            {
                int NewServiceID = Convert.ToInt32(Session["NewServiceID"]);
                if (NewServiceID != 0)
                {
                    model.ServiceId = NewServiceID;

                }
            }
        }

        [Authorize]
        public AddService GetEditService(int id)
        {
            var model = new AddService();
            TrackerDataContext db = new TrackerDataContext();
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            var engs = db.ServiceJobEngineers.Where(i => i.BranchID == BranchID && i.ServiceJobID == id).Select(i => i.EngineerID).ToList();
            model.Engineers = new List<int?>();
            foreach (var item in engs)
            {
                model.Engineers.Add(item);
            }
            while (model.Engineers.Count < 3)
            {
                model.Engineers.Add(0);
            }
            Utility.Audit(HttpContext.User.Identity.Name, "Edit ServiceJob Request", id, Request);
            ViewBag.EngineerIDX = db.Engineers.Where(ee => ee.BranchID == BranchID).ToList();
            model.CustomerList = Utility.GetCustomerList();
            model.ChargeType = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.TravelList = Utility.GetTravelList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));

            Session["NewServiceID"] = id;
            var ServiceData = db.ServiceJobs.Where(i => i.ServiceJobUID == id && i.BranchID == BranchID).ToList();
            model.ServiceId = id;

            //if (ServiceData[0].ActualDateEnd.HasValue)
            //{
            //    model.ServiceCompleteDate = ServiceData[0].ActualDateEnd.Value;
            //    model.ServiceCompleteTime = ServiceData[0].ActualDateEnd.Value;

            //}
            //if (ServiceData[0].ActualDateStart.HasValue)
            //{
            //    model.ServiceStartDate = ServiceData[0].ActualDateStart.Value;
            //    model.ServiceStartTime = ServiceData[0].ActualDateStart.Value;
            //}
            if (ServiceData[0].InvoiceDate.HasValue)
            {
                model.InvoiceDate = ServiceData[0].InvoiceDate.Value;
            }
            if (ServiceData[0].DateProgrammed.HasValue)
            {
                model.ProgrammedDate = ServiceData[0].DateProgrammed.Value.Date;
                model.ProgrammedTime = ServiceData[0].DateProgrammed.Value.Hour * 100;
                if (ServiceData[0].DateProgrammed.Value.Minute > 45)
                    model.ProgrammedTime = model.ProgrammedTime + 100;
                else if (ServiceData[0].DateProgrammed.Value.Minute > 15)
                    model.ProgrammedTime = model.ProgrammedTime + 30;
            }
            else
                model.ProgrammedTime = 1200;
            if (ServiceData[0].DateProgrammedEnd.HasValue)
            {

                model.ProgrammedEnd = ServiceData[0].DateProgrammedEnd.Value.Hour * 100;
                if (ServiceData[0].DateProgrammedEnd.Value.Minute > 45)
                    model.ProgrammedEnd = model.ProgrammedEnd + 100;
                else if (ServiceData[0].DateProgrammedEnd.Value.Minute > 15)
                    model.ProgrammedEnd = model.ProgrammedEnd + 30;
            }
            else
                model.ProgrammedEnd = 1200;
            ViewBag.ProgrammedEndX = FillHourMinDDL().ToList();
            ViewBag.ProgrammedTimeX = FillHourMinDDL().ToList();
            model.JobCode = Utility.GetBranchCode() + "S" + ServiceData[0].JobCode;
            model.CustomerSignatureX = ServiceData[0].CustomerSignature;
            model.IsaApprove = ServiceData[0].ApprovalRequired;
            model.Isorder = ServiceData[0].OrderNumberRequired;
            model.TechnicalContact = ServiceData[0].TechContact;
            model.ApprovalContact = ServiceData[0].ApprovalContact;
            model.BookingNotes = ServiceData[0].BookingNotes;
            model.Customerid = ServiceData[0].CustomerCode;
            model.VerbalApproval = ServiceData[0].VerbalApprovalObtained;
            model.IsApprovalReceived = ServiceData[0].ApprovedBy;
            model.OrderNo = ServiceData[0].OrderNumber;
            model.SpecialNotes = ServiceData[0].ServiceNotes;
            model.CustomerSignature = ServiceData[0].ClientSignature;
            model.LocationID = ServiceData[0].LocationID.ToString();
            model.TravelHours = ServiceData[0].TravelHours;
            model.Invoice = ServiceData[0].InvoiceNumber;
            model.Charges = ServiceData[0].Amount;
            model.DeptID = Convert.ToString(ServiceData[0].DeptID);
            //       model.EngineerID = ServiceData[0].EngineerID.ToString();
            model.DontChangeCust = ServiceData[0].NoCharge;
            model.PropotionalExpenses = ServiceData[0].ExpensesProportion;
            model.TravelTypeCode = ServiceData[0].TravelTypecode;
            model.HasJobInvoice = ServiceData[0].HasBeenInvoiced;
            model.Branchid = Convert.ToString(ServiceData[0].BranchID);
            model.ServiceComplete_Outstanding = ServiceData[0].CompletedOutstanding;
            model.ServiceComplete_BERED = ServiceData[0].CompletedBERs;
            model.ServiceComplete = ServiceData[0].CompletedOK;
            model.TravelRate = ServiceData[0].TravelChargeRate.HasValue ? ServiceData[0].TravelChargeRate.Value.ToString("C") : "$0.00";
            model.LabourRate = ServiceData[0].ChargeRate.HasValue ? ServiceData[0].ChargeRate.Value.ToString("C") : "$0.00";
            model.ChargeTypeCode = ServiceData[0].ChargeTypecode;
            model.LabourHours = ServiceData[0].ServiceHours;

            model.CostOfServicePartsForServiceJob = Math.Round(db.CostOfServicePartsForServiceJob(id, LoginController.BranchID(HttpContext.User.Identity.Name)).Value, 2);
            model.CostOfTotalRepairsForServiceJob = Math.Round(db.CostOfTotalRepairsForServiceJob(id, LoginController.BranchID(HttpContext.User.Identity.Name)).Value, 2);
            model.ChargeTypeHours = LabourSummary(id, db, BranchID,false);

            return model;
        }

        public ActionResult ServiceWorkTimes(int ServiceJobID,int BranchID,bool? Repair)
        {
            if (Repair == null)
                Repair = false;
            TrackerDataContext db = new TrackerDataContext();

            var WorkTimes = (from sew in db.ServiceEngineerWorks
                               join eng in db.Engineers on new { EngineerID = sew.EngineerID, BranchID = sew.BranchID }
                                                           equals new { EngineerID = eng.EngineerID, BranchID = eng.BranchID.GetValueOrDefault() }
                               join chrg in db.ChargeTypes on new { ChargeTypecode = sew.ChargeTypecode, BranchID = sew.BranchID }
                                                           equals new { ChargeTypecode = chrg.ChargeTypecode, BranchID = chrg.BranchID } into gj
                             from subchrg in gj.DefaultIfEmpty()

                             where sew.Repair== Repair && sew.ServiceJobID == ServiceJobID && sew.BranchID == BranchID
                               select new ServiceWorkTime { EngineerID = eng.EngineerID, Engineer = eng.EngineerName, ChargeTypecode = subchrg.ChargeTypecode, ChargeType = subchrg.ChargeType1, StartTime = sew.StartTime, EndTime = sew.EndTime }
                      ).ToList();
            return View(WorkTimes);
        }

        public JsonResult ServiceWorkEngTimesDelete(int ServiceJobID, int BranchID,int EngineerID, DateTime StartTime,bool? Repair)
        {
            if (Repair == null)
                Repair = false;
            TrackerDataContext db = new TrackerDataContext();
            var sew=db.ServiceEngineerWorks.Where(i => i.Repair== Repair && i.BranchID == BranchID && i.ServiceJobID == ServiceJobID && i.EngineerID == EngineerID && i.StartTime == StartTime).FirstOrDefault();
            db.ServiceEngineerWorks.DeleteOnSubmit(sew);
            db.SubmitChanges();
            //DateTime StartTime;
            return Json(new { Error = "" }, JsonRequestBehavior.AllowGet);
        }

        public class HourMin
        {
            public decimal Time24Hour { get; set; }
            public string Desc { get; set; }
        }
        public ActionResult ServiceWorkEngTimes(int ServiceJobUID, int BranchID,bool Repair, int? EngineerID, string StartDate, string StartTime)
        {
            TrackerDataContext db = new TrackerDataContext();
            MDS.Models.ServiceWorkTime sew = new MDS.Models.ServiceWorkTime();
            if ((EngineerID==null) || (StartTime==null))
            {
                sew.BranchID = BranchID;
                sew.StartTime = DateTime.Now.Date;
                sew.StartTime2 = 900;
                sew.EndTime2 = 1200;
                sew.EndTime = DateTime.Now.Date;
                sew.ServiceJobID = ServiceJobUID;
                sew.Repair = Repair;
                sew.Exist = false;
            }
            else
            {
                sew.Exist = true;
                DateTime StartDateTime = Convert.ToDateTime(StartDate);
                StartDateTime = StartDateTime.AddHours(Convert.ToDateTime(StartTime).Hour);
                StartDateTime = StartDateTime.AddMinutes(Convert.ToDateTime(StartTime).Minute);
                StartDateTime = StartDateTime.AddSeconds(Convert.ToDateTime(StartTime).Second);


                var sewdb = db.ServiceEngineerWorks.Where(i => i.ServiceJobID == ServiceJobUID && i.BranchID == BranchID  && i.EngineerID == EngineerID && i.StartTime == StartDateTime && i.Repair == Repair).FirstOrDefault();
                sew.BranchID = sewdb.BranchID;
                sew.StartTime = sewdb.StartTime;
                sew.StartTime2 = 0;// sewdb.StartTime.Hour * 100 + sewdb.StartTime.Minute;

                //if (sewdb.StartTime.Minute > 45)
                //    sew.StartTime2 = sew.StartTime2 + 100;
                //else if (sewdb.StartTime.Minute > 15)
                //    sew.StartTime2 = sew.StartTime2 + 30;


                if (sewdb.EndTime == null)
                {
                    sew.EndTime = sewdb.StartTime.AddHours(1);
                    sew.EndTime2 = 0;

                }
                else
                {
                    sew.EndTime = sewdb.EndTime.Value;
                    sew.EndTime2 = 0;// sewdb.EndTime.Value.Hour * 100 + sewdb.EndTime.Value.Minute;
                    //if (sewdb.EndTime.Value.Minute > 45)
                    //    sew.EndTime2 = sew.EndTime2 + 100;
                    //else if (sewdb.EndTime.Value.Minute > 15)
                    //    sew.EndTime2 = sew.EndTime2 + 30;

                }
                sew.ServiceJobID = sewdb.ServiceJobID;
                sew.Repair = sewdb.Repair;
                sew.ChargeTypecode = sewdb.ChargeTypecode;
            }

            ViewBag.EngineerIDX = (from eng in db.Engineers where eng.BranchID == BranchID select new { EngineerID = eng.EngineerID, EngineerName = eng.EngineerName }).ToList();// (i => i.EngineerID).ToList();
            ViewBag.ChargeTypeX = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));

            ViewBag.StartTimeX = FillHourMinDDL().ToList();
            ViewBag.EndTimeX = FillHourMinDDL().ToList();

            return View(sew);
        }

        public static List<HourMin> FillHourMinDDL()
        {
            List<HourMin> hms = new List<HourMin>();
            for (int Hr = 7; Hr < 18; Hr++)
            {
                for (int min = 0; min < 60; min = min + 30)
                {
                    HourMin hm = new HourMin();
                    hm.Time24Hour = Hr * 100 + min;
                    hm.Desc = Hr.ToString("00") + ":" + min.ToString("00");
                    if (Hr < 12)
                        hm.Desc = hm.Desc + " am";
                    else
                        hm.Desc = hm.Desc + " pm";
                    hms.Add(hm);
                }
            }

            return hms;
        }

        public JsonResult ServiceWorkEngTimesSave(MDS.Models.ServiceWorkTime ret)
        {
            if (ret.EngineerID==null)
                return Json(new { Error = "Please select an engineer" }, JsonRequestBehavior.AllowGet);
            if (ret.StartTime==null)
                return Json(new { Error = "Please enter a valid start date" }, JsonRequestBehavior.AllowGet);
            if (ret.StartTime2 == null)
                return Json(new { Error = "Please enter a valid start time" }, JsonRequestBehavior.AllowGet);
            if (ret.EndTime == null)
                return Json(new { Error = "Please enter a valid end date" }, JsonRequestBehavior.AllowGet);
            if (ret.EndTime2 == null)
                return Json(new { Error = "Please enter a valid end time" }, JsonRequestBehavior.AllowGet);
            if (ret.ChargeTypecode == null)
                return Json(new { Error = "Please enter a charge type" }, JsonRequestBehavior.AllowGet);
            TrackerDataContext db = new TrackerDataContext();
            decimal hour;
            decimal min;

            DB.ServiceEngineerWork sew;
            if (ret.Exist == true)
            {

                DateTime StartDateTime = Convert.ToDateTime(ret.StartTime);


                //hour = Math.Floor((decimal)ret.StartTime2.Value / (decimal)100);
                //StartDateTime = StartDateTime.AddHours(Convert.ToInt32(hour));

                //min = ret.StartTime2.Value - hour * 100;
                //StartDateTime = StartDateTime.AddMinutes(Convert.ToInt32(min));

                sew = db.ServiceEngineerWorks.Where(i => i.ServiceJobID == ret.ServiceJobID && i.BranchID == ret.BranchID && i.EngineerID == ret.EngineerID && i.StartTime == StartDateTime && i.Repair == ret.Repair).FirstOrDefault();
                if (sew == null)
                    sew = db.ServiceEngineerWorks.Where(i => i.ServiceJobID == ret.ServiceJobID && i.BranchID == ret.BranchID && i.EngineerID == ret.EngineerID && i.EndTime == ret.EndTime && i.Repair == ret.Repair).FirstOrDefault();


            }
            else
            {
                sew = new ServiceEngineerWork();
                sew.BranchID = Convert.ToInt32(ret.BranchID);
                sew.ServiceJobID = Convert.ToInt32(ret.ServiceJobID);

                sew.StartTime = Convert.ToDateTime(ret.StartTime).Date;


                hour = Math.Floor((decimal)ret.StartTime2.Value / (decimal)100);
                sew.StartTime = Convert.ToDateTime(sew.StartTime).AddHours(Convert.ToInt32(hour));

                min = ret.StartTime2.Value - hour * 100;
                sew.StartTime = Convert.ToDateTime(sew.StartTime).AddMinutes(Convert.ToInt32(min));

                sew.EndTime = Convert.ToDateTime(ret.EndTime).Date;

                hour = Math.Floor((decimal)ret.EndTime2.Value / (decimal)100);
                sew.EndTime = Convert.ToDateTime(sew.EndTime).AddHours(Convert.ToInt32(hour));

                min = ret.EndTime2.Value - hour * 100;
                sew.EndTime = Convert.ToDateTime(sew.EndTime).AddMinutes(Convert.ToInt32(min));
                sew.Repair = ret.Repair;
                sew.EngineerID = Convert.ToInt32(ret.EngineerID);

            }

            sew.ChargeTypecode = ret.ChargeTypecode;
            if (ret.Exist!=true)
                db.ServiceEngineerWorks.InsertOnSubmit(sew);
            db.SubmitChanges();
            //save the data
            return Json(new { Error=""}, JsonRequestBehavior.AllowGet);

            //TrackerDataContext db = new TrackerDataContext();
            //MDS.Models.ServiceWorkTime sew = new MDS.Models.ServiceWorkTime();
            ////sew.EngineerID = LoginController.EngineerID(User.Identity.Name);
            ////ViewBag.EngineerIDX = db.ServiceJobEngineers.Where(i => i.BranchID == BranchID && i.ServiceJobID == ServiceJobUID).Select(i => i.EngineerID).ToList();
            //ViewBag.EngineerIDX = (from eng in db.Engineers where eng.BranchID == BranchID select new { EngineerID = eng.EngineerID, EngineerName = eng.EngineerName }).ToList();// (i => i.EngineerID).ToList();
            //sew.BranchID = BranchID;
            //ViewBag.ChargeTypeX = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            //sew.ServiceJobID = ServiceJobUID;
            //return View(sew);
        }

        public static List<ChargeTypeHour> LabourSummary(int id, TrackerDataContext db, int BranchID,bool? Repair)
        {
            if (Repair == null)
                Repair = false;
            List<ChargeTypeHour> xx = (from sew in db.ServiceEngineerWorks
                                       join chrg in db.ChargeTypes on new { ChargeTypecode = sew.ChargeTypecode, BranchID = sew.BranchID }
                                       equals new { ChargeTypecode = chrg.ChargeTypecode, BranchID = chrg.BranchID }
                                       into gj
                                       from subchrg in gj.DefaultIfEmpty()

                                       where sew.Repair==Repair && sew.ServiceJobID == id && sew.BranchID == BranchID
                                       && sew.StartTime != null && sew.EndTime != null
                                       select new ChargeTypeHour { ChargeRate = (subchrg.ChargeRate == null) ? 0 : Convert.ToDecimal(subchrg.ChargeRate), ChargeType = (subchrg.ChargeType1 == null) ? "Not specified" : subchrg.ChargeType1, StartTime = sew.StartTime, EndTime = sew.EndTime, Hours = 0 }
                                  ).ToList();
            double total = 0;
            double totalHours = 0;
            foreach (var sew in xx)
            {
                sew.Hours = Convert.ToDateTime(sew.EndTime).Subtract(Convert.ToDateTime(sew.StartTime)).TotalMinutes / (double)60;
                totalHours += sew.Hours;
                sew.Total = sew.Hours * Convert.ToDouble(sew.ChargeRate);
                total = total + sew.Total;
            }
            ChargeTypeHour Total = new ChargeTypeHour();
            Total.ChargeType = "Total";
            Total.Total = total;
            Total.Hours = totalHours;
            List<ChargeTypeHour> yy = (from x in xx
                                       group x by new { x.ChargeType, x.ChargeRate }
                                       into g
                                       select new ChargeTypeHour { ChargeType = g.Key.ChargeType, ChargeRate = g.Key.ChargeRate, Hours = g.Sum(i => i.Hours), Total = g.Sum(i => i.Total) }).ToList();
            yy.Add(Total);
            return yy;
        }

        [Authorize]
        public JsonResult GetEquipInfomation(int EquipUID)
        {

            TrackerDataContext db = new TrackerDataContext();
            var model = db.USP_GetEquipNameByEquipId(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (model.Count > 0)
            {
                return Json(new { CustomerInfo = model[0].CustomerInfo, CustomerCode = model[0].CustomerCode, EquipItem = model[0].EquipDesc }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { CustomerInfo = "", CustomerCode = "", EquipItem = "" }, JsonRequestBehavior.AllowGet);
            }


        }

        [Authorize]
        void UpdateService(AddService model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Update Service Submit", model.ServiceId, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            setNewServiceJobIDFromSession(model);
            var obj = db.ServiceJobs.Where(i => i.ServiceJobUID == Convert.ToInt32(model.ServiceId) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            //if (model.ServiceStartDate.HasValue)
            //{
            //    if (!model.ServiceStartTime.HasValue)
            //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value);
            //    else
            //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value).Date.AddHours(model.ServiceStartTime.Value.Hour).AddMinutes(model.ServiceStartTime.Value.Minute); 
            //}
            //if (model.ServiceCompleteDate.HasValue)
            //{
            //    if (!model.ServiceCompleteTime.HasValue)
            //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value);
            //    else
            //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value).Date.AddHours(model.ServiceCompleteTime.Value.Hour).AddMinutes(model.ServiceCompleteTime.Value.Minute);
            //}
            if (model.InvoiceDate.HasValue)
            {
                obj.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value);
            }
            if (model.ProgrammedDate.HasValue)
            {
                if (model.ProgrammedTime.HasValue)
                {
                    //obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value).AddHours(model.ProgrammedTime.Value.Hour).AddMinutes(model.ProgrammedTime.Value.Minute);

                    obj.DateProgrammed= model.ProgrammedDate.Value;
                    decimal hour = Math.Floor((decimal)model.ProgrammedTime.Value/(decimal)100);
                    obj.DateProgrammed= Convert.ToDateTime(obj.DateProgrammed).AddHours(Convert.ToInt32(hour));

                    decimal min = model.ProgrammedTime.Value - hour * 100;
                    obj.DateProgrammed= Convert.ToDateTime(obj.DateProgrammed).AddMinutes(Convert.ToInt32(min));

                }
                else
                {
                    obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value);

                }
                if (model.ProgrammedEnd.HasValue)
                {
                    //   obj.DateProgrammedEnd = Convert.ToDateTime(model.ProgrammedDate.Value).AddHours(model.ProgrammedEnd.Value.Hour).AddMinutes(model.ProgrammedEnd.Value.Minute);
                    obj.DateProgrammedEnd = model.ProgrammedDate.Value;
                    decimal hour = Math.Floor(Convert.ToDecimal( model.ProgrammedEnd.Value) / (decimal)100);
                    obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddHours(Convert.ToInt32(hour));

                    decimal min = Convert.ToDecimal(model.ProgrammedEnd.Value) - hour * 100;
                    obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddMinutes(Convert.ToInt32(min));




                }
            }
                obj.ApprovalContact = model.ApprovalContact;
            obj.TechContact = model.TechnicalContact;
            obj.ApprovalRequired = model.IsaApprove.Value;
            obj.ApprovedBy = model.IsApprovalReceived;
            obj.CustomerCode = model.Customerid;
            obj.VerbalApprovalObtained = model.VerbalApproval.Value;
            obj.BookingNotes = model.BookingNotes;
            obj.OrderNumber = model.OrderNo;
            obj.OrderNumberRequired = model.Isorder.Value;
            obj.ServiceNotes = model.SpecialNotes;
            obj.ClientSignature = model.CustomerSignature;
            obj.NoCharge = model.DontChangeCust;
            obj.LocationID = Convert.ToInt32(model.LocationID);
            obj.DeptID = Convert.ToInt32( model.DeptID);
            obj.InvoiceNumber = model.Invoice;
            obj.Amount = model.Charges;
            obj.TravelHours = model.TravelHours;
            obj.ExpensesProportion = model.PropotionalExpenses;
            obj.ChargeRate = model.LabourRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.LabourRate.Replace("$", ""));
            obj.TravelChargeRate = model.TravelRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.TravelRate.Replace("$", ""));
            obj.ChargeTypecode = model.ChargeTypeCode;
            obj.TravelTypecode = model.TravelTypeCode;
            obj.CompletedOutstanding = model.ServiceComplete_Outstanding;
            obj.CompletedBERs = model.ServiceComplete_BERED;
            obj.CompletedOK = model.ServiceComplete;
            obj.HasBeenInvoiced = model.HasJobInvoice;
            List<DB.ServiceJobEngineer> SjEng;
            SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            foreach (var sjitem in SjEng)
            {
                if (model.Engineers.Contains(sjitem.EngineerID))
                    ;
                else
                    db.ServiceJobEngineers.DeleteOnSubmit(sjitem);

            }
            db.SubmitChanges();
            SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();

            foreach (var item in model.Engineers)
            {
                if (item != null)
                {
                    int eng = Convert.ToInt32(item);
                    if (SjEng.Select(i => i.EngineerID).ToList().Contains(eng))
                        ;
                    else
                    {
                        ServiceJobEngineer sje = new ServiceJobEngineer();
                        sje.EngineerID = eng;
                        sje.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                        sje.ServiceJobID = model.ServiceId;
                        db.ServiceJobEngineers.InsertOnSubmit(sje);
                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            ex = ex;
                        }
                    }
                }
            }
            db.SubmitChanges();
            //thamstodo if (obj.EngineerID != Convert.ToUInt16(model.EngineerID))
            //{
            //    obj.EngineerID = Convert.ToUInt16(model.EngineerID);
            //    var serviceWorkOrders = db.Services.Where(ii => ii.ServiceJobUID == obj.ServiceJobUID).ToList();
            //    foreach (var serviceWorkOrder in serviceWorkOrders)
            //    {
            //        serviceWorkOrder.EngineerID =Convert.ToInt32( model.EngineerID);
            //    }
                    
            //}
            //obj.EngineerName = model.EngineerName;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            obj.ServiceHours = model.LabourHours;
            db.SubmitChanges();
            db.Dispose();
        }

        [Authorize]
        int SaveService(AddService model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Save Service Submit", model.ServiceId, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            ServiceJob obj = new ServiceJob();
            int JobNo;
            var sjjobno=db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode);
            if (sjjobno == null)
                JobNo = 1;
            else
                JobNo =Convert.ToInt32( sjjobno) + 1;
            int Serviceid;
            try
            {
                 Serviceid = db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceJobUID) + 1;
            }
            catch (Exception ex)
            {
                Serviceid = 1;
            }
            //if (JobNo != null)
          //  {
                obj.JobCode = JobNo;
            //}
            //else { obj.JobCode = 1; }
            obj.ServiceJobUID = Serviceid;
            Session["NewServiceID"] = Serviceid;
            obj.BNQLocationCode = Utility.GetBranchCode().Substring(0, 1);
            //if (model.ServiceStartDate.HasValue)
            //{
            //    if (!model.ServiceStartTime.HasValue)
            //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value);
            //    else
            //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value).Date.AddHours(model.ServiceStartTime.Value.Hour).AddMinutes(model.ServiceStartTime.Value.Minute);
            //}
            //if (model.ServiceCompleteDate.HasValue)
            //{
            //    if (!model.ServiceCompleteTime.HasValue)
            //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value);
            //    else
            //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value).Date.AddHours(model.ServiceCompleteTime.Value.Hour).AddMinutes(model.ServiceCompleteTime.Value.Minute);
            //}
            if (model.InvoiceDate.HasValue)
            {
                obj.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value);
            }
            if (model.ProgrammedDate.HasValue)
            {
                if (model.ProgrammedTime.HasValue)
                {
                    //obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value).AddHours(model.ProgrammedTime.Value.Hour).AddMinutes(model.ProgrammedTime.Value.Minute);


                    obj.DateProgrammed = model.ProgrammedDate.Value;
                    decimal hour = Math.Floor((decimal)model.ProgrammedTime.Value / (decimal)100);
                    obj.DateProgrammed = Convert.ToDateTime(obj.DateProgrammed).AddHours(Convert.ToInt32(hour));

                    decimal min = model.ProgrammedTime.Value - hour * 100;
                    obj.DateProgrammed = Convert.ToDateTime(obj.DateProgrammed).AddMinutes(Convert.ToInt32(min));

                }
                else
                    obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value);
                if (model.ProgrammedEnd.HasValue)
                {
                    //  obj.DateProgrammedEnd = Convert.ToDateTime(model.ProgrammedDate.Value).AddHours(model.ProgrammedEnd.Value.Hour).AddMinutes(model.ProgrammedEnd.Value.Minute);
                    obj.DateProgrammedEnd = model.ProgrammedDate.Value;
                    decimal hour = Math.Floor(Convert.ToDecimal(model.ProgrammedEnd.Value) / (decimal)100);
                    obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddHours(Convert.ToInt32(hour));

                    decimal min = Convert.ToDecimal(model.ProgrammedEnd.Value) - hour * 100;
                    obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddMinutes(Convert.ToInt32(min));

                }
            }
                obj.ApprovalContact = model.ApprovalContact;
            obj.TechContact = model.TechnicalContact;
            obj.ApprovalRequired = model.IsaApprove.Value;
            obj.ApprovedBy = model.IsApprovalReceived;
            obj.CustomerCode = model.Customerid;
            obj.VerbalApprovalObtained = model.VerbalApproval.Value;
            obj.BookingNotes = model.BookingNotes;
            obj.OrderNumber = model.OrderNo;
            obj.OrderNumberRequired = model.Isorder.Value;
            obj.ServiceNotes = model.SpecialNotes;
            obj.ClientSignature = model.CustomerSignature;
            obj.NoCharge = model.DontChangeCust;
            obj.LocationID = Convert.ToInt32( model.LocationID);
            obj.DeptID = Convert.ToInt32( model.DeptID);
            obj.InvoiceNumber = model.Invoice;
            obj.Amount = model.Charges;
            obj.TravelHours = model.TravelHours;
            obj.ExpensesProportion = model.PropotionalExpenses;
            obj.ChargeRate = model.LabourRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.LabourRate.Replace("$", ""));
            obj.TravelChargeRate = model.TravelRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.TravelRate.Replace("$", ""));
            obj.ChargeTypecode = model.ChargeTypeCode;
            obj.TravelTypecode = model.TravelTypeCode;
            obj.CompletedOutstanding = model.ServiceComplete_Outstanding;
            obj.CompletedBERs = model.ServiceComplete_BERED;
            obj.CompletedOK = model.ServiceComplete;
            obj.HasBeenInvoiced = model.HasJobInvoice;
            //           if (obj.EngineerID != Convert.ToUInt16(model.EngineerID))

            //    obj.EngineerID = Convert.ToUInt16(model.EngineerID);
            //}
            List<DB.ServiceJobEngineer> SjEng;
            SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == Serviceid && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            foreach (var sjitem in SjEng)
            {
                if (model.Engineers.Contains(sjitem.EngineerID))
                    ;
                else
                    db.ServiceJobEngineers.DeleteOnSubmit(sjitem);

            }
            db.SubmitChanges();
            SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == Serviceid && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();

            foreach (var item in model.Engineers)
            {
                if (item != null)
                {
                    int eng = Convert.ToInt32(item);
                    if (SjEng.Select(i => i.EngineerID).ToList().Contains(eng))
                        ;
                    else
                    {
                        ServiceJobEngineer sje = new ServiceJobEngineer();
                        sje.EngineerID = eng;
                        sje.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                        sje.ServiceJobID = Serviceid;
                        db.ServiceJobEngineers.InsertOnSubmit(sje);
                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            ex = ex;
                        }
                    }
                }
            }
            db.SubmitChanges();

            obj.ServiceHours = model.LabourHours;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            db.ServiceJobs.InsertOnSubmit(obj);
            db.SubmitChanges();
            db.Dispose();
            model.ServiceId = obj.ServiceJobUID;
            return obj.ServiceJobUID;

        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveUpdateService(AddService model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Save Service Submit", model.ServiceId, Request);

            setNewServiceJobIDFromSession(model);
            if (model.ServiceId != 0)
            {
                UpdateService(model);
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = "Service Job updated successfully." });
                }
                else
                {
                    return Json(new { msg = "1", message = "Service Job updated successfully." });
                }
            }
            else
            {
                SaveService(model);
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = "Service Job created successfully." });
                }
                else
                {
                    return Json(new { msg = "1", message = "Service Job created successfully." });
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(AddService model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Service Submit",model.ServiceId, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            model.CustomerList = Utility.GetCustomerList();
            model.ChargeType = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.TravelList = Utility.GetTravelList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            setNewServiceJobIDFromSession(model);
            if (model.ServiceId != 0)
            {
                var obj = db.ServiceJobs.Where(i => i.ServiceJobUID == Convert.ToInt32(model.ServiceId) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                //todothams
                //if (model.ServiceStartDate.HasValue)
                //{
                //    if (!model.ServiceStartTime.HasValue)
                //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value);
                //    else
                //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value).Date.AddHours(model.ServiceStartTime.Value.Hour).AddMinutes(model.ServiceStartTime.Value.Minute);
                //}
                //if (model.ServiceCompleteDate.HasValue)
                //{
                //    if (!model.ServiceCompleteTime.HasValue)
                //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value);
                //    else
                //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value).Date.AddHours(model.ServiceCompleteTime.Value.Hour).AddMinutes(model.ServiceCompleteTime.Value.Minute);
                //}
                if (model.InvoiceDate.HasValue)
                {
                    obj.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value);
                }
                if (model.ProgrammedDate.HasValue)
                {
                    if (model.ProgrammedTime.HasValue)
                    {
                        obj.DateProgrammed = model.ProgrammedDate.Value;
                        decimal hour = Math.Floor((decimal)model.ProgrammedTime.Value / (decimal)100);
                        obj.DateProgrammed = Convert.ToDateTime(obj.DateProgrammed).AddHours(Convert.ToInt32(hour));

                        decimal min = model.ProgrammedTime.Value - hour * 100;
                        obj.DateProgrammed = Convert.ToDateTime(obj.DateProgrammed).AddMinutes(Convert.ToInt32(min));
                    }
                    else
                        obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value);
                    if (model.ProgrammedEnd.HasValue)
                    {
                        obj.DateProgrammedEnd = model.ProgrammedDate.Value;
                        decimal hour = Math.Floor(Convert.ToDecimal(model.ProgrammedEnd.Value) / (decimal)100);
                        obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddHours(Convert.ToInt32(hour));

                        decimal min = Convert.ToDecimal(model.ProgrammedEnd.Value) - hour * 100;
                        obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddMinutes(Convert.ToInt32(min));

                    }
                }
                obj.ApprovalContact = model.ApprovalContact;
                obj.TechContact = model.TechnicalContact;
                obj.ApprovalRequired = model.IsaApprove.Value;
                obj.ApprovedBy = model.IsApprovalReceived;
                obj.CustomerCode = model.Customerid;
                obj.VerbalApprovalObtained = model.VerbalApproval.Value;
                obj.BookingNotes = model.BookingNotes;
                obj.OrderNumber = model.OrderNo;
                obj.OrderNumberRequired = model.Isorder.Value;
                obj.ServiceNotes = model.SpecialNotes;
                obj.ClientSignature = model.CustomerSignature;
                obj.NoCharge = model.DontChangeCust;
                obj.LocationID = Convert.ToInt32( model.LocationID);
                obj.DeptID  = Convert.ToInt32( model.DeptID);
                obj.InvoiceNumber = model.Invoice;
                obj.Amount = model.Charges;
                obj.TravelHours = model.TravelHours;
                obj.ExpensesProportion = model.PropotionalExpenses;
                obj.ChargeRate = model.LabourRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.LabourRate.Replace("$", ""));
                obj.TravelChargeRate = model.TravelRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.TravelRate.Replace("$", ""));
                obj.ChargeTypecode = model.ChargeTypeCode;
                obj.TravelTypecode = model.TravelTypeCode;
                obj.CompletedOutstanding = model.ServiceComplete_Outstanding;
                obj.CompletedBERs = model.ServiceComplete_BERED;
                obj.CompletedOK = model.ServiceComplete;
                obj.HasBeenInvoiced = model.HasJobInvoice;
                //obj.EngineerID = Convert.ToUInt16(model.EngineerID);
                //obj.EngineerName = model.EngineerName;
                obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                obj.ServiceHours = model.LabourHours;
                db.SubmitChanges();
                
                List<DB.ServiceJobEngineer> SjEng;
                //TrackerDataContext dbb = new TrackerDataContext();
                int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID ==  BranchID).ToList();
                foreach (var sjitem in SjEng)
                {
                    if (model.Engineers.Contains(sjitem.EngineerID))
                        ;
                    else
                        db.ServiceJobEngineers.DeleteOnSubmit(sjitem);

                }
                db.SubmitChanges();
                SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();

                foreach (var item in model.Engineers)
                {
                    if (item != null)
                    {
                        int eng = Convert.ToInt32(item);
                        if (SjEng.Select(i => i.EngineerID).ToList().Contains(eng))
                            ;
                        else
                        {
                            ServiceJobEngineer sje = new ServiceJobEngineer();
                            sje.EngineerID = eng;
                            sje.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                            sje.ServiceJobID = model.ServiceId;
                            db.ServiceJobEngineers.InsertOnSubmit(sje);
                            try
                            {
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                ex = ex;
                            }
                        }
                    }
                }
                db.SubmitChanges();
                this.SetNotification("Service Job updated successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    db.Dispose();

                    return RedirectToAction("EditPopUp", new { id = model.ServiceId });
                }
                else
                {
                    db.Dispose();

                    return RedirectToAction("Index", "Service");
                }

            }
            else
            {
                ServiceJob obj = new ServiceJob();
                var JobNo = db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode) + 1;
                var Serviceid = db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceJobUID) + 1;
                Session["NewServiceID"] = Serviceid;

                if (JobNo != null)
                {
                    obj.JobCode = JobNo;
                }
                else { obj.JobCode = 1; }
                obj.ServiceJobUID = Serviceid;
                obj.BNQLocationCode = Utility.GetBranchCode().Substring(0, 1);
                //todothams
                //if (model.ServiceStartDate.HasValue)
                //{
                //    if (!model.ServiceStartTime.HasValue)
                //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value);
                //    else
                //        obj.ActualDateStart = Convert.ToDateTime(model.ServiceStartDate.Value).Date.AddHours(model.ServiceStartTime.Value.Hour).AddMinutes(model.ServiceStartTime.Value.Minute);
                //}
                //if (model.ServiceCompleteDate.HasValue)
                //{
                //    if (!model.ServiceCompleteTime.HasValue)
                //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value);
                //    else
                //        obj.ActualDateEnd = Convert.ToDateTime(model.ServiceCompleteDate.Value).Date.AddHours(model.ServiceCompleteTime.Value.Hour).AddMinutes(model.ServiceCompleteTime.Value.Minute);
                //}
                if (model.InvoiceDate.HasValue)
                {
                    obj.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value);
                }
                if (model.ProgrammedDate.HasValue)
                {
                    if (model.ProgrammedTime.HasValue)
                    {
                        //obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value).AddHours(model.ProgrammedTime.Value.Hour).AddMinutes(model.ProgrammedTime.Value.Minute);


                        obj.DateProgrammed = model.ProgrammedDate.Value;
                        decimal hour = Math.Floor(Convert.ToDecimal(model.ProgrammedTime.Value) / (decimal)100);
                        obj.DateProgrammed = Convert.ToDateTime(obj.DateProgrammed).AddHours(Convert.ToInt32(hour));

                        decimal min = Convert.ToDecimal(model.ProgrammedTime.Value) - hour * 100;
                        obj.DateProgrammed = Convert.ToDateTime(obj.DateProgrammed).AddMinutes(Convert.ToInt32(min));

                    }
                    else
                        obj.DateProgrammed = Convert.ToDateTime(model.ProgrammedDate.Value);
                    if (model.ProgrammedEnd.HasValue)
                    {
                        obj.DateProgrammedEnd = model.ProgrammedDate.Value;
                        decimal hour = Math.Floor(Convert.ToDecimal(model.ProgrammedEnd.Value) / (decimal)100);
                        obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddHours(Convert.ToInt32(hour));

                        decimal min = Convert.ToDecimal(model.ProgrammedEnd.Value) - hour * 100;
                        obj.DateProgrammedEnd = Convert.ToDateTime(obj.DateProgrammedEnd).AddMinutes(Convert.ToInt32(min));

                    }
                }
                obj.ApprovalContact = model.ApprovalContact;
                obj.TechContact = model.TechnicalContact;
                obj.ApprovalRequired = model.IsaApprove.Value;
                obj.ApprovedBy = model.IsApprovalReceived;
                obj.CustomerCode = model.Customerid;
                obj.VerbalApprovalObtained = model.VerbalApproval.Value;
                obj.BookingNotes = model.BookingNotes;
                obj.OrderNumber = model.OrderNo;
                obj.OrderNumberRequired = model.Isorder.Value;
                obj.ServiceNotes = model.SpecialNotes;
                obj.NoCharge = model.DontChangeCust;
                obj.LocationID = Convert.ToInt32( model.LocationID);
                obj.DeptID =Convert.ToInt32( model.DeptID);
                obj.InvoiceNumber = model.Invoice;
                obj.Amount = model.Charges;
                obj.TravelHours = model.TravelHours;
                obj.ExpensesProportion = model.PropotionalExpenses;
                obj.ChargeRate = model.LabourRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.LabourRate.Replace("$", ""));
                obj.TravelChargeRate = model.TravelRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.TravelRate.Replace("$", ""));
                obj.ChargeTypecode = model.ChargeTypeCode;
                obj.TravelTypecode = model.TravelTypeCode;
                obj.CompletedOutstanding = model.ServiceComplete_Outstanding;
                obj.CompletedBERs = model.ServiceComplete_BERED;
                obj.CompletedOK = model.ServiceComplete;
                obj.HasBeenInvoiced = model.HasJobInvoice;


                //List<DB.ServiceJobEngineer> SjEng;
                //SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                //foreach (var sjitem in SjEng)
                //{
                //    if (model.Engineers.Contains(sjitem.EngineerID))
                //        ;
                //    else
                //        db.ServiceJobEngineers.DeleteOnSubmit(sjitem);

                //}
                //db.SubmitChanges();
                //SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();

                foreach (var item in model.Engineers)
                {
                    if (item != null)
                    {
                        int eng = Convert.ToInt32(item);
                        //if (SjEng.Select(i => i.EngineerID).ToList().Contains(eng))
                        //    ;
                        //else
                        {
                            ServiceJobEngineer sje = new ServiceJobEngineer();
                            sje.EngineerID = eng;
                            sje.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                            sje.ServiceJobID = Serviceid;
                            db.ServiceJobEngineers.InsertOnSubmit(sje);
                            try
                            {
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                ex = ex;
                            }
                        }
                    }
                }

                db.SubmitChanges();
                obj.ServiceHours = model.LabourHours;
                obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);

                //var g = Guid.NewGuid();
                //obj.s_GUID = g;
                db.ServiceJobs.InsertOnSubmit(obj);
                db.SubmitChanges();
                List<DB.ServiceJobEngineer> SjEng;
                SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                foreach (var sjitem in SjEng)
                {
                    if (model.Engineers.Contains(sjitem.EngineerID))
                        ;
                    else
                        db.ServiceJobEngineers.DeleteOnSubmit(sjitem);

                }
                db.SubmitChanges();
                SjEng = db.ServiceJobEngineers.Where(i => i.ServiceJobID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();

                foreach (var item in model.Engineers)
                {
                    if (item != null)
                    {
                        int eng = Convert.ToInt32(item);
                        if (SjEng.Select(i => i.EngineerID).ToList().Contains(eng))
                            ;
                        else
                        {
                            ServiceJobEngineer sje = new ServiceJobEngineer();
                            sje.EngineerID = eng;
                            sje.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                            sje.ServiceJobID = model.ServiceId;
                            db.ServiceJobEngineers.InsertOnSubmit(sje);
                            try
                            {
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                ex = ex;
                            }
                        }
                    }
                }
                db.SubmitChanges();
                this.SetNotification("Service Job created successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    db.Dispose();

                    return RedirectToAction("EditPopUp", new { id = obj.ServiceJobUID });
                }
                else
                {
                    db.Dispose();

                    return RedirectToAction("Index", "Service");
                }
            }

        }


        //[Authorize]
        //[HttpPost]
        //public JsonResult PrintServiceWorkOrderAddForm(AddService model)
        //{
        //    setNewServiceJobIDFromSession(model);
        //    if (model.ServiceId != 0)
        //    {
        //        UpdateService(model);
        //    }
        //    else
        //    {
        //        SaveService(model);
        //    }
        //    if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
        //        return Json(new { URL = "../Reports/ServiceJobWorkOrderReport.aspx?ServiceJobUID=" + model.ServiceId + "&CacheCombine=True&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
        //    else
        //        return Json(new { URL = "../../Reports/ServiceJobWorkOrderReport.aspx?ServiceJobUID=" + model.ServiceId + "&CacheCombine=True&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
        //}

        [Authorize]
        [HttpPost]
        public JsonResult AssociateServiceWorkOrderAddForm(AddService model)
        {
            setNewServiceJobIDFromSession(model);
            if (model.ServiceId != 0)
            {
                UpdateService(model);
                return Json(new { ID = model.ServiceId });
            }
            else
            {
                var ID = SaveService(model);
                return Json(new { ID = ID });
            }
        }

        public ActionResult PrintSvrcJobWOSched(int ServiceJobUID)
        {
//            Reports.ReportsDBDataContext context = new Reports.ReportsDBDataContext();
            context.CommandTimeout = 120;
            int BranchID = LoginController.BranchID(User.Identity.Name);
            BytesMime bm1;
            {
                LocalReport localReport = new LocalReport();
                //                localReport.ReportPath = Server.MapPath("~/Reports/ServicePrevMntncRptCheckIncParts.rdlc");
                localReport.ReportPath = Server.MapPath("~/Reports/ServicePrevMntncRptCheck.rdlc");
                var ret = context.ServicesJobServicesEquip(ServiceJobUID, BranchID).ToList();
                //List<Reports.ServicesJobServicesEquipPartsNewResult> dssub = new List<Reports.ServicesJobServicesEquipPartsNewResult>();
                //foreach (var item in ret)
                //{
                //    var ServicesJobServicesEquipParts = context.ServicesJobServicesEquipPartsNew(item.serviceuid, BranchID).ToList();
                //    foreach (var sitem in ServicesJobServicesEquipParts)
                //    {
                //        dssub.Add(sitem);
                //    }
                //}
                bm1 = Renderdoc2(localReport, ret);
            }
            BytesMime bm2;
            {
                LocalReport localReport2 = new LocalReport();
                localReport2.ReportPath = Server.MapPath("~/Reports/ServiceWorkOrderRpt.rdlc");
                var ds = context.ServiceWorkOrderRpt(ServiceJobUID, BranchID).ToList();//.AsQueryable();
                bm2 = Renderdoc(localReport2, ds);
            }
            FileContentResult ff;
            //ff = File(bm2.renderedBytes, "application/pdf");
            //return ff;
            byte[] Prevbytespdf = bm1.renderedBytes;
            byte[] bytespdf = bm2.renderedBytes;
            string mimeType = bm1.mimeType;
            byte[] concatpdf = MDS.Reports.ReportGlobal.concatAndAddContent(new List<byte[]> { bytespdf, Prevbytespdf });
            ff = File(concatpdf, mimeType);
            return ff;
        }

        public BytesMime Renderdoc2(LocalReport localReport, List<Reports.ServicesJobServicesEquipResult> ds)
        {
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ds); //
            localReport.DataSources.Add(reportDataSource);

            //ReportDataSource reportDataSourceSub = new ReportDataSource("DataSetSub", dssub); //
            //localReport.DataSources.Add(reportDataSourceSub);
            ReportParameter rp = new ReportParameter("CompanyName", MDS.Controllers.LoginController.CompanyName);
            localReport.SetParameters(new ReportParameter[] { rp });

            localReport.SubreportProcessing += LocalReport_SubreportProcessing1;
            //AddHandler localReport.SubreportProcessing, AddressOf subReportHandling

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "  <PageWidth>11in</PageWidth>" +
            "  <PageHeight>8.5in</PageHeight>" +
            "  <MarginTop>0.3in</MarginTop>" +
            "  <MarginLeft>0.3in</MarginLeft>" +
            "  <MarginRight>0.3in</MarginRight>" +
            "  <MarginBottom>0.3in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return new BytesMime(renderedBytes,mimeType);
        }
        Reports.ReportsDBDataContext context = new Reports.ReportsDBDataContext();

        private void LocalReport_SubreportProcessing1(object sender, SubreportProcessingEventArgs e)
        {
            context.CommandTimeout = 120;
            int BranchID = LoginController.BranchID(User.Identity.Name);

            var serviceuid = Convert.ToInt32( e.Parameters[0].Values[0]);
            var ServicesJobServicesEquipParts = context.ServicesJobServicesEquipPartsNew(serviceuid, BranchID).ToList();

            ReportDataSource reportDataSourceSub = new ReportDataSource("DataSetSub", ServicesJobServicesEquipParts); //
            e.DataSources.Add(reportDataSourceSub);

            //Dim rds As ReportDataSource
            //'# refresh the datasources using their existing name for the datasource name and also the dataset tablename...
            //'# ie. on report, dataset name is "subreport1" and table name in dataset is also "subreport1"
            //For Each n As String In e.DataSourceNames
            //    rds = New ReportDataSource(n, ds.Tables(n))
            //    e.DataSources.Add(rds)
            //Next n
        }


        public class BytesMime
        {
            public BytesMime(byte[] _renderedBytes,string _mimeType)
            {
                renderedBytes = _renderedBytes;
                mimeType = _mimeType;
            }
            public byte[] renderedBytes;
            public string mimeType;
        }

        public BytesMime Renderdoc(LocalReport localReport, List<Reports.ServiceWorkOrderRptResult> ds)
        {
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", ds); //
            ReportParameter rp = new ReportParameter("CompanyName", MDS.Controllers.LoginController.CompanyName);
            localReport.SetParameters(new ReportParameter[] { rp });

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "  <PageWidth>11in</PageWidth>" +
            "  <PageHeight>8.5in</PageHeight>" +
            "  <MarginTop>0.3in</MarginTop>" +
            "  <MarginLeft>0.3in</MarginLeft>" +
            "  <MarginRight>0.3in</MarginRight>" +
            "  <MarginBottom>0.3in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            //            FileContentResult ff;
            //              ff = File(renderedBytes, mimeType);
            return new BytesMime(renderedBytes, mimeType);


//            return ff;
        }


        [Authorize]
        [HttpPost]
        public JsonResult PrintServicePrevMaintenanceAddForm(AddService model)
        {
            setNewServiceJobIDFromSession(model);

            if (model.ServiceId != 0)
            {
                UpdateService(model);
            }
            else
            {
                SaveService(model);
            }
            if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                return Json(new { URL = "../Reports/ServicePrevMaintenanceCheck.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            else
                return Json(new { URL = "../../Reports/ServicePrevMaintenanceCheck.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
        }

        [HttpPost]
        public JsonResult PrintPartListSave(AddService model)
        {
            if (model.ServiceId != 0)
            {
                UpdateService(model);

            }
            else
            {
                SaveService(model);
            }
            return Json(new { Error = "" }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpPost]
        public JsonResult PrintPartList(AddService model)
        {
            if (model.ServiceId != 0)
            {
                UpdateService(model);

            }
            else
            {
                SaveService(model);
            }
            if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                return Json(new { URL = "../Reports/ServiceJobPartsList.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            else
                return Json(new { URL = "../../Reports/ServiceJobPartsList.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
        }


        [Authorize]
        [HttpPost]
        public JsonResult PrintWorkOrderSave(AddService model)
        {
            setNewServiceJobIDFromSession(model);
            if (model.ServiceId != 0)
            {
                UpdateService(model);

            }
            else
            {
                SaveService(model);
            }
            return Json(new { Error = "" }, JsonRequestBehavior.AllowGet);

            //if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
            //    return Json(new { URL = "../Reports/ServiceJobWorkOrderReport.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            //else
            //    return Json(new { URL = "../../Reports/ServiceJobWorkOrderReport.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });


        }


        [Authorize]
        [HttpPost]
        public JsonResult PrintWorkOrder(AddService model)
        {
            setNewServiceJobIDFromSession(model);
            if (model.ServiceId != 0)
            {
                UpdateService(model);
     
            }
            else
            {
                SaveService(model);
            }
             if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                 return Json(new { URL = "../Reports/ServiceJobWorkOrderReport.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
             else
                 return Json(new { URL = "../../Reports/ServiceJobWorkOrderReport.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });


        }
        [Authorize]
        [HttpPost]
        public JsonResult PrintWorkOrderSchedule(AddService model)
        {
            setNewServiceJobIDFromSession(model);
            if (model.ServiceId != 0)
            {
                UpdateService(model);

            }
            else
            {
                SaveService(model);
            }
                if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                    return Json(new { URL = "../Reports/ServicePrevMaintenance.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
                else
                    return Json(new { URL = "../../Reports/ServicePrevMaintenance.aspx?ServiceJobUID=" + model.ServiceId + "&BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });


        }

        [Authorize]
        public JsonResult StopSupply(string CustomerCode)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (CustomerCode != "")
            {
                var customer = db.Customers.Where(i => i.CustomerCode == CustomerCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (customer.StopSupply)
                {
                    return Json(new { StopSupplymsg = "1" });
                }
                else
                {
                    return Json(new { StopSupplymsg = "0" });
                }
            }
            else
            {
                return Json(new { StopSupplymsg = "0" });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteService(int ServiceID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var d = db.DeleteServiceJob(ServiceID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            return Json(new { msg = d.msg });
        }
    }
}
