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
    public class CustomerController : BaseController
    {

        private int CurrentBranchID()
        {
            Object oo = HttpContext.User.Identity.Name;
            //Object oo = System.Web.HttpContext.User.Identity.Name;
            return LoginController.BranchID(oo.ToString());
            //System.
        }
        //
        // GET: /Customer/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Customers Request", 0, Request);
            var model = new CustomerSearch();
            model.CompanyName = "";
            model.CompanyNameStart = "";
            model.ContractOverDue = false;
            model.CustomerCode = "";
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            TrackerDataContext db = new TrackerDataContext();
            var s = db.CustomerSearch("", "", "", false, model.Branchid).Select(i => new CustomerSearchList
            {
                Code = i.CustomerCode,
                CompanyName = i.CompanyName,
                ContactName = i.ContactName,
                ContractDate = i.ContractDate.HasValue ? i.ContractDate.Value.ToString("dd/MM/yyyy") : "",
                Phone = i.Phone,
                RenewalDate = i.ContractRenewalDate.HasValue ? i.ContractRenewalDate.Value.ToString("dd/MM/yyyy") : "",

            }).ToList();
            model.Customers = s;
            return View(model);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Index(CustomerSearch model)
        {
            //model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            Utility.Audit(HttpContext.User.Identity.Name, "All Customers Search", 0, Request);
            model.BranchList = Utility.GetBranchList();

            if (model.CompanyName == null)
            {
                model.CompanyName = "";
            }
            if (model.CompanyNameStart == null)
            {
                model.CompanyNameStart = "";
            }
            if (model.ContractOverDue == null)
            {
                model.ContractOverDue = false;
            }
            if (model.CustomerCode == null)
            {
                model.CustomerCode = "";
            }



            var CompanyNameContains = model.CompanyName;
            var CustomerCodeContains = model.CustomerCode;
            var CompanyNameStart = model.CompanyNameStart;
            var ContractOverDue = model.ContractOverDue;
            var Branch = model.Branchid;



            TrackerDataContext db = new TrackerDataContext();
            var s = db.CustomerSearch(CompanyNameContains, CustomerCodeContains, CompanyNameStart, ContractOverDue, Branch).Select(i => new CustomerSearchList
            {
                Code = i.CustomerCode,
                CompanyName = i.CompanyName,
                ContactName = i.ContactName,
                ContractDate = i.ContractDate.HasValue ? i.ContractDate.Value.ToString("dd/MM/yyyy") : "",
                Phone = i.Phone,
                RenewalDate = i.ContractRenewalDate.HasValue ? i.ContractRenewalDate.Value.ToString("dd/MM/yyyy") : "",

            }).ToList();
            model.Customers = s;

            return View(model);
        }
          [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add new Customers", 0, Request);
            var model = new CustomerForm();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(string id,int? BranchID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Customer Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            var model = new CustomerForm();
            if (BranchID == null)
                BranchID=LoginController.BranchID(HttpContext.User.Identity.Name);
            var CustomerData = db.Customers.Where(i => i.CustomerCode == id && i.BranchID == BranchID).FirstOrDefault();
            model.Branchid = CustomerData.BranchID;
            model.BranchList = Utility.GetBranchList();
            model.CompanyName = CustomerData.CompanyName;
            model.ContactName = CustomerData.ContactName;
            if (CustomerData.ContractDate.HasValue)
            {
                model.ContractDate = CustomerData.ContractDate.Value;
            }
            if (CustomerData.ContractRenewalDate.HasValue)
            {
                model.ContractRenewalDate = CustomerData.ContractRenewalDate.Value;
            }
            model.CustomerCode = CustomerData.CustomerCode;
            model.Code = CustomerData.CustomerCode;
            model.Email = CustomerData.Email;
            model.Fax = CustomerData.Fax;
            model.Notes = CustomerData.Notes;
            model.Phone1 = CustomerData.Phone;
            model.Phone2 = CustomerData.Phone2;
            model.PhysicalAddress = CustomerData.PhysicalAddress;
            model.PostalAddress = CustomerData.PostalAddress;
            if (CustomerData.StopDate.HasValue)
            {
                model.StopDate = CustomerData.StopDate.Value;
            }
            model.StoppedBy = CustomerData.StoppedBy;
            model.StopSupply = CustomerData.StopSupply;
            model.Url = CustomerData.URL;
            model.OnLinePassword = CustomerData.OnLinePassword;
            return View("Add", model);
        }
          [Authorize]
        [HttpPost]
        public ActionResult Add(CustomerForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add New Customers Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);

            if (model.Code != null)
            {
                var CheckDuplicateCode = db.Customers.Where(i => i.CustomerCode.ToLower() == model.CustomerCode.ToLower() && i.CustomerCode.ToLower() != model.Code.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateCode != null)
                {
                    ModelState.AddModelError("", "Customer code already exists.");
                    return View(model);
                }
                else
                {
                    var CustomerData = db.Customers.Where(i => i.CustomerCode.ToLower() == model.Code.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                    db.Customers.DeleteOnSubmit(CustomerData);
                    db.SubmitChanges();

                    var customer = new MDS.DB.Customer();
                    customer.BranchID = model.Branchid;
                    customer.CompanyName = model.CompanyName;
                    customer.ContactName = model.ContactName;
                    if (model.ContractDate.HasValue)
                    {
                        customer.ContractDate = model.ContractDate;
                    }
                    if (model.ContractRenewalDate.HasValue)
                    {
                        customer.ContractRenewalDate = model.ContractRenewalDate;
                    }
                    customer.CustomerCode = model.CustomerCode;
                    customer.Email = model.Email;
                    customer.Fax = model.Fax;
                    customer.Notes = model.Notes;
                    customer.Phone = model.Phone1;
                    customer.Phone2 = model.Phone2;
                    customer.PhysicalAddress = model.PhysicalAddress;
                    customer.PostalAddress = model.PostalAddress;
                    customer.StoppedBy = model.StoppedBy;
                    if (model.StopDate.HasValue)
                    {
                        customer.StopDate = model.StopDate;
                    }
                    customer.StopSupply = model.StopSupply;
                    customer.URL = model.Url;
                    customer.OnLinePassword = model.OnLinePassword;
                    var g = Guid.NewGuid();
                    //customer.s_GUID = g;

                    db.Customers.InsertOnSubmit(customer);
                    db.SubmitChanges();
                    this.SetNotification("Customer updated successfully.", NotificationEnumeration.Success);
                    return RedirectToAction("Index", "Customer");
                    //return RedirectToAction("Index");
                }
            }
            else
            {
                var CheckDuplicateCode = db.Customers.Where(i => i.CustomerCode.ToLower() == model.CustomerCode.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateCode != null)
                {
                    ModelState.AddModelError("", "Customer code already exists.");
                    return View(model);
                }
                else
                {
                    var customer = new MDS.DB.Customer();
                    customer.BranchID = model.Branchid;
                    customer.CompanyName = model.CompanyName;
                    customer.ContactName = model.ContactName;
                    if (model.ContractDate.HasValue)
                    {
                        customer.ContractDate = model.ContractDate;
                    }
                    if (model.ContractRenewalDate.HasValue)
                    {
                        customer.ContractRenewalDate = model.ContractRenewalDate;
                    }
                    customer.CustomerCode = model.CustomerCode;
                    customer.Email = model.Email;
                    customer.Fax = model.Fax;
                    customer.Notes = model.Notes;
                    customer.Phone = model.Phone1;
                    customer.Phone2 = model.Phone2;
                    customer.PhysicalAddress = model.PhysicalAddress;
                    customer.PostalAddress = model.PostalAddress;
                    customer.StoppedBy = model.StoppedBy;
                    if (model.StopDate.HasValue)
                    {
                        customer.StopDate = model.StopDate;
                    }
                    customer.StopSupply = model.StopSupply;
                    customer.URL = model.Url;
                    customer.OnLinePassword = model.OnLinePassword;
                    var g = Guid.NewGuid();
                    //customer.s_GUID = g;

                    db.Customers.InsertOnSubmit(customer);
                    db.SubmitChanges();
                    this.SetNotification("Customer created successfully.", NotificationEnumeration.Success);
                    return RedirectToAction("Index", "Customer");
                    // return RedirectToAction("Index");
                }
            }
        }

          [Authorize]
        public ActionResult GetCustomerLocations(string id)
        {
            Session["CId"] = id;
            var model = new CustomerLocationList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.CustomerCode = id;
            TrackerDataContext db = new TrackerDataContext();
            var LocationData = db.CustomerLocations.Where(i => i.CustomerCode == id && i.BranchID == CurrentBranchID()).Select(i => new CLocationList
            {
                CustomerCode = i.CustomerCode,
                Location = i.Location,
                LocationID =Convert.ToInt32( i.LocationID)

            }).ToList();
            model.LocationList = LocationData;
            return View("~/Views/Customer/CustomerLocations.cshtml", model);
        }
          [Authorize]
        [HttpPost]
        public ActionResult GetCustomerLocations(CustomerLocationList model)
        {
            Session["CId"] = model.CustomerCode; ;
            model.CustomerList = Utility.GetCustomerListByBranchId();
            TrackerDataContext db = new TrackerDataContext();
            var LocationData = db.CustomerLocations.Where(i => i.CustomerCode == model.CustomerCode && i.BranchID == CurrentBranchID()).Select(i => new CLocationList
            {
                CustomerCode = i.CustomerCode,
                LocationID = Convert.ToInt32(i.LocationID),
                Location = i.Location

            }).ToList();
            model.LocationList = LocationData;
            return View("~/Views/Customer/CustomerLocations.cshtml", model);
        }
          [Authorize]
        public ActionResult CreateLocation([DataSourceRequest]DataSourceRequest request, CLocationList s)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ModelState.IsValid)
            {
                var loc = new CustomerLocation();
                loc.Location = s.Location;
                loc.CustomerCode = Session["CId"].ToString();
                s.CustomerCode = Session["CId"].ToString();
                loc.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                db.CustomerLocations.InsertOnSubmit(loc);
                db.SubmitChanges();
               s.LocationID= loc.LocationID;
            }
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult UpdateLocation([DataSourceRequest]DataSourceRequest request, CLocationList s)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ModelState.IsValid)
            {
                s.CustomerCode = Session["CId"].ToString();
                var p = db.CustomerLocations.Where(i => i.LocationID==s.LocationID).FirstOrDefault();//.CustomerCode == s.CustomerCode && i.Location == s.LocationEdit && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            ///    db.CustomerLocations.DeleteOnSubmit(p);
                p.Location = s.Location;
                
                db.SubmitChanges();

            }
           
            // Return the updated product. Also return any validation errors.
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public JsonResult GetCustomerLocationsDDL(string id)
        {
            TrackerDataContext db = new TrackerDataContext();
            var CustomerLocation = db.CustomerLocations.Where(i => i.CustomerCode == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).AsEnumerable().Select(i => new LocationList
            {
                Location = i.Location,
                LocationID = i.LocationID.ToString()
            }).ToList();
            CustomerLocation.Insert(0, new LocationList { LocationID = "-1", Location = "--Location--" });
            return Json(CustomerLocation, JsonRequestBehavior.AllowGet);
        }
          [Authorize]
        public ActionResult GetCustomerSites1()
        {
            var id = "";
            //Session["CId"] = id;
            var model = new CustomerDepartmentList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.CustomerCode = id;
            TrackerDataContext db = new TrackerDataContext();
            var DepartmentData = db.CustomerDepartments.Where(i => i.CustomerCode == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new CCustomerSite
            {
                CustomerCode = i.CustomerCode,
                Department = i.Department   ,
                DeptID = i.DeptID

            }).ToList();
            model.DepartmentList = DepartmentData;
            return View("~/Views/Customer/CustomerSites1.cshtml", model);
        }
          [Authorize]
        [HttpPost]
        public ActionResult GetCustomerSites1(CCustomerSite model1)
        {
            //var id = "";
            Session["CId"] = model1.CustomerCode;
            var model = new CustomerDepartmentList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.CustomerCode = model1.CustomerCode;
            TrackerDataContext db = new TrackerDataContext();
            var DepartmentData = db.CustomerDepartments.Where(i => i.CustomerCode == model1.CustomerCode && i.BranchID == CurrentBranchID()).Select(i => new MDS.Models.CCustomerSite
            {
                CustomerCode = i.CustomerCode,
                Department = i.Department,
                DeptID = i.DeptID

            }).ToList();
            model.DepartmentList = DepartmentData;
            return View("~/Views/Customer/CustomerSites1.cshtml", model);
        }
          [Authorize]
        public ActionResult GetCustomerSites(string id)
        {
            Session["CId"] = id;
            var model = new CustomerDepartmentList();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.CustomerCode = id;
            TrackerDataContext db = new TrackerDataContext();
            var DepartmentData = db.CustomerDepartments.Where(i => i.CustomerCode == id && i.BranchID == CurrentBranchID()).Select(i => new MDS.Models.CCustomerSite
            {
                CustomerCode = i.CustomerCode,
                Department=i.Department,
                DeptID=i.DeptID

            }).ToList();
            model.DepartmentList = DepartmentData;
            return View("~/Views/Customer/CustomerSites.cshtml", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetCustomerSites(CustomerDepartmentList model)
        {
            Session["CId"] = model.CustomerCode; ;
            model.CustomerList = Utility.GetCustomerListByBranchId();
            TrackerDataContext db = new TrackerDataContext();
            //var DepartmentData = db.CustomerDepartments.Where(i => i.CustomerCode == model.CustomerCode && i.BranchID == CurrentBranchID()).Select(i => new CCustomerSite
            var DepartmentData = db.CustomerDepartments.Where(i => i.CustomerCode == model.CustomerCode && i.BranchID == CurrentBranchID()).Select(i => new CCustomerSite
            {
                CustomerCode = i.CustomerCode,
                Department = i.Department,
                DeptID = i.DeptID

            }).ToList();
            model.DepartmentList = DepartmentData;
            return View("~/Views/Customer/CustomerSites.cshtml", model);
        }

        [Authorize]
        public ActionResult CreateSite([DataSourceRequest]DataSourceRequest request, CCustomerSite s)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ModelState.IsValid)
            {
                var dep = new MDS.DB.CustomerDepartment();
                dep.Department = s.Department;
                dep.CustomerCode = Session["CId"].ToString();
                s.CustomerCode = Session["CId"].ToString();
               // s.DeptID = s.DeptID;
                dep.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                //var g = Guid.NewGuid();
                //dep.s_GUID = g;
                db.CustomerDepartments.InsertOnSubmit(dep);
                db.SubmitChanges();
                s.DeptID = dep.DeptID;

            }
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult UpdateSite([DataSourceRequest]DataSourceRequest request, CCustomerSite s)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ModelState.IsValid)
            {
                s.CustomerCode = Session["CId"].ToString();
                var p = db.CustomerDepartments.Where(i => i.DeptID == s.DeptID).FirstOrDefault();// .CustomerCode == s.CustomerCode && i.Department == s.DepartmentEdit && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                p.Department = s.Department;

                //                db.CustomerDepartments.DeleteOnSubmit(p);
                db.SubmitChanges();

/*                var dep = new MDS.DB.CustomerDepartment();
                dep.Department = s.Department;
                dep.CustomerCode = Session["CId"].ToString();
                s.CustomerCode = Session["CId"].ToString();
                dep.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                s.//DepartmentEdit = dep.Department;
                //var g = Guid.NewGuid();
                //dep.s_GUID = g;
                db.CustomerDepartments.InsertOnSubmit(dep);
                db.SubmitChanges();
                */
            }

            // Return the updated product. Also return any validation errors.
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }

        
    }
}
