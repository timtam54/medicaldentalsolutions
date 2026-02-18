using MDS.DB;
using MDS.Helper;
using MDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDS.Controllers
{
    public class EquipManufacturerController : BaseController
    {
        //
        // GET: /EquipManufacturer/
         [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Manufacturer Request", 0, Request);
            var model = new EquipManufacturerSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EquipManufacturerSearch("").Select(i => new EquipManufacturerList
            {
               
                EquipManufacturerName = i.Manufacturer,
            }).ToList();
            model.Manufacturer = s;
            return View(model);
            
        }

         [Authorize]
         public ActionResult IndexPopUp()
         {
            Utility.Audit(HttpContext.User.Identity.Name, "All Manufacturer Request", 0, Request);
            var model = new EquipManufacturerSearch();
             TrackerDataContext db = new TrackerDataContext();
             var s = db.EquipManufacturerSearch("").Select(i => new EquipManufacturerList
             {

                 EquipManufacturerName = i.Manufacturer,
             }).ToList();
             model.Manufacturer = s;
             ViewBag.PopUp = 1;
             model.PopUp = true;
             return View("Index", model);
         }

         [Authorize]
         [HttpPost]
         public ActionResult Index(EquipManufacturerSearch model)
         {
            Utility.Audit(HttpContext.User.Identity.Name, "All Manufacturer Search", 0, Request);
            if (model.EquipManufacturerName == null)
             {
                 model.EquipManufacturerName = "";
             }
             TrackerDataContext db = new TrackerDataContext();
             var s = db.EquipManufacturerSearch(model.EquipManufacturerName).Select(i => new EquipManufacturerList
             {
               EquipManufacturerName=i.Manufacturer,
             }).ToList();
             model.Manufacturer = s;
             return View(model);
         }
          [Authorize]
         public ActionResult Add()
         {
            Utility.Audit(HttpContext.User.Identity.Name, "Add New Manufacturer Request", 0, Request);
            var model = new EMForm();
             return View(model);
         }
          [Authorize]
         public ActionResult AddPopup()
         {
            Utility.Audit(HttpContext.User.Identity.Name, "Add New Manufacturer Request", 0, Request);
            var model = new EMForm();
             ViewBag.PopUp = 1;
             model.PopUp = true;
             return View("Add", model);
         }
          [Authorize]
         public EMForm Editdata(string id)
         {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Manufacturer Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
             var model = new EMForm();
             var EMData = db.EquipManufacturers.Where(i => i.Manufacturer == id).FirstOrDefault();
             model.Manufacturer = EMData.Manufacturer;
             model.Code = EMData.Manufacturer;
             return model;
         }
         [Authorize]
         public ActionResult EditPopUp(string id)
         {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Manufacturer Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
             var model = new EMForm();
             model = Editdata(id);
             ViewBag.PopUp = 1;
             model.PopUp = true;
             return View("Add", model);
         }
         [Authorize]
         public ActionResult Edit(string id)
         {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Manufacturer Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
             var model = new EMForm();
             model = Editdata(id);
             return View("Add", model);
         }
          [Authorize]
         [HttpPost]
         public ActionResult Add(EMForm model)
         {
            Utility.Audit(HttpContext.User.Identity.Name, "Add/Edit Manufacturer Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
             if (model.Code !=null)
             {
                 var CheckDuplicateCode = db.EquipManufacturers.Where(i => i.Manufacturer.ToLower() == model.Manufacturer.ToLower() && i.Manufacturer.ToLower() != model.Code.ToLower()).FirstOrDefault();
                 if (CheckDuplicateCode != null)
                 {
                     ModelState.AddModelError("", "Manufacturer code already exists.");
                     return View(model);
                 }

                 var EMData = db.EquipManufacturers.Where(i => i.Manufacturer == model.Code).FirstOrDefault();
                 db.EquipManufacturers.DeleteOnSubmit(EMData);
                 db.SubmitChanges();
                 var EMData1 = new EquipManufacturer();
                 EMData1.Manufacturer = model.Manufacturer;
                 //var g = Guid.NewGuid();
                 //EMData1.s_GUID = g;
                 db.EquipManufacturers.InsertOnSubmit(EMData1);
                 db.SubmitChanges();
                 this.SetNotification("Manufacturer updated successfully.", NotificationEnumeration.Success);            
                 if (model.PopUp)
                 {
                     return RedirectToAction("IndexPopUp", "EquipManufacturer");
                 }
                 else
                 {
                     return RedirectToAction("Index", "EquipManufacturer");
                 }
             }
             else
             {
                 var CheckDuplicateCode = db.EquipManufacturers.Where(i => i.Manufacturer.ToLower() == model.Manufacturer.ToLower()).FirstOrDefault();
                 if (CheckDuplicateCode != null)
                 {
                     ModelState.AddModelError("", "Manufacturer code already exists.");
                     return View(model);
                 }
                 var EMData = new EquipManufacturer();
                 EMData.Manufacturer = model.Manufacturer;
                 //var g = Guid.NewGuid();
                 //EMData.s_GUID = g;
                 db.EquipManufacturers.InsertOnSubmit(EMData);
                 db.SubmitChanges();
                 this.SetNotification("Manufacturer updated successfully.", NotificationEnumeration.Success);
                 if (model.PopUp)
                 {
                     return RedirectToAction("IndexPopUp", "EquipManufacturer");
                 }
                 else
                 {
                     return RedirectToAction("Index", "EquipManufacturer");
                 }
             }
             //return RedirectToAction("Index");
         }

    }
}
