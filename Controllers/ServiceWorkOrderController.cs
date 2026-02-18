using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDS.Models;
using MDS.Helper;
using MDS.DB;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Configuration;
using System.IO;

namespace MDS.Controllers
{
    public class ServiceWorkOrderController : BaseController
    {
        //
        // GET: /ServiceWorkOrder/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All WorkOrder Request", 0, Request);

            var model = new ServiceWorkOderSearch();
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
            if (model.SelCnt == 0) model.SelCnt = 400;

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else if (Session["W_Customer"] != null)
            {
                model.Customerid = Session["W_Customer"].ToString();
            }
            else if (model.Customerid==null)
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
            var ServiceID = model.ServiceJobID;

            if (Session["W_Equip"] != null)
            {
                model.Equiptype = Session["W_Equip"].ToString();
            }
            else
            {
                if (model.Equiptype == null)
                {
                    model.Equiptype = "";
                }
            }
            int? EquipID;
            if (Session["W_EquipID"] != null)
            {
                model.SelectedEquipment = Session["W_EquipID"].ToString();
                EquipID = Convert.ToInt32(model.SelectedEquipment);
            }
            else
            {
                if (model.SelectedEquipment == null)
                {
                    model.SelectedEquipment = "";
                    EquipID = -1;
                }
                else
                    EquipID = EquipID = Convert.ToInt32(model.SelectedEquipment); ;
            }
            if (Session["W_EquipmentData"] != null)
                model.EquipmentData = Session["W_EquipmentData"].ToString();


            if (Session["W_CustSite"] != null)
            {
                model.Department = Session["W_CustSite"].ToString();
            }
            else
            {
                model.Department = "";
            }

            if (model.Location == "--Location--")
                model.Location = "";
            if (model.Department == "--Select Customer Site--")
                model.Department = "";

            TrackerDataContext db = new TrackerDataContext();
            var serviceworkorder = db.ServiceWorkOrderSearchNew(model.Equiptype, Convert.ToInt32(ServiceID), model.Customerid, model.Department, Convert.ToInt32(Engineerid), model.Branchid, false, model.ServicesAfterDate, model.Location, EquipID, false,model.SelCnt).Select(i => new ServiceworkorderList
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

            return View(model);
        }
        [Authorize]
        public JsonResult GetCustomerData(String Customerid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Customersite = db.CustomerDepartments.Where(i => i.CustomerCode == Customerid && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).AsEnumerable().Select(i => new MDS.Models.CustomerSite
            {
                Department = i.Department,
                DeptID = i.DeptID.ToString()
            }).ToList();
            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID = "-1", Department = "--Select Customer Site--" });
            return Json(Customersite, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult GetServiceJobSearch()
        {

            var model = new ServiceSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.EngineerList = Utility.GetEngineerList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            return PartialView(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchServiceJob(ServiceSearch model)
        {
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            if (model.SelCnt == 0)
                model.SelCnt = 100;
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
            if (model.ServiceWork == null)
            {
                model.ServiceWork = "";
            }
            if (model.Invoice == null)
            {
                model.Invoice = "";
            }
            var Customer = model.Customerid;
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

            return View("GetServiceJobSearch", model);
        }

        [Authorize]

        public ActionResult stdWorkDone()
        {
            TrackerDataContext db = new TrackerDataContext();
            List<StdWorkDone> model = db.StdWorkDones.ToList();
            return PartialView(model);
        }

        [Authorize]
        public ActionResult SearchServiceJobLoad(ServiceSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search ServiceJob Submit", 0, Request);

            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceWork = "I";
            model.Invoice = "E";
            TrackerDataContext db = new TrackerDataContext();
            if (model.Locationid == "--Location--")
                model.Locationid = "";
            if (model.Department == "--Select Customer Site--")
                model.Department = "";
            var servicedata = db.ServiceJobSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), "", model.Department, -1, model.Branchid, "I", "E", model.Locationid, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false,false,500).Select(i => new ServicesearchList
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

            return View("GetServiceJobSearch", model);
        }
          [Authorize]
        public ActionResult UpdateScript([DataSourceRequest]DataSourceRequest request, CLSTestScript s)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                var p = db.ServiceTestScripts.Where(i => i.ServiceTestScriptID == s.ServiceTestScriptID).FirstOrDefault();
                p.EquipTypeTestID = s.TestScriptID;
                p.Checked = s.CheckedScript;
                p.Comment = s.Comment;
                db.SubmitChanges();
            }

            var TestScript = db.EquipTypeTests.Where(i => i.EquipTypeTestID == s.TestScriptID).FirstOrDefault();
            s.TestScriptText = TestScript.Test;
            s.CheckedScriptText = s.CheckedScript ? "Yes" : "No";

            db.Dispose();
              return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }

          [Authorize]
        public ActionResult DeleteScript([DataSourceRequest]DataSourceRequest request, CLSTestScript s)
        {
            if (ModelState.IsValid)
            {
                TrackerDataContext db = new TrackerDataContext();
                db.CommandTimeout = 90;
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                var p = db.ServiceTestScripts.Where(i => i.ServiceTestScriptID == s.ServiceTestScriptID).FirstOrDefault();
                db.ServiceTestScripts.DeleteOnSubmit(p);
                db.SubmitChanges();
                db.Dispose();
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult GetServiceTestScript(Int32 ServiceID)
        {
            var model = new MDS.Models.ServiceTestScript();
            TrackerDataContext db = new TrackerDataContext();

            ViewBag.TSList = Utility.GetServiceTestScript();

            var Service = db.Services.Where(c => c.ServiceUID == ServiceID && c.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (Service[0].EquipUID.HasValue)
            {
                var Data = db.Equipments.Where(i => i.EquipUID == Service[0].EquipUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                model.SerialNo = Data[0].SerialNumber;
                model.CustEquipCode = Data[0].CustomerEquipCode;
                model.ItemCode = Data[0].BNQItemCode;
                var EquipCode = db.EquipModels.Where(i => i.ModelUID == Data[0].ModelUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                var EquipTypesCode = db.EquipTypes.Where(t => t.EquipTypeCode == EquipCode[0].EquipTypeCode && t.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                model.TypeCode = EquipTypesCode[0].Name;
                model.EquipmentTypeCode = EquipTypesCode[0].EquipTypeCode;
                model.TSLabel = EquipTypesCode[0].TSTesterSNLabel;
            }
            try
            {
                var job = db.ServiceJobs.Where(i => i.ServiceJobUID == Service[0].ServiceJobUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                model.ServiceJobCode = job[0].BNQLocationCode + 'S' + job[0].JobCode;
            }
            catch (Exception ex) { }

            var TestScript1 = db.ServiceTestScripts.Where(i => i.ServiceUID == Convert.ToInt64(ServiceID)).Join(db.EquipTypeTests, r => r.EquipTypeTestID, p => p.EquipTypeTestID, (r, p) => new CLSTestScript
            {
                CheckedScript = r.Checked,
                Comment = r.Comment,
                TestScriptID = r.EquipTypeTestID.HasValue ? r.EquipTypeTestID.Value : 0,
                ServiceUID = r.ServiceUID.HasValue ? r.ServiceUID.Value : 0,
                ServiceTestScriptID = r.ServiceTestScriptID,
                TestScriptText = p.Test,
                CheckedScriptText = r.Checked ? "Yes" : "No"
            }).ToList();
            model.TS = TestScript1;
            //if (TestScript1.Count > 0)
            //{
            //    model.CheckedScript = TestScript1[0].Checked;
            //    model.Comment = TestScript1[0].Comment;
            //    model.TestScriptID = TestScript1[0].EquipTypeTestID.Value;
            //    model.ServiceTestScriptID = TestScript1[0].ServiceTestScriptID;
            //}
            //else
            //{
            //    model.ServiceTestScriptID = 0;
            //}
            model.ServiceUID = ServiceID;
            model.PassFail = Service[0].TSPassFail;
            model.Parts = Service[0].TSParts;
            model.Remarks = Service[0].TSRemarks;
            model.TSSN = Service[0].TSTesterSN;
            model.CalibrationDate = Service[0].TSCalibDate;

            return PartialView(model);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Index(ServiceWorkOderSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search WorkOrder Submit", 0, Request);

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
            if (model.SelCnt == 0) model.SelCnt = 400;

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else if (Session["W_Customer"] != null)
            {
                model.Customerid = Session["W_Customer"].ToString();
            }
            else if (model.Customerid == null)
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
            var EquipID = -1;
            if (model.SelectedEquipment != "" && model.SelectedEquipment != null)
            {
                EquipID = Convert.ToInt32(model.SelectedEquipment);
            }
            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;
            var ServiceID = model.ServiceJobID;

            Session["W_Customer"] = Customer;
            Session["W_CustSite"] = CustDepartment;
            Session["W_Equip"] = Equiptype;
            Session["W_EquipID"] = EquipID;
            if (model.EquipmentData!=null)
                Session["W_EquipmentData"] = model.EquipmentData;

            if (model.Location == "--Location--")
                model.Location = "";
            if (CustDepartment == "--Select Customer Site--")
                CustDepartment = "";

            TrackerDataContext db = new TrackerDataContext();
            var serviceworkorder = db.ServiceWorkOrderSearchNew(Equiptype, Convert.ToInt32(ServiceID), Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, model.DataEntryIncompeteOnly, model.ServicesAfterDate, model.Location, EquipID, model.ServicesAfterDateFilter, model.SelCnt).Select(i => new ServiceworkorderList
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

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult WOSearchResult(ServiceWorkOderSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search WorkOrder Submit", 0, Request);

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
            var EquipID = -1;
            if (model.SelectedEquipment != "" && model.SelectedEquipment != null)
            {
                EquipID = Convert.ToInt32(model.SelectedEquipment);
            }
            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;
            var ServiceID = model.ServiceJobID;

            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            if (model.SelCnt == 0) model.SelCnt = 100;

            if (model.Location == "--Location--")
                model.Location = "";
            if (CustDepartment == "--Select Customer Site--")
                CustDepartment = "";
            TrackerDataContext db = new TrackerDataContext();
            var serviceworkorder = db.ServiceWorkOrderSearchNew(Equiptype, Convert.ToInt32(ServiceID), Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, model.DataEntryIncompeteOnly, model.ServicesAfterDate, model.Location, EquipID, model.ServicesAfterDateFilter, model.SelCnt).Select(i => new ServiceworkorderList
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

            return View("~/Views/Service/GetWOForService.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateRepairJob(ServiceWorkOderSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Create Repair Job", 0, Request);

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
            var EquipID = -1;
            if (model.SelectedEquipment != "" && model.SelectedEquipment != null)
            {
                EquipID = Convert.ToInt32(model.SelectedEquipment);
            }
            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;
            var ServiceID = model.ServiceJobID;
            TrackerDataContext db = new TrackerDataContext();
            db.CreateRepairJobForWorkOrdersNotServiceable(Equiptype, Convert.ToInt32(ServiceID), Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, model.DataEntryIncompeteOnly, model.ServicesAfterDate, model.Location, EquipID);
            return Json(new { success = true });

        }
          [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Request", 0, Request);

            var model = new AddServiceWorkOrder();
            model.EquipTypeList = Utility.GetEquipTypeListByBranchId();

            model.ServiceFunctionList = Utility.GetServiceFunctionList();
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.ConditionList = Utility.GetConditionList();
            List<PartsUsedList> PartUsedRepair = new List<PartsUsedList>();
            Session["PartusedSWO"] = PartUsedRepair;
            TrackerDataContext db = new TrackerDataContext();
            var JobNo = db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode) + 1;
            if (JobNo == null)
            {
                JobNo = 1;
            }
            model.JobCode = Utility.GetBranchCode() + "S" + JobNo;
         //   Session["NewWorkOrderID"] = 0;
            
            model.Year = Convert.ToInt16(DateTime.Now.Year);
            return View(model);
        }
          [Authorize]
        public JsonResult GetEquipmentItem()
        {
            TrackerDataContext db = new TrackerDataContext();
            string Search = Request.Params["filter[filters][0][value]"];
            var model = db.USP_GetEquipName(Search, LoginController.BranchID(HttpContext.User.Identity.Name));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AddWO(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model.SelectedEquipId = id;
            model.EquipId = id;
            model.SelectedEquipment = id.ToString();
            model.EquipTypeList = Utility.GetEquipTypeListByBranchId();
            model.ServiceFunctionList = Utility.GetServiceFunctionList();
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.ConditionList = Utility.GetConditionList();
            List<PartsUsedList> PartUsedRepair = new List<PartsUsedList>();
            Session["PartusedSWO"] = PartUsedRepair;
            TrackerDataContext db = new TrackerDataContext();
            var JobNo = db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode) + 1;
            if (JobNo == null)
            {
                JobNo = 1;
            }
            model.JobCode = Utility.GetBranchCode() + "S" + JobNo;
            ViewBag.Popup = 1;
            ViewBag.ServiceJob = 1;
            model.PopUp = false;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model = GetEditServiceWorkOrder(id);
           // Session["NewWorkOrderID"] = id;
            
            model.PopUp = false;
            ViewBag.Popup = 0;
            return View("Add", model);
        }
        [Authorize]
        public ActionResult EditNew(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model = GetEditServiceWorkOrder(id);
          //  Session["NewWorkOrderID"] = id;
            ViewBag.AddNew = 1;
            return View("Add", model);
        }
        [Authorize]
        public ActionResult EditPopUpService(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model = GetEditServiceWorkOrder(id);
      //      Session["NewWorkOrderID"] = id;
            
            model.PopUp = true;
            ViewBag.PopUpService = Session["ServiceJobID"].ToString();
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopup(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model = GetEditServiceWorkOrder(id);
     //       Session["NewWorkOrderID"] = id;
            ViewBag.Popup = 1;
            model.PopUp = true;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopupWorkOrder(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model = GetEditServiceWorkOrder(id);
        //    Session["NewWorkOrderID"] = id;
            
            ViewBag.Popup = 1;
            ViewBag.ServiceJob = 1;
            model.PopUp = true;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopupAssociated()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", 0, Request);

            var model = new AddServiceWorkOrder();
            model.EquipTypeList = Utility.GetEquipTypeListByBranchId();

            model.ServiceFunctionList = Utility.GetServiceFunctionList();
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.ConditionList = Utility.GetConditionList();
            model.ServiceId = 0;
            model.ServiceJobUID = Session["ServiceJobUID"].ToString();
            List<PartsUsedList> PartUsedRepair = new List<PartsUsedList>();
            Session["PartusedSWO"] = PartUsedRepair;
            model.EquipId = Convert.ToInt32(Session["EquipUID"]);
            model.SelectedEquipId = Convert.ToInt32(Session["EquipUID"]);
            model.SelectedEquipment = Session["EquipUID"].ToString();
            ViewBag.Popup = 1;
            model.PopUp = true;
            return View("Add", model);
        }
          [Authorize]
        public AddServiceWorkOrder GetEditServiceWorkOrder(Int32 id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit WorkOrder Request", id, Request);

            var model = new AddServiceWorkOrder();
            model.EquipTypeList = Utility.GetEquipTypeListByBranchId();

            model.ServiceFunctionList = Utility.GetServiceFunctionList();
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.ConditionList = Utility.GetConditionList();
            TrackerDataContext db = new TrackerDataContext();

            var ServiceData = db.Services.Where(i => i.ServiceUID == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            model.ServiceId = id;


            model.ServiceJobUID = ServiceData[0].ServiceJobUID.ToString();
            if (ServiceData[0].DateServiced.HasValue)
            {
                model.ActualServiceDate = ServiceData[0].DateServiced.Value;
            }

            model.Parts = Utility.GetServicePartList(id, LoginController.BranchID(HttpContext.User.Identity.Name));
            Session["PartusedSWO"] = model.Parts;

            model.BookingNotes = ServiceData[0].Notes;
            model.Compeleterecord = ServiceData[0].DataEntryComplete;
            if (ServiceData[0].EarthResistance.HasValue)
            {
                model.EarthResistance = ServiceData[0].EarthResistance.Value;
            }
            model.EngineerName = ServiceData[0].EngineerName;
            if (ServiceData[0].EquipUID.HasValue)
            {
                model.EquipId = ServiceData[0].EquipUID.Value;
                model.SelectedEquipId = ServiceData[0].EquipUID.Value;
                model.SelectedEquipment = ServiceData[0].EquipUID.Value.ToString();
            }
            if (ServiceData[0].InsulationResistance.HasValue)
            {
                model.InsulationResistance = ServiceData[0].InsulationResistance.Value;
            }
            if (ServiceData[0].LeakageCurrent.HasValue)
            { model.LeakageCurrent = ServiceData[0].LeakageCurrent.Value; }

            model.MajorService = ServiceData[0].MajorService;
            if (ServiceData[0].ServiceMonth.HasValue)
            {
                model.Month = Convert.ToInt16(ServiceData[0].ServiceMonth.Value);
            }
            if (ServiceData[0].ServiceYear.HasValue)
            {
                model.Year =  Convert.ToInt16(ServiceData[0].ServiceYear.Value);
            }
            model.WorkDone = ServiceData[0].WorkDone;
            if (ServiceData[0].ServiceFormRefNumber.HasValue)
            {
                model.ServiceReferenceNo = ServiceData[0].ServiceFormRefNumber.Value;
            }

            model.ServiceConculsion = ServiceData[0].Servicable;
            model.ServiceNotes = ServiceData[0].FutureService;
            model.SafetyTest = ServiceData[0].SafetyTestDone;
            model.ServiceFunctionCode = ServiceData[0].ServiceFunctionCode;
            model.EngineerID = ServiceData[0].EngineerID.HasValue ? ServiceData[0].EngineerID.ToString() : "";
            model.ConditionID = ServiceData[0].ConditionID.HasValue ? ServiceData[0].ConditionID.ToString() : "";
            return model;
        }
          [Authorize]
        public JsonResult CreateWO(int ID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Request", ID, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var s = db.Services.Where(i => i.ServiceUID == ID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            Service obj = new Service();
            var ServiceUID = db.Services.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceUID) + 1;
            obj.ServiceUID = ServiceUID;
            obj.EquipUID = s.EquipUID;
            obj.ServiceJobUID = s.ServiceJobUID;
            //var g = Guid.NewGuid();
            //obj.s_GUID = g;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            db.Services.InsertOnSubmit(obj);
            db.SubmitChanges();
            db.Dispose();
            return Json(new { NewServiceID = obj.ServiceUID }, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        public JsonResult CreateWOfromJob(int ID, int EquipUID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Request", ID, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var s = db.ServiceJobs.Where(i => i.ServiceJobUID == ID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            Service obj = new Service();
            var ServiceUID = db.Services.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceUID) + 1;
            obj.ServiceUID = ServiceUID;
            obj.EquipUID = EquipUID;
            obj.ServiceJobUID = s.ServiceJobUID;
            //var g = Guid.NewGuid();
            //obj.s_GUID = g;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            db.Services.InsertOnSubmit(obj);
            db.SubmitChanges();
            db.Dispose();
            return Json(new { NewServiceID = obj.ServiceUID }, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        public JsonResult CreateWOfromJobAssociated(int ID, int EquipUID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Request", ID, Request);

            TrackerDataContext db = new TrackerDataContext();
            var s = db.ServiceJobs.Where(i => i.ServiceJobUID == ID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            Service obj = new Service();

            int ServiceUID;
            try
            {
                ServiceUID = db.Services.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceUID) + 1;
            }
            catch (Exception ex)
            {
                ServiceUID=1;
            }
            // obj.ServiceUID = ServiceUID;
            Session["EquipUID"] = EquipUID;
            Session["ServiceJobUID"] = s.ServiceJobUID;

            return Json(new { NewServiceID = "" }, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        public JsonResult GetCustomerCode(int ID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var s = db.ServiceJobs.Where(i => i.ServiceJobUID == ID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            return Json(new { CustomerCode = s.CustomerCode.Replace("&","~") }, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        void UpdateWorkOrder(AddServiceWorkOrder model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Update WorkOrder Submit", model.ServiceId, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var obj = db.Services.Where(i => i.ServiceUID == Convert.ToInt32(model.ServiceId) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            // obj.EquipUID = Convert.ToInt32(model.EquipId);
            obj.EquipUID = Convert.ToInt32(model.SelectedEquipment);
            if (model.ActualServiceDate.HasValue)
            {
                obj.DateServiced = Convert.ToDateTime(model.ActualServiceDate.Value);
            }

            obj.WorkDone = model.WorkDone;
            obj.EngineerName = model.EngineerName;
            obj.MajorService = model.MajorService.Value;
            obj.Notes = model.ServiceNotes;
            obj.ServiceFormRefNumber = model.ServiceReferenceNo;
            obj.ServiceMonth = Convert.ToInt16(model.Month);
            obj.ServiceYear = Convert.ToInt16(model.Year);
            obj.DataEntryComplete = model.Compeleterecord.Value;
            obj.ServiceFunctionCode = model.ServiceFunctionCode;
            obj.EarthResistance = model.EarthResistance;
            obj.InsulationResistance = model.InsulationResistance;
            obj.LeakageCurrent = model.LeakageCurrent;
            obj.SafetyTestDone = model.SafetyTest.Value;
            obj.ServiceJobUID = Convert.ToInt32(model.ServiceJobUID);
            obj.FutureService = model.ServiceNotes;
            obj.Servicable = model.ServiceConculsion.Value;
            if (model.EngineerID != "")
            {
                obj.EngineerID = Convert.ToInt32(model.EngineerID);
            }
            else
            {
                obj.EngineerID = null;
            }
            if (model.ConditionID != "")
            {
                obj.ConditionID= Convert.ToInt32(model.ConditionID);
            }
            else
            {
                obj.ConditionID= null;
            }

            obj.Notes = model.BookingNotes;
            db.SubmitChanges();

            var NextServiceData = db.NextServices.Where(i => i.EquipUID == model.EquipId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (NextServiceData != null)
            {
                db.NextServices.DeleteOnSubmit(NextServiceData);
                db.SubmitChanges();
            }

            var EquipmentsData = db.Equipments.Where(i => i.CurrentlyServicedByBNQ != false && i.InService != false && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (EquipmentsData.Count > 0)
            {
                var refreshService = db.RefreshNextDueService(model.EquipId, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            }
            var parts = db.ServiceParts.Where(i => i.RepairOrServiceUID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            db.ServiceParts.DeleteAllOnSubmit(parts);
            db.SubmitChanges();
            if (Session["PartusedSWO"] != null)
            {
                if (model.ServiceId != 0)
                {
                    List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                    foreach (var item in PartUsedRepair)
                    {
                        var part = new ServicePart();
                        part.CostPerUnit = item.Price;
                        part.RepairOrServiceUID = model.ServiceId;
                        part.NumberUsed = Convert.ToInt16(item.NoOfParts);
                        part.PartDesc = item.PartName;
                        part.PartNumber = item.PartNumber;
                        part.PartID = item.PartId;
                        part.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);                       
                        db.ServiceParts.InsertOnSubmit(part);
                        db.SubmitChanges();
                    }
                }
            }
            db.Dispose();
        }
          [Authorize]
        int SaveWorkOrder(AddServiceWorkOrder model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Save WorkOrder Submit", model.ServiceId, Request);

            using (TrackerDataContext db = new TrackerDataContext())
            {
                db.CommandTimeout = 90;
                Service obj = new Service();
                int ServiceUID;
                try
                {
                    ServiceUID = db.Services.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceUID) + 1;
                }
                catch (Exception ex)
                {
                    ServiceUID = 1;
                }
                obj.ServiceUID = ServiceUID;
                if (model.ActualServiceDate.HasValue)
                {
                    obj.DateServiced = Convert.ToDateTime(model.ActualServiceDate.Value);
                }
                //obj.EquipUID = Convert.ToInt32(model.EquipId);
                obj.EquipUID = Convert.ToInt32(model.SelectedEquipment);
                obj.WorkDone = model.WorkDone;
                obj.EngineerName = model.EngineerName;
                obj.MajorService = model.MajorService.Value;
                obj.Notes = model.ServiceNotes;
                obj.ServiceFormRefNumber = model.ServiceReferenceNo;
                obj.ServiceMonth = Convert.ToInt16(model.Month);
                obj.ServiceYear = Convert.ToInt16(model.Year);
                obj.DataEntryComplete = model.Compeleterecord.Value;
                obj.ServiceFunctionCode = model.ServiceFunctionCode;
                obj.EarthResistance = model.EarthResistance;
                obj.InsulationResistance = model.InsulationResistance;
                obj.LeakageCurrent = model.LeakageCurrent;
                obj.SafetyTestDone = model.SafetyTest.Value;
                obj.ServiceJobUID = Convert.ToInt32(model.ServiceJobUID);
                obj.FutureService = model.ServiceNotes;
                obj.Servicable = model.ServiceConculsion.Value;
                obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                if (model.EngineerID != "")
                {
                    obj.EngineerID = Convert.ToInt32(model.EngineerID);
                }
                else
                {
                    obj.EngineerID = null;
                }
                if (model.ConditionID != "")
                {
                    obj.ConditionID = Convert.ToInt32(model.ConditionID);
                }
                else
                {
                    obj.ConditionID = null;
                }

                obj.Notes = model.BookingNotes;
                //var g = Guid.NewGuid();
                //obj.s_GUID = g;
                db.Services.InsertOnSubmit(obj);
                db.SubmitChanges();

                if (Session["PartusedSWO"] != null)
                {
                    if (obj.ServiceUID != 0)
                    {
                        List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                        foreach (var item in PartUsedRepair)
                        {
                            var part = new ServicePart();
                            part.CostPerUnit = item.Price;
                            part.RepairOrServiceUID = obj.ServiceUID;
                            part.NumberUsed = Convert.ToInt16(item.NoOfParts);
                            part.PartDesc = item.PartName;
                            part.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                            part.PartNumber = item.PartNumber;
                            part.PartID = item.PartId;                            
                            db.ServiceParts.InsertOnSubmit(part);
                            db.SubmitChanges();
                        }
                    }
                }

                var NextServiceData = db.NextServices.Where(i => i.EquipUID == model.EquipId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (NextServiceData != null)
                {
                    db.NextServices.DeleteOnSubmit(NextServiceData);
                    db.SubmitChanges();
                }

                var EquipmentsData = db.Equipments.Where(i => i.CurrentlyServicedByBNQ != false && i.InService != false && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                if (EquipmentsData.Count > 0)
                {
                    var refreshService = db.RefreshNextDueService(model.EquipId, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                }
                db.Dispose();
                return ServiceUID;
            }
            //return -1;
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveUpdateWorkOrder(AddServiceWorkOrder model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Submit", model.ServiceId, Request);

            if (model.ServiceId == 0)
            {
                  TrackerDataContext db = new TrackerDataContext();
                int Branchid=LoginController.BranchID(HttpContext.User.Identity.Name);
                int ServiceJobUID =Convert.ToInt32( model.ServiceJobUID);


                    var SWOData = db.Services.Where(i => i.ServiceJobUID == ServiceJobUID && i.EquipUID == model.EquipId && i.BranchID == Branchid).FirstOrDefault();
            if (SWOData != null)
            {
                model.ServiceId = SWOData.ServiceUID;
            }
            }
            if (model.ServiceId != 0)
            {
                UpdateWorkOrder(model);
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = model.ServiceId.ToString() });
                }
                else
                {
                    return Json(new { msg = "1", message = model.ServiceId.ToString() });
                }
            }
            else
            {
                int serviceid = SaveWorkOrder(model);
         //       Session["NewWorkOrderID"] = model.ServiceId;
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = serviceid.ToString() });//"Service Work Order created successfully."
                }
                else
                {
                    return Json(new { msg = "1", message = serviceid.ToString() });//"Service Work Order created successfully."
                }
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult 
            Add(AddServiceWorkOrder model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add WorkOrder Submit", model.ServiceId, Request);

            TrackerDataContext db1 = new TrackerDataContext();
            model.EquipTypeList = Utility.GetEquipTypeListByBranchId();

            model.ServiceFunctionList = Utility.GetServiceFunctionList();
            model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
            model.ConditionList = Utility.GetConditionList();

            if (model.SelectedEquipment == null || model.SelectedEquipment == "" || model.SelectedEquipment == "0")
            {
                ModelState.AddModelError("", "You must pick the equipment item.");
                return View(model);
            }

            var CheckServiceCustomerCode = db1.ServiceJobs.Where(i => i.ServiceJobUID == Convert.ToInt32(model.ServiceJobUID) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (CheckServiceCustomerCode.CustomerCode != model.CustomerCode)
            {
                ModelState.AddModelError("", "The Equipment specified in this Service record is not owned by the same customer as is specified in the selected Service Job..  You should select or add a matching Service Job record, or correct the customer details associated with the equipment.");
                return View(model);
            }
        //    Session["NewWorkOrderID"] = model.ServiceId;
            
            if (model.ServiceId != 0)
            {
                TrackerDataContext db = new TrackerDataContext();
                db.CommandTimeout = 90;
                model.EquipTypeList = Utility.GetEquipTypeListByBranchId();
                model.ServiceJobList = Utility.GetServiceJob();
                model.ServiceFunctionList = Utility.GetServiceFunctionList();
                model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
                model.ConditionList = Utility.GetConditionList();

                var obj = db.Services.Where(i => i.ServiceUID == Convert.ToInt32(model.ServiceId) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                obj.EquipUID = Convert.ToInt32(model.SelectedEquipment);
                if (model.ActualServiceDate.HasValue)
                {
                    obj.DateServiced = Convert.ToDateTime(model.ActualServiceDate.Value);
                }

                obj.WorkDone = model.WorkDone;
                obj.EngineerName = model.EngineerName;
                obj.MajorService = model.MajorService.Value;
                obj.Notes = model.ServiceNotes;
                obj.ServiceFormRefNumber = model.ServiceReferenceNo;
                obj.ServiceMonth = Convert.ToInt16(model.Month);
                obj.ServiceYear = Convert.ToInt16(model.Year);
                obj.DataEntryComplete = model.Compeleterecord.Value;
                obj.ServiceFunctionCode = model.ServiceFunctionCode;
                obj.EarthResistance = model.EarthResistance;
                obj.InsulationResistance = model.InsulationResistance;
                obj.LeakageCurrent = model.LeakageCurrent;
                obj.SafetyTestDone = model.SafetyTest.Value;
                obj.ServiceJobUID = Convert.ToInt32(model.ServiceJobUID);
                obj.FutureService = model.ServiceNotes;
                obj.Servicable = model.ServiceConculsion.Value;
                if (model.EngineerID != "")
                {
                    obj.EngineerID = Convert.ToInt32(model.EngineerID);
                }
                else
                {
                    obj.EngineerID = null;
                }
                if (model.ConditionID != "")
                {
                    obj.ConditionID = Convert.ToInt32(model.ConditionID);
                }
                else
                {
                    obj.ConditionID = null;
                }
                db.SubmitChanges();
                int BranchID = Controllers.LoginController.BranchID(HttpContext.User.Identity.Name);
                var NextServiceData = db.NextServices.Where(i => i.EquipUID == model.EquipId && i.BranchID == BranchID).FirstOrDefault();
                if (NextServiceData != null)
                {
                    db.NextServices.DeleteOnSubmit(NextServiceData);
                    db.SubmitChanges();
                }

                var EquipmentsData = db.Equipments.Where(i => i.CurrentlyServicedByBNQ != false && i.InService != false && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                if (EquipmentsData.Count > 0)
                {
                    var refreshService = db.RefreshNextDueService(model.EquipId, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                }
                var parts = db.ServiceParts.Where(i => i.RepairOrServiceUID == model.ServiceId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                db.ServiceParts.DeleteAllOnSubmit(parts);
                db.SubmitChanges();
                if (Session["PartusedSWO"] != null)
                {
                    if (model.ServiceId != 0)
                    {
                        List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                        foreach (var item in PartUsedRepair)
                        {
                            var part = new ServicePart();
                            part.CostPerUnit = item.Price;
                            part.RepairOrServiceUID = model.ServiceId;
                            part.NumberUsed = Convert.ToInt16(item.NoOfParts);
                            part.PartDesc = item.PartName;
                            part.PartNumber = item.PartNumber;
                            part.PartID = item.PartId;
                            part.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);                            
                            db.ServiceParts.InsertOnSubmit(part);
                            db.SubmitChanges();
                        }
                    }
                }
                db.Dispose();
                this.SetNotification("Service Work Order updated successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("EditPopUp", new { id = model.ServiceId });
                }
                else
                {
                    return RedirectToAction("Index", "ServiceWorkOrder");
                }
                // return RedirectToAction("Edit", new { id = model.ServiceId });

            }
            else
            {

                using (TrackerDataContext db = new TrackerDataContext())
                {
                    db.CommandTimeout = 90;
                    Service obj = new Service();
                    var ServiceUID = db.Services.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceUID) + 1;
                    obj.ServiceUID = ServiceUID;
            //        Session["NewWorkOrderID"] = ServiceUID;

                    if (model.ActualServiceDate.HasValue)
                    {
                        obj.DateServiced = Convert.ToDateTime(model.ActualServiceDate.Value);
                    }
                    //obj.EquipUID = Convert.ToInt32(model.EquipId);
                    obj.EquipUID = Convert.ToInt32(model.SelectedEquipment);
                    obj.WorkDone = model.WorkDone;
                    obj.EngineerName = model.EngineerName;
                    obj.MajorService = model.MajorService.Value;
                    obj.Notes = model.ServiceNotes;
                    obj.ServiceFormRefNumber = model.ServiceReferenceNo;
                    obj.ServiceMonth = Convert.ToInt16(model.Month);
                    obj.ServiceYear = Convert.ToInt16(model.Year);
                    obj.DataEntryComplete = model.Compeleterecord.Value;
                    obj.ServiceFunctionCode = model.ServiceFunctionCode;
                    obj.EarthResistance = model.EarthResistance;
                    obj.InsulationResistance = model.InsulationResistance;
                    obj.LeakageCurrent = model.LeakageCurrent;
                    obj.SafetyTestDone = model.SafetyTest.Value;
                    obj.ServiceJobUID = Convert.ToInt32(model.ServiceJobUID);
                    obj.FutureService = model.ServiceNotes;
                    obj.Servicable = model.ServiceConculsion.Value;
                    obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                    if (model.EngineerID != "")
                    {
                        obj.EngineerID = Convert.ToInt32(model.EngineerID);
                    }
                    else
                    {
                        obj.EngineerID = null;
                    }
                    if (model.ConditionID != "")
                    {
                        obj.ConditionID = Convert.ToInt32(model.ConditionID);
                    }
                    else
                    {
                        obj.ConditionID = null;
                    }
                    db.Services.InsertOnSubmit(obj);
                    db.SubmitChanges();

                    if (Session["PartusedSWO"] != null)
                    {
                        if (obj.ServiceUID != 0)
                        {
                            List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                            foreach (var item in PartUsedRepair)
                            {
                                var part = new ServicePart();
                                part.CostPerUnit = item.Price;
                                part.RepairOrServiceUID = obj.ServiceUID;
                                part.NumberUsed = Convert.ToInt16(item.NoOfParts);
                                part.PartDesc = item.PartName;
                                part.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                                part.PartNumber = item.PartNumber;
                                part.PartID = item.PartId;                              
                                db.ServiceParts.InsertOnSubmit(part);
                                db.SubmitChanges();
                            }
                        }
                    }

                    var NextServiceData = db.NextServices.Where(i => i.EquipUID == model.EquipId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                    if (NextServiceData != null)
                    {
                        db.NextServices.DeleteOnSubmit(NextServiceData);
                        db.SubmitChanges();
                    }

                    var EquipmentsData = db.Equipments.Where(i => i.CurrentlyServicedByBNQ != false && i.InService != false && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                    if (EquipmentsData.Count > 0)
                    {
                        var refreshService = db.RefreshNextDueService(model.EquipId, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                    }
                    this.SetNotification("Service Work Order created successfully.", NotificationEnumeration.Success);
                    db.Dispose();
                    if (model.PopUp)
                    {
                        return RedirectToAction("EditPopUp", new { id = obj.ServiceUID });
                    }
                    else
                    {
                        return RedirectToAction("Index", "ServiceWorkOrder");
                    }
                    // return RedirectToAction("Edit", new { id = obj.ServiceUID });
                }

            }
            // return RedirectToAction("Index");
        }


        //added by thams
        [Authorize]
        public ActionResult GetParts([DataSourceRequest]DataSourceRequest request)
        {

            List<PartsUsedList> PartUsedRepair;
          
                PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
        DataSourceResult result = PartUsedRepair.ToDataSourceResult(request);
            return Json(result);

        }
        [Authorize]
        public ActionResult CreatePart([DataSourceRequest]DataSourceRequest request, PartsUsedList part)
        {
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                var p = new PartsUsedList();
                p.Id = PartUsedRepair.Count + 1;
                p.PartId = part.PartId;
                p.PartName = part.PartName.Replace("(" + part.PartNumber + ") ", "");
                if (part.Stocked_Part)
                {
                    p.PartNumber = part.PartNumber;
                }
                else
                {
                    p.PartNumber = "";
                    part.PartNumber = "";
                }
                p.NoOfParts = part.NoOfParts;
                p.Price = part.Price;
                p.Stocked_Part = part.Stocked_Part;
                PartUsedRepair.Add(p);
                part.Id = p.Id;
                Session["PartusedSWO"] = PartUsedRepair;

            }
            return Json(new[] { part }.ToDataSourceResult(request, ModelState));
            // Return the inserted product. The grid needs the generated ProductID. Also return any validation errors.

        }
        [Authorize]
        public ActionResult UpdatePart([DataSourceRequest]DataSourceRequest request, PartsUsedList part)
        {
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                var p = PartUsedRepair.Where(i => i.Id == part.Id).FirstOrDefault();
                p.Id = part.Id;
                p.PartId = part.PartId;
                p.PartName = part.PartName;
                p.PartNumber = part.PartNumber;
                p.NoOfParts = part.NoOfParts;
                p.Stocked_Part = part.Stocked_Part;
                p.Price = part.Price;
                Session["PartusedSWO"] = PartUsedRepair;
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { part }.ToDataSourceResult(request, ModelState));
        }
        [Authorize]
        public ActionResult DeletePart([DataSourceRequest]DataSourceRequest request, PartsUsedList part)
        {
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];
                var p = PartUsedRepair.Where(i => i.PartId == part.PartId).FirstOrDefault();
                PartUsedRepair.Remove(p);
                for (int i = 0; i < PartUsedRepair.Count; i++)
                {
                    PartUsedRepair[i].Id = i + 1;
                }
                Session["PartusedSWO"] = PartUsedRepair;
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { part }.ToDataSourceResult(request, ModelState));
        }
        /// <summary>
        /// added by thams
        /// </summary>
        /// <param name="model"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult TestScript(MDS.Models.ServiceTestScript model, string command)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            if (command == "Clear and rebuild Check List")
            {

                var TS = db.ServiceTestScripts.Where(i => i.ServiceUID == model.ServiceUID).ToList();
                db.ServiceTestScripts.DeleteAllOnSubmit(TS);
                db.SubmitChanges();


                var ETest = db.EquipTypeTests.Where(i => i.EquipTypeCode == model.EquipmentTypeCode).ToList();

                if (ETest.Count > 0)
                {
                    foreach (var t in ETest)
                    {
                        var objTS = new MDS.DB.ServiceTestScript();
                        objTS.EquipTypeTestID = t.EquipTypeTestID;
                        var ID = db.ServiceTestScripts.Max(i => i.ServiceTestScriptID) + 1;
                        objTS.ServiceTestScriptID = ID;
                        objTS.ServiceUID = model.ServiceUID;
                        db.ServiceTestScripts.InsertOnSubmit(objTS);
                        db.SubmitChanges();
                    }
                }



                ViewBag.TSList = Utility.GetServiceTestScript();

                var TestScript1 = db.ServiceTestScripts.Where(i => i.ServiceUID == model.ServiceUID).Join(db.EquipTypeTests, r => r.EquipTypeTestID, p => p.EquipTypeTestID, (r, p) => new CLSTestScript
                {
                    CheckedScript = r.Checked,
                    Comment = r.Comment,
                    TestScriptID = r.EquipTypeTestID.HasValue ? r.EquipTypeTestID.Value : 0,
                    ServiceUID = r.ServiceUID.HasValue ? r.ServiceUID.Value : 0,
                    ServiceTestScriptID = r.ServiceTestScriptID,
                    TestScriptText = p.Test,
                    CheckedScriptText = r.Checked ? "Yes" : "No"
                }).ToList();
                model.TS = TestScript1;
                ModelState.Clear();
                db.Dispose();
                return View("GetServiceTestScript", model);
            }
            else
            {
                var Service = db.Services.Where(i => i.ServiceUID == model.ServiceUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                Service.TSPassFail = model.PassFail;
                Service.TSParts = model.Parts;
                Service.TSRemarks = model.Remarks;
                Service.TSTesterSN = model.TSSN;
                Service.TSCalibDate = model.CalibrationDate;
                db.SubmitChanges();
                db.Dispose();
                return Json(new { success = true });
            }

        }

          [Authorize]
        public JsonResult GetEquipInfomation(int EquipUID)
        {

            TrackerDataContext db = new TrackerDataContext();
            var model = db.USP_GetEquipNameByEquipId(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (model.Count > 0)
            {
                return Json(new { CustomerInfo = model[0].CustomerInfo, CustomerCode = model[0].CustomerCode, EquipItem = model[0].EquipDesc, Location = model[0].Location, WarrantyExpirationDate = model[0].WarrantyExpirationDate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { CustomerInfo = "", CustomerCode = "", EquipItem = "", Location = "", WarrantyExpirationDate = "" }, JsonRequestBehavior.AllowGet);
            }


        }

          [Authorize]
        public JsonResult GetServiceJobDetails(int ServiceJobId)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get ServiceJob Request", ServiceJobId, Request);

            var model = Utility.ServiceJobDetailbyId(ServiceJobId);
            if (model.Count > 0)
            {
                return Json(new { ServiceInfo = model[0].JobCode.ToString() + "," + model[0].Customer + "," + model[0].CustomerSite }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ServiceInfo = "" }, JsonRequestBehavior.AllowGet);
            }
        }
          [Authorize]
        public JsonResult RefreshNextDueService(int EquipID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get RefreshNextDueService Request", EquipID, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var NextServiceData = db.NextServices.Where(i => i.EquipUID == EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (NextServiceData != null)
            {
                db.NextServices.DeleteOnSubmit(NextServiceData);
                db.SubmitChanges();
            }

            var EquipmentsData = db.Equipments.Where(i => i.CurrentlyServicedByBNQ != false && i.InService != false && i.EquipUID == EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (EquipmentsData.Count > 0)
            {
                var refreshService = db.RefreshNextDueService(EquipID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                db.Dispose();
                return Json(new { Worked = 1, DueMonth = refreshService.DueMonth, DueYear = refreshService.DueYear, DueServiceType = refreshService.DueServiceType, Overdue = refreshService.Overdue, LastServiceUID = refreshService.LastServiceUID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Dispose();
                return Json(new { Worked = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [Authorize]
        public JsonResult CreateRepairJobfromWorkOrderSave(AddServiceWorkOrder model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get CreateRepairJobfromWorkOrderSave Submit", model.ServiceId, Request);

            SaveUpdateWorkOrder(model);

            int ServiceJobId=Convert.ToInt32( model.ServiceJobUID);
            int EquipID=model.EquipId;
            int ServiceId = model.ServiceId;

            TrackerDataContext db = new TrackerDataContext();
            var RepairData = db.Repairs.Where(i => i.ServiceJobUID == ServiceJobId && i.EquipUID == EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (RepairData == null)
            {
                var r = db.CreateRepairJobFromWorkOrder(ServiceId, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                return Json(new { Added = r.Added, Reason = r.Reason, RepairID = r.RepairID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Added = true, Reason = "", RepairID = RepairData.RepairUID }, JsonRequestBehavior.AllowGet);
            }
        }

          [Authorize]
        public JsonResult CreateRepairJobfromWorkOrder(int ServiceJobId, int EquipID, int ServiceId)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Get CreateRepairJobfromWorkOrderSave Request", ServiceId, Request);

            TrackerDataContext db = new TrackerDataContext();
            var RepairData = db.Repairs.Where(i => i.ServiceJobUID == ServiceJobId && i.EquipUID == EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (RepairData == null)
            {
                var r = db.CreateRepairJobFromWorkOrder(ServiceId, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                return Json(new { Added = r.Added, Reason = r.Reason, RepairID = r.RepairID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Added = true, Reason = "", RepairID = RepairData.RepairUID }, JsonRequestBehavior.AllowGet);
            }
        }
          [Authorize]
        public JsonResult CheckCreateRepairJobfromWorkOrder(int ServiceJobId, int EquipID)
        {
            TrackerDataContext db = new TrackerDataContext();

            var RepairData = db.Repairs.Where(i => i.ServiceJobUID == ServiceJobId && i.EquipUID == EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (RepairData == null)
            {
                return Json(new { Added = false, RepairID = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Added = true, RepairID = RepairData.RepairUID }, JsonRequestBehavior.AllowGet);
            }
        }
          [Authorize]
        public JsonResult GetServiceJobs(string customercode)
        {
            TrackerDataContext db = new TrackerDataContext();
            var ServicejobData = db.ServiceJobs.Where(i => i.CustomerCode == customercode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).OrderByDescending(i => i.JobCode).Select(i => new ServiceJobList
            {
                ServiceJobUID = i.ServiceJobUID.ToString(),
                FullJobCode = (i.BNQLocationCode + "S" + i.JobCode).ToString()
            }).ToList();

            ServicejobData.Insert(0, new ServiceJobList { ServiceJobUID = "", FullJobCode = "--Select Service Job--" });

            return Json(ServicejobData, JsonRequestBehavior.AllowGet);
        }
          [Authorize]
        public JsonResult CanBeServiced(int EquipID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var eq = db.Equipments.Where(i => i.EquipUID == EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            var CurrentlyServicedByMDS = eq.CurrentlyServicedByBNQ;
            var InService = eq.InService;
            if (CurrentlyServicedByMDS && InService)
            {
                return Json(new { service = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { service = false }, JsonRequestBehavior.AllowGet);
            }
        }
          [Authorize]
        public JsonResult RefreshLastServiceOrRepair(int EquipUID, int RepairUID, int ServiceUID, DateTime ActualDateOfService)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ActualDateOfService == null)
            {
                ActualDateOfService = DateTime.Now;
            }

            var lastRepair = db.EquipLastRepairOrServiceDateReason(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name), ActualDateOfService, ServiceUID, RepairUID).FirstOrDefault();
            return Json(new { RepairServiceDate = lastRepair.RepairServiceDate, notes = lastRepair.notes, bWithin90Days = lastRepair.bWithin90Days,lastnotes=lastRepair.LastNotes }, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        public JsonResult ValidateServicejob(string CustomerCode, int ServiceJobUID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var job = db.ServiceJobs.Where(i => i.ServiceJobUID == ServiceJobUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (job != null)
            {
                if (job.CustomerCode == CustomerCode) { return Json(new { msg = 0 }, JsonRequestBehavior.AllowGet); }
                else { return Json(new { msg = 1 }, JsonRequestBehavior.AllowGet); }

            }
            else
            {
                return Json(new { msg = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

/*          [Authorize]
          [HttpPost]
          public ActionResult PrintWOListExcel(ServiceWorkOderSearch model)
          {
              return PrintPDFExcel(model, true);
          }

        [Authorize]
        [HttpPost]
        public ActionResult PrintWOList(ServiceWorkOderSearch model)
        {
            return PrintPDFExcel(model,false);
        }

        private ActionResult PrintPDFExcel(ServiceWorkOderSearch model, bool Excel)
        {
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else if (model.Customerid == null)
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
            var EquipID = -1;
            if (model.SelectedEquipment != "" && model.SelectedEquipment != null)
            {
                EquipID = Convert.ToInt32(model.SelectedEquipment);
            }
            var Customer = model.Customerid;
            var Engineerid = model.EngineerID;
            var CustDepartment = model.Department;
            var Equiptype = model.Equiptype;
            var ServiceID = model.ServiceJobID;
            if (Excel)
                return Json(new { URL =  "Reports/ServiceWorkOrderList.aspx?customerID=" + Customer + "&equipmentType=" + model.Equiptype + "&BranchID=" + model.Branchid + "&dataEntryIncompleteOnly=" + model.DataEntryIncompeteOnly + "&serviceJobID=" + model.ServiceJobID + "&equipmentID=" + EquipID + "&department=" + CustDepartment + "&location=" + model.Location + "&engineerID=" + Engineerid + "&servicesAfterDate=" + model.ServicesAfterDate.ToString("yyyy-MM-dd") + "&Excel=true" });
            return Json(new { URL = "Reports/ServiceWorkOrderList.aspx?customerID=" + Customer + "&equipmentType=" + model.Equiptype + "&BranchID=" + model.Branchid + "&dataEntryIncompleteOnly=" + model.DataEntryIncompeteOnly + "&serviceJobID=" + model.ServiceJobID + "&equipmentID=" + EquipID + "&department=" + CustDepartment + "&location=" + model.Location + "&engineerID=" + Engineerid + "&servicesAfterDate=" + model.ServicesAfterDate.ToString("yyyy-MM-dd") + "" });
        }*/

    

        [Authorize]
        [HttpPost]
        public JsonResult DeleteServiceWokOrder(int ServiceID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Delete ServiceWorkOrder Request", ServiceID, Request);

            TrackerDataContext db = new TrackerDataContext();
            int BranchID=LoginController.BranchID(HttpContext.User.Identity.Name);
            var ServiceR = db.Services.Where(i => i.ServiceUID == ServiceID && i.BranchID == BranchID).First();//.First();
            int EquipUID = Convert.ToInt32(ServiceR.EquipUID);
            var d = db.DeleteServiceWorkOrder(ServiceID, BranchID).FirstOrDefault();
            var messg = d.msg;
            db.RefreshNextDueService(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name));
            return Json(new { msg = messg });
        }

        //[HttpPost]
        //public JsonResult UploadFileSave(HttpPostedFileBase File, int? ServiceID, int? BranchID, string Title)
        //{

        //    if (File.ContentLength == 0)
        //    {
        //        return Json(new { Error = "File is empty - 0 size" });
        //    }
        //    int servid = Convert.ToInt32(ServiceID);
        //    int brid = Convert.ToInt32(BranchID);
        //    var contenttype = File.ContentType;
        //    var fileName = Path.GetFileName(File.FileName);
        //    fileName = "Srvc_"+servid.ToString("######") + "-" + brid.ToString() + "-" + fileName.Replace(' ', '-'); //For TheDock naming convention

        //    var path = Path.Combine(Server.MapPath(uploadfolder), fileName);
        //    if (System.IO.File.Exists(path))
        //    {
        //        return Json(new { Error = "A file with this name already exists. Please rename your file, or remove the previous file prior to uploading" });
        //    }
        //    File.SaveAs(path);
        //    Photo pt = new Photo();
        //    pt.Title = Title;
        //    pt.ServiceID = servid;
        //    pt.BranchID = brid;
        //    pt.URL = fileName;
        //    pt.Type = contenttype;
        //    TrackerDataContext db = new TrackerDataContext();
        //    db.Photos.InsertOnSubmit(pt);
        //    db.SubmitChanges();
        //    return Json(new { Error = "" });
        //}

        //public PartialViewResult UploadFile(int? RepairID, int? ServiceID)
        //{
        //    int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
        //    Photo uf = new Photo();
        //    uf.ServiceID = ServiceID;
        //    uf.BranchID = BranchID;
        //    return PartialView(uf);
        //}

        public static string uploadfolder = "~/App_Data/UploadedFiles";

        public ActionResult Photos(int ServiceID,bool? Readonly)
        {
            ViewBag.Readonly = Readonly;
            RepairPhotos ret = new RepairPhotos();
            ret.Id = ServiceID;
            TrackerDataContext db = new TrackerDataContext();
            int BranchID= LoginController.BranchID(HttpContext.User.Identity.Name);
            ret.Photos = db.Photos.Where(p => p.ServiceID == ServiceID && p.BranchID==BranchID).ToList();
            return View(ret);
        }

    }
}
