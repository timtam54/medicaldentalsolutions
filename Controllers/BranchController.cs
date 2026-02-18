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
    public class BranchController : BaseController
    {
        //
        // GET: /Branch/
        [Authorize]
        public ActionResult Index()
        {
            var model = new BranchSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.BranchSearch("").Select(i => new BranchList
            {
                BranchID = i.BranchID.ToString(),
                BranchName = i.BranchName,
                PrefixCode = i.PrefixCode
        //        PasswordROTech=i.PasswordROTech
            }).ToList();
            model.Branches = s;
            Utility.Audit(HttpContext.User.Identity.Name, "All Branches Request", 0, Request);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(BranchSearch model)
        {
            if (model.BranchName == null)
            {
                model.BranchName = "";
            }
            TrackerDataContext db = new TrackerDataContext();
            var s = db.BranchSearch(model.BranchName).Select(i => new BranchList
            {
                BranchID = i.BranchID.ToString(),
                BranchName = i.BranchName,
                PrefixCode = i.PrefixCode,
            }).ToList();
            model.Branches = s;
            Utility.Audit(HttpContext.User.Identity.Name, "All Branches Submit", 0, Request);
            return View(model);
        }

          [Authorize]
        public ActionResult Add()
        {
            var model = new BranchForm();
            Utility.Audit(HttpContext.User.Identity.Name, "New Branches Request", 0, Request);
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            TrackerDataContext db = new TrackerDataContext();
            var model = new BranchForm();
            var BranchData = db.Branches.Where(i => i.BranchID == id).FirstOrDefault();
            model.BranchID = id;
            model.BranchName = BranchData.BranchName;
            model.Password = BranchData.Password;
            model.PrefixCode = BranchData.PrefixCode;
            model.PasswordROTech = BranchData.PasswordROTech;
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Existing Branch Detail Request", 0, Request);
            return View("Add", model);
        }

          [Authorize]
        [HttpPost]
        public ActionResult Add(BranchForm model)
        {
            TrackerDataContext db = new TrackerDataContext();
            Utility.Audit(HttpContext.User.Identity.Name, "Add Update Branch - Submit", 0, Request);
            db.CommandTimeout = 90;
            if (model.BranchID > 0)
            {
                var BranchData = db.Branches.Where(i => i.BranchID == model.BranchID).FirstOrDefault();
                BranchData.BranchName = model.BranchName;
                BranchData.Password = model.Password;
                BranchData.PrefixCode = model.PrefixCode;
                BranchData.PasswordROTech = model.PasswordROTech;
                db.SubmitChanges();
                db.Dispose();
                this.SetNotification("Branch updated successfully.", NotificationEnumeration.Success);
                return RedirectToAction("Index", "Branch");
            }
            else
            {
                var BranchData = new Branch();
                BranchData.BranchName = model.BranchName;
                BranchData.Password = model.Password;
                BranchData.PrefixCode = model.PrefixCode;
                BranchData.PasswordROTech = model.PasswordROTech;
                var BranchID = db.Branches.Max(i => i.BranchID) + 1;
                BranchData.BranchID = BranchID;
                db.Branches.InsertOnSubmit(BranchData);
                db.SubmitChanges();
                db.Dispose();
                this.SetNotification("Branch created successfully.", NotificationEnumeration.Success);
                return RedirectToAction("Index", "Branch");
            }

        }
    }
}
