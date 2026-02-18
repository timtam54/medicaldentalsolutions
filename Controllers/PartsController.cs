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
    public class PartsController :  BaseController
    {
        //
        // GET: /Parts/

        //DeleteRepair
        //
        [Authorize]
        [HttpPost]
        public JsonResult DeletePart(int PartID)
        {
            TrackerDataContext db = new TrackerDataContext();
            Utility.Audit(HttpContext.User.Identity.Name, "Delete Part", PartID, Request);

            var d = db.DeletePart(PartID, LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            return Json(new { msg = d.msg });
        }

        [Authorize]
        public ActionResult Index()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All Part Request", 0, Request);
            var model = new PartsSearch();
            TrackerDataContext db = new TrackerDataContext();
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            var s = db.Parts.Where(i => i.BranchID == BranchID).AsEnumerable().Select(i => new PartsSearch
            {
                Type     = i.Type,
                ID=i.ID,
                Item = i.Item,
                Descrip = i.Descrip,
                 Price = i.Price,
                Account = i.Account
                    ,Active =(i.ActiiveStatus==null)?true:( (i.ActiiveStatus=="Active")?true:false)
            }).ToList();
            model.Parts= s;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(PartsSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search Part Submit", 0, Request);
            if (model.Item == null)
            {
                model.Item = "";
            }
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var s = db.Parts.Where(i => (i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)) && (i.Type.ToLower().Contains(model.Item) || i.Item.ToLower().Contains(model.Item) || i.Descrip.ToLower().Contains(model.Item))).Select(i => new PartsSearch
            {
                Item = i.Item,
                Type=i.Type,
                ID=i.ID,
                Descrip=i.Descrip,
                Price = i.Price,
                Account = i.Account
                    ,
                Active = (i.ActiiveStatus == null) ? true : ((i.ActiiveStatus == "Active") ? true : false)
            }).ToList();
            model.Parts = s;
            return View(model);
        }



        [Authorize]
        public ActionResult Add()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Part Request", 0, Request);
            var model = new PartsForm();
            model.TypeList = Utility.GetTypeList();
            model.AccountList = Utility.GetAccountList();
            model.TaxCodeList = Utility.GetTaxCodeList();
            return View(model);
        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Part Request", 0, Request);
            var model = new PartsForm();
            model = Editdata(id);

            return View("Add", model);
        }
        [Authorize]
        public PartsForm Editdata(string id)

        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit Part Request", 0, Request);
            Session["Id"] = id;
            TrackerDataContext db = new TrackerDataContext();
            var model = new PartsForm();
            model.TypeList = Utility.GetTypeList();
            model.AccountList = Utility.GetAccountList();
            model.TaxCodeList = Utility.GetTaxCodeList();
            var EquipFormData = db.Parts.Where(i => i.ID == Convert.ToInt32(id) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            model.ID = EquipFormData.ID;
            model.Type = EquipFormData.Type;
            model.Descrip = EquipFormData.Descrip;
            model.Item = EquipFormData.Item;
            model.Price = EquipFormData.Price;
            model.TaxCode = EquipFormData.TaxCode;
            model.Account = EquipFormData.Account;
            model.Cost = EquipFormData.Cost;
            if (EquipFormData.ActiiveStatus == "Active")
                model.Active=true;
            else
                model.Active=false;
            return model;
        }

        [Authorize]
        void UpdateParts(PartsForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Update Parts Submit", model.ID, Request);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            var obj = db.Parts.Where(i => i.ID == model.ID && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            if (model.Price.HasValue)
            {
                obj.Price = Convert.ToDecimal(model.Price.Value);
            }
            obj.Descrip = model.Descrip;
            obj.Type = model.Type;
            obj.Account = model.Account;
            obj.TaxCode = model.TaxCode;
            obj.Item = model.Item;
            obj.Cost = model.Cost;
            if (model.Active)
                obj.ActiiveStatus = "Active";
            else
                obj.ActiiveStatus = "Not-active";
            db.SubmitChanges();
        }

         [Authorize]
        void SaveParts(PartsForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Save Parts Submit", model.ID, Request);
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
            Part obj = new Part();
            var ID = db.Parts.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ID) + 1;
            obj.ID = ID;
            if (model.Price.HasValue)
            {
                obj.Price = Convert.ToDecimal(model.Price.Value);
            }
            obj.Descrip = model.Descrip;
            obj.Type = model.Type;
            obj.TaxCode = model.TaxCode;
            obj.Account = model.Account;
            obj.Item = model.Item;
            obj.Cost = model.Cost;
            obj.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            if (model.Active)
                obj.ActiiveStatus = "Active";
            else
                obj.ActiiveStatus = "Not-active";

            db.Parts.InsertOnSubmit(obj);
            db.SubmitChanges();
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveUpdateParts(PartsForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Save/Update Parts Submit", model.ID, Request);
            if (model.ID != 0)
            {
                UpdateParts(model);
                //if (model.PopUp)
                //{
                //    return Json(new { msg = "0", message = "Equipment updated successfully." });
                //}
                //else
                {
                    return Json(new { msg = "1", message = "Parts updated successfully." });
                }
            }
            else
            
            {
                TrackerDataContext db = new TrackerDataContext();
                db.CommandTimeout = 90;
                var CheckDuplicateCode = db.Parts.Where(i => i.Item.ToLower() == model.Item.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateCode != null)
                {
                    return Json(new { msg = "0", message = "This part '"+model.Item+"' already exists - cannot add duplicate item records." });
                }
                SaveParts(model);               
                {
                    return Json(new { msg = "1", message = "Equipment created successfully." });
                }
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult Add(PartsForm model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Add Part Submit", model.ID, Request);
            model.TypeList = Utility.GetTypeList();
            model.AccountList = Utility.GetAccountList();
            model.TaxCodeList = Utility.GetTaxCodeList();

            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 90;
//            if (model.Item != null)
            {
                var CheckDuplicateCode = db.Parts.Where(i => i.Item.ToLower() == model.Item.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateCode != null)
                {
                    ModelState.AddModelError("", "Item code already exists.");
                    return View(model);
                }


                db.Parts.DeleteOnSubmit(CheckDuplicateCode);
                db.SubmitChanges();

                var EquipData = new Part();

                var ID = db.Parts.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).Max(i => i.ID) + 1;
                EquipData.ID = ID;
            

                EquipData.Item = model.Item;
                EquipData.Descrip = model.Descrip;
                EquipData.Type = model.Type;
                EquipData.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                EquipData.TaxCode = model.TaxCode;
                EquipData.Account = model.Account;
                EquipData.Price = model.Price;
                EquipData.Cost = model.Cost;
                if (model.Active)
                    EquipData.ActiiveStatus = "Active";
                else
                    EquipData.ActiiveStatus = "Not-active";

                db.Parts.InsertOnSubmit(EquipData);
                db.SubmitChanges();
                this.SetNotification("Parts updated successfully.", NotificationEnumeration.Success);
                {
                    return RedirectToAction("Index", "Parts");
                }
            }
        }

    }
}
