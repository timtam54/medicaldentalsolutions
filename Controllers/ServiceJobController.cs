using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDS.Helper;
using MDS.Models;
using MDS.DB;
using System.Configuration;

namespace MDS.Controllers
{
    public class ServiceJobController : BaseController
    {
        //
        // GET: /ServiceJob/
          [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All BulkCreateServiceJob Request", 0, Request);
            var model = new ServiceJobSearch();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.monthdataList = Utility.GetMonthyearsData();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.OverDue = "OVD";
            model.Cnt = new List<Int32>();
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.Cnt.Add(5000);
            model.Cnt.Add(10000);
            model.SelCnt = 1000;

            TrackerDataContext db = new TrackerDataContext();
            var s = db.NextServiceSearch("OVD", "", "", "", -1, LoginController.BranchID(HttpContext.User.Identity.Name), -1,model.SelCnt).Select(i => new ServiceJobSearchList
            {
                Customer = i.CompanyName,
                DueOn = i.DueOn,
                EquipmentType = i.EquipTypeName,
                CustomerSite = i.Department,
                Location = i.Location,
                Manufacturer = i.Manufacturer,
                EquipId = i.EquipUID,
                NextServiceMonth = i.NextServiceMonth.HasValue ? i.NextServiceMonth : 0,
                NextServiceYear = i.NextServiceYear.HasValue ? i.NextServiceYear : 0,
                DateServiced = i.DateServiced.HasValue ? i.DateServiced.Value.ToString("dd/MM/yyyy") : "",
                LastServiceUID = i.LastServiceUID.HasValue ? i.LastServiceUID.Value : 0,
                MDSItemCode = i.BNQItemCode,
                SerialNo = i.SerialNumber,
                FutureService=i.FutureService

            }).ToList();
            model.Service = s;


            return View(model);
        }
          [Authorize]
        public ActionResult GetEquipmentSearch()
        {
            var model = new EquipmentSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            return PartialView(model);
        }
          [Authorize]
        public ActionResult SearchEquipmentLoad()
        {
            var model = new EquipmentSearch();
            model.CustomerList = Utility.GetCustomerListByBranchId();

            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceMDS = true;
            model.DisplayEquip = true;
            model.ModelID = "-1";
            TrackerDataContext db = new TrackerDataContext();

            var s = db.SearchEquipment("", "", "", "", LoginController.BranchID(HttpContext.User.Identity.Name), true, "", true, false, Convert.ToInt32(model.ModelID), model.SelCnt).Select(i => new EquipmentSearchList
            {
                Customer = i.Customer,
                MDSItemNo = i.MDSItemNo,
                SerialNumber = i.SerialNumber,
                CustomerSite = i.CustomerSite,
                Location = i.Location,
                EquipDesc = i.EquipDesc,
                EquipementType = i.EquipementType,
                Manufacturer = i.Manufacturer,
                Model = i.Model,
                VendorName = i.VendorName,
                CurrentlyServicedByMDS = i.CurrentlyServicedByMDS ? "Yes" : "No",
                InService = i.InService ? "Yes" : "No",
                EquipID = i.EquipUID.ToString(),
                WarrantyExpirationDate = i.WarrantyExpirationDate,
                TotalCostOfRepairs = i.TotalCostOfRepairs,
                TotalRepairHours = i.TotalRepairHours,
                Cost = i.Cost



            }).ToList();
            model.EquipmentSearchList = s;
            return View("GetEquipmentSearch", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchEquipment(EquipmentSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search Equipment - BulkCreateServiceJob Request", 0, Request);

            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
            model.BranchList = Utility.GetBranchList();

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            if (model.ModelID == null)
                model.ModelID = "-1";
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }
            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.SerachMDS == null)
            {
                model.SerachMDS = "";
            }
            if (model.Customerid != null)
            {
                var Customer = model.Customerid;
                if (Customer == "-1")
                    Customer = "";
                var Equip = model.Equiptype;
                var Location = model.Locationid;
                var Customersite = model.Department;
                var SerachMDS = model.SerachMDS;
                var Branch = model.Branchid;
                var DisplayEquip = model.DisplayEquip.Value;
                TrackerDataContext db = new TrackerDataContext();
                if (Location == "--Location--")
                    Location = "";
                if (Customersite == "--Select Customer Site--")
                    Customersite = "";
                var s = db.SearchEquipment(Customer, Customersite, Location, Equip, LoginController.BranchID(HttpContext.User.Identity.Name), DisplayEquip, SerachMDS, model.ServiceMDS, false, Convert.ToInt32(model.ModelID), model.SelCnt).Select(i => new EquipmentSearchList
                {
                    EquipID = i.EquipUID.ToString(),
                    Customer = i.Customer,
                    MDSItemNo = i.MDSItemNo,
                    SerialNumber = i.SerialNumber,
                    CustomerSite = i.CustomerSite,
                    Location = i.Location,
                    EquipDesc = i.EquipDesc,
                    EquipementType = i.EquipementType,
                    Manufacturer = i.Manufacturer,
                    Model = i.Model,
                    VendorName = i.VendorName,
                    CurrentlyServicedByMDS = i.CurrentlyServicedByMDS ? "Yes" : "No",
                    InService = i.InService ? "Yes" : "No",
                    WarrantyExpirationDate = i.WarrantyExpirationDate,
                    TotalCostOfRepairs = i.TotalCostOfRepairs,
                    TotalRepairHours = i.TotalRepairHours,
                    Cost = i.Cost



                }).ToList();
                model.EquipmentSearchList = s;
            }
            return View("GetEquipmentSearch", model);
        }
          [Authorize]
        public JsonResult GetEquipInfomationData(int EquipUID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var model = db.USP_GetEquipNameByEquipId(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (model.Count > 0)
            {
                return Json(new { CustomerInfo = model[0].EquipDesc + ", " + model[0].CustomerInfo }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { CustomerInfo = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(ServiceJobSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search BulkCreateServiceJob submit", 0, Request);

            model.monthdataList = Utility.GetMonthyearsData();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
           

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }

            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }

            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }
            if (model.MonthVal == null)
            {
                model.MonthVal = "-1";
            }
            if (model.Customerid != null)
            {
                var Customer = model.Customerid;
                var Location = model.Locationid;
                var Customersite = model.Department;
                var Branch = model.Branchid;
                var Radiobutton = model.OverDue;
                var EquipID = Convert.ToInt32(model.SelectedEquipment);
                var MonthYear_DueBy = model.MonthVal;
                if (Location == "--Location--")
                    Location = "";
                if (Customersite == "--Select Customer Site--")
                    Customersite = "";
                TrackerDataContext db = new TrackerDataContext();
                var s = db.NextServiceSearch(Radiobutton, Customer, Customersite, Location, EquipID, model.Branchid, Convert.ToInt32(MonthYear_DueBy),model.SelCnt).Select(i => new ServiceJobSearchList
                {
                    Customer = i.CompanyName,
                    DueOn = i.DueOn,
                    EquipmentType = i.EquipTypeName,
                    CustomerSite = i.Department,
                    Location = i.Location,
                    Manufacturer = i.Manufacturer,
                    EquipId = i.EquipUID,
                    NextServiceMonth = i.NextServiceMonth.HasValue ? i.NextServiceMonth : 0,
                    NextServiceYear = i.NextServiceYear.HasValue ? i.NextServiceYear : 0,
                    DateServiced = i.DateServiced.HasValue ? i.DateServiced.Value.ToString("dd/MM/yyyy") : "",
                    LastServiceUID = i.LastServiceUID.HasValue ? i.LastServiceUID.Value : 0,
                    MDSItemCode = i.BNQItemCode,
                    SerialNo = i.SerialNumber,
                    FutureService = i.FutureService

                }).ToList();
                model.Service = s;
            }
            model.Cnt = new List<Int32>();
        //    model.Cnt.Add(100);
        //    model.Cnt.Add(200);
         //   model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
         //   model.Cnt.Add(3000);
          //  model.Cnt.Add(4000);
            model.Cnt.Add(5000);
            model.Cnt.Add(10000);

            return View(model);
        }

        [Authorize]
        //        [HttpPost]
        public JsonResult UpdateEquipServiceMonth(ServiceJobSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Update Service Job Month", 0, Request);

            model.monthdataList = Utility.GetMonthyearsData();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.BranchList = Utility.GetBranchList();


            if (model.Customerid == null)
            {
                model.Customerid = "";
            }

            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }

            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }
            if (model.MonthVal == null)
            {
                model.MonthVal = "-1";
            }

            var Customer = model.Customerid;
            var Location = model.Locationid;
            var Customersite = model.Department;
            var Branch = model.Branchid;
            var Radiobutton = model.OverDue;
            var EquipID = Convert.ToInt32(model.SelectedEquipment);
            var MonthYear_DueBy = model.MonthVal;

            TrackerDataContext db = new TrackerDataContext();
            int EquipServiceMonth1 = model.progDate.Month;
            int EquipServiceMonth2 = 0;
            short NumServicesPerYear = Convert.ToInt16(model.progDate.Year - 200);
            if (NumServicesPerYear > 1)
                EquipServiceMonth2 = model.progDate.Day;
            // var successcount = 0;
            //var html = "";
            if (model.SelCnt == null)
                model.SelCnt = 1000;

            {
                if (Location == "--Location--")
                    Location = "";
                if (Customersite == "--Select Customer Site--")
                    Customersite = "";
                var s = db.NextServiceSearch(Radiobutton, Customer, Customersite, Location, EquipID, model.Branchid, Convert.ToInt32(MonthYear_DueBy), model.SelCnt);
                foreach (var item in s.ToList())
                {
                    var eqp = db.Equipments.Where(e => e.EquipUID == item.EquipUID && e.BranchID == model.Branchid).First();
                    eqp.ServiceMonth1 = EquipServiceMonth1;
                    eqp.ServiceMonth2 = EquipServiceMonth2;
                    eqp.NumServicesPerYear = NumServicesPerYear;

                    db.SubmitChanges();
                }
            }
            if (Location == "--Location--")
                Location = "";
            if (Customersite == "--Select Customer Site--")
                Customersite = "";
            db.RecalcNextServiceAllEquipment(Radiobutton, Customer, Customersite, Location, EquipID, model.Branchid, Convert.ToInt32(MonthYear_DueBy), model.SelCnt);

            //{
            //    if (Location == "--Location--")
            //        Location = "";
            //    if (Customersite == "--Select Customer Site--")
            //        Customersite = "";
            //    if (model.SelCnt == null)
            //        model.SelCnt = 1000;
            //    var s = db.NextServiceSearch(Radiobutton, Customer, Customersite, Location, EquipID, model.Branchid, Convert.ToInt32(MonthYear_DueBy),model.SelCnt).Select(i => new ServiceJobSearchList
            //    {
            //        Customer = i.CompanyName,
            //        DueOn = i.DueOn,
            //        EquipmentType = i.EquipTypeName,
            //        CustomerSite = i.Department,
            //        Location = i.Location,
            //        Manufacturer = i.Manufacturer,
            //        EquipId = i.EquipUID,
            //        NextServiceMonth = i.NextServiceMonth.HasValue ? i.NextServiceMonth : 0,
            //        NextServiceYear = i.NextServiceYear.HasValue ? i.NextServiceYear : 0,
            //        DateServiced = i.DateServiced.HasValue ? i.DateServiced.Value.ToString("dd/MM/yyyy") : "",
            //        LastServiceUID = i.LastServiceUID.HasValue ? i.LastServiceUID.Value : 0,
            //        MDSItemCode = i.BNQItemCode,
            //        SerialNo = i.SerialNumber,
            //        FutureService = i.FutureService

            //    }).ToList();
            //    var res = new ServiceJobSearch();

            //    res.Service = s;


            //    return View(res);


            //}
            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CreateServiceJobExpiry(int EquipID, string CustomerCode,DateTime ProgrammedDate,DateTime ProgrammedStartTime, DateTime ProgrammedEndTime)
        {
            TrackerDataContext db = new TrackerDataContext();
            var eq = db.Equipments.Where(i => i.EquipUID == EquipID).FirstOrDefault();
            ServiceJob sj = new ServiceJob();
            sj.DateProgrammed = ProgrammedDate.Date.AddHours(ProgrammedStartTime.Hour).AddMinutes(ProgrammedStartTime.Minute);
            sj.DateProgrammedEnd = ProgrammedDate.Date.AddHours(ProgrammedEndTime.Hour).AddMinutes(ProgrammedEndTime.Minute);
            sj.LocationID = eq.LocationID;
            sj.BranchID = eq.BranchID;
            sj.CustomerCode = CustomerCode;
            sj.DeptID = eq.DeptID;
            sj.JobCode = 1234;//todo
            sj.Location = eq.Location;
            int sjid;
            if (db.ServiceJobs.Where(i => i.BranchID == eq.BranchID).Count() == 0)
                sjid = 0;
            else
                sjid = db.ServiceJobs.Where(i => i.BranchID == eq.BranchID).Max(i => i.ServiceJobUID);
            sjid++;
            sj.ServiceJobUID = sjid ;
            db.ServiceJobs.InsertOnSubmit(sj);
            db.SubmitChanges();

            Service sv = new Service();
            sv.BranchID=eq.BranchID;
            sv.EquipUID = EquipID;
            sv.ServiceJobUID = sjid;
            int swid;
            if (db.Services.Where(i => i.BranchID == eq.BranchID).Count() == 0)
                swid =0;
            else
                swid = db.Services.Where(i => i.BranchID == eq.BranchID).Max(i => i.ServiceUID);
            swid++;
            sv.ServiceUID = swid;
            db.Services.InsertOnSubmit(sv);
            db.SubmitChanges();

            return RedirectToAction("Edit","Service", new { id = sjid,BranchID=eq.BranchID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateServiceJob(ServiceJobSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "BulkCreateServiceJob Submit", 0, Request);

            model.monthdataList = Utility.GetMonthyearsData();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.BranchList = Utility.GetBranchList();


            if (model.Customerid == null)
            {
                model.Customerid = "";
            }

            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }

            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }
            if (model.MonthVal == null)
            {
                model.MonthVal = "-1";
            }

            var Customer = model.Customerid;
            var Location = model.Locationid;
            var Customersite = model.Department;
            var Branch = model.Branchid;
            var Radiobutton = model.OverDue;
            var EquipID = Convert.ToInt32(model.SelectedEquipment);
            var MonthYear_DueBy = model.MonthVal;

            TrackerDataContext db = new TrackerDataContext();

            if (Location == "--Location--")
                Location = "";
            if (Customersite == "--Select Customer Site--")
                Customersite = "";

            var s = db.AddNewServiceJobAndCustomersWorkOrders2(Radiobutton, Customer, Customersite, Location, EquipID, model.Branchid, Convert.ToInt32(MonthYear_DueBy), model.progDate, model.SelCnt,Convert.ToInt32(model.progEngineer)).ToList();
            var html = "";
            var successcount = 0;
            foreach (var item in s)
            {
                html = html + "<tr><td width='500' style='border:solid 1px black;padding:5px;'>" + item.messages + "</td>";
                if (item.success.Value)
                {
                    html = html + "<td style='border:solid 1px black;padding:5px;' width='50'><img src='images/greentick.png' /></td></tr>";
                    successcount = successcount + 1;
                }
                else
                {
                    html = html + "<td style='border:solid 1px black;padding:5px;'  width='50'><img src='images/redcross.png' /></td></tr>";
                }
            }
            var tophtml = "";

            tophtml = tophtml + "<tr><td colspan='2'>" + successcount.ToString() + " work orders were added to the new service job - Do you wish to view new service job?</td></tr>";
            tophtml = tophtml + "<tr><td colspan='2'> <input type='button' class='inputbuttonbg inputbig' value='Yes' id='btnYes' onclick='yes();'>&nbsp;&nbsp;&nbsp;&nbsp;<input type='button' class='inputbuttonbg inputbig' value='No' id='btnNo' onclick='no();'></td></tr>";
            tophtml = tophtml + "<tr><td colspan='2'>&nbsp;</td></tr>";
            return Json(new { msg = tophtml + html });

        }
          [Authorize]
        public ActionResult SearchServiceWorkorder()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All ServiceWorkOrder Request", 0, Request);

            var model = new EquipmentSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.AddNewServiceJobAndCustomersWorkOrders2("", "", "", "", -1, LoginController.BranchID(HttpContext.User.Identity.Name), 1, DateTime.Now, model.SelCnt,-1).Select(i => new EquipmentSearchList
            {
                Customer = i.messages,

            }).ToList();
            model.EquipmentSearchList = s;
            return PartialView(model);
        }
          [Authorize]
        public ActionResult CheckService(int EquipUID, short ServiceMonth, short ServiceYear)
        {
            var model = new EquipmentSearch();
            TrackerDataContext db = new TrackerDataContext();
            var Check = db.Services.Where(i => i.EquipUID == EquipUID && i.ServiceMonth == ServiceMonth && i.ServiceYear == ServiceYear && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (Check != null)
            {
                return Json(new { msg = "There already exists a service for " + ServiceMonth + "/" + ServiceYear, id = Check.ServiceUID });
            }
            else
            {
                return Json(new { msg = "" });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateServiceJobWorkOrder(ServiceJobSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "BulkCreateServiceJob Submit", 0, Request);

            model.monthdataList = Utility.GetMonthyearsData();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.BranchList = Utility.GetBranchList();

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }

            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }

            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }
            if (model.MonthVal == null)
            {
                model.MonthVal = "-1";
            }
            if (model.Customerid != null)
            {
                var Customer = model.Customerid;
                var Location = model.Locationid;
                var Customersite = model.Department;
                var Branch = model.Branchid;
                var Radiobutton = model.OverDue;
                var EquipID = Convert.ToInt32(model.SelectedEquipment);
                var MonthYear_DueBy = model.MonthVal;
                TrackerDataContext db = new TrackerDataContext();
                if (Location == "--Location--")
                    Location = "";
                if (Customersite == "--Select Customer Site--")
                    Customersite = "";

                if (model.SelCnt == null)
                    model.SelCnt = 1000;
                db.RecalcNextServiceAllEquipment(Radiobutton, Customer, Customersite, Location, EquipID, model.Branchid, Convert.ToInt32(MonthYear_DueBy), model.SelCnt);

            }
            return Json(new { success = true });

        }
          [Authorize]
        public JsonResult ServiceCount(int EquipUID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var SCount = db.Services.Where(i => i.EquipUID == EquipUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Count();
            var model = db.USP_GetEquipNameByEquipId(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            return Json(new { Count = SCount }, JsonRequestBehavior.AllowGet);
        }
          [Authorize]
        public JsonResult GetJobId()
        {
            TrackerDataContext db = new TrackerDataContext();
            var Serviceid = db.ServiceJobs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ServiceJobUID);
            return Json(new { id = Serviceid }, JsonRequestBehavior.AllowGet);
        }
          [Authorize]
          public ActionResult GetWOForEquipment(Int32 EquipID)
          {
            Utility.Audit(HttpContext.User.Identity.Name, "Get WorkOrder for Equipment Request", EquipID, Request);

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

              if (model.Location == null)
              {
                  model.Location = "";
              }
              model.Cnt = new List<Int32>();
              model.Cnt.Add(100);
              model.Cnt.Add(200);
              model.Cnt.Add(400);
              model.Cnt.Add(500);
              model.Cnt.Add(800);
              model.Cnt.Add(1000);
              model.Cnt.Add(2000);
              if (model.SelCnt == 0) model.SelCnt = 100;

              var Customer = model.Customerid;
              var Engineerid = model.EngineerID;
              var CustDepartment = model.Department;
              var Equiptype = model.Equiptype;
              TrackerDataContext db = new TrackerDataContext();

              var Location = model.Location;
              if (Location == "--Location--")
                  Location = "";
              if (CustDepartment == "--Select Customer Site--")
                  CustDepartment = "";

              var serviceworkorder = db.ServiceWorkOrderSearchNew(Equiptype, -1, Customer, CustDepartment, Convert.ToInt32(Engineerid), model.Branchid, false, model.ServicesAfterDate, Location, EquipID, model.ServicesAfterDateFilter, model.SelCnt).Select(i => new ServiceworkorderList
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

              var s = db.USP_GetEquipNameByEquipId(EquipID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
              if (s.Count > 0)
              {
                  model.EquipmentData = s[0].EquipDesc + ", " + s[0].CustomerInfo;
              }
              else
              {
                  model.EquipmentData = "";
              }

              return PartialView(model);
          }

          [Authorize]
          public ActionResult EquipServiceMonth()
          {
              return PartialView();
          }

          [Authorize]
          public ActionResult GetProgarmmeDate()
          {
            DateEngineer model = new DateEngineer();
            model.EngineerList = Utility.GetEngineerListByBranchId();

            return PartialView(model);
          }

          [Authorize]
          [HttpPost]
          public JsonResult PrintListExcel(ServiceJobSearch model)
          {
              return PrintPDFEXcel(model, true);
          }

        [Authorize]
        [HttpPost]
        public JsonResult PrintList(ServiceJobSearch model)
        {
            return PrintPDFEXcel(model,false);
        }

        private JsonResult PrintPDFEXcel(ServiceJobSearch model, bool Excel)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Print All BulkCreateServiceJob Request", 0, Request);

            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else if (model.Customerid == null)
            {
                model.Customerid = "";
            }

            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }

            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }
            if (model.MonthVal == null)
            {
                model.MonthVal = "-1";
            }

            var Customer = model.Customerid;
            var Location = model.Locationid;
            var Customersite = model.Department;
            var Branch = model.Branchid;
            var Radiobutton = model.OverDue;
            var EquipID = Convert.ToInt32(model.SelectedEquipment);
            var MonthYear_DueBy = model.MonthVal;
            if (model.SelCnt == null)
                model.SelCnt = 1000;
            if (Excel)
                return Json(new { URL = /*ConfigurationManager.AppSettings["URL"].ToString() +*/ "Reports/CreateServiceJobListRpt.aspx?dueOption=" + model.OverDue + "&monthYear=" + model.MonthVal + "&customerID=" + Customer + "&BranchID=" + Branch + "&&equipmentID=" + EquipID + "&department=" + Customersite + "&location=" + Location + "&Excel=true" + "&Count=" + model.SelCnt.ToString() });

            return Json(new { URL = /*ConfigurationManager.AppSettings["URL"].ToString() + */"Reports/CreateServiceJobListRpt.aspx?dueOption=" + model.OverDue + "&monthYear=" + model.MonthVal + "&customerID=" + Customer + "&BranchID=" + Branch + "&&equipmentID=" + EquipID + "&department=" + Customersite + "&location=" + Location + "&Count=" + model.SelCnt.ToString() });
        }
    }
}
