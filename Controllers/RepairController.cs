
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
using MDS.Reports;
using System.IO;
using System.Web.Security;

namespace MDS.Controllers
{
    public class RepairController : BaseController
    {
        public ActionResult PDFReportX(int APPLICATION_ID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Repair PDF", APPLICATION_ID, Request);
            DateTime dateInFrom = DateTime.Now;
            DateTime dateInTo = DateTime.Now;
            string customerID = "";
            string equipmentType = "";
            int BranchID = LoginController.BranchID(User.Identity.Name);
            char resolved_NotResolved_Either = char.Parse("N");
            char handoverCompleted_Incomplete_Either = char.Parse("E");
            int serviceJobID = -1;
                int equipmentID = -1;
            string department = "";
            string location = "";
            int engineerID = -1;
            string custOrderNo = "";
            string repairJobNo = "";
            DateTime dateOutFrom = DateTime.Now;
            DateTime dateOutTo = DateTime.Now;
            bool DateInFilter = false;
            bool DateOutFilter = false;
            int Cnt = 100;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/RepairListRpt.rdlc");

            ReportParameter rp = new ReportParameter("CompanyName", MDS.Controllers.LoginController.CompanyName);
            localReport.SetParameters(new ReportParameter[] { rp });


            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            if (location == "--Location--")
                location = "";
            if (department == "--Select Customer Site--")
                department = "";
            var dt = context.RepairsSearch(dateInFrom, dateInTo, customerID, equipmentType, BranchID, resolved_NotResolved_Either, handoverCompleted_Incomplete_Either, serviceJobID, equipmentID, department, location, engineerID, custOrderNo, repairJobNo, dateOutFrom, dateOutTo, DateInFilter, DateOutFilter, Cnt).AsQueryable();

            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt.ToList());

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
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.2in</MarginTop>" +
            "  <MarginLeft>0.2in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.2in</MarginBottom>" +
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
            FileContentResult ff = File(renderedBytes, mimeType);

            return ff;

        }

        public ActionResult PDFReport(DateTime dateInFrom, DateTime dateInTo, string customerID, string equipmentType, int BranchID, char resolved_NotResolved_Either, char handoverCompleted_Incomplete_Either, int serviceJobID, int equipmentID, string department, string location, int engineerID, string custOrderNo, string repairJobNo, DateTime dateOutFrom, DateTime dateOutTo, bool DateInFilter, bool DateOutFilter, int Cnt)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Repair PDF", 0, Request);
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/RepairListRpt.rdlc");

            ReportParameter rp = new ReportParameter("CompanyName", MDS.Controllers.LoginController.CompanyName);
            localReport.SetParameters(new ReportParameter[] { rp });


            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            if (location == "--Location--")
                location = "";
            if (department == "--Select Customer Site--")
                department = "";
            var dt= context.RepairsSearch(dateInFrom, dateInTo, customerID, equipmentType, BranchID, resolved_NotResolved_Either, handoverCompleted_Incomplete_Either, serviceJobID, equipmentID, department, location, engineerID, custOrderNo, repairJobNo, dateOutFrom, dateOutTo, DateInFilter, DateOutFilter, Cnt).AsQueryable();

            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt.ToList());

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
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.2in</MarginTop>" +
            "  <MarginLeft>0.2in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.2in</MarginBottom>" +
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
            FileContentResult ff = File(renderedBytes, mimeType);

            return ff;
        }


        [Authorize]
        public ActionResult Index(int? Customerid)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Repair Request", 0, Request);
            var model = new RepairSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.Cnt = new List<Int32>();
                model.Cnt.Add(100);
                model.Cnt.Add(200);
        //        model.Cnt.Add(400);
                model.Cnt.Add(500);
          //      model.Cnt.Add(800);
                model.Cnt.Add(1000);
                model.Cnt.Add(2000);
            model.Cnt.Add(5000);
            model.Cnt.Add(10000);
            model.SelCnt = 100;
            
            if (Session["R_Equip"] != null)
            {
                model.Equiptype = Session["R_Equip"].ToString();
            }
            else
            {
                model.Equiptype = "";
            }
            if (Session["R_Engineer"] != null)
            {
                model.EngineerID = Session["R_Engineer"].ToString();
            }
            else
            {
                model.EngineerID = "";
            }
            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if (Customerid != null)
                Session["R_Customer"] = Customerid;
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else if (Session["R_Customer"]!= null)
            {
                model.Customerid = Session["R_Customer"].ToString();
            }
            else
            {
                model.Customerid = "";
            }
            if(Session["R_CustSite"]!=null)
            {
                model.Department = Session["R_CustSite"].ToString();
            }
            else
            {
                model.Department = "";
            }
            if (Session["R_Location"] != null)
            {
                model.Locationid = Session["R_Location"].ToString();
            }
            else
            {
                model.Locationid = "";
            }
            if (Session["R_WorkOrderResolved"] != null)
            {
                model.Resolved = Session["R_WorkOrderResolved"].ToString();
            }
            else
            {
                if (!MDS.Controllers.LoginController.AllCustomers(@User.Identity.Name))
                    model.Resolved = "R";
                else
                    model.Resolved = "N";

            }
            if (Session["R_WorkOrderComplete"] != null)
            {
                model.Complete = Session["R_WorkOrderComplete"].ToString();
            }
            else
            {
                model.Complete = "E";
            }
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 120;
            int EngID = -1;
            if (model.EngineerID != "")
                EngID = Convert.ToInt32(model.EngineerID);

            if (model.Locationid == "--Location--")
                model.Locationid = "";
            if (model.Department == "--Select Customer Site--")
                model.Department = "";
            var s = db.RepairsSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), model.Customerid, model.Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name),Convert.ToChar(model.Resolved),Convert.ToChar(model.Complete), -1, -1, model.Department, model.Locationid, EngID, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false,model.SelCnt).Select(i => new RepairSearchList
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
                    FaultDetails=i.FaultDetails,
                    WorkDone=i.WorkDone,
                RepairTravelExpenseCost = i.RepairTravelExpenseCost,
                PartsCost = i.PartsCost,
                    WarrantyExpirationDate=i.WarrantyExpirationDate

            }).ToList();
            model.Repairs = s;
            model.FromDate = DateTime.Now.AddMonths(-1);
            model.ToDate = DateTime.Now;

            model.OutFromDate = DateTime.Now.AddMonths(-1);
            model.OutToDate = DateTime.Now;
            return View(model);
        }

        [Authorize]
        public JsonResult GetCustomerSite(String Customerid, Int32 Branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Customersite = db.CustomerDepartments.Where(i => i.CustomerCode == Customerid && i.BranchID == Branchid).AsEnumerable().Select(i => new MDS.Models.CustomerSite
            {
                Department = i.Department,
                DeptID = i.DeptID.ToString()
            }).ToList();
            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID = "-1", Department = "--Select Customer Site--" });
            return Json(Customersite, JsonRequestBehavior.AllowGet);
        }
          [Authorize]
        public ActionResult GetParts([DataSourceRequest]DataSourceRequest request)
        {

            List<PartsUsedList> PartUsedRepair;
              if (Session["Partused"]==null)
                  PartUsedRepair = (List<PartsUsedList>)Session["PartusedSWO"];

              else
                  PartUsedRepair= (List<PartsUsedList>)Session["Partused"];
            DataSourceResult result = PartUsedRepair.ToDataSourceResult(request);
            return Json(result);

        }

        [HttpPost]
        public JsonResult DocumentDelete(int PhotosID,string url)
        {
            try
            {
                TrackerDataContext db = new TrackerDataContext();
                var photo= db.Photos.Where(p => p.PhotosID == PhotosID).FirstOrDefault();
                db.Photos.DeleteOnSubmit(photo);
                db.SubmitChanges();

                var path = Path.Combine(Server.MapPath(uploadfolder), url);


                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);//DO WE DELETE

                return Json(new { Error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });

            }
        }

        //public enum FileType
        //{
        //    Attach=0,
        //    Audio=1,
        //    Photo=2,
        //    Video=3
        //}
        public PartialViewResult UploadFile(string Attach_Audio,string field,int? RepairID, int? ServiceID)
        {
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            Photo uf = new Photo();
            uf.RepairID = RepairID;
            uf.ServiceID = ServiceID;
            uf.Title = field;
            uf.BranchID = BranchID;
            ViewBag.Attach_Audio = Attach_Audio;
            return PartialView(uf);
        }

        [HttpPost]
        public JsonResult UploadFileSave(HttpPostedFileBase File, int? RepairID, int? ServiceID, int? BranchID)//,string Title)
        {
            try
            {
                if (File.ContentLength == 0)
                {
                    return Json(new { Error = "File is empty - 0 size" });
                }
                int id;
                string tpe;
                if (RepairID != null)
                {
                    tpe = "Rpr";
                    id = Convert.ToInt32(RepairID);
                }
                else
                {
                    tpe = "Svc";
                    id = Convert.ToInt32(ServiceID);
                }
                int brid = Convert.ToInt32(BranchID);
                var contenttype = File.ContentType;
                var fileName = Path.GetFileName(File.FileName);
                fileName = tpe + "_" + id.ToString("######") + "-" + brid.ToString() + "-" + fileName.Replace(' ', '-'); //For TheDock naming convention
                var path = Path.Combine(Server.MapPath(uploadfolder), fileName);
                if (System.IO.File.Exists(path))
                {
                    return Json(new { Error = "A file with this name already exists. Please rename your file, or remove the previous file prior to uploading" });
                }
                File.SaveAs(path);
                Photo pt = new Photo();
                pt.Title = File.FileName.ToString();
                pt.RepairID = RepairID;
                pt.ServiceID = ServiceID;
                pt.BranchID = brid;
                pt.URL = path;// fileName;
                pt.Type = contenttype;
                TrackerDataContext db = new TrackerDataContext();
                db.Photos.InsertOnSubmit(pt);
                db.SubmitChanges();
                return Json(new { Error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message});

            }
        }

        public static string uploadfolder = "~/App_Data/UploadedFiles";
        public FileResult Download(string Fle, string contentType)
        {
            string fileName = Path.Combine(Server.MapPath(uploadfolder), Fle);//"~/Views/Applications/Uploads"
                                                                              //string contentType = "jpg";
            return new FilePathResult(fileName, contentType);
        }


     
        [Authorize]
          public AddRepaire GetEditRepair(Int32 id, int branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var model = new AddRepaire();
            model.EngineerList = Utility.GetEngineerList();
            model.ConditionList = Utility.GetConditionList();

            model.TravelList = Utility.GetTravelList(branchid);
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ChargeType = Utility.GetLabourList(branchid);
            var RepairData = db.Repairs.Where(i => i.RepairUID == id && i.BranchID == branchid).ToList();
            model.RepairId = id;
              
            Session["NewRepairId"] = id;
            model.EquipId = RepairData[0].EquipUID.Value;
            model.SelectedEquipment = RepairData[0].EquipUID.Value.ToString();
            if (RepairData[0].DateCallReceived.HasValue)
            {
                model.DateInitalCall = RepairData[0].DateCallReceived.Value;
            }
            if (RepairData[0].TimeCallReceived.HasValue)
            {
                model.TimeInitalCall = RepairData[0].TimeCallReceived.Value;
            }
            if (RepairData[0].DateIn.HasValue)
            {
                model.DateEquipRepair = RepairData[0].DateIn.Value;
            }

            if (RepairData[0].TimeIn.HasValue)
            {
                model.TimeEquipRepair = RepairData[0].TimeIn.Value;
            }
            if (RepairData[0].TimeOut.HasValue)
            {
                model.TimeOutEquipRepair = RepairData[0].TimeOut.Value;
            }
            model.CustomerSignature = RepairData[0].CustomerSignature;
            model.JobCode = Utility.GetBranchCode() + "R" + RepairData[0].JobCode;
            model.HasItem = RepairData[0].HandoverCompleted;
            model.Approve = RepairData[0].ApprovalRequired;
            model.ApprovalContact = RepairData[0].ApprovalContact;
            model.orderNumber = RepairData[0].OrderNumberRequired;
            model.Accessories = RepairData[0].Accessories;
            model.VerbalApproval = RepairData[0].VerbalApprovalObtained;
            model.ApprovalReceived = RepairData[0].ApprovedBy;
            model.OrderNo = RepairData[0].OrderNumber;
            model.FaultDetail = RepairData[0].FaultDetails;
            model.WorkDone = RepairData[0].WorkDone;
            model.LabourHours = RepairData[0].RepairHours;
            model.TravelHours = RepairData[0].TravelHours;
            model.SafetyTestDone = RepairData[0].SafetyTestDone;
            model.Charge = RepairData[0].Charge;
            model.ChargePartsOnly = RepairData[0].ChargePartsOnly;
            model.NoCharge = RepairData[0].NoCharge;
            model.PromotionalExpenses = RepairData[0].ExpensesProportion;
            model.AllSpecified = RepairData[0].RepairCompleted;
            model.ResultRepair = RepairData[0].ResultedInRetirement;
            model.ItemRepair = RepairData[0].NotRepaired;
            model.HasJob = RepairData[0].HasBeenInvoiced;
            model.Notes = RepairData[0].Notes;
           
            model.PersonName = RepairData[0].ReceiverName;
            model.TravelTypeCode = RepairData[0].TravelTypecode;
            model.ChargeTypeCode = RepairData[0].ChargeTypecode;
            model.TravelRate = RepairData[0].TravelChargeRate.HasValue ? RepairData[0].TravelChargeRate.Value.ToString("C") : "$0.00";
            model.LabourRate = RepairData[0].ChargeRate.HasValue ? RepairData[0].ChargeRate.Value.ToString("C") : "$0.00";
           

            model.Invoice = RepairData[0].InvoiceNumber;
            if (RepairData[0].InvoiceDate.HasValue)
            {
                model.InvoiceDate = RepairData[0].InvoiceDate.Value;
            }
            model.EarthResistant = RepairData[0].EarthResistance.HasValue ? RepairData[0].EarthResistance.Value : 0.00M;
            model.Insulation = RepairData[0].InsulationResistance.HasValue ? RepairData[0].InsulationResistance.Value : 0.00M;
            model.LeakageCurrent = RepairData[0].LeakageCurrent.HasValue ? RepairData[0].LeakageCurrent.Value : 0.00M;

            model.ChargesInvoice = RepairData[0].Amount;
            model.AcctuallyWorkDone = RepairData[0].WorkDone;


            if (RepairData[0].DateRepairFinished.HasValue)
            {
                model.RepairDate = RepairData[0].DateRepairFinished.Value;
            }
            if (RepairData[0].DateOut.HasValue)
            {
                model.JobCompletionDate = RepairData[0].DateOut;
            }
            if (RepairData[0].TimeOut.HasValue)
            {
               // model.JobCompletionTime = RepairData[0].TimeOut;
                model.JobCompletionTime = RepairData[0].TimeOut.Value.Hour * 100;
                if (RepairData[0].TimeOut.Value.Minute > 45)
                    model.JobCompletionTime = model.JobCompletionTime + 100;
                else if (RepairData[0].TimeOut.Value.Minute > 15)
                    model.JobCompletionTime = model.JobCompletionTime + 30;
            }
            else
                model.JobCompletionTime = 1200;

            if (RepairData[0].ReceiptDate.HasValue)
            {
                model.ReceiptDate = RepairData[0].ReceiptDate;
            }
            model.PersonName = RepairData[0].ReceiverName;
            model.RetirementReportPrinted = RepairData[0].RetirementReportPrinted;
            model.InvoiceThruServiceJob = RepairData[0].InvoiceThruServiceJob;
            model.EngineerID = RepairData[0].EngineerID.HasValue ? RepairData[0].EngineerID.Value.ToString() : "";
            model.ConditionID = RepairData[0].ConditionID.HasValue ? RepairData[0].EngineerID.Value.ToString() : "";
            if (RepairData[0].ServiceJobUID != null)
            {
                model.AssociateServiceJob = db.ServiceJobCode(RepairData[0].ServiceJobUID, LoginController.BranchID(HttpContext.User.Identity.Name));
            }
            else
            {
                model.AssociateServiceJob = "";
            }
            model.ServiceJobID = RepairData[0].ServiceJobUID.HasValue ? RepairData[0].ServiceJobUID.Value : 0;
            model.TechnicalContact = RepairData[0].TechContact;
            model.ChargeTypeHours = ServiceController.LabourSummary(id, db, branchid, true);
            ViewBag.JobCompletionTimeX = MDS.Controllers.ServiceController.FillHourMinDDL().ToList();

            db.Dispose();
            return model;
        }

        public ActionResult Photos(int RepairID,int? BranchID,bool? Readonly)
        {
            ViewBag.Readonly = Readonly;
            RepairPhotos ret =new RepairPhotos();
            ret.Id = RepairID;
            TrackerDataContext db = new TrackerDataContext();
            if (BranchID==null)
                BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            ret.Photos = db.Photos.Where(p=>p.RepairID==RepairID && p.BranchID==BranchID).ToList();
            return View( ret);
        }

          public ActionResult LabourChargeIndex()
          {
            //  var id = "";
              var model = new LabourChargeList();
              TrackerDataContext db = new TrackerDataContext();
              var LabourCharges = db.ChargeTypes.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new LabourCharge
              {
                  ChargeTypeCode = i.ChargeTypecode,
                  ChargeType = i.ChargeType1 ,
                  ChargeRate = i.ChargeRate

              }).ToList();
              model.LabourCharges = LabourCharges.OrderBy(i=>i.ChargeType).ToList();
              return View("~/Views/Repair/LabourCharges.cshtml", model);
          }
          [Authorize]
          [HttpPost]
          public ActionResult LabourChargeIndex(LabourCharge model1)
          {
              var model = new LabourChargeList();
              TrackerDataContext db = new TrackerDataContext();
              var LabourCharges = db.ChargeTypes.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new LabourCharge
              {
                  ChargeTypeCode = i.ChargeTypecode,
                  ChargeType = i.ChargeType1,
                  ChargeRate = i.ChargeRate

              }).ToList();
              model.LabourCharges = LabourCharges;
              return View("~/Views/Repair/LabourCharges.cshtml", model);
          }


          [Authorize]
          public ActionResult CreateLabourCharge([DataSourceRequest]DataSourceRequest request, LabourCharge s)
          {
              TrackerDataContext db = new TrackerDataContext();
              if (ModelState.IsValid)
              {
                  var lc = new MDS.DB.ChargeType();
                  lc.ChargeTypecode = s.ChargeTypeCode;
                  lc.ChargeType1 = s.ChargeType.ToString();
                  lc.ChargeRate = s.ChargeRate;
                  lc.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                  db.ChargeTypes.InsertOnSubmit(lc);
                  db.SubmitChanges();

              }
              return Json(new[] { s }.ToDataSourceResult(request, ModelState));
          }
          [Authorize]
          public ActionResult UpdateLabourCharge([DataSourceRequest]DataSourceRequest request, LabourCharge s)
          {
              TrackerDataContext db = new TrackerDataContext();
              if (ModelState.IsValid)
              {
                  var lc = db.ChargeTypes.Where(i => i.ChargeTypecode==s.ChargeTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                  //db.ChargeTypes.DeleteOnSubmit(p);
                  //db.SubmitChanges();

                  //var lc = new MDS.DB.ChargeType();
                  lc.ChargeTypecode = s.ChargeTypeCode;
                  lc.ChargeType1 = s.ChargeType.ToString();
                  lc.ChargeRate = s.ChargeRate;
                  //dep.s_GUID = g;
            //      db.ChargeTypes.InsertOnSubmit(lc);
                  db.SubmitChanges();

              }

              // Return the updated product. Also return any validation errors.
              return Json(new[] { s }.ToDataSourceResult(request, ModelState));
          }

/// ///////////
          public ActionResult TravelChargeIndex()
          {
              var model = new LabourChargeList();
              TrackerDataContext db = new TrackerDataContext();
              var LabourCharges = db.TravelTypes.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new LabourCharge
              {
                  ChargeTypeCode = i.TravelTypeCode,
                  ChargeType = i.TravelType1,
                  ChargeRate = i.TravelChargeRate

              }).ToList();
              model.LabourCharges = LabourCharges;
              return View("~/Views/Repair/TravelCharges.cshtml", model);
          }
          [Authorize]
          [HttpPost]
          public ActionResult TravelChargeIndex(LabourCharge model1)
          {
              var model = new LabourChargeList();
              TrackerDataContext db = new TrackerDataContext();
              var LabourCharges = db.TravelTypes.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new LabourCharge
              {
                  ChargeTypeCode = i.TravelTypeCode,
                  ChargeType = i.TravelType1,
                  ChargeRate = i.TravelChargeRate

              }).ToList();
              model.LabourCharges = LabourCharges;
              return View("~/Views/Repair/TravelCharges.cshtml", model);
          }


          [Authorize]
          public ActionResult CreateTravelCharge([DataSourceRequest]DataSourceRequest request, LabourCharge s)
          {
              TrackerDataContext db = new TrackerDataContext();
              if (ModelState.IsValid)
              {
                  var lc = new MDS.DB.TravelType();
                  lc.TravelTypeCode = s.ChargeTypeCode;
                  lc.TravelType1 = s.ChargeType.ToString();
                  lc.TravelChargeRate = s.ChargeRate;
                  lc.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                  db.TravelTypes.InsertOnSubmit(lc);
                  db.SubmitChanges();

              }
              return Json(new[] { s }.ToDataSourceResult(request, ModelState));
          }
          [Authorize]
          public ActionResult UpdateTravelCharge([DataSourceRequest]DataSourceRequest request, LabourCharge s)
          {
              TrackerDataContext db = new TrackerDataContext();
              if (ModelState.IsValid)
              {
                  var lc = db.TravelTypes.Where(i => i.TravelTypeCode == s.ChargeTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                  lc.TravelTypeCode = s.ChargeTypeCode;
                  lc.TravelType1 = s.ChargeType.ToString();
                  lc.TravelChargeRate = s.ChargeRate;
                  db.SubmitChanges();

              }

              // Return the updated product. Also return any validation errors.
              return Json(new[] { s }.ToDataSourceResult(request, ModelState));
          }


        [Authorize]
        public ActionResult EditCharge(string id,string LabTrav)
        {
            ChargeTypes ct = new ChargeTypes();
            ct.LabTrav = LabTrav;
            if (LabTrav == "Lab")
            {
                if (id != null)
                {
                    TrackerDataContext db = new TrackerDataContext();
                    var ctx = db.ChargeTypes.Where(i => i.ChargeTypecode == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                    ct.ChargeTypeCode = ctx.ChargeTypecode;
                    if (ctx.ChargeRate == null)
                        ct.Rate = 0;
                    else
                        ct.Rate = (decimal)ctx.ChargeRate;
                    ct.ChargeTypeDesc = ctx.ChargeType1;
                    ct.New = false;
                }
                else

                    ct.New = true;
            }
            else
            {
                if (id != null)
                {
                    TrackerDataContext db = new TrackerDataContext();
                    var ctx = db.TravelTypes.Where(i => i.TravelTypeCode == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                    ct.ChargeTypeCode = ctx.TravelTypeCode;
                    if (ctx.TravelChargeRate == null)
                        ct.Rate = 0;
                    else
                        ct.Rate = (decimal)ctx.TravelChargeRate;
                    ct.ChargeTypeDesc = ctx.TravelType1;
                    ct.New = false;
                }
                else

                    ct.New = true;

            }
            return View(ct);
        }
        [HttpPost]
        public ActionResult EditCharge(ChargeTypes ct)
        {
            if (!ModelState.IsValid)
                return View(ct);
            TrackerDataContext db = new TrackerDataContext();
            if (ct.LabTrav == "Lab")
            {
                ChargeType ctx = db.ChargeTypes.Where(i => i.ChargeTypecode == ct.ChargeTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (ctx == null)
                {
                    ctx = new ChargeType();
                    ctx.ChargeRate = ct.Rate;
                    ctx.ChargeTypecode = ct.ChargeTypeCode;
                    ctx.ChargeType1 = ct.ChargeTypeDesc;
                    ctx.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                    db.ChargeTypes.InsertOnSubmit(ctx);
                }
                else
                {
                    if (ct.New)
                    {
                        ViewBag.Error = "There is already a charge type with code " + ct.ChargeTypeCode + " Please enter a new code.";
                        ct.ChargeTypeCode = null;
                        return View(ct);
                    }
                    ctx.ChargeRate = ct.Rate;
                    ctx.ChargeTypecode = ct.ChargeTypeCode;
                    ctx.ChargeType1 = ct.ChargeTypeDesc;
                }
            }
            else
            {
                TravelType ctx = db.TravelTypes.Where(i => i.TravelTypeCode == ct.ChargeTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (ctx == null)
                {
                    ctx = new TravelType();
                    ctx.TravelChargeRate = ct.Rate;
                    ctx.TravelTypeCode = ct.ChargeTypeCode;
                    ctx.TravelType1 = ct.ChargeTypeDesc;
                    ctx.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                    db.TravelTypes.InsertOnSubmit(ctx);
                }
                else
                {
                    if (ct.New)
                    {
                        ViewBag.Error = "There is already a charge type with code " + ct.ChargeTypeCode + " Please enter a new code.";
                        ct.ChargeTypeCode = null;
                        return View(ct);
                    }
                    ctx.TravelChargeRate = ct.Rate;
                    ctx.TravelTypeCode = ct.ChargeTypeCode;
                    ctx.TravelType1 = ct.ChargeTypeDesc;
                }
            }
            db.SubmitChanges();
            if (ct.LabTrav=="Lab")
                return RedirectToAction("LabourChargeIndex");
            return RedirectToAction("TravelChargeIndex");
        }

        [Authorize]
        public ActionResult Edit(int id, int? BranchID)
        {
            if (BranchID != null)
            {
                if (BranchID != LoginController.BranchID(User.Identity.Name))
                {
                    FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name) + "," + "A", true);
                    return RedirectToAction("Edit", new { id = id, BranchID = BranchID });
                }
            }
            var data = HttpContext.User.Identity.Name;
            ATC atc = MDS.Controllers.LoginController.IsAdmin(data);
            if (atc == ATC.Tech)
                return RedirectToAction("Repair", "Tech", new { id = id, BranchID = BranchID });

            Utility.Audit(HttpContext.User.Identity.Name, "Edit Repair Request", id, Request);
            var model = new AddRepaire();
            model = GetEditRepair(id, LoginController.BranchID(HttpContext.User.Identity.Name));
            Session["NewRepairID"] = id;
            model.PopUp = false;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditNew(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit New Repair Request", id, Request);
            var model = new AddRepaire();
            model = GetEditRepair(id, LoginController.BranchID(HttpContext.User.Identity.Name));
            Session["NewRepairID"] = id;
            ViewBag.AddNew = 1;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUp(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Repair Request", id, Request);
            var model = new AddRepaire();
            model = GetEditRepair(id, LoginController.BranchID(HttpContext.User.Identity.Name));
            Session["NewRepairID"] = id;
            model.PopUp = true;
            ViewBag.Popup = 1;
            return View("Add", model);
        }
          [Authorize]
        public ActionResult Add()
        {
            //ViewBag.CustomerX = Utility.GetCustomerListByBranchIdSelect();

            Utility.Audit(HttpContext.User.Identity.Name, "Add Repair Request", 0, Request);
            var model = new AddRepaire();
            model.EngineerList = Utility.GetEngineerList();
            model.ConditionList = Utility.GetConditionList();
            model.TravelList = Utility.GetTravelList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ChargeType = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.LabourRate = "$0.00";
            model.TravelRate = "$0.00";
            model.TravelHours = 0.0f;
            model.LabourHours = 0.0f;
            model.RepairId = 0;
            model.ServiceJobID = 0;
            model.AssociateServiceJob = "";
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var JobNo = db.Repairs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode) + 1;
            model.JobCode = Utility.GetBranchCode() + "R" + JobNo;
            model.CustomerSignature = false;
           // List<PartsUsedList> PartUsedRepair = new List<PartsUsedList>();
           // Session["Partused"] = PartUsedRepair;
            Session["NewRepairID"] = 0;
            ViewBag.JobCompletionTimeX = MDS.Controllers.ServiceController.FillHourMinDDL().ToList();

            return View(model);
        }
          [Authorize]
        public JsonResult GetEquipmentItem()
        {
            TrackerDataContext db = new TrackerDataContext();
            string Search = Request.Params["filter[filters][0][value]"];
            var model = db.USP_GetEquipName(Search, 1);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
          [Authorize]
        public JsonResult GetCustomerName(String EquipUID)
        {

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var model = db.USP_GetCustomerEquipment(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        public JsonResult GetEquipInfomation(int EquipUID)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var model = db.USP_GetEquipNameByEquipId(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (model.Count > 0)
            {
                return Json(new { CustomerInfo = model[0].CustomerInfo, CustomerCode = model[0].CustomerCode, EquipItem = model[0].EquipDesc, WarrantyExpirationDate = model[0].WarrantyExpirationDate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { CustomerInfo = "", CustomerCode = "", EquipItem = "", WarrantyExpirationDate = "" }, JsonRequestBehavior.AllowGet);
            }
        }
          [Authorize]
        public JsonResult GetEquipInfomationData(int EquipUID)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
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
        public ActionResult CreatePart([DataSourceRequest]DataSourceRequest request, PartsUsedList part)
        {
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["Partused"];
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
                Session["Partused"] = PartUsedRepair;
            }
            return Json(new[] { part }.ToDataSourceResult(request, ModelState));
        }
        
        [Authorize]
        public ActionResult UpdatePart([DataSourceRequest]DataSourceRequest request, PartsUsedList part)
        {
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["Partused"];
                var p = PartUsedRepair.Where(i => i.Id == part.Id ).FirstOrDefault();
                p.Id = part.Id;
                p.PartId = part.PartId;
                p.PartName = part.PartName;
                p.PartNumber = part.PartNumber;
                p.NoOfParts = part.NoOfParts;
                p.Stocked_Part = part.Stocked_Part;
                p.Price = part.Price;
                Session["Partused"] = PartUsedRepair;
            }
            // Return the updated product. Also return any validation errors.
            return Json(new[] { part }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult DeletePart([DataSourceRequest]DataSourceRequest request, PartsUsedList part)
        {
            if (ModelState.IsValid)
            {
                List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["Partused"];
                var p = PartUsedRepair.Where(i => i.PartId == part.PartId).FirstOrDefault();
                PartUsedRepair.Remove(p);
                for (int i = 0; i < PartUsedRepair.Count; i++)
                {
                    PartUsedRepair[i].Id = i + 1;
                }
                Session["Partused"] = PartUsedRepair;
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { part }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult PartItem()
        {
            return PartialView();
        }
          [Authorize]
        public JsonResult GetPartItem()
        {
            TrackerDataContext db = new TrackerDataContext();

            db.CommandTimeout = 90;
            string Search = Request.Params["filter[filters][0][value]"];
            var Partitems = db.PartsSearch(Search, MDS.Controllers.LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            return Json(Partitems, JsonRequestBehavior.AllowGet);
        }

          [Authorize]
        [HttpPost]
        public JsonResult ChargeTypeRate(string ChargeTypeCode)
        {
            TrackerDataContext db = new TrackerDataContext();
            var ChargeTypeRate = db.ChargeTypes.Where(i => i.ChargeTypecode == ChargeTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            return Json(new { rate = ChargeTypeRate.ChargeRate });
        }
          [Authorize]
        [HttpPost]
        public JsonResult TravelTypeRate(string TravelTypeCode)
        {
            if (TravelTypeCode != "")
            {
                TrackerDataContext db = new TrackerDataContext();
                var TravelTypeRate = db.TravelTypes.Where(i => i.TravelTypeCode == TravelTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                return Json(new { rate = TravelTypeRate.TravelChargeRate });
            }
            else { return Json(new { rate = 0 }); }
        }
          [Authorize]
        [HttpPost]
        public JsonResult PartPrice()
        {
            List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["Partused"];
            var cost = 0.0M;
            if (PartUsedRepair != null)
            {
                foreach (var item in PartUsedRepair)
                {
                    cost = cost + (item.Price    * item.NoOfParts);
                }
            }
            return Json(new { cost = cost.ToString("C") });
        }
          [Authorize]
        public ActionResult GetServiceJobSearch()
        {
            var model = new ServiceSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.EngineerList = Utility.GetEngineerList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            return PartialView(model);
        }
          [Authorize]
        public ActionResult GetEquipmentSearch()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Repair/Service Equipment Search", 0, Request);
            var model = new EquipmentSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            return PartialView(model);
        }

          [Authorize]
        public ActionResult SearchEquipmentLoadByCustomerCode(string id)
        {
            id = id.Replace("~","&");
            var model = new EquipmentSearch();
            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceMDS = true;
            model.DisplayEquip = true;
            model.Customerid = id;
            model.ModelID = "-1";
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.SelCnt = 100;
            if (model.SelCnt == 0)
                model.SelCnt = 100;

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            if (id == "-1")
                id = "";
            var s = db.SearchEquipment(id, "", "", "", LoginController.BranchID(HttpContext.User.Identity.Name), true, "", true, false, Convert.ToInt32(model.ModelID), model.SelCnt).Select(i => new EquipmentSearchList
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
        /// /////

        [Authorize]
        [HttpPost]
        public ActionResult SearchEquipment(EquipmentSearch model)
        {

            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
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

            if (model.SelCnt == 0)
                model.SelCnt = 100;

            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                // Session["E_Customer_ID"] = model.Customerid;
                model.Customerid = Cust;
            }
            else if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            else if (model.Customerid == "-1")
            {
                model.Customerid = "";
            }
            Session["E_Customer_ID"] = model.Customerid;

            if (model.Department == null)
            {
                model.Department = "";
            }
            Session["E_Department"] = model.Department;

            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            Session["E_Equiptype"] = model.Equiptype;
            if (model.ModelID == null)
            {
                model.ModelID = "-1";
            }
            Session["E_ModelID"] = model.ModelID;

            if (model.DisplayEquip == null)
                model.DisplayEquip = true;
            Session["E_DisplayEquip"] = model.DisplayEquip;

            if (model.ServiceMDS == null)
                model.ServiceMDS = true;
            Session["E_ServiceMDS"] = model.ServiceMDS;

            if (model.Locationid == null)
            {
                model.Locationid = "";
            }
            Session["E_Location"] = model.Locationid;

            if (model.SerachMDS == null)
            {
                model.SerachMDS = "";
            }

            Session["E_SerialNo"] = model.SerachMDS;

            /*  TrackerDataContext db = new TrackerDataContext();
              db.CommandTimeout = 90;
              var s = db.SearchEquipment(model.Customerid, model.Department, model.Locationid, model.Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToBoolean(model.DisplayEquip), model.SerachMDS, model.ServiceMDS, false, Convert.ToInt32(model.ModelID), model.SelCnt).Select(i => new EquipmentSearchList

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
              */
            return View("GetEquipmentSearch", model);
        }

        [Authorize]
        public ActionResult SearchEquipmentLoad()
        {
            var model = new EquipmentSearch();
            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            //    model.CustomerSiteList = Utility.GetCustomerSitesList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceMDS = true;
            model.DisplayEquip = true;
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(400);
            model.Cnt.Add(500);
            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.SelCnt = 100;

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else if (Session["E_Customer_ID"] != null)
            {
                model.Customerid = Session["E_Customer_ID"].ToString();
            }
            else
                model.Customerid = "";
            if (Session["E_Department"] != null)
            {
                model.Department = Session["E_Department"].ToString();
            }
            else
                model.Department = "";

            if (Session["E_Location"] != null)
            {
                model.Locationid = Session["E_Location"].ToString();
            }
            else
                model.Locationid = "";
            if (Session["E_Equiptype"] != null)
            {
                model.Equiptype = Session["E_Equiptype"].ToString();
            }
            else
                model.Equiptype = "";

            if (Session["E_ModelID"] != null)
            {
                model.ModelID = Session["E_ModelID"].ToString();
            }
            else
                model.ModelID = "-1";

            if (Session["E_SerialNo"] != null)
            {
                model.SerachMDS = Session["E_SerialNo"].ToString();
            }
            else
                model.SerachMDS = "";

            if (Session["E_DisplayEquip"] != null)
            {
                model.DisplayEquip = Convert.ToBoolean(Session["E_DisplayEquip"]);
            }
            else
                model.DisplayEquip = true;

            if (Session["E_ServiceMDS"] != null)
            {
                model.ServiceMDS = Convert.ToBoolean(Session["E_ServiceMDS"]);
            }
            else
                model.ServiceMDS = true;

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            /* var s = db.SearchEquipment(model.Customerid, model.Department, model.Locationid, model.Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToBoolean(model.DisplayEquip), model.SerachMDS, model.ServiceMDS,false,Convert.ToInt32( model.ModelID),model.SelCnt).Select(i => new EquipmentSearchList
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
             model.EquipmentSearchList = s;*/
            return View("GetEquipmentSearch", model);
        }
/// ///////////////////////


        [Authorize]
        public ActionResult SearchEquipmentMain(string CustCode)
        {
            EquipmentSearchNew es = new EquipmentSearchNew();
            ViewBag.CustomerX = Utility.GetCustomerListByBranchIdSelect();
            ViewBag.EquipTypeX = Utility.GetEquipTypeList();
            es.ServiceMDS = true;
            es.DisplayEquip = true;
            return View(es); 
        }
        [HttpPost]

        [Authorize]
          public PartialViewResult SearchEquipmentLoad(string CustCode,bool? InService,bool? ServiceMDS,string Search,string EquipType)
        {
            //var model = new EquipmentSearch();
            //model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            //    model.CustomerSiteList = Utility.GetCustomerSitesList();
            //model.EquipTypeList = Utility.GetEquipTypeList();
            //model.BranchList = Utility.GetBranchList();
            //model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            //model.ServiceMDS = true;
            //model.DisplayEquip = true;
            //model.Cnt = new List<Int32>();
            //model.Cnt.Add(100);
            //model.Cnt.Add(200);
            //model.Cnt.Add(400);
            //model.Cnt.Add(500);
            //model.Cnt.Add(800);
            //model.Cnt.Add(1000);
            //model.Cnt.Add(2000);
            //model.SelCnt = 100;

            //string Cust;
            //if (CustCode != null)
            //    Cust = CustCode;
            //else
            //    Cust = LoginController.AdminTechCustomer(User.Identity.Name);

            //if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            //{
            //    model.Customerid = Cust;
            //}
            //else if (Session["E_Customer_ID"] != null)
            //{
            //    model.Customerid = Session["E_Customer_ID"].ToString();
            //}
            //else
            //    model.Customerid = "";
            //if (Session["E_Department"] != null)
            //{
            //    model.Department = Session["E_Department"].ToString();
            //}
            //else
            //    model.Department = "";

            //if (Session["E_Location"] != null)
            //{
            //    model.Locationid = Session["E_Location"].ToString();
            //}
            //else
            //    model.Locationid = "";
            //if (Session["E_Equiptype"] != null)
            //{
            //    model.Equiptype = Session["E_Equiptype"].ToString();
            //}
            //else
            //    model.Equiptype = "";

            //if (Session["E_ModelID"] != null)
            //{
            //    model.ModelID = Session["E_ModelID"].ToString();
            //}
            //else
            //    model.ModelID = "-1";

            //if (Session["E_SerialNo"] != null)
            //{
            //    model.SerachMDS = Session["E_SerialNo"].ToString();
            //}
            //else
            //    model.SerachMDS = "";
            if (Search == null)
                Search = "";
            //if (Session["E_DisplayEquip"] != null)
            //{
            //    model.DisplayEquip = Convert.ToBoolean(Session["E_DisplayEquip"]);
            //}
            //else
            //    model.DisplayEquip = true;

            //if (Session["E_ServiceMDS"] != null)
            //{
            //    model.ServiceMDS = Convert.ToBoolean(Session["E_ServiceMDS"]);
            //}
            //else
            //    model.ServiceMDS = true;
            if (CustCode == "-1")
                CustCode = "";
            if (InService == null)
                InService = true;
            if (ServiceMDS == null)
                ServiceMDS = true;
            if (EquipType == null)
                EquipType = "";
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 200;
            List<EquipmentSearchList> results = db.SearchEquipment(CustCode, "", "", EquipType, LoginController.BranchID(HttpContext.User.Identity.Name), InService, Search, ServiceMDS, false, -1, 100).Select(i => new EquipmentSearchList
           // var s = db.SearchEquipment(Customerid,    Department,     Locationid,         Equiptype,          LoginController.BranchID(HttpContext.User.Identity.Name), DisplayEquip,                     SerachMDS,              ServiceMDS, false, Convert.ToInt32(ModelID), Cnt).Select(i => new EquipmentSearchList
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

            return PartialView( results);
        //    return View("GetEquipmentSearch",model);
        }

          //public PartialViewResult SearchEquipmentLoad(EquipmentSearch model)
          //{
             
          //    model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
          //    model.EquipTypeList = Utility.GetEquipTypeList();
          //    model.LocationList = Utility.GetLocationListName();
          //    model.BranchList = Utility.GetBranchList();
          //    model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
          //    string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
          //    model.Cnt = new List<Int32>();
          //    model.Cnt.Add(100);
          //    model.Cnt.Add(200);
          //    model.Cnt.Add(400);
          //    model.Cnt.Add(500);
          //    model.Cnt.Add(800);
          //    model.Cnt.Add(1000);
          //    model.Cnt.Add(2000);
              
          //    if (model.SelCnt == 0)
          //        model.SelCnt = 100;

          //    if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
          //    {
          //        // Session["E_Customer_ID"] = model.Customerid;
          //        model.Customerid = Cust;
          //    }
          //    else if (model.Customerid == null)
          //    {
          //        model.Customerid = "";
          //    }
          //    else if (model.Customerid == "-1")
          //  {
          //      model.Customerid = "";
          //  }
          //  Session["E_Customer_ID"] = model.Customerid;

          //    if (model.Department == null)
          //    {
          //        model.Department = "";
          //    }
          //    Session["E_Department"] = model.Department;

          //    if (model.Equiptype == null)
          //    {
          //        model.Equiptype = "";
          //    }
          //    Session["E_Equiptype"] = model.Equiptype;
          //    if (model.ModelID == null)
          //    {
          //        model.ModelID = "-1";
          //    }
          //    Session["E_ModelID"] = model.ModelID;

          //    if (model.DisplayEquip == null)
          //        model.DisplayEquip = true;
          //    Session["E_DisplayEquip"] = model.DisplayEquip;

          //    if (model.ServiceMDS == null)
          //        model.ServiceMDS = true;
          //    Session["E_ServiceMDS"] = model.ServiceMDS;

          //    if (model.Locationid == null)
          //    {
          //        model.Locationid = "";
          //    }
          //    Session["E_Location"] = model.Locationid;

          //    if (model.SerachMDS == null)
          //    {
          //        model.SerachMDS = "";
          //    }

          //    Session["E_SerialNo"] = model.SerachMDS;

          //  TrackerDataContext db = new TrackerDataContext();
          //  /*    db.CommandTimeout = 90;
          //    var s = db.SearchEquipment(model.Customerid, model.Department, model.Locationid, model.Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToBoolean(model.DisplayEquip), model.SerachMDS, model.ServiceMDS, false, Convert.ToInt32(model.ModelID), model.SelCnt).Select(i => new EquipmentSearchList

          //    {
          //        Customer = i.Customer,
          //        MDSItemNo = i.MDSItemNo,
          //        SerialNumber = i.SerialNumber,
          //        CustomerSite = i.CustomerSite,
          //        Location = i.Location,
          //        EquipDesc = i.EquipDesc,
          //        EquipementType = i.EquipementType,
          //        Manufacturer = i.Manufacturer,
          //        Model = i.Model,
          //        VendorName = i.VendorName,
          //        CurrentlyServicedByMDS = i.CurrentlyServicedByMDS ? "Yes" : "No",
          //        InService = i.InService ? "Yes" : "No",
          //        EquipID = i.EquipUID.ToString(),
          //        WarrantyExpirationDate = i.WarrantyExpirationDate,
          //        TotalCostOfRepairs = i.TotalCostOfRepairs,
          //        TotalRepairHours = i.TotalRepairHours,
          //        Cost = i.Cost



          //    }).ToList();
          //    model.EquipmentSearchList = s;
          //    */
          //  model.EquipmentSearchList = db.SearchEquipment(model.Customerid, model.Department, model.Locationid, model.Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToBoolean(model.DisplayEquip), model.SerachMDS, model.ServiceMDS, false, Convert.ToInt32(model.ModelID), model.SelCnt).Select(i => new EquipmentSearchList
          //  // var s = db.SearchEquipment(Customerid,    Department,     Locationid,         Equiptype,          LoginController.BranchID(HttpContext.User.Identity.Name), DisplayEquip,                     SerachMDS,              ServiceMDS, false, Convert.ToInt32(ModelID), Cnt).Select(i => new EquipmentSearchList
          //  {
          //      Customer = i.Customer,
          //      MDSItemNo = i.MDSItemNo,
          //      SerialNumber = i.SerialNumber,
          //      CustomerSite = i.CustomerSite,
          //      Location = i.Location,
          //      EquipDesc = i.EquipDesc,
          //      EquipementType = i.EquipementType,
          //      Manufacturer = i.Manufacturer,
          //      Model = i.Model,
          //      VendorName = i.VendorName,
          //      CurrentlyServicedByMDS = i.CurrentlyServicedByMDS ? "Yes" : "No",
          //      InService = i.InService ? "Yes" : "No",
          //      EquipID = i.EquipUID.ToString(),
          //      WarrantyExpirationDate = i.WarrantyExpirationDate,
          //      TotalCostOfRepairs = i.TotalCostOfRepairs,
          //      TotalRepairHours = i.TotalRepairHours,
          //      Cost = i.Cost

          //  }).ToList();


          //  return PartialView( model);
          //}


          [Authorize]
        public JsonResult GetEquipmentDetails(int EquipmentID)
        {

            var model = Utility.ServiceJobDetailbyId(EquipmentID);
            return Json(new { ServiceInfo1 = model[0].JobCode.ToString() + "," + model[0].Customer + "," + model[0].CustomerSite }, JsonRequestBehavior.AllowGet);

        }
          [Authorize]
        public JsonResult CreateJob(int ID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Repair - Create Job", ID, Request);
            TrackerDataContext db = new TrackerDataContext();

            db.CommandTimeout = 90;
            var r = db.Repairs.Where(i => i.RepairUID == ID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            Repair obj = new Repair();
            var JobNo = db.Repairs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode) + 1;
            var RepairID = db.Repairs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.RepairUID) + 1;
            obj.JobCode = JobNo;
            obj.RepairUID = RepairID;
            obj.BNQLocationCode = Utility.GetBranchCode().Substring(0, 1);
            obj.EquipUID = r.EquipUID;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            //var g = Guid.NewGuid();
            //obj.s_GUID = g;
            db.Repairs.InsertOnSubmit(obj);
            db.SubmitChanges();
            db.Dispose();
            return Json(new { NewRepairID = obj.RepairUID }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(RepairSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search Repair Submit", 0, Request);
            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
        //    model.Cnt.Add(400);
            model.Cnt.Add(500);
          //  model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.Cnt.Add(5000);
            model.Cnt.Add(10000);

            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
                model.Customerid = Cust;
            }
            else  if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            else if (model.Customerid == "-1")
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
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }
            if (model.Department == null)
            {
                model.Department= "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }
            if (model.CustomerOrderNo == null)
            {
                model.CustomerOrderNo = "";
            }
            if (model.RepairJob == null)
            {
                model.RepairJob = "";
            }
           
            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }

            if (model.Resolved == null)
            {
                model.Resolved = "";
            }
            if (model.Complete == null)
            {
                model.Complete = "";
            }

            if (model.Customerid != null)
            {
                string Customer = model.Customerid;
                string Equip = model.Equiptype;
                var FromDate = model.FromDate;
                var ToDate = model.ToDate;
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
                Session["R_Customer"] = Customer;
                Session["R_CustSite"] = Customersite;
                Session["R_Equip"] = Equip;
                Session["R_Engineer"] = Engineer;
                Session["R_Location"] = Location;
                Session["R_WorkOrderResolved"]=model.Resolved;
                Session["R_WorkOrderComplete"] = model.Complete;
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
                    Engineer=i.EngineerName,
                    RepairCompleted=i.RepairCompleted,
                    TotalCharge = Convert.ToDecimal(i.RepairTravelExpenseCost)+i.PartsCost,
                    FaultDetails = i.FaultDetails,
                    WorkDone = i.WorkDone,
                    RepairTravelExpenseCost=i.RepairTravelExpenseCost,
                    PartsCost=i.PartsCost,
                    WarrantyExpirationDate = i.WarrantyExpirationDate

                }).ToList();
                model.Repairs = s;
            }
            model.FromDate = DateTime.Now.AddMonths(-1);
            model.ToDate = DateTime.Now;

            model.OutFromDate = DateTime.Now.AddMonths(-1);
            model.OutToDate = DateTime.Now;
            //model.OutToDate = DateTime.Now.AddMonths(-1);
            //model.OutToDate = DateTime.Now;

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public ActionResult RepairSearchList(RepairSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search Repair Submit", 0, Request);

            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

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
            if (model.SelCnt==0) model.SelCnt = 100;

            if (model.Customerid == null)
            {
                model.Customerid = "";
            }
            else if (model.Customerid == "-1")
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
            if (model.CustomerOrderNo == null)
            {
                model.CustomerOrderNo = "";
            }
            if (model.RepairJob == null)
            {
                model.RepairJob = "";
            }
          /*  if (model.Locationid == null)
            {
                model.Locationid = "";
            }*/
            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }

            if (model.Resolved == null)
            {
                model.Resolved = "";
            }
            if (model.Complete == null)
            {
                model.Complete = "";
            }

            if (model.Customerid != null)
            {
                var Customer = model.Customerid;
                var Equip = model.Equiptype;
                var FromDate = model.FromDate;
                var ToDate = model.ToDate;
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
                    RepairCompleted = i.RepairCompleted,
                    TotalCharge = Convert.ToDecimal(i.RepairTravelExpenseCost) + i.PartsCost,
                    FaultDetails = i.FaultDetails,
                    WorkDone = i.WorkDone,
                    RepairTravelExpenseCost = i.RepairTravelExpenseCost,
                    PartsCost = i.PartsCost,
                    WarrantyExpirationDate = i.WarrantyExpirationDate

                }).ToList();
                model.Repairs = s;
            }
            return View("~/Views/Service/GetRepairForService.cshtml", model);
        }
        [Authorize]
        void UpdateRepair(AddRepaire model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Update Repair Submit", 0, Request);

            setNewRepairIDFromSession(model);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var obj = db.Repairs.Where(i => i.RepairUID == model.RepairId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            obj.EquipUID = Convert.ToInt32(model.SelectedEquipment);
            if (model.DateInitalCall.HasValue)
            {
                obj.DateCallReceived = Convert.ToDateTime(model.DateInitalCall.Value);
            }
            else
                obj.DateCallReceived = null;
            if (model.TimeInitalCall.HasValue)
            {
                obj.TimeCallReceived = Convert.ToDateTime(model.TimeInitalCall.Value);
            }
            else
                obj.TimeCallReceived = null;
            if (model.DateEquipRepair.HasValue)
            {
                obj.DateIn = Convert.ToDateTime(model.DateEquipRepair.Value);
                if (model.TimeEquipRepair.HasValue)
                {
                    obj.DateIn = obj.DateIn.Value.Date.AddHours(Convert.ToDateTime(model.TimeEquipRepair).Hour).AddMinutes(Convert.ToDateTime(model.TimeEquipRepair).Minute);
                }
                obj.TimeIn = obj.DateIn;
                if (model.TimeOutEquipRepair.HasValue)
                {
                    obj.DateOut = obj.DateIn.Value.Date.AddHours(Convert.ToDateTime(model.TimeOutEquipRepair).Hour).AddMinutes(Convert.ToDateTime(model.TimeOutEquipRepair).Minute);
                }
                obj.TimeOut = obj.DateOut;
            }
            else
            {
                obj.TimeIn = null;
                obj.DateIn = null;
                obj.TimeOut = null;
                obj.DateOut=null;

            }
            //if (model.ScheduledStartDate.HasValue)
            //{
            //    DateTime ScheduledStart = Convert.ToDateTime(model.ScheduledStartDate.Value).Date;
            //    if (model.ScheduledStartTime.HasValue)
            //        obj.ScheduledStart = ScheduledStart.AddHours(model.ScheduledStartTime.Value.Hour).AddMinutes(model.ScheduledStartTime.Value.Minute);
            //    if (model.ScheduledEnd.HasValue)
            //        obj.ScheduledEnd = ScheduledStart.AddHours(model.ScheduledEnd.Value.Hour).AddMinutes(model.ScheduledEnd.Value.Minute);



            //}
            //else
            //{
            //    obj.ScheduledStart = null;
            //    obj.ScheduledEnd = null;
            //}

            //if (model.TimeEquipRepair.HasValue)
            //    obj.TimeIn = Convert.ToDateTime(model.TimeEquipRepair.Value);
            //else
            //    obj.TimeIn = null;
            
            //if (model.TimeOutEquipRepair.HasValue)
            //{
            //    obj.TimeOut = Convert.ToDateTime(model.TimeOutEquipRepair.Value);
            //}
            //else
            //    obj.TimeOut = null;

            obj.ApprovalRequired = model.Approve;
            obj.ApprovalContact = model.ApprovalContact;
            obj.OrderNumberRequired = model.orderNumber.Value;
            obj.Accessories = model.Accessories;
            obj.VerbalApprovalObtained = model.VerbalApproval;
            obj.ApprovedBy = model.ApprovalReceived;
            obj.OrderNumber = model.OrderNo;
            obj.FaultDetails = model.FaultDetail;
            obj.WorkDone = model.WorkDone;
            obj.RepairHours = model.LabourHours;
            obj.SafetyTestDone = model.SafetyTestDone.Value;
            obj.Charge = model.Charge.Value;
            obj.ChargePartsOnly = model.ChargePartsOnly.Value;
            obj.NoCharge = model.NoCharge.Value;
            obj.Notes = model.Notes;
            obj.RepairCompleted = model.AllSpecified.Value;
            obj.ResultedInRetirement = model.ResultRepair.Value;
            obj.NotRepaired = model.ItemRepair.Value;
            obj.HasBeenInvoiced = model.HasJob;
            obj.InvoiceNumber = model.Invoice;
            if (model.InvoiceDate.HasValue)
            {
                obj.InvoiceDate = model.InvoiceDate;
            }
            obj.Amount = model.ChargesInvoice;
            obj.LeakageCurrent = model.LeakageCurrent;
            obj.EarthResistance = model.EarthResistant;
            obj.InsulationResistance = model.Insulation;
            obj.TravelHours = model.TravelHours;
            obj.ExpensesProportion = model.PromotionalExpenses;
            obj.ChargeRate = model.LabourRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.LabourRate.Replace("$", ""));
            obj.TravelChargeRate = model.TravelRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.TravelRate.Replace("$", ""));
            obj.ChargeTypecode = model.ChargeTypeCode;
            obj.TravelTypecode = model.TravelTypeCode;
            obj.RetirementReportPrinted = model.RetirementReportPrinted.Value;
            obj.InvoiceThruServiceJob = model.InvoiceThruServiceJob;
            if (model.RepairDate.HasValue)
            {
                obj.DateRepairFinished = model.RepairDate.Value;
            }
            if (model.JobCompletionDate.HasValue)
            {
                obj.DateOut = model.JobCompletionDate.Value;
            }
            if (model.JobCompletionTime.HasValue && model.JobCompletionDate.HasValue)
            {
                obj.TimeOut = obj.DateOut;
                decimal hour = Math.Floor((decimal)model.JobCompletionTime.Value / (decimal)100);
                obj.TimeOut = Convert.ToDateTime(obj.TimeOut).AddHours(Convert.ToInt32(hour));

                decimal min = model.JobCompletionTime.Value - hour * 100;
                obj.TimeOut = Convert.ToDateTime(obj.TimeOut).AddMinutes(Convert.ToInt32(min));
//                obj.TimeOut = model.JobCompletionTime.Value;
            }
            obj.ReceiverName = model.PersonName;

            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            obj.HandoverCompleted = model.HasItem;
            if (model.ReceiptDate.HasValue)
            {
                obj.ReceiptDate = model.ReceiptDate.Value;
            }
            if (model.EngineerID != "")
            {
                obj.EngineerID = Convert.ToInt32(model.EngineerID);
            }
            else
            {
                obj.EngineerID = null;
            }

            if (model.ConditionID != null)
            {
                obj.ConditionID = Convert.ToInt32(model.ConditionID);
            }
            else
            {
                obj.ConditionID = null;
            }
            obj.TechContact = model.TechnicalContact;
            db.SubmitChanges();

            //var parts = db.RepairParts.Where(i => i.RepairOrServiceUID == model.RepairId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            //db.RepairParts.DeleteAllOnSubmit(parts);
            //db.SubmitChanges();
            //if (Session["Partused"] != null)
            //{
            //    if (model.RepairId != 0)
            //    {
            //        List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["Partused"];
            //        foreach (var item in PartUsedRepair)
            //        {
            //            var part = new RepairPart();
            //            part.CostPerUnit = item.Price;
            //            part.RepairOrServiceUID = obj.RepairUID;
            //            part.NumberUsed = Convert.ToInt16(item.NoOfParts);
            //            part.PartDesc = item.PartName;
            //            part.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            //            if (item.Stocked_Part)
            //            {
            //                part.PartNumber = item.PartNumber;
            //                part.PartID = item.PartId;
            //            }
            //            else
            //            {
            //                part.PartNumber = "Non Stock";
            //            }
            //            var g1 = Guid.NewGuid();
            //            //part.s_GUID = g1;
            //            db.RepairParts.InsertOnSubmit(part);
            //            db.SubmitChanges();
            //        }
            //    }
            //}
            db.Dispose();
        }

          [Authorize]
        public int AddRepair(AddRepaire model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Repair Submit", 0, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            Repair obj = new Repair();
            int? JobNo;
            var jn = db.Repairs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.JobCode);
            if (jn == null)
                JobNo = 1;
            else
                JobNo = Convert.ToInt32(jn+1);
            int RepairID;
            try
            {
                RepairID = db.Repairs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.RepairUID) + 1;
            }
            catch (Exception ex)
            {
                RepairID = 1;
            }
            obj.JobCode = JobNo;
            obj.RepairUID = RepairID;
            obj.BNQLocationCode = Utility.GetBranchCode().Substring(0, 1);
            obj.EquipUID = Convert.ToInt32(model.SelectedEquipment);
            if (model.DateInitalCall.HasValue)
            {
                obj.DateCallReceived = Convert.ToDateTime(model.DateInitalCall.Value);
            }
            if (model.TimeInitalCall.HasValue)
            {
                obj.TimeCallReceived = Convert.ToDateTime(model.TimeInitalCall.Value);
            }
            if (model.DateEquipRepair.HasValue)
            {
                obj.DateIn = Convert.ToDateTime(model.DateEquipRepair.Value);
                if (model.TimeEquipRepair.HasValue)
                {
                    obj.DateIn= obj.DateIn.Value.Date.AddHours(Convert.ToDateTime(model.TimeEquipRepair).Hour).AddMinutes(Convert.ToDateTime(model.TimeEquipRepair).Minute);
                }
                obj.TimeIn = obj.DateIn;
                if (model.TimeOutEquipRepair.HasValue)
                {
                    obj.DateOut = obj.DateIn.Value.Date.AddHours(Convert.ToDateTime(model.TimeOutEquipRepair).Hour).AddMinutes(Convert.ToDateTime(model.TimeOutEquipRepair).Minute);
                }
                obj.TimeOut = obj.DateOut;
            }

            else
            {
                obj.TimeIn = null;
                obj.DateIn = null;
                obj.TimeOut = null;
                obj.DateOut = null;

            }


            //if (model.TimeEquipRepair.HasValue)
            //{
            //    obj.TimeIn = Convert.ToDateTime(model.TimeEquipRepair.Value);
            //}

            //if (model.ScheduledStartDate.HasValue)
            //{
            //    DateTime ScheduledStart = Convert.ToDateTime(model.ScheduledStartDate.Value).Date;
            //    if (model.ScheduledStartTime.HasValue)
            //        obj.ScheduledStart = ScheduledStart.AddHours(model.ScheduledStartTime.Value.Hour).AddMinutes(model.ScheduledStartTime.Value.Minute);
            //    if (model.ScheduledEnd.HasValue)
            //        obj.ScheduledEnd = ScheduledStart.AddHours(model.ScheduledEnd.Value.Hour).AddMinutes(model.ScheduledEnd.Value.Minute);


            //}
            //else
            //{
            //    obj.ScheduledStart = null;
            //    obj.ScheduledEnd = null;
            //}
            //if (model.ScheduledStart.HasValue)
            //{
            //    obj.ScheduledStart = Convert.ToDateTime(model.ScheduledStart.Value);
            //}
            //if (model.ScheduledEnd.HasValue)
            //{
            //    obj.ScheduledEnd = Convert.ToDateTime(model.ScheduledEnd.Value);
            //}
            obj.ApprovalRequired = model.Approve;
            obj.ApprovalContact = model.ApprovalContact;
            obj.OrderNumberRequired = model.orderNumber.Value;
            obj.Accessories = model.Accessories;
            obj.VerbalApprovalObtained = model.VerbalApproval;
            obj.ApprovedBy = model.ApprovalReceived;
            obj.OrderNumber = model.OrderNo;
            obj.FaultDetails = model.FaultDetail;
            obj.WorkDone = model.WorkDone;
            obj.RepairHours = model.LabourHours;
            obj.SafetyTestDone = model.SafetyTestDone.Value;
            obj.Charge = model.Charge.Value;
            obj.ChargePartsOnly = model.ChargePartsOnly.Value;
            obj.NoCharge = model.NoCharge.Value;
            obj.Notes = model.Notes;

            obj.RepairCompleted = model.AllSpecified.Value;
            obj.ResultedInRetirement = model.ResultRepair.Value;
            obj.NotRepaired = model.ItemRepair.Value;
            obj.HasBeenInvoiced = model.HasJob;
            obj.InvoiceNumber = model.Invoice;
            obj.RetirementReportPrinted = model.RetirementReportPrinted.Value;
            obj.InvoiceThruServiceJob = model.InvoiceThruServiceJob;
            obj.HandoverCompleted = model.HasItem;
            if (model.InvoiceDate.HasValue)
            {
                obj.InvoiceDate = model.InvoiceDate;
            }
            obj.Amount = model.ChargesInvoice;
            obj.LeakageCurrent = model.LeakageCurrent;
            obj.EarthResistance = model.EarthResistant;
            obj.InsulationResistance = model.Insulation;
            obj.TravelHours = model.TravelHours;
            obj.ExpensesProportion = model.PromotionalExpenses;
            obj.ChargeRate = model.LabourRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.LabourRate.Replace("$", ""));
            obj.TravelChargeRate = model.TravelRate == "$0.00" ? 0.0M : Convert.ToDecimal(model.TravelRate.Replace("$", ""));
            obj.ChargeTypecode = model.ChargeTypeCode;
            obj.TravelTypecode = model.TravelTypeCode;
            if (model.RepairDate.HasValue)
            {
                obj.DateRepairFinished = model.RepairDate.Value;
            }
            if (model.JobCompletionDate.HasValue)
            {
                obj.DateOut = model.JobCompletionDate.Value;
            }
            if (model.JobCompletionTime.HasValue)
            {
                obj.TimeOut = obj.DateOut;
                decimal hour = Math.Floor((decimal)model.JobCompletionTime.Value / (decimal)100);
                obj.TimeOut = Convert.ToDateTime(obj.TimeOut).AddHours(Convert.ToInt32(hour));

                decimal min = model.JobCompletionTime.Value - hour * 100;
                obj.TimeOut = Convert.ToDateTime(obj.TimeOut).AddMinutes(Convert.ToInt32(min));
                //obj.TimeOut = model.JobCompletionTime.Value;
            }
            obj.ReceiverName = model.PersonName;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            if (model.ReceiptDate.HasValue)
            {
                obj.ReceiptDate = model.ReceiptDate.Value;
            }
            if (model.EngineerID != "")
            {
                obj.EngineerID = Convert.ToInt32(model.EngineerID);
            }
            else
            {
                obj.EngineerID = null;
            }
            if (model.ConditionID != null)
            {
                obj.ConditionID = Convert.ToInt32(model.ConditionID);
            }
            else
            {
                obj.ConditionID = null;
            }

            obj.TechContact = model.TechnicalContact;
            //var g = Guid.NewGuid();
            //obj.s_GUID = g;
            db.Repairs.InsertOnSubmit(obj);
            db.SubmitChanges();
            //if (Session["Partused"] != null)
            //{
            //    if (obj.RepairUID != 0)
            //    {
            //        List<PartsUsedList> PartUsedRepair = (List<PartsUsedList>)Session["Partused"];
            //        foreach (var item in PartUsedRepair)
            //        {
            //            var part = new RepairPart();
            //            part.CostPerUnit = item.Price;
            //            part.RepairOrServiceUID = obj.RepairUID;
            //            part.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            //            part.NumberUsed = Convert.ToInt16(item.NoOfParts);
            //            part.PartDesc = item.PartName;
            //            if (item.Stocked_Part)
            //            {
            //                part.PartNumber = item.PartNumber;
            //                part.PartID = item.PartId;
            //            }
            //            //var g1 = Guid.NewGuid();
            //            //part.s_GUID = g1;
            //            db.RepairParts.InsertOnSubmit(part);
            //            db.SubmitChanges();
            //        }
            //    }
            //}
            db.Dispose();
            return obj.RepairUID;
        }
          [Authorize]
        [HttpPost]
        public ActionResult Add(AddRepaire model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Repair Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            model.EngineerList = Utility.GetEngineerList();
            model.ConditionList= Utility.GetConditionList();
            model.TravelList = Utility.GetTravelList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ChargeType = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            setNewRepairIDFromSession(model);
            if (model.RepairId != 0)
            {
                UpdateRepair(model);
                this.SetNotification("Repair updated successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("EditPopUp", new { id = model.RepairId });
                }
                else
                {
                    return RedirectToAction("Index", "Repair");
                }
            }
            else
            {
                var id = AddRepair(model);
                model.RepairId = id;
                Session["NewRepairID"] = id;
                this.SetNotification("Repair created successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("EditPopUp", new { id = id });
                }
                else
                {
                    return RedirectToAction("Index", "Repair");
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult RetirementPost(Retirement model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Retire Repair Submit", model.EquipID, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90; 
              var eq = db.Equipments.Where(i => i.EquipUID == model.EquipID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            eq.BERDisposal = model.BERDisposal;
            eq.DateRetired = model.DateRetired;
            eq.NoLongerRequired = model.NoLongerRequired;
            eq.Obselete = model.Obselete;
            eq.PartsNotAvailable = model.PartsNotAvailable;
            eq.RetirementComment = model.RetirementComment;
            db.SubmitChanges();
            db.Dispose();
            return Json(new { success = true });
        }

      /*    [Authorize]
          [HttpPost]
          public JsonResult PrintListExcel(RepairSearch model)
          {
              return PrintRepairList(model, true);
          }

        [Authorize]
        [HttpPost]
        public JsonResult PrintList(RepairSearch model)
        {
            return PrintRepairList(model,false);
        }

        private JsonResult PrintRepairList(pRepairSearch model, bool Excel)
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
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }
            if (model.Department == null)
            {
                model.Department = "";
            }
          
            if (model.CustomerOrderNo == null)
            {
                model.CustomerOrderNo = "";
            }
            if (model.RepairJob == null)
            {
                model.RepairJob = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }
            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }
            if (model.Resolved == null)
            {
                model.Resolved = "";
            }
            if (model.Complete == null)
            {
                model.Complete = "";
            }


            var Customer = model.Customerid;
            var Equip = model.Equiptype;
            var FromDate = model.FromDate;
            var ToDate = model.ToDate;
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
            if (Excel)
                return Json(new { URL =  "Reports/RepairListRpt.aspx?dateInFrom=" + FromDate.ToString("yyyy-MM-dd") + "&dateInTo=" + ToDate.ToString("yyyy-MM-dd") + "&customerID=" + Customer + "&equipmentType=" + Equip + "&BranchID=" + Branch + "&resolved_NotResolved_Either=" + Repair + "&handoverCompleted_Incomplete_Either=" + Handover + "&serviceJobID=" + ServiceJobID + "&equipmentID=" + EquipID + "&department=" + Customersite + "&location=" + Location + "&engineerID=" + Engineer + "&custOrderNo=" + CustomerOrderNo + "&repairJobNo=" + RepairJob + "&dateOutFrom=" + OutFromDate.ToString("yyyy-MM-dd") + "&dateOutTo=" + OutToDate.ToString("yyyy-MM-dd") + "&DateInFilter=" + model.DateInFilter + "&DateOutFilter=" + model.DateOutFilter + "&Excel=true" });
            return Json(new { URL =  "Reports/RepairListRpt.aspx?dateInFrom=" + FromDate.ToString("yyyy-MM-dd") + "&dateInTo=" + ToDate.ToString("yyyy-MM-dd") + "&customerID=" + Customer + "&equipmentType=" + Equip + "&BranchID=" + Branch + "&resolved_NotResolved_Either=" + Repair + "&handoverCompleted_Incomplete_Either=" + Handover + "&serviceJobID=" + ServiceJobID + "&equipmentID=" + EquipID + "&department=" + Customersite + "&location=" + Location + "&engineerID=" + Engineer + "&custOrderNo=" + CustomerOrderNo + "&repairJobNo=" + RepairJob + "&dateOutFrom=" + OutFromDate.ToString("yyyy-MM-dd") + "&dateOutTo=" + OutToDate.ToString("yyyy-MM-dd") + "&DateInFilter=" + model.DateInFilter + "&DateOutFilter=" + model.DateOutFilter + "" });

        }*/
        [Authorize]
        public ActionResult RetirementItem(Int32 EquipId)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Retire Equipment Repair Submit", EquipId, Request);

            var model = new Retirement();

            TrackerDataContext db = new TrackerDataContext();
            var eq = db.Equipments.Where(i => i.EquipUID == EquipId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            model.EquipID = EquipId;
            model.BERDisposal = eq.BERDisposal;
            model.DateRetired = eq.DateRetired;
            model.NoLongerRequired = eq.NoLongerRequired;
            model.Obselete = eq.Obselete;
            model.RetirementComment = eq.RetirementComment;
            model.PartsNotAvailable = eq.PartsNotAvailable;
            return PartialView(model);
        }

        [Authorize]
        public JsonResult GetRetirementDetails(Int32 EquipId)
        {
            var model = new Retirement();

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var eq = db.Equipments.Where(i => i.EquipUID == EquipId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (eq.DateRetired == null)
            {
                eq.DateRetired = DateTime.Now;//added by tim hams
                db.SubmitChanges();
            }
            return Json(new { msg = "Retired on: " + eq.DateRetired.Value.ToString() + " Retirement comment: " + eq.RetirementComment });

        }

        [Authorize]
        public JsonResult StopSupply(Int32 EquipId)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (EquipId != -1)
            {
                var eq = db.Equipments.Where(i => i.EquipUID == EquipId && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (eq == null)
                    return Json(new { StopSupplymsg = "0" });
                var customer = db.Customers.Where(i => i.CustomerCode == eq.CustomerCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

                if (customer != null)
                {
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
                    return Json(new { StopSupplymsg = "0" });

            }
            else
            {
                return Json(new { StopSupplymsg = "0" });
            }

        }

        [Authorize]
        [HttpPost]
        public JsonResult PrintRepairWO(AddRepaire model)
        {
            setNewRepairIDFromSession(model);
            if (model.RepairId != 0)
            {
                UpdateRepair(model);
            }
            else
            {
                var id = AddRepair(model);
                model.RepairId = id;
                Session["NewRepairID"] = id;
            }
            if (Request.UrlReferrer == null)
                return Json(new { URL = "../../Reports/RepairWorkOrderRpt.aspx?repairID=" + model.RepairId + "&branchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            else
            {
                if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                    return Json(new { URL = "../Reports/RepairWorkOrderRpt.aspx?repairID=" + model.RepairId + "&branchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
                else
                    return Json(new { URL = "../../Reports/RepairWorkOrderRpt.aspx?repairID=" + model.RepairId + "&branchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            }
        }



        [Authorize]
        [HttpPost]
        public JsonResult PrintRepairDetail(AddRepaire model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Print Repair Report", model.RepairId, Request);

            setNewRepairIDFromSession(model);
            if (model.RepairId != 0)
            {
                UpdateRepair(model);

            }
            else
            {
                var id = AddRepair(model);
                model.RepairId = id;
                Session["NewRepairID"] = id;
            }
            if (Request.UrlReferrer == null)
                return Json(new { URL = "../../Reports/RepairDetailRpt.aspx?repairID=" + model.RepairId + "&branchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            else
            {
                if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                    return Json(new { URL = "../Reports/RepairDetailRpt.aspx?repairID=" + model.RepairId + "&branchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
                else
                    return Json(new { URL = "../../Reports/RepairDetailRpt.aspx?repairID=" + model.RepairId + "&branchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "" });
            }
        }

        private void setNewRepairIDFromSession(AddRepaire model)
        {
            if (model.RepairId == 0)
            {
                int NewRepairID = Convert.ToInt32(Session["NewRepairID"]);
                if (NewRepairID != 0)
                {
                    model.RepairId = NewRepairID;
                    
                }
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult PrintRetirementDetail(AddRepaire model)
        {
            setNewRepairIDFromSession(model);
            if (model.RepairId != 0)
            {
                UpdateRepair(model);
                
            }
            else
            {
                var id = AddRepair(model);
                model.RepairId = id;
                Session["NewRepairID"] = id;
            }
               if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                   return Json(new { URL = "../Reports/EquipmentRetirement.aspx?BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "&equipmentID=" + model.EquipId.ToString() + "" });
               else
                   return Json(new { URL = "../../Reports/EquipmentRetirement.aspx?BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name).ToString() + "&equipmentID=" + model.EquipId.ToString() + "" });


        }



    /*    [Authorize]
        [HttpPost]
        public ActionResult AddRepairStayOnForm(AddRepaire model)
        {

            var id = AddRepair(model);
            model.RepairId = id;
            Session["RepairId"] = id;
            return View(model);
        }
        */


        [Authorize]
        [HttpPost]
        public JsonResult SaveUpdateRepair(AddRepaire model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Save Update Report", model.RepairId, Request);

            setNewRepairIDFromSession(model);
            if (model.RepairId != 0)
            {
                UpdateRepair(model);
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = "Repair updated successfully." });
                }
                else
                {
                    return Json(new { msg = "1", message = "Repair updated successfully." });
                }
            }
            else
            {
                var id = AddRepair(model);
                model.RepairId = id;
                Session["NewRepairID"] = id;
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = "Repair created successfully.",id=id });
                }
                else
                {
                    return Json(new { msg = "1", message = "Repair created successfully.", id = id });
                }
            }

        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteRepair(int RepairID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Delete Report", RepairID, Request);

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var d = db.DeleteRepair(RepairID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
          //  return Json(new { msg = "TEST" });
            return Json(new { msg = d.msg });

        }
          [Authorize]
        public JsonResult GetEngineers()
        {
            return Json(Utility.GetEngineerList(), JsonRequestBehavior.AllowGet);
        }
    }
}
