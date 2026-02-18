
using MDS.DB;
using MDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MDS.Helper;

namespace MDS.Controllers
{
    public class EquipTypeController : BaseController
    {
        //
        // GET: /EquipType/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All EquipType Request", 0, Request);
            var model = new EquipTypeSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EquipTypes.Where(i=>i.BranchID== LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EquipTypeList
            {
                Code = i.EquipTypeCode,
                Description = i.Description,
                Name = i.Name
            }).ToList();
            model.EquipTypes = s;
            return View(model);
        }

        [Authorize]
        public ActionResult IndexPopUp()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All EquipType Request", 0, Request);
            var model = new EquipTypeSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EquipTypes.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EquipTypeList
            {
                Code = i.EquipTypeCode,
                Description = i.Description,
                Name = i.Name
            }).ToList();
            model.EquipTypes = s;
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Index", model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Index(EquipTypeSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All EquipType Search", 0, Request);
            if (model.Name == null)
            {
                model.Name = "";
            }
            if (model.Code == null)
            {
                model.Code = "";
            }

            TrackerDataContext db = new TrackerDataContext();
            var s = db.EquipTypes.Where(i => i.Name.ToLower().Contains(model.Name) && i.EquipTypeCode.ToLower().Contains(model.Code) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EquipTypeList
            {
                Code = i.EquipTypeCode,
                Description = i.Description,
                Name = i.Name
            }).ToList();
            model.EquipTypes = s;
            return View(model);
        }

        [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add EquipType Request", 0, Request);
            var model = new EquipForm();
            return View(model);
        }
          [Authorize]
        public ActionResult AddPopup()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add EquipType Request", 0, Request);
            var model = new EquipForm();
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Add", model);
        }
          [Authorize]
        public EquipForm Editdata(string id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit EquipType Request", 0, Request);
            Session["Id"] = id;
            TrackerDataContext db = new TrackerDataContext();
            var model = new EquipForm();
            var EquipFormData = db.EquipTypes.Where(i => i.EquipTypeCode == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
//            if (EquipFormData.EquipTypeCode!=null)
                model.Code = EquipFormData.EquipTypeCode;
            model.ECode = id;
            model.Description = EquipFormData.Description;
            model.Name = EquipFormData.Name;

            var Tests = db.EquipTypeTests.Where(i => i.EquipTypeCode == id ).Select(i => new TestScirpt
            {
                Test = i.Test,
                TestScriptID = i.EquipTypeTestID.ToString(),
                TestID = i.EquipTypeTestID

            }).ToList();
            model.Tests = Tests;
            return model;
        }
        [Authorize]
        public ActionResult EditPopUp(string id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit EquipType Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            var model = new EquipForm();
            model = Editdata(id);
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Add", model);
        }
        [Authorize]
        public ActionResult Edit(string id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit EquipType Request", 0, Request);
            var model = new EquipForm();
            model = Editdata(id);
            return View("Add", model);
        }

          [Authorize]
        [HttpPost]
        public ActionResult Add(EquipForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add EquipType Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();

            if (model.ECode != null)
            {
                var CheckDuplicateCode = db.EquipTypes.Where(i => i.EquipTypeCode.ToLower() == model.Code.ToLower() && i.EquipTypeCode.ToLower() != model.ECode.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateCode != null)
                {
                    ModelState.AddModelError("", "Equip Type code already exists.");
                    return View(model);
                }

                var EquipFormData = db.EquipTypes.Where(i => i.EquipTypeCode == model.ECode && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();

                var Tests = db.EquipTypeTests.Where(i => i.EquipTypeCode == model.ECode ).ToList();

                foreach (var t in Tests)
                {
                    t.EquipTypeCode = model.Code;
                }

                //  db.EquipTypeTests.DeleteAllOnSubmit(Tests);

                db.EquipTypes.DeleteOnSubmit(EquipFormData);
                db.SubmitChanges();

                var EquipData = new EquipType();
                EquipData.Description = model.Description;
                EquipData.Name = model.Name;
                EquipData.EquipTypeCode = model.Code;
                EquipData.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name); 
                //var g = Guid.NewGuid();
                //EquipData.s_GUID = g;
                db.EquipTypes.InsertOnSubmit(EquipData);
                db.SubmitChanges();
                this.SetNotification("Equip Type updated successfully.", NotificationEnumeration.Success);             
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "EquipType");
                }
                else
                {
                    return RedirectToAction("Index", "EquipType");
                }
            }
            else
            {
                //if (model.Code != null)
                //{
                var CheckDuplicateCode = db.EquipTypes.Where(i => i.EquipTypeCode.ToLower() == model.Code.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateCode != null)
                {
                    ModelState.AddModelError("", "Equip Type code already exists.");
                    return View(model);
                }
                var EquipData = new EquipType();
                EquipData.Description = model.Description;
                EquipData.Name = model.Name;
                EquipData.EquipTypeCode = model.Code;
                EquipData.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name); 
             
               // EquipData.s_GUID = g;
                db.EquipTypes.InsertOnSubmit(EquipData);
                db.SubmitChanges();
                this.SetNotification("Equip Type created successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "EquipType");
                }
                else
                {
                    return RedirectToAction("Index", "EquipType");
                }
                //}
            }
            // return RedirectToAction("Index");
        }
          [Authorize]
        public ActionResult UpdateScript([DataSourceRequest]DataSourceRequest request, TestScirpt s)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ModelState.IsValid)
            {
                var p = db.EquipTypeTests.Where(i => i.EquipTypeTestID == s.TestID).FirstOrDefault();
                p.Test = s.Test;
                db.SubmitChanges();
            }

            var Tests = db.EquipTypeTests.Where(i => i.EquipTypeCode == Session["Id"].ToString()).Select(i => new TestScirpt
            {
                Test = i.Test,
                TestScriptID = i.EquipTypeTestID.ToString(),
                TestID = i.EquipTypeTestID

            }).ToList();

            // Return the updated product. Also return any validation errors.
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult CreateScript([DataSourceRequest]DataSourceRequest request, TestScirpt s)
        {
            TrackerDataContext db = new TrackerDataContext();
            if (ModelState.IsValid)
            {
                var ID = db.EquipTypeTests.Max(i => i.EquipTypeTestID) + 1;
                var entity = new EquipTypeTest
                {
                    Test = s.Test,
                    EquipTypeTestID = ID,
                    EquipTypeCode = Session["Id"].ToString()

                };
                db.EquipTypeTests.InsertOnSubmit(entity);
                db.SubmitChanges();
                s.TestID = entity.EquipTypeTestID;
                s.TestScriptID = entity.EquipTypeTestID.ToString();

            }
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }
          [Authorize]
        public ActionResult DeleteScript([DataSourceRequest]DataSourceRequest request, TestScirpt s)
        {
            if (ModelState.IsValid)
            {
                TrackerDataContext db = new TrackerDataContext();
                var p = db.EquipTypeTests.Where(i => i.EquipTypeTestID == s.TestID).FirstOrDefault();
                db.EquipTypeTests.DeleteOnSubmit(p);
                db.SubmitChanges();
            }
            // Return the removed product. Also return any validation errors.
            return Json(new[] { s }.ToDataSourceResult(request, ModelState));
        }

    }
}
