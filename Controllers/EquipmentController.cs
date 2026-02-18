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

namespace MDS.Controllers
{
    public class EquipmentController : BaseController
    {

        [Authorize]
        public JsonResult GetModelFromEquipType(String Equiptype, Int32 Branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var eqmodel = db.EquipModels.Where(i => i.EquipTypeCode == Equiptype && i.BranchID == Branchid).AsEnumerable().Select(i => new MDS.Models.EquipmentModelList
            {
                Model = i.Model + " " + i.Manufacturer,
                ModelUID = i.ModelUID.ToString()
            }).ToList();
            eqmodel.Insert(0, new MDS.Models.EquipmentModelList { ModelUID = "-1", Model = "--Select Model--" });
            return Json(eqmodel, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult OverdueService()
        {
            var model = new EquipmentSearch();

            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceMDS = true;
            model.DisplayEquip = true;

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
                model.ModelID = Session["E_ModelID"].ToString();
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

            return View(model);
        }

        //
        // GET: /Equipment/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Equipment Request", 0, Request);
            var model = new EquipmentSearch();
            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.ServiceMDS = true;
            model.DisplayEquip = true;
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
                model.ModelID = Session["E_ModelID"].ToString();
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

            return View(model);
        }

        [Authorize]
        public ActionResult GetOverdue([DataSourceRequest] DataSourceRequest request, string Customerid, string Equiptype, string Locationid, string Department, string SerachMDS, bool DisplayEquip, bool ServiceMDS, string ModelID)
        {
            Session["E_Customer_ID"] = Customerid;
            Session["E_Department"] = Department;
            Session["E_Equiptype"] = Equiptype;
            Session["E_SerialNo"] = SerachMDS;
            Session["E_DisplayEquip"] = DisplayEquip;
            Session["E_ServiceMDS"] = ServiceMDS;
            if (ModelID == "")
                ModelID = "-1";
            Session["E_ModelID"] = ModelID;

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var s = db.EquipmentOverdueService(/*, Department, Locationid,*/ LoginController.BranchID(HttpContext.User.Identity.Name), Customerid, Equiptype/*, DisplayEquip, SerachMDS, ServiceMDS, false, Convert.ToInt32(ModelID)*/).Select(i => new EquipmentSearchList
            {
                Customer = i.Cust,

                CustomerID = Convert.ToInt32(i.NextServiceMonth) * 10000 + Convert.ToInt32(i.NextServiceYear),
                //SerialNumber = i.SerialNumber,
                //CustomerSite = i.CustomerSite,
                //Location = i.Location,
                EquipDesc = i.EquipDesc,
                EquipementType = i.EquipementType,
                Manufacturer = i.Manufacturer,
                Model = i.Model,
                SerialNumber = i.NumServicesPerYear.ToString(),
                //CurrentlyServicedByMDS = i.CurrentlyServicedByMDS ? "Yes" : "No",
                //InService = i.InService ? "Yes" : "No",
                EquipID = i.EquipUID.ToString(),
                WarrantyExpirationDate = i.lastserviced//,
                                                       //TotalCostOfRepairs = i.TotalCostOfRepairs,
                                                       //TotalRepairHours = i.TotalRepairHours,
                                                       //Cost = i.Cost

            }).ToList();

            var jsonResult = Json(s.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;

            return jsonResult;
        }

        [Authorize]
        public ActionResult Get([DataSourceRequest] DataSourceRequest request, string Customerid, string Equiptype, string Locationid, string Department, string SerachMDS, bool DisplayEquip, bool ServiceMDS, string ModelID, int Cnt)
        {
            Session["E_Customer_ID"] = Customerid;
            Session["E_Department"] = Department;
            Session["E_Equiptype"] = Equiptype;
            Session["E_SerialNo"] = SerachMDS;
            Session["E_DisplayEquip"] = DisplayEquip;
            Session["E_ServiceMDS"] = ServiceMDS;
            if (ModelID == "")
                ModelID = "-1";
            Session["E_ModelID"] = ModelID;

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            if (Locationid == "--Location--")
                Locationid = "";
            if (Department == "--Select Customer Site--")
                Department = "";
            if (Customerid == "-1")
                Customerid = "";
            var s = db.SearchEquipment(Customerid, Department, Locationid, Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name), DisplayEquip, SerachMDS, ServiceMDS, false, Convert.ToInt32(ModelID), Cnt).Select(i => new EquipmentSearchList
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

            var jsonResult = Json(s.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;

            return jsonResult;
        }


        [Authorize]
        public JsonResult GetCustomerSite(String Customerid, Int32 Branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Customersite = db.CustomerDepartments.Where(i => i.CustomerCode == Customerid && i.BranchID == Branchid).AsEnumerable().Select(i => new CustomerSite
            {
                Department = i.Department,
                DeptID = i.DeptID.ToString()
            }).ToList();
            Customersite.Insert(0, new MDS.Models.CustomerSite { DeptID = "-1", Department = "--Select Customer Site--" });
            return Json(Customersite, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetCustomerLocation(String Customerid, Int32 Branchid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var CustomerLocation = db.CustomerLocations.Where(i => i.CustomerCode == Customerid && i.BranchID == Branchid).AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                LocationID = i.LocationID.ToString()
            }).ToList();
            CustomerLocation.Insert(0, new LocationList { LocationID = "-1", Location = "--Location--" });
            return Json(CustomerLocation, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult OverdueService(EquipmentSearch model)
        {
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.BranchList = Utility.GetBranchList();
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
                model.ModelID = "-1";
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
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(EquipmentSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Equipment Search", 0, Request);
            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            //    model.Cnt.Add(400);
            model.Cnt.Add(500);
            //            model.Cnt.Add(800);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.Cnt.Add(5000);
            model.Cnt.Add(10000);
            //      model.SelCnt = 100;
            string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            {
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
                model.ModelID = "-1";
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
            return View(model);
        }

        [Authorize]
        public JsonResult GetDueDate(Int32 EquipID, int ServicePerYear, int ServicePerMonth, int ServicePerMonth1, string DeliveryDate, int MinorService)
        {

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            if (DeliveryDate == "")
            {
                var DueDateData = db.CalcNextDueService(EquipID, ServicePerYear, ServicePerMonth, ServicePerMonth1, null, MinorService, LoginController.BranchID(HttpContext.User.Identity.Name)).AsEnumerable().Select(i => new AddEquipment
                {
                    Duemonth = i.DueMonth.Value,
                    DueYear = Convert.ToInt16(i.DueYear),
                    DueServiceType = i.DueServiceType,
                    OverDue = i.Overdue
                }).ToList();
                return Json((new { DueDateData1 = DueDateData[0].Duemonth + "," + DueDateData[0].DueYear + "( Type:" + DueDateData[0].DueServiceType + ")", Overdue = DueDateData[0].OverDue }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var DueDateData = db.CalcNextDueService(EquipID, ServicePerYear, ServicePerMonth, ServicePerMonth1, Convert.ToDateTime(DeliveryDate), MinorService, LoginController.BranchID(HttpContext.User.Identity.Name)).AsEnumerable().Select(i => new AddEquipment
                {
                    Duemonth = i.DueMonth.Value,
                    DueYear = Convert.ToInt16(i.DueYear),
                    DueServiceType = i.DueServiceType,
                    OverDue = i.Overdue
                }).ToList();
                return Json((new { DueDateData1 = DueDateData[0].Duemonth + "," + DueDateData[0].DueYear + "( Type:" + DueDateData[0].DueServiceType + ")", Overdue = DueDateData[0].OverDue }), JsonRequestBehavior.AllowGet);
            }

        }
        [Authorize]
        public JsonResult GetLocationList(String Customerid)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Locationlist = db.CustomerLocations.Where(i => i.CustomerCode == Customerid && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                LocationID = i.LocationID.ToString()
            }).ToList();
            Locationlist.Insert(0, new LocationList { LocationID = "-1", Location = "--Select Location--" });
            return Json(Locationlist, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult GetEquipmentModel()
        {
            var model = new EquipmentModel();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ManufactureList = Utility.GetManufaturerList();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EquipModelSearch("", "", "", LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EquipmentModelList
            {
                EquipType = i.EquipTypeCode,
                Manufacture = i.Manufacturer,
                Model = i.Model,
                Notes = i.ModelNotes,
                ModelUID = i.ModelUID.ToString()

            }).ToList();
            model.EquipmentModelList = s;
            return PartialView(model);
        }
        [Authorize]
        public JsonResult GetEquiptype(string ManufactureId)
        {
            TrackerDataContext db = new TrackerDataContext();
            var Equiptype = db.SP_GetEquipTypeByManufacture(ManufactureId, LoginController.BranchID(HttpContext.User.Identity.Name)).AsEnumerable().Select(i => new ManufacturerEquipType
            {
                EquipTypeCode = i.EquipTypeCode,
                Name = i.Name
            }).ToList();
            Equiptype.Insert(0, new ManufacturerEquipType { EquipTypeCode = "", Name = "--Select Equipment Type--" });
            return Json(Equiptype, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public ActionResult SearchEquipmentModel(EquipmentModel model)
        {

            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ManufactureList = Utility.GetManufaturerList();
            if (model.ManufactureId == null)
            {
                model.ManufactureId = " ";
            }
            if (model.Equiptype == null)
            {
                model.Equiptype = " ";
            }
            if (model.ModelContain == null)
            {
                model.ModelContain = " ";
            }
            var manufacture = model.ManufactureId;
            var Equiptype = model.Equiptype;
            var Search = model.ModelContain;
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EquipModelSearch(manufacture, Equiptype, Search, LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EquipmentModelList
            {
                EquipType = i.EquipTypeCode.ToString(),
                Manufacture = i.Manufacturer,
                Model = i.Model,
                Notes = i.ModelNotes,
                ModelUID = i.ModelUID.ToString()

            }).ToList();
            model.EquipmentModelList = s;
            return View("GetEquipmentModel", model);
        }

        [Authorize]
        public AddEquipment GetEditEquipment(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Equipment Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            var model = new AddEquipment();
            model.BranchList = Utility.GetBranchList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.VendorList = Utility.GetVendorList();
            model.ServiceAreaList = Utility.GetServiceAreaList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
            model.YearList = Utility.GetServiceYear();
            model.MonthList2 = Utility.GetMonths();
            model.MonthList1 = Utility.GetMonths();
            db.CommandTimeout = 90;
            var EquipmentData = db.Equipments.Where(i => i.EquipUID == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            model.EquipIDEdit = id;
            model.EquipID = id;
            Session["NewEquipID"] = id;

            if (EquipmentData.Count > 0)
            {
                model.Customerid = EquipmentData[0].CustomerCode;
                if (EquipmentData[0].DeliveryDate.HasValue)
                {
                    model.DeliveryDate = EquipmentData[0].DeliveryDate.Value;
                }
                if (EquipmentData[0].DateRetired.HasValue)
                {
                    model.DateRequired = EquipmentData[0].DateRetired.Value;
                }
                if (EquipmentData[0].WarrantyExpirationDate.HasValue)
                {
                    model.ExpirationDate = EquipmentData[0].WarrantyExpirationDate.Value;
                }
                var EquipmentModel = db.EquipModels.Where(i => i.ModelUID == Convert.ToInt32(EquipmentData[0].ModelUID.ToString()) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                if (EquipmentModel.Count() > 0)
                {
                    model.EquipModel = EquipmentModel[0].Model;
                
                var equiptype = db.EquipTypes.Where(i => i.EquipTypeCode == EquipmentModel[0].EquipTypeCode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
                
                model.Equiptype = equiptype[0].Name;
                
                model.Manufacturer = EquipmentModel[0].Manufacturer;
                }
                model.ModelUID = EquipmentData[0].ModelUID.ToString();
                model.CustomerCodeEuip = EquipmentData[0].CustomerEquipCode;
                model.AlternativeItem = EquipmentData[0].AlternativeEquipCode;
                model.Cost = EquipmentData[0].Cost;
                model.DeptID = EquipmentData[0].DeptID.ToString();
                model.GeneralNotes = EquipmentData[0].Notes;
                model.LocationID = EquipmentData[0].LocationID.ToString();
                model.ManualLibrary = EquipmentData[0].Manuals;
                model.Obselete = EquipmentData[0].Obselete;
                model.BERDisposal = EquipmentData[0].BERDisposal;
                model.CustomerRequired = EquipmentData[0].NoLongerRequired;
                model.LongerParts = EquipmentData[0].PartsNotAvailable;
                model.RetirementComment = EquipmentData[0].RetirementComment;
                model.InitialCondition = EquipmentData[0].InitialCondition;
                model.ServicePerYear = Convert.ToInt16(EquipmentData[0].NumServicesPerYear);
                model.ServicePerMonth = Convert.ToInt16(EquipmentData[0].ServiceMonth1);
                model.ServicePerMonth1 = Convert.ToInt16(EquipmentData[0].ServiceMonth2);
                model.MinorService = Convert.ToInt16(EquipmentData[0].MinorsBetweenMajors);
                model.ServiceAreaUID = EquipmentData[0].ServiceAreaUID;
                model.VendorUID = EquipmentData[0].VendorUID.ToString();
                model.SerialNo = EquipmentData[0].SerialNumber;
                model.MDSQNSell = EquipmentData[0].SoldByBNQ;
                model.MDSQNService = EquipmentData[0].CurrentlyServicedByBNQ;
                model.Branchid = EquipmentData[0].BranchID;
                if (EquipmentData[0].InService)
                {
                    model.TickItem = false;
                }
                else
                {
                    model.TickItem = true;
                }

                model.MDSQNItem = EquipmentData[0].BNQItemCode;
            }
            return model;
        }


        [Authorize]
        public ActionResult Edit(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Equipment Request", 0, Request);
            if (Request.UrlReferrer != null)
                Session["Referer"] = Request.UrlReferrer.LocalPath;
            var model = new AddEquipment();
            model = GetEditEquipment(id);
            Session["NewEquipID"] = id;
            model.PopUp = false;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditNew(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add New Equipment Request", 0, Request);
            var model = new AddEquipment();
            model = GetEditEquipment(id);
            Session["NewEquipID"] = id;
            ViewBag.AddNew = 1;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUp(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Equipment Request", 0, Request);
            var model = new AddEquipment();
            model = GetEditEquipment(id);
            Session["NewEquipID"] = id;
            model.PopUp = true;
            ViewBag.Popup = 1;
            ViewBag.Popup1 = 1;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUpNew(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Equipment Request", 0, Request);
            var model = new AddEquipment();
            model = GetEditEquipment(id);
            Session["NewEquipID"] = id;
            model.PopUp = true;
            ViewBag.Popup = 1;
            ViewBag.Popup1 = 1;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUpDialog(int id)
        {
            var model = new AddEquipment();
            model = GetEditEquipment(id);
            Session["NewEquipID"] = id;

            model.PopUp = true;
            ViewBag.AddPopup = 1;
            return View("Add", model);
        }
        [Authorize]
        public ActionResult EditPopUpPostback(int id)
        {
            var model = new AddEquipment();
            model = GetEditEquipment(id);
            Session["NewEquipID"] = id;

            model.PopUp = true;
            ViewBag.Popup = 1;
            return View("Add", model);
        }
        [Authorize]
        public ActionResult Add(string Customerid,string message)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Equipment Request", 0, Request);
            var model = new AddEquipment();
            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();
            model.message = message;
            //            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.VendorList = Utility.GetVendorList();
            model.ServiceAreaList = Utility.GetServiceAreaList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
            model.YearList = Utility.GetServiceYear();
            model.MonthList2 = Utility.GetMonths();
            model.MonthList1 = Utility.GetMonths();
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            int EquipmentID;
            if (db.Equipments.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Count()==0) 
                EquipmentID = 1;
            else
                EquipmentID = 1 + db.Equipments.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.EquipUID); ;
            model.EquipID = EquipmentID;
            Session["NewEquipID"] = 0;
            if (Customerid!=null)
                model.Customerid = Customerid;
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            return View(model);
        }
        [Authorize]
        public ActionResult AddPopUp()
        {
            var model = new AddEquipment();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.VendorList = Utility.GetVendorList();
            model.ServiceAreaList = Utility.GetServiceAreaList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();
            model.YearList = Utility.GetServiceYear();
            model.MonthList2 = Utility.GetMonths();
            model.MonthList1 = Utility.GetMonths();
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var EquipmentID = db.Equipments.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.EquipUID) + 1;
            model.EquipID = EquipmentID;
            Session["NewEquipID"] = 0;


            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            model.PopUp = true;
            ViewBag.AddPopup = 1;
            return View("add", model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Add(AddEquipment model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Equipment Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 120;
            model.CustomerList = Utility.GetCustomerListByBranchIdSelect();

            //model.CustomerList = Utility.GetCustomerList();
            model.VendorList = Utility.GetVendorList();
            model.ServiceAreaList = Utility.GetServiceAreaList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationListName();

            if (model.CustomerCodeEuip == "" && model.MDSQNItem == "")
            {
                ModelState.AddModelError("", "You must enter EITHER a customer code from this item of equipment, OR a an MDS Code.");
                return View(model);
            }

            if (model.MDSQNSell.Value)
            {
                if (model.VendorUID == "0" && model.VendorUID == null)
                {
                    ModelState.AddModelError("", "Because "+LoginController.CompanyName+" sold this item, you must specify the VENDOR from the list.");
                    return View(model);
                }
                if (model.ExpirationDate == null)
                {
                    ModelState.AddModelError("", "Because " + LoginController.CompanyName + " sold this item, you should specify the warranty expiration date.");
                    return View(model);
                }
            }

            if (model.MDSQNService.Value)
            {
                if (model.ServicePerMonth == 0)
                {
                    ModelState.AddModelError("", "Because " + LoginController.CompanyName + " services this item, you must specify the month in which the first service of a year will fall.");
                    return View(model);
                }

                if (model.ServicePerYear == 2)
                {
                    if (model.ServicePerMonth1 == 0)
                    {
                        ModelState.AddModelError("", "Because " + LoginController.CompanyName + " services this item, and there are TWO services per year, you must specify the month on which the second service will fall.");
                        return View(model);
                    }
                }

                if (!model.ServiceAreaUID.HasValue || model.ServiceAreaUID.Value == 0)
                {
                    ModelState.AddModelError("", "Because " + LoginController.CompanyName + " services this item, you should select the SERVICE AREA from the list.  (This information is required to generate a service schedule.)");
                    return View(model);
                }

            }

            if (!model.ServiceAreaUID.HasValue || model.ServiceAreaUID.Value == 0)
            {
                var customerdata = db.Customers.Where(i => i.CustomerCode == model.Customerid && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                model.ServiceAreaUID = customerdata.DefaultServiceAreaUID;
            }

            setNewEquipIDFromSession(model);
            if (model.EquipIDEdit != 0)
            {

                var obj = db.Equipments.Where(i => i.EquipUID == model.EquipIDEdit && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

                if (model.DeliveryDate.HasValue)
                {
                    obj.DeliveryDate = Convert.ToDateTime(model.DeliveryDate.Value);
                }
                if (model.DateRequired.HasValue)
                {
                    obj.DateRetired = Convert.ToDateTime(model.DateRequired.Value);
                }
                if (model.ExpirationDate.HasValue)
                {
                    obj.WarrantyExpirationDate = Convert.ToDateTime(model.ExpirationDate.Value);
                }
                obj.ModelUID = Convert.ToInt32(model.ModelUID);
                obj.BERDisposal = model.BERDisposal.Value;
                obj.AlternativeEquipCode = model.AlternativeItem;
                if (model.Cost.HasValue)
                {
                    obj.Cost = model.Cost.Value;
                }
                obj.CustomerCode = model.Customerid;
                obj.CustomerEquipCode = model.CustomerCodeEuip;
                obj.DeptID = Convert.ToInt32(model.DeptID);
                obj.InitialCondition = model.InitialCondition;
                if (Convert.ToInt32(model.LocationID)!=-1)//thams 13/10
                    obj.LocationID = Convert.ToInt32(model.LocationID);
                obj.Manuals = model.ManualLibrary;
                obj.MinorsBetweenMajors = Convert.ToInt16(model.MinorService);
                obj.NoLongerRequired = model.CustomerRequired.Value;
                obj.Notes = model.GeneralNotes;
                obj.NumServicesPerYear = Convert.ToInt16(model.ServicePerYear);
                obj.ServiceMonth1 = Convert.ToInt16(model.ServicePerMonth);
                obj.ServiceMonth2 = Convert.ToInt16(model.ServicePerMonth1);
                obj.SoldByBNQ = model.MDSQNSell.Value;
                obj.VendorUID = Convert.ToInt32(model.VendorUID);
                obj.CurrentlyServicedByBNQ = model.MDSQNService.Value;
                obj.BNQItemCode = model.MDSQNItem;
                obj.SerialNumber = model.SerialNo;
                obj.BranchID = model.Branchid;
                if (model.ServiceAreaUID.HasValue)
                {
                    obj.ServiceAreaUID = model.ServiceAreaUID;
                }
                if (model.TickItem.Value)
                {
                    obj.InService = false;
                }
                else
                {
                    obj.InService = true;
                }

                obj.Obselete = model.Obselete.Value;
                obj.NoLongerRequired = model.CustomerRequired.Value;
                obj.BERDisposal = model.BERDisposal.Value;
                obj.PartsNotAvailable = model.LongerParts.Value;
                obj.RetirementComment = model.RetirementComment;

                db.SubmitChanges();
                db.RefreshNextDueService(model.EquipIDEdit, LoginController.BranchID(HttpContext.User.Identity.Name));
                db.Dispose();
                this.SetNotification("Equipment updated successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("EditPopUpPostback", new { id = model.EquipIDEdit });
                }
                else
                {
                    if (Session["Referer"] != null)
                    {
                        if (Session["Referer"].ToString().Contains("OverdueSerice"))
                            return RedirectToAction("OverdueService", "Equipment");
                    }
                    return RedirectToAction("Index", "Equipment");
                }
                //return RedirectToAction("Edit", new { id = model.EquipIDEdit });
            }
            else
            {

                Equipment obj = new Equipment();
                var EquipUID = db.Equipments.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.EquipUID) + 1;
                obj.EquipUID = EquipUID;
                model.EquipIDEdit = EquipUID;
                Session["NewEquipID"] = EquipUID;
                if (model.DeliveryDate.HasValue)
                {
                    obj.DeliveryDate = Convert.ToDateTime(model.DeliveryDate.Value);
                }
                if (model.DateRequired.HasValue)
                {
                    obj.DateRetired = Convert.ToDateTime(model.DateRequired.Value);
                }
                if (model.ExpirationDate.HasValue)
                {
                    obj.WarrantyExpirationDate = Convert.ToDateTime(model.ExpirationDate.Value);
                }
                obj.ModelUID = Convert.ToInt32(model.ModelUID);
                obj.BERDisposal = model.BERDisposal.Value;
                obj.AlternativeEquipCode = model.AlternativeItem;
                if (model.Cost.HasValue)
                {
                    obj.Cost = model.Cost.Value;
                }
                obj.CustomerCode = model.Customerid;
                obj.CustomerEquipCode = model.CustomerCodeEuip;
                obj.DeptID = Convert.ToInt32(model.DeptID);
                obj.InitialCondition = model.InitialCondition;
//                if (Convert.ToInt32(model.LocationID) != -1)//thams 13/10
                    obj.LocationID = Convert.ToInt32(model.LocationID);
                obj.Manuals = model.ManualLibrary;
                obj.MinorsBetweenMajors = Convert.ToInt16(model.MinorService);
                obj.NoLongerRequired = model.CustomerRequired.Value;
                obj.Notes = model.GeneralNotes;
                obj.NumServicesPerYear = Convert.ToInt16(model.ServicePerYear);
                obj.ServiceMonth1 = Convert.ToInt16(model.ServicePerMonth);
                obj.VendorUID = Convert.ToInt32(model.VendorUID);
                obj.SoldByBNQ = model.MDSQNSell.Value;
                obj.CurrentlyServicedByBNQ = model.MDSQNService.Value;
                obj.BNQItemCode = model.MDSQNItem;
                obj.SerialNumber = model.SerialNo;
                obj.ServiceMonth2 = Convert.ToInt16(model.ServicePerMonth1);
                obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                if (model.ServiceAreaUID.HasValue)
                {
                    obj.ServiceAreaUID = model.ServiceAreaUID;
                }
                if (model.TickItem.Value)
                {
                    obj.InService = false;
                }
                else
                {
                    obj.InService = true;
                }
                obj.BranchID = model.Branchid;
                obj.Obselete = model.Obselete.Value;
                obj.NoLongerRequired = model.CustomerRequired.Value;
                obj.BERDisposal = model.BERDisposal.Value;
                obj.PartsNotAvailable = model.LongerParts.Value;
                obj.RetirementComment = model.RetirementComment;
                db.Equipments.InsertOnSubmit(obj);
                db.SubmitChanges();
                db.RefreshNextDueService(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name));
                db.Dispose();
                this.SetNotification("Equipment created successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("EditPopUpPostback", new { id = obj.EquipUID });
                }
                else
                {
                    if (Session["Referer"] != null)
                    {
                        if (Session["Referer"].ToString().Contains("OverdueSerice"))
                            return RedirectToAction("OverdueService", "Equipment");
                    }

                    return RedirectToAction("Index", "Equipment");
                }
                //  return RedirectToAction("Edit", new { id = obj.EquipUID });
            }
            //return RedirectToAction("Index");
        }

        [Authorize]
        void UpdateEquipment(AddEquipment model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Equipment Submit", model.EquipID, Request);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var obj = db.Equipments.Where(i => i.EquipUID == model.EquipIDEdit && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

            if (model.DeliveryDate.HasValue)
            {
                obj.DeliveryDate = Convert.ToDateTime(model.DeliveryDate.Value);
            }
            if (model.DateRequired.HasValue)
            {
                obj.DateRetired = Convert.ToDateTime(model.DateRequired.Value);
            }
            if (model.ExpirationDate.HasValue)
            {
                obj.WarrantyExpirationDate = Convert.ToDateTime(model.ExpirationDate.Value);
            }
            obj.ModelUID = Convert.ToInt32(model.ModelUID);
            obj.BERDisposal = model.BERDisposal.Value;
            obj.AlternativeEquipCode = model.AlternativeItem;
            if (model.Cost.HasValue)
            {
                obj.Cost = model.Cost.Value;
            }
            obj.CustomerCode = model.Customerid;
            obj.CustomerEquipCode = model.CustomerCodeEuip;
            obj.DeptID = Convert.ToInt32(model.DeptID);
            obj.InitialCondition = model.InitialCondition;
            if (Convert.ToInt32(model.LocationID) != -1)//thams 13/10
                obj.LocationID = Convert.ToInt32(model.LocationID);
            obj.Manuals = model.ManualLibrary;
            obj.MinorsBetweenMajors = Convert.ToInt16(model.MinorService);
            obj.NoLongerRequired = model.CustomerRequired.Value;
            obj.Notes = model.GeneralNotes;
            obj.NumServicesPerYear = Convert.ToInt16(model.ServicePerYear);
            obj.ServiceMonth1 = Convert.ToInt16(model.ServicePerMonth);
            obj.ServiceMonth2 = Convert.ToInt16(model.ServicePerMonth1);
            obj.SoldByBNQ = model.MDSQNSell.Value;
            obj.VendorUID = Convert.ToInt32(model.VendorUID);
            obj.CurrentlyServicedByBNQ = model.MDSQNService.Value;
            obj.BNQItemCode = model.MDSQNItem;
            obj.SerialNumber = model.SerialNo;
            obj.BranchID = model.Branchid;
            if (model.ServiceAreaUID.HasValue)
            {
                obj.ServiceAreaUID = model.ServiceAreaUID;
            }
            if (model.TickItem.Value)
            {
                obj.InService = false;
            }
            else
            {
                obj.InService = true;
            }

            obj.Obselete = model.Obselete.Value;
            obj.NoLongerRequired = model.CustomerRequired.Value;
            obj.BERDisposal = model.BERDisposal.Value;
            obj.PartsNotAvailable = model.LongerParts.Value;
            obj.RetirementComment = model.RetirementComment;
            db.SubmitChanges();
            db.RefreshNextDueService(model.EquipIDEdit, LoginController.BranchID(HttpContext.User.Identity.Name));
            db.Dispose();
        }

        private void setNewEquipIDFromSession(AddEquipment model)
        {
            if (model.EquipIDEdit == 0)
            {
                int NewID = Convert.ToInt32(Session["NewEquipID"]);
                if (NewID != 0)
                {
                    model.EquipIDEdit = NewID;

                }
            }
        }


        [Authorize]
        void SaveEquipment(AddEquipment model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Equipment Submit", model.EquipID, Request);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            Equipment obj = new Equipment();
            int EquipUID;
            if (db.Equipments.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Count() == 0)
                EquipUID = 1;
            else
                EquipUID = db.Equipments.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.EquipUID) + 1;
            obj.EquipUID = EquipUID;
            model.EquipIDEdit = EquipUID;
            Session["NewEquipID"] = EquipUID;
            if (model.DeliveryDate.HasValue)
            {
                obj.DeliveryDate = Convert.ToDateTime(model.DeliveryDate.Value);
            }
            if (model.DateRequired.HasValue)
            {
                obj.DateRetired = Convert.ToDateTime(model.DateRequired.Value);
            }
            if (model.ExpirationDate.HasValue)
            {
                obj.WarrantyExpirationDate = Convert.ToDateTime(model.ExpirationDate.Value);
            }
            obj.ModelUID = Convert.ToInt32(model.ModelUID);
            obj.BERDisposal = model.BERDisposal.Value;
            obj.AlternativeEquipCode = model.AlternativeItem;
            if (model.Cost.HasValue)
            {
                obj.Cost = model.Cost.Value;
            }
            obj.CustomerCode = model.Customerid;
            obj.CustomerEquipCode = model.CustomerCodeEuip;
            obj.DeptID = Convert.ToInt32(model.DeptID);
            obj.InitialCondition = model.InitialCondition;
//            if (Convert.ToInt32(model.LocationID) != -1)//thams 13/10
                obj.LocationID = Convert.ToInt32(model.LocationID);
            obj.Manuals = model.ManualLibrary;
            obj.MinorsBetweenMajors = Convert.ToInt16(model.MinorService);
            obj.NoLongerRequired = model.CustomerRequired.Value;
            obj.Notes = model.GeneralNotes;
            obj.NumServicesPerYear = Convert.ToInt16(model.ServicePerYear);
            obj.ServiceMonth1 = Convert.ToInt16(model.ServicePerMonth);
            obj.VendorUID = Convert.ToInt32(model.VendorUID);
            obj.SoldByBNQ = model.MDSQNSell.Value;
            obj.CurrentlyServicedByBNQ = model.MDSQNService.Value;
            obj.BNQItemCode = model.MDSQNItem;
            obj.SerialNumber = model.SerialNo;
            obj.ServiceMonth2 = Convert.ToInt16(model.ServicePerMonth1);
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            if (model.ServiceAreaUID.HasValue)
            {
                obj.ServiceAreaUID = model.ServiceAreaUID;
            }
            if (model.TickItem.Value)
            {
                obj.InService = false;
            }
            else
            {
                obj.InService = true;
            }
            obj.BranchID = model.Branchid;
            obj.Obselete = model.Obselete.Value;
            obj.NoLongerRequired = model.CustomerRequired.Value;
            obj.BERDisposal = model.BERDisposal.Value;
            obj.PartsNotAvailable = model.LongerParts.Value;
            obj.RetirementComment = model.RetirementComment;
            db.Equipments.InsertOnSubmit(obj);
            db.SubmitChanges();
            db.RefreshNextDueService(EquipUID, LoginController.BranchID(HttpContext.User.Identity.Name));
            db.Dispose();
        }

        /*       [Authorize]
               [HttpPost]
               public ActionResult PrintEquipmentsExcel(EquipmentSearch model)
               {
                   return PrintEquip(model,true);
               }
             [Authorize]
             [HttpPost]
             public ActionResult PrintEquipments(EquipmentSearch model)
             {
                 return PrintEquip(model,false);
             }


     /*        [Authorize]
             [HttpPost]
             public JsonResult EquipmentHistoryService(Int32 equipmentID)
             {
                 return Json(new { URL = ConfigurationManager.AppSettings["U_RL"].ToString() + "Reports/EquipmentHistoryService.aspx?BranchID=" + LoginController.BranchID( HttpContext.User.Identity.Name) + "&equipmentID=" + equipmentID + "" });
             }

             [Authorize]
             [HttpPost]
             public JsonResult EquipmentHistoryRepair(Int32 equipmentID)
             {
                 return Json(new { URL = ConfigurationManager.AppSettings["U_RL"].ToString() + "Reports/EquipmentHistoryRepair.aspx?BranchID=" + LoginController.BranchID(HttpContext.User.Identity.Name) + "&equipmentID=" + equipmentID + "" });
             }*/

        [Authorize]
        [HttpPost]
        public JsonResult AddNEWEquipmentExisting(Int32 equipmentID)
        {
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var data = db.CreateNewEquipUIDFromExisting(equipmentID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            Session["NewEquipID"] = 0;

            return Json(new { EquipmentID = data.EquipID });
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetTotalRepaiCost(Int32 equipmentID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var data = db.TotalRepairCostTime(equipmentID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            return Json(new { Cost = data.Cost, RHours = data.RHours });
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveUpdateEquipment(AddEquipment model)
        {
            setNewEquipIDFromSession(model);

            if (model.EquipIDEdit != 0)
            {
                UpdateEquipment(model);
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = "Equipment updated successfully." });
                }
                else
                {
                    return Json(new { msg = "1", message = "Equipment updated successfully." });
                }
            }
            else
            {
                SaveEquipment(model);
                if (model.PopUp)
                {
                    return Json(new { msg = "0", message = "Equipment created successfully." });
                }
                else
                {
                    return Json(new { msg = "1", message = "Equipment created successfully." });
                }
            }

        }

        [Authorize]
        [HttpPost]
        public JsonResult PrintRetirementDetail(AddEquipment model)
        {
            setNewEquipIDFromSession(model);
            if (model.EquipIDEdit != 0)
            {
                UpdateEquipment(model);
            }
            else
            {
                SaveEquipment(model);
            }
            if (Request.UrlReferrer.ToString().ToLower().Contains("add"))
                return Json(new { URL = "../Reports/EquipmentRetirement.aspx?BranchID=" + model.Branchid.ToString() + "&equipmentID=" + model.EquipIDEdit.ToString() + "" });
            else
                return Json(new { URL = "../../Reports/EquipmentRetirement.aspx?BranchID=" + model.Branchid.ToString() + "&equipmentID=" + model.EquipIDEdit.ToString() + "" });
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteEquipment(int EquipID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var d = db.DeleteEquipment(EquipID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            return Json(new { msg = d.msg });
        }
        public JsonResult GetVendors()
        {
            return Json(Utility.GetVendorList(), JsonRequestBehavior.AllowGet);
        }
    }
}
