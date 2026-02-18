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
    public class EngineerController : BaseController
    {
        //
        // GET: /Engineer/
        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Engineer Search", 0, Request);
            var model = new EngineerSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EngineerSearch("", LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EngineerList
            {
                EngineerID = i.EngineerID.ToString(),
                EngineerName = i.EngineerName,
                BranchName = i.BranchName,
            }).ToList();
            model.Engineers = s;
            return View(model);
        }

        [Authorize]
        public ActionResult IndexPopUp()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Engineer Search", 0, Request);
            var model = new EngineerSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EngineerSearch("", LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EngineerList
            {
                EngineerID = i.EngineerID.ToString(),
                EngineerName = i.EngineerName,
                BranchName = i.BranchName,
            }).ToList();
            model.Engineers = s;
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Index", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(EngineerSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Engineer Filter", 0, Request);
            if (model.EngineerName == null)
            {
                model.EngineerName = "";
            }
            TrackerDataContext db = new TrackerDataContext();
            var s = db.EngineerSearch(model.EngineerName, LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new EngineerList
            {
                EngineerID = i.EngineerID.ToString(),
                EngineerName = i.EngineerName,
                BranchName = i.BranchName,
            }).ToList();
            model.Engineers = s;
            return View(model);
        }
          [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add new Engineer Request", 0, Request);
            var model = new EngineerForm();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            TrackerDataContext db = new TrackerDataContext();
            ViewBag.UserNameX = db.UserBranches.Where(i => i.BranchID == Controllers.LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();

            return View(model);
        }
          [Authorize]
        public ActionResult AddPopup()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add new Engineer Request", 0, Request);
            var model = new EngineerForm();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Add", model);
        }


        [Authorize]
        public ActionResult Edit(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Engineer Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            var model = new EngineerForm();
            var EngineerData = db.Engineers.Where(i => i.EngineerID == id && i.BranchID == Controllers.LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            model.EngineerId = id;
            model.Branchid = EngineerData.BranchID.Value;
            model.BranchList = Utility.GetBranchList();
            ViewBag.UserNameX = db.UserBranches.Where(i=>i.BranchID== Controllers.LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
            model.EngineerName = EngineerData.EngineerName;
            model.UserName = EngineerData.UserName;
            model.AdminEmail = EngineerData.AdminEmail;
            return View("Add", model);
        }

        [Authorize]
        public ActionResult EditPopUp(int id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Engineer Request", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            var model = new EngineerForm();
            var EngineerData = db.Engineers.Where(i => i.EngineerID == id ).FirstOrDefault();
            model.EngineerId = id;
            model.Branchid = EngineerData.BranchID.Value;
            model.BranchList = Utility.GetBranchList();
            model.EngineerName = EngineerData.EngineerName;
            model.AdminEmail = EngineerData.AdminEmail;
            model.UserName = EngineerData.UserName;
            ViewBag.PopUp = 1;
            model.PopUp = true;
            return View("Add", model);
        }
          [Authorize]
        [HttpPost]
        public ActionResult Add(EngineerForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add New / Edit Engineer Submit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();

            if (model.EngineerId > 0)
            {
                var EngineerData = db.Engineers.Where(i => i.EngineerID == model.EngineerId && i.BranchID == Controllers.LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                EngineerData.BranchID = model.Branchid;
                EngineerData.EngineerName = model.EngineerName;
                EngineerData.UserName = model.UserName;
                EngineerData.AdminEmail = model.AdminEmail;
                db.SubmitChanges();
                this.SetNotification("Engineer updated successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "Engineer");
                }
                else
                {
                    return RedirectToAction("Index", "Engineer");
                }
                //  return RedirectToAction("Index");

            }
            else
            {
                var EngineerData = new MDS.DB.Engineer();
                EngineerData.BranchID = model.Branchid;
                EngineerData.EngineerName = model.EngineerName;
                EngineerData.AdminEmail = model.AdminEmail;
                EngineerData.UserName = model.UserName;
                var EngineerID = db.Engineers.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.EngineerID) + 1;
                EngineerData.EngineerID = EngineerID;
               // var g = Guid.NewGuid();
                //EngineerData.s_GUID = g;
                db.Engineers.InsertOnSubmit(EngineerData);
                db.SubmitChanges();
                this.SetNotification("Engineer created successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "Engineer");
                }
                else
                {
                    return RedirectToAction("Index", "Engineer");
                }
                // return RedirectToAction("Index");

            }
        }

    }
}
