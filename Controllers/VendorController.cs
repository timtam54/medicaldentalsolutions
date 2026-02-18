using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDS.DB;
using MDS.Models;
using MDS.Helper;
using System.Configuration;

namespace MDS.Controllers
{
    public class VendorController : BaseController
    {
        //
        // GET: /Vendor/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Vendor Request", 0, Request);

            var model = new VendorSearch();
            model.VendorName = "";
            TrackerDataContext db = new TrackerDataContext();
            var s = db.VendorSearch("").Select(i => new VendorSearchList
            {
                CompanyName = i.CompanyName,
                ContactName = i.ContactName,
                Phone = i.Phone,
                VendorUID = i.VendorUID.ToString()
            }).ToList();
            model.Vendors = s;
            return View(model);
        }

        [Authorize]
        public ActionResult IndexPopUp()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Vendor Request", 0, Request);

            var model = new VendorSearch();
            model.VendorName = "";
            TrackerDataContext db = new TrackerDataContext();
            var s = db.VendorSearch("").Select(i => new VendorSearchList
            {
                CompanyName = i.CompanyName,
                ContactName = i.ContactName,
                Phone = i.Phone,
                VendorUID = i.VendorUID.ToString()
            }).ToList();
            model.Vendors = s;
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Index", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(VendorSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search Vendor Submit", 0, Request);

            if (model.VendorName == null)
            {
                model.VendorName = "";
            }
            TrackerDataContext db = new TrackerDataContext();
            var s = db.VendorSearch(model.VendorName).Select(i => new VendorSearchList
            {
                CompanyName = i.CompanyName,
                ContactName = i.ContactName,
                Phone = i.Phone,
                VendorUID = i.VendorUID.ToString()
            }).ToList();
            model.Vendors = s;
            return View(model);
        }
          [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Vendor Request", 0, Request);

            var model = new VendorForm();
            return View(model);
        }
          [Authorize]
        public ActionResult AddPopup()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Vendor Request", 0, Request);

            var model = new VendorForm();
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUp(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Vendor Request", id, Request);

            TrackerDataContext db = new TrackerDataContext();
            var model = new VendorForm();
            model = Editdata(id);
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Add", model);
        }
          [Authorize]
        public VendorForm Editdata(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Vendor Request", id, Request);

            TrackerDataContext db = new TrackerDataContext();
            var model = new VendorForm();
            var VendorData = db.Vendors.Where(i => i.VendorUID == id ).FirstOrDefault();

            model.CompanyName = VendorData.CompanyName;
            model.ContactName = VendorData.ContactName;
            if (VendorData.ContractDate.HasValue)
            {
                model.ContractDate = VendorData.ContractDate.Value;
            }

            model.VendorCode = VendorData.VendorUID;
            model.Code = VendorData.VendorUID;
            model.Email = VendorData.Email;
            model.Fax = VendorData.Fax;
            model.Notes = VendorData.Notes;
            model.Phone1 = VendorData.Phone;
            model.Phone2 = VendorData.Phone2;
            model.PhysicalAddress = VendorData.PhysicalAddress;
            model.PostalAddress = VendorData.PostalAddress;
            model.ContractDetails = VendorData.ContractDetails;
            model.Url = VendorData.URL;
            return model;
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Vendor Request", id, Request);

            TrackerDataContext db = new TrackerDataContext();
            var model = new VendorForm();
            model = Editdata(id);
            return View("Add", model);
        }

          [Authorize]
        [HttpPost]
        public ActionResult Add(VendorForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Vendor Submit", 0, Request);

            TrackerDataContext db = new TrackerDataContext();

            if (model.VendorCode > 0)
            {
                var VendorData = db.Vendors.Where(i => i.VendorUID == model.VendorCode).FirstOrDefault();

                VendorData.CompanyName = model.CompanyName;
                VendorData.ContactName = model.ContactName;
                if (model.ContractDate.HasValue)
                {
                    VendorData.ContractDate = model.ContractDate;
                }


                VendorData.Email = model.Email;
                VendorData.Fax = model.Fax;
                VendorData.Notes = model.Notes;
                VendorData.Phone = model.Phone1;
                VendorData.Phone2 = model.Phone2;
                VendorData.PhysicalAddress = model.PhysicalAddress;
                VendorData.PostalAddress = model.PostalAddress;
                VendorData.URL = model.Url;
                VendorData.ContractDetails = model.ContractDetails;


                db.SubmitChanges();
                this.SetNotification("Vendor updated successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "Vendor");
                }
                else
                {
                    return RedirectToAction("Index", "Vendor");
                }
                //return RedirectToAction("Index");

            }
            else
            {

                var VendorData = new MDS.DB.Vendor();
                VendorData.CompanyName = model.CompanyName;
                VendorData.ContactName = model.ContactName;
                if (model.ContractDate.HasValue)
                {
                    VendorData.ContractDate = model.ContractDate;
                }
                var VendorID = db.Vendors.Max(i => i.VendorUID) + 1;
                VendorData.VendorUID = VendorID;
                VendorData.Email = model.Email;
                VendorData.Fax = model.Fax;
                VendorData.Notes = model.Notes;
                VendorData.Phone = model.Phone1;
                VendorData.Phone2 = model.Phone2;
                VendorData.PhysicalAddress = model.PhysicalAddress;
                VendorData.PostalAddress = model.PostalAddress;
                VendorData.URL = model.Url;
                VendorData.ContractDetails = model.ContractDetails;
                //var g = Guid.NewGuid();
                //VendorData.s_GUID = g;
                db.Vendors.InsertOnSubmit(VendorData);
                db.SubmitChanges();
                this.SetNotification("Vendor created successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "Vendor");
                }
                else
                {
                    return RedirectToAction("Index", "Vendor");
                }
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult PrintVendorList(VendorSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Print Vendor List", 0, Request);

            if (model.VendorName == null)
            {
                model.VendorName = "";
            }
            return Json(new { URL =/*ConfigurationManager.AppSettings["URL"].ToString() +*/ "Reports/VendorSearchList.aspx?vendorNameContains=" + model.VendorName + "" });
        }
    }
}
