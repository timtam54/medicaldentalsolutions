
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
    public class ClientReportingController : BaseController
    {
        //
        // GET: /EquipType/
        [Authorize]
        public ActionResult Index()
        {
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);

            var model = new ClientReportingSearch();
            TrackerDataContext db = new TrackerDataContext();
            var s = (from l in db.LoginTables
                     join c in db.Customers on l.Location equals c.CustomerCode
                     into Inners
                     from b in Inners.DefaultIfEmpty()
                     where l.BranchID == BranchID  
                     select new LoginTableVM {
                         UserName=l.UserName,
                         Password=l.Password,
                         Customer = b.CompanyName
                     }
                     ).OrderBy(c => c.UserName).ToList();
            model.LoginList = s;
            return View(model);
        }
       
      

        [Authorize]
        public ActionResult Add()
        {
            var model  = new LoginTableVM();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            return View(model);
        }

          [Authorize]
        public LoginTableVM Editdata(string UserName)
        {  
            TrackerDataContext db = new TrackerDataContext();
            var model = new LoginTableVM();

            var LoginTableData = db.LoginTables.Where(i => i.UserName == UserName && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            model.UserName = LoginTableData.UserName;
            model.Password = LoginTableData.Password;
            model.Customer = LoginTableData.Location;
            model.IsUpdate = true;
            model.OldUserName = LoginTableData.UserName;
            model.CustomerList = Utility.GetCustomerListByBranchId();
            
            return model;
        }
      
        [Authorize]
        public ActionResult Edit(string id)
        {
            var model = new LoginTableVM();            
            model = Editdata(id);
            return View("Add", model);
        }
          [Authorize]
        [HttpPost]
        public ActionResult Add(LoginTableVM model)
        {
            TrackerDataContext db = new TrackerDataContext();
            var LoginVM = new LoginTable();
            if (model.Customer == null || model.Customer == "")
            {
                model.Customer = "All";
            }
            if (model.IsUpdate == true)
            {
                var CheckDuplicateUser = db.LoginTables.Where(i => i.UserName.ToLower() == model.UserName.ToLower() && i.UserName.ToLower() != model.OldUserName.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateUser != null)
                {
                    model.CustomerList = Utility.GetCustomerListByBranchId();
                    ModelState.AddModelError("", "User name already exists.");
                    return View(model);
                }

                var LoginTableData = db.LoginTables.Where(i => i.UserName.ToLower() == model.OldUserName.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                db.LoginTables.DeleteOnSubmit(LoginTableData);
                db.SubmitChanges();

                LoginVM.UserName = model.UserName;
                LoginVM.Password = model.Password;
                LoginVM.Location = model.Customer;
                LoginVM.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);

                db.LoginTables.InsertOnSubmit(LoginVM);
                db.SubmitChanges();
                this.SetNotification("Login added successfully.", NotificationEnumeration.Success);
                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "ClientReporting");
                }
                else
                {
                    return RedirectToAction("Index", "ClientReporting");
                }

                if (model.PopUp)
                {
                    return RedirectToAction("IndexPopUp", "ClientReporting");
                }
                else
                {
                    return RedirectToAction("Index", "ClientReporting");
                }
            }
            else
            {
                LoginVM.UserName = model.UserName;
                LoginVM.Password = model.Password;
                LoginVM.Location = model.Customer;
                LoginVM.BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
                var CheckDuplicateUser = db.LoginTables.Where(i => i.UserName.ToLower() == model.UserName.ToLower() && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
                if (CheckDuplicateUser != null)
                {
                    model.CustomerList = Utility.GetCustomerListByBranchId();
                    ModelState.AddModelError("", "User name already exists.");
                    return View(model);
                }
                else
                {
                    db.LoginTables.InsertOnSubmit(LoginVM);
                    db.SubmitChanges();
                    this.SetNotification("Login added successfully.", NotificationEnumeration.Success);
                    if (model.PopUp)
                    {
                        return RedirectToAction("IndexPopUp", "ClientReporting");
                    }
                    else
                    {
                        return RedirectToAction("Index", "ClientReporting");
                    }
                }
            }
            // return RedirectToAction("Index");
        }
        
    }
}
