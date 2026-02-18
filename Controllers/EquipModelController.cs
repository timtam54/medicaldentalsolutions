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
    public class EquipModelController : BaseController
    {
        //
        // GET: /EquipModel/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Model Request", 0, Request);
            var model = new EquipmentModel();        
            model.ManufactureList = Utility.GetManufaturerList();
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
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
        public JsonResult CheckRefAndDelete(int ModelID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var model = db.Equipments.Where(i=>i.ModelUID== ModelID && i.BranchID== LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            if (model.Count > 0)

            {
                var s = string.Join(",", model.Select(p => p.BNQItemCode));
                return Json(new { message="Cannot Delete the model as there are "+ model.Count.ToString() + " items of Equipment referencing this model - these are: "+s,Deleted=false }, JsonRequestBehavior.AllowGet);
                //return Json(new { CustomerInfo = model[0].CustomerInfo, CustomerCode = model[0].CustomerCode, EquipItem = model[0].EquipDesc, Location = model[0].Location, WarrantyExpirationDate = model[0].WarrantyExpirationDate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var em = db.EquipModels.Where(i => i.ModelUID == ModelID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                db.EquipModels.DeleteOnSubmit(em);
                db.SubmitChanges();
                return Json(new { message = "Model Deleted - No Equipment references this model", Deleted = true }, JsonRequestBehavior.AllowGet);
            }

        }
        [Authorize]
        [HttpPost]
        public ActionResult Index(EquipmentModel model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Model Search", 0, Request);
            model.ManufactureList = Utility.GetManufaturerList();
            TrackerDataContext db = new TrackerDataContext();
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
            db.CommandTimeout = 90;
            var s = db.EquipModelSearch(manufacture, Equiptype, Search, LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EquipmentModelList
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
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Model Request", 0, Request);
            var model = new EquipModelForm();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ManufactureList = Utility.GetManufaturerList();
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Model Request", id, Request);
            TrackerDataContext db = new TrackerDataContext();

            var model = new EquipModelForm();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ManufactureList = Utility.GetManufaturerList();
            var ModelData = db.EquipModels.Where(i => i.ModelUID == id && i.BranchID== LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            model.ModelUID = id;
            model.EquipModel = ModelData.Model;
            model.Manufacturer = ModelData.Manufacturer;
            model.Notes = ModelData.ModelNotes;
            model.EquipTypeCode = ModelData.EquipTypeCode;

            return View("Add", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Add(EquipModelForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Model Submit", model.ModelUID, Request);
            TrackerDataContext db = new TrackerDataContext();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.ManufactureList = Utility.GetManufaturerList();
            if (model.ModelUID != 0)
            {

                var ModelData = db.EquipModels.Where(i => i.ModelUID == model.ModelUID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (ModelData.BranchID == 0)
                    ModelData.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);

                ModelData.Model = model.EquipModel;
                ModelData.Manufacturer = model.Manufacturer;
                ModelData.ModelNotes = model.Notes;
                ModelData.EquipTypeCode = model.EquipTypeCode;
                db.SubmitChanges();
                this.SetNotification("Model updated successfully.", NotificationEnumeration.Success);
                return RedirectToAction("GetEquipmentModel", "Equipment");
            }
            else
            {
                var ModelData = new EquipModel();
                ModelData.Model = model.EquipModel;
                ModelData.Manufacturer = model.Manufacturer;
                ModelData.ModelNotes = model.Notes;
                ModelData.EquipTypeCode = model.EquipTypeCode;
                var ID = db.EquipModels.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ModelUID) + 1;
                ModelData.ModelUID = ID;
                ModelData.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                //var g = Guid.NewGuid();
                //ModelData.s_GUID = g;
                db.EquipModels.InsertOnSubmit(ModelData);
                db.SubmitChanges();
                this.SetNotification("Model created successfully.", NotificationEnumeration.Success);
                return RedirectToAction("GetEquipmentModel", "Equipment");
            }
           // return RedirectToAction("Index");
        }
          [Authorize]
        public JsonResult EquipType()
        {
            return Json(Utility.GetEquipTypeList(), JsonRequestBehavior.AllowGet);
        }
          [Authorize]
        public JsonResult Manufacture()
        {
            return Json(Utility.GetManufaturerList(), JsonRequestBehavior.AllowGet);
        }
    
    }
}
