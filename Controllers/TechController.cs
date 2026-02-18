using MDS.DB;
using MDS.Helper;
using MDS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MDS.Controllers
{
    public class TechController : Controller
    {
        public JsonResult CheckOutServiceToEng(int ServiceID,int BranchID)
        {
            try
            {
                int EngineerID = LoginController.EngineerID(User.Identity.Name);
                TrackerDataContext db = new TrackerDataContext();
                var sv = db.Services.Where(i => i.ServiceUID == ServiceID && i.BranchID == BranchID).FirstOrDefault();
                sv.EngineerID = EngineerID;
                db.SubmitChanges();
                return Json(new { Error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }
        public JsonResult PartSearch(string searchString, int BranchID)
        {
            TrackerDataContext mydb = new TrackerDataContext();
            //RM 30/11/18 - search was changed to include rank, since it was impossible to just pick 'Acropora GENUS' 
            var taxon = (from pt in mydb.Parts
                         where pt.BranchID == BranchID && (pt.Descrip + pt.Item).Contains(searchString)

                         select new PartSearch { ItemNo = pt.Item, Descrip = pt.Descrip, ID = pt.ID, Price = (pt.Cost == null) ? 0 : Convert.ToDecimal(pt.Cost) }
                         ).Take(15).ToList();
                //var res = taxon.OrderBy(i => i.Item).ThenBy(i => i.Descrip).Take(15).ToList();
            return Json(new { results = taxon });
        }


        public ActionResult EditPartsService(int ServicePartID, int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();

            ServicePart ret = db.ServiceParts.Where(rp => rp.ServicePartId == ServicePartID && rp.BranchID == BranchID).FirstOrDefault();
            return View("CreatePartsService", ret);
        }


        public ActionResult EditParts(int RepairPartId, int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();

            RepairPart ret = db.RepairParts.Where(rp => rp.RepairPartId == RepairPartId && rp.BranchID == BranchID).FirstOrDefault();
            return View("CreateParts", ret);
        }

        public ActionResult CreatePartsService(int ServiceID, int BranchID)
        {
            ServicePart ret = new ServicePart();
            ret.RepairOrServiceUID = ServiceID;
            ret.BranchID = BranchID;
            return View(ret);
        }

        public ActionResult CreateParts(int RepairID, int BranchID)
        {

            Utility.Audit(HttpContext.User.Identity.Name, "TechAddParts", RepairID, Request);

            RepairPart ret = new RepairPart();
            ret.RepairOrServiceUID = RepairID;
            ret.BranchID = BranchID;
            return View(ret);
        }

        [HttpPost]
        public JsonResult DeletePartService(int ServiceID, int ServicePartID, int BranchID)
        {
            try
            {
                TrackerDataContext db = new TrackerDataContext();
                var photo = db.ServiceParts.Where(p => p.ServicePartId == ServicePartID && p.BranchID == BranchID).FirstOrDefault();
                db.ServiceParts.DeleteOnSubmit(photo);
                db.SubmitChanges();

                return Json(new { Error = "", ServiceID = ServiceID });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });

            }
        }


        [HttpPost]
        public JsonResult DeletePart(int RepairPartId, int BranchID)
        {
            try
            {
                TrackerDataContext db = new TrackerDataContext();
                var photo = db.RepairParts.Where(p => p.RepairPartId == RepairPartId && p.BranchID == BranchID).FirstOrDefault();
                db.RepairParts.DeleteOnSubmit(photo);
                db.SubmitChanges();
                //var rps = db.RepairParts.Where(p => p.RepairOrServiceUID == RepairID && p.BranchID == BranchID).ToList();
                //decimal cost=0;
                //foreach (RepairPart rp in rps)
                //{
                //    if (rp.CostPerUnit != null)
                //    {
                //        if (rp.NumberUsed!=null)
                //            cost += Convert.ToDecimal(rp.NumberUsed) * Convert.ToDecimal(rp.CostPerUnit);
                //    }
                //}
                return Json(new { Error = ""/*,cost=cost */});
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });

            }
        }

        [HttpPost]
        public JsonResult CostOfPart(int RepairID, int BranchID)
        {
            try
            {
                TrackerDataContext db = new TrackerDataContext();
                var rps = db.RepairParts.Where(p => p.RepairOrServiceUID == RepairID && p.BranchID == BranchID).ToList();
                decimal cost = 0;
                foreach (RepairPart rp in rps)
                {
                    if (rp.CostPerUnit != null)
                    {
                        if (rp.NumberUsed != null)
                            cost += Convert.ToDecimal(rp.NumberUsed) * Convert.ToDecimal(rp.CostPerUnit);
                    }
                }
                return Json(new { Error = "", cost = cost.ToString("c") });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message,cost="$0" });

            }
        }

        [HttpPost]
        public JsonResult CreatePartsSave(RepairPart ret)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Tech-CreatePartsSave", 0, Request);

            try
            {
                TrackerDataContext db = new TrackerDataContext();

                if ((ret.RepairPartId == 0) || (ret.RepairPartId == null))
                {
                    if (ret.NumberUsed == null)
                        ret.NumberUsed = 1;
                    db.RepairParts.InsertOnSubmit(ret);
                }
                else
                {
                    var rpdb = db.RepairParts.Where(rp => rp.RepairPartId == ret.RepairPartId && rp.BranchID == ret.BranchID).FirstOrDefault();
                    rpdb.PartID = ret.PartID;
                    rpdb.PartNumber = ret.PartNumber;
                    rpdb.PartDesc = ret.PartDesc;
                    rpdb.CostPerUnit = ret.CostPerUnit;
                    if (ret.NumberUsed == null)
                        rpdb.NumberUsed = 1;
                    else
                        rpdb.NumberUsed = ret.NumberUsed;

                }
                db.SubmitChanges();

                return Json(new { Error = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreatePartsServiceSave(ServicePart ret)
        {
            try
            {
                TrackerDataContext db = new TrackerDataContext();

                if ((ret.ServicePartId == 0) || (ret.ServicePartId == null))
                {
                    if (ret.NumberUsed == null)
                        ret.NumberUsed = 1;

                    db.ServiceParts.InsertOnSubmit(ret);
                }
                else
                {
                    var rpdb = db.ServiceParts.Where(rp => rp.ServicePartId == ret.ServicePartId && rp.BranchID == ret.BranchID).FirstOrDefault();
                    rpdb.PartID = ret.PartID;
                    rpdb.PartNumber = ret.PartNumber;
                    rpdb.PartDesc = ret.PartDesc;
                    rpdb.CostPerUnit = ret.CostPerUnit;
                    //rpdb.NumberUsed = ret.NumberUsed;
                    if (ret.NumberUsed == null)
                        rpdb.NumberUsed = 1;
                    else
                        rpdb.NumberUsed = ret.NumberUsed;
                }
                db.SubmitChanges();

                return Json(new { Error = "", ServiceID = ret.RepairOrServiceUID });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PicProcess(object blob, int? RepairID, int? ServiceID, int BranchID, string Field)
        {
            int idx;
            if (Field == "null")
                Field = "Picture@" + DateTime.Now.ToString("yyyyMMddHHmmss");
           

            string contenttype = "image/png";
            string fileName, path;
            GetFileName(RepairID, ServiceID, BranchID, Field, out idx, out fileName, out path);
            if (System.IO.File.Exists(path))
            {
                return Json(new { Error = "A file with this name already exists. Please rename your file, or remove the previous file prior to uploading" });
            }
            string fff = ((string[])blob)[0].Replace("data:image/png;base64,", "");
            byte[] data = Convert.FromBase64String(fff);
            using (var stream = new MemoryStream(data, 0, data.Length))
            {
                //System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();
                //TODO: do something with image
            }

            Photo pt = new Photo();
           
                pt.Title = Field;


            pt.RepairID = RepairID;
            pt.ServiceID = ServiceID;
            pt.BranchID = BranchID;
            pt.URL = fileName;
            //pt.Attach_Audio = true;
            pt.Type = contenttype;
            TrackerDataContext db = new TrackerDataContext();
            db.Photos.InsertOnSubmit(pt);
            db.SubmitChanges();

            Utility.Audit(HttpContext.User.Identity.Name, "TEch-PIC", idx, Request);

            return Json(new { Error = "" });

            
        }

        [HttpPost]
        public JsonResult AudioProcess(System.Web.HttpPostedFileWrapper blob,int? RepairID, int? ServiceID, int BranchID,string Field)
        {
            int idx;
            string contenttype = "audio/wav";
            string fileName, path;
            GetFileName(RepairID, ServiceID, BranchID, Field,out idx,out fileName, out path);
            if (System.IO.File.Exists(path))
            {
                return Json(new { Error = "A file with this name already exists. Please rename your file, or remove the previous file prior to uploading" });
            }
            blob.SaveAs(path);
            Photo pt = new Photo();
            pt.Title = Field;
            pt.RepairID = RepairID;
            pt.ServiceID = ServiceID;
            pt.BranchID = BranchID;
            pt.URL = fileName;
            pt.Attach_Audio = true;
            pt.Type = contenttype;
            TrackerDataContext db = new TrackerDataContext();
            db.Photos.InsertOnSubmit(pt);
            db.SubmitChanges();

            Utility.Audit(HttpContext.User.Identity.Name, "TEch-AudioBlob", idx, Request);

            return Json(new { Error = "" });
        }

        private void GetFileName(int? RepairID, int? ServiceID, int BranchID, string Field, out int idx, out string fileName, out string path)
        {
            idx = -1;
            if (RepairID != null)
                idx = Convert.ToInt32(RepairID);
            else if (ServiceID != null)
                idx = Convert.ToInt32(ServiceID);
            int id;
            string tpe;
            if (RepairID != null)
            {
                id = Convert.ToInt32(RepairID);
                tpe = "Rpr";
            }
            else
            {
                id = Convert.ToInt32(ServiceID);
                tpe = "Svc";
            }
            fileName = "Audio for " + Field;
            fileName = tpe + "_" + id.ToString("######") + "-" + BranchID.ToString() + "-" + Field.Replace(' ', '-') + DateTime.Now.ToString("HHmmss") + ".wav"; //For TheDock naming convention

            path = Path.Combine(Server.MapPath(RepairController.uploadfolder), fileName);
        }

        public ActionResult Signature(bool cs,int ID,string RepairOrService,int BranchID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "TechSignature", 0, Request);

            Signature oo = new Signature();
            oo.ID = ID;
            oo.BranchID = BranchID;
            oo.RepairOrService = RepairOrService;

            if (cs)
            {
                return View("ShowSignature",oo);
            }
            return View();
        }
        [HttpPost]
        public JsonResult Signature(Signature oo)
        {
            try
            {
                //int id = 1;
                //string RepairService = "R";
                //int BranchID = 4;
                if (String.IsNullOrWhiteSpace(oo.SignatureDataUrl))
                    return Json(new { Error = "Signature is blank" });


                char[] chars = new char[] { char.Parse(",") };
                var base64Signature = oo.SignatureDataUrl.Split(chars)[1];
                var binarySignature = Convert.FromBase64String(base64Signature);

                string fileName = "Signature_" + oo.ID.ToString("######") + "-" + oo.RepairOrService + "-" + oo.BranchID.ToString() + ".png";
                var path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    //return Json(new { Error = "A file with this name already exists. Please rename your file, or remove the previous file prior to uploading" });
                }
                //File.SaveAs(path);


                System.IO.File.WriteAllBytes(path, binarySignature);
                return Json(new { Error = "",path=path });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }


        public ActionResult Audio2()
        {
            return View();
        }
        public ActionResult Audio3()
        {
            return View();
        }
        //public ActionResult ServiceWorkStartEndTime(int ServiceJobUID, int BranchID)
        //{
        //    DB.ServiceEngineerWork sew = new ServiceEngineerWork();
        //    return View(sew);
        //}
        public ActionResult ServiceWorkEnd(int ID, int BranchID,bool Repair)
        {
            TrackerDataContext db = new TrackerDataContext();
            DB.ServiceEngineerWork sew;
                sew= db.ServiceEngineerWorks.Where(ii => ii.Repair==Repair && ii.ServiceJobID == ID && ii.BranchID == BranchID && ii.EngineerID == LoginController.EngineerID(User.Identity.Name) && (ii.EndTime == null) && (ii.StartTime != null)).FirstOrDefault();
            if (sew == null)
                {
                    return RedirectToAction("ServiceWorkStart", new { ServiceJobUID = ID, BranchID = BranchID });
                }
            ViewBag.ChargeTypeX = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            return View(sew);
        }
        [HttpPost]
        public JsonResult ServiceWorkEndSave(DB.ServiceEngineerWork sew)
        {

            //DB.ServiceEngineerWork sew = new ServiceEngineerWork();
            if (sew.EndTime != null)
            {
                TrackerDataContext db = new TrackerDataContext();
                var sewnew = db.ServiceEngineerWorks.Where(ii => ii.Repair==sew.Repair && ii.ServiceJobID == sew.ServiceJobID && ii.BranchID == sew.BranchID && ii.EngineerID == sew.EngineerID && (ii.EndTime == null) && (ii.StartTime != null)).FirstOrDefault();
                if (sewnew==null)
                    return Json(new { Error = "There is no open time record" });

//                sewnew.ChargeTypecode = sew.ChargeTypecode;
                sewnew.EndTime = sew.EndTime;
               //db.ServiceEngineerWorks.InsertOnSubmit(sewnew);
                db.SubmitChanges();
                return Json(new { Error = "" });
            }
            return Json(new { Error = "Please specify and end time" });
        }

        public ActionResult ServiceWorkStart(int ServiceJobUID, int BranchID,bool? Repair)
        {
            if (Repair == null)
                Repair = false;
            MDS.Models.ServiceWorkTime sew = new MDS.Models.ServiceWorkTime();
            sew.Repair = Repair;
            sew.EngineerID = LoginController.EngineerID(User.Identity.Name);
            sew.BranchID = BranchID;
            ViewBag.ChargeTypeX = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
            sew.ServiceJobID = ServiceJobUID;
            return View(sew);
        }
        [HttpPost]
        public JsonResult ServiceWorkStartSave(MDS.Models.ServiceWorkTime sew)
        {
            TrackerDataContext db = new TrackerDataContext();

            //DB.ServiceEngineerWork sew = new ServiceEngineerWork();
            if (sew.StartTime != null)
            {
                ServiceEngineerWork sewnew = new ServiceEngineerWork();
                sewnew.ServiceJobID =Convert.ToInt32( sew.ServiceJobID);
                sewnew.BranchID = Convert.ToInt32(sew.BranchID);
                //sewnew.ChargeTypecode = sew.ChargeTypecode;
                sewnew.EngineerID = Convert.ToInt32(sew.EngineerID);
                sewnew.Repair = sew.Repair;
                sewnew.StartTime = Convert.ToDateTime(sew.StartTime);
                db.ServiceEngineerWorks.InsertOnSubmit(sewnew);
                db.SubmitChanges();
                return Json(new { Error = "" });
            }
            return Json(new { Error = "Please specify and end time" });
        }
        [Authorize]
        public ActionResult Service(int ServiceJobUID, int BranchID)
        {
            var data = HttpContext.User.Identity.Name;
            ATC atc = MDS.Controllers.LoginController.IsAdmin(data);
            if (atc != ATC.Tech)
                return RedirectToAction("Edit", "Service", new { id = ServiceJobUID, BranchID = BranchID });
            Utility.Audit(HttpContext.User.Identity.Name, "TechService", ServiceJobUID, Request);
            TrackerDataContext db = new TrackerDataContext();
            //new
            if (db.ServiceEngineerWorks.Where(i => i.Repair==false && i.ServiceJobID == ServiceJobUID && i.BranchID == BranchID).Count() == 0)
            {
                var servs = db.Services.Where(i => i.ServiceJobUID == ServiceJobUID && i.BranchID == BranchID).ToList();
                foreach (var serv in servs)
                {
                    serv.EngineerID = null;
                }
                db.SubmitChanges();
            }
            ServiceJobService model = new ServiceJobService();
            model.ServiceJob = db.ServiceJobs.Where(sj => sj.ServiceJobUID == ServiceJobUID && sj.BranchID == BranchID).FirstOrDefault();
            List<Service> Services = db.Services.Where(sj => sj.ServiceJobUID == ServiceJobUID && sj.BranchID == BranchID).ToList();
            model.Services = new List<ServiceEquip>();
            model.Engineers = (from sje in db.ServiceJobEngineers join eng in db.Engineers on sje.EngineerID equals eng.EngineerID where sje.ServiceJobID == ServiceJobUID && sje.BranchID == BranchID select eng).ToList();
            bool JobCompleteSigatureDone = GetJobCompleteSigatureDone(model,db, LoginController.EngineerID(User.Identity.Name));
            if (!JobCompleteSigatureDone)//active job
            {
                if (!model.Engineers.Select(i => i.EngineerID).Contains(MDS.Controllers.LoginController.EngineerID(User.Identity.Name)))
                    model.TechClockOnPrompt = TechClockOnOffStatus.NoClock_JobCompleted;
                else
                {
                    var openSEW = db.ServiceEngineerWorks.Where(ii => ii.Repair==false && ii.ServiceJobID == ServiceJobUID && ii.BranchID == BranchID && ii.EngineerID == LoginController.EngineerID(User.Identity.Name) && (ii.EndTime == null) && (ii.StartTime != null)).ToList();
                    if (openSEW.Count() == 0)
                        model.TechClockOnPrompt = TechClockOnOffStatus.ClockOn_NoActiveShifts;
                    else
                        model.TechClockOnPrompt = TechClockOnOffStatus.NoClockOn_ActiveShiftExists;
                }
            }
            else
            {
                model.TechClockOnPrompt = TechClockOnOffStatus.NoClock_JobCompleted;
            }
            foreach (Service item in Services)
            {
                if (!item.SafetyTestDone)
                    item.DataEntryComplete = false;
                ServiceEquip se = new ServiceEquip();
                se.Service = item;
                if (item.ServiceFunctionCode != null)
                    se.ServiceFunction = true;
                else
                    se.ServiceFunction = false;
                if (item.DateServiced != null)
                    se.DateServiced = item.DateServiced.Value;
                else
                    ;// se.DateServiced = DateTime.Now;
                model.Services.Add(se);
                se.CustomerEquipment = (from eq in db.Equipments join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                        join tp in db.EquipTypes on em.EquipTypeCode equals tp.EquipTypeCode
                                        //.EquipModels
                                 where eq.BranchID == BranchID && eq.CustomerCode == model.ServiceJob.CustomerCode
                                        select new IdText { Id = eq.EquipUID, Text = ((em.Model.Length>10)?em.Model.Substring(0,10):em.Model) + " " + em.Manufacturer + " "+ ((tp.Name.Length > 14) ? tp.Name.Substring(0, 14) : tp.Name) + " " +eq.SerialNumber }).ToList();//.Take(100);

                //if (se.Service.ServiceFunctionCode == null)
                //    se.ItemNotSeen = true;
                //else if (se.Service.ServiceFunctionCode.ToLower() == "ser")
                //    se.ItemNotSeen = false;
                //else
                //    se.ItemNotSeen = true;
            }
            model.Customer = db.Customers.Where(cu => cu.CustomerCode == model.ServiceJob.CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            ViewBag.EngineerIDX = db.Engineers.Where(ee => ee.BranchID == BranchID).ToList();
            ViewBag.TravelTypecodeX = (from ct in db.TravelTypes where ct.BranchID == BranchID select new ChargeTypeList { ChargeTypeCode = ct.TravelTypeCode, ChargeType = ct.TravelType1 + " - $" + ct.TravelChargeRate.ToString() }).ToList();
            var conds = db.Conditions.ToList();
            foreach (DB.Condition item in conds)
            {
                if (item.ConditionDesc.IndexOf(" - ") > 0)
                    item.ConditionDesc = item.ConditionDesc.Substring(0, item.ConditionDesc.IndexOf(" - "));
            }

            ViewBag.ConditionIDX = conds;
            if (model.ServiceJob.CompletedOK)
                model.Status = "CompletedOK";
            else if (model.ServiceJob.CompletedOutstanding)
                model.Status = "OutstandingRepairs";
            else if (model.ServiceJob.CompletedBERs)
                model.Status = "CompletedBER";
            else
                model.Status = "Ongoing";
            model.ServicesIDs = "";
            foreach (var item in model.Services)
            {
                if (model.ServicesIDs == "")
                    model.ServicesIDs = item.Service.ServiceUID.ToString();
                else
                    model.ServicesIDs = model.ServicesIDs + "," + item.Service.ServiceUID.ToString();
            }
            return View(model);

            //Utility.Audit(HttpContext.User.Identity.Name, "Edit ServiceJob Request", id, Request);

            //var model = new AddService();
            //model = GetEditService(id);
            //Session["NewServiceID"] = id;
            //Session["bytespdf"] = null;
            //model.PopUp = false;
            return View(model);
        }

        private static bool GetJobCompleteSigatureDone(ServiceJobService model, TrackerDataContext db,int EngineerID)
        {
            
            //job complete
            bool JobComplete = model.ServiceJob.CompletedBERs || model.ServiceJob.CompletedOK || model.ServiceJob.CompletedOutstanding;
            if (!JobComplete)
                return false;
            bool CustomerSignature=model.ServiceJob.CustomerSignature;
            if (!CustomerSignature)
                return false;
            var openSEW = db.ServiceEngineerWorks.Where(ii => ii.Repair==false && ii.ServiceJobID == model.ServiceJob.ServiceJobUID && ii.BranchID ==model.ServiceJob.BranchID && ii.EngineerID == EngineerID && (ii.EndTime == null) && (ii.StartTime != null)).ToList();
            if (openSEW.Count()>0)
                return false;
            return true;
        }
        private static bool GetJobCompleteSigatureDoneRepair(RepairEquip model, TrackerDataContext db, int EngineerID)
        {

            //job complete
            bool JobComplete = model.Repair.RepairCompleted || model.Repair.ResultedInRetirement || model.Repair.NotRepaired;
            if (!JobComplete)
                return false;
            bool CustomerSignature = model.Repair.CustomerSignature;
            if (!CustomerSignature)
                return false;
            var openSEW = db.ServiceEngineerWorks.Where(ii => ii.Repair==true && ii.ServiceJobID == model.Repair.RepairUID && ii.BranchID == model.Repair.BranchID && ii.EngineerID == EngineerID && (ii.EndTime == null) && (ii.StartTime != null) && (ii.Repair==true)).ToList();
            if (openSEW.Count() > 0)
                return false;
            return true;
        }

        public static string GetCustomerContact(int? EquipUID, int BranchID)
        {
            if (EquipUID == null)
                return "";
            TrackerDataContext db = new TrackerDataContext();
            var equip = db.Equipments.Where(eq => eq.EquipUID == EquipUID).FirstOrDefault();

            var cust = db.Customers.Where(cu => cu.CustomerCode == equip.CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            if (cust == null)
                return "";
            return cust.ContactName;
        }

        public static string GetCustomerbyCustCode(string CustomerCode, int BranchID)
        {
            if (CustomerCode == null)
                return "";
            if (CustomerCode == "")
                return "";
            TrackerDataContext db = new TrackerDataContext();

            var cust = db.Customers.Where(cu => cu.CustomerCode == CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            if (cust == null)
                return "";
            return cust.CompanyName;
        }

        public static string GetCustomerContactCu(string CustomerCode, int BranchID)
        {
            if (CustomerCode == null)
                return "";
            if (CustomerCode == "")
                return "";
            TrackerDataContext db = new TrackerDataContext();

            var cust = db.Customers.Where(cu => cu.CustomerCode == CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            if (cust == null)
                return "";
            return cust.ContactName;
        }

        public static string GetWarrantyExpireData(int? EquipUID, int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();
            var cust = db.Equipments.Where(eq => eq.EquipUID == EquipUID && eq.BranchID == BranchID).FirstOrDefault();
            if (cust.WarrantyExpirationDate == null)
                return "";
            return Convert.ToDateTime(cust.WarrantyExpirationDate).ToString("dd-MMM-yyyy");
        }
        //

        public static string GetAddresCustomer(string CustomerCode, int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();

            var cust = db.Customers.Where(cu => cu.CustomerCode == CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            if (cust == null)
                return "";
            return cust.PhysicalAddress;
        }

        public static string GetCustomer(int? EquipUID, int BranchID)
        {
            if (EquipUID == null)
                return "";
            TrackerDataContext db = new TrackerDataContext();
            var equip = db.Equipments.Where(eq => eq.EquipUID == EquipUID && eq.BranchID == BranchID).FirstOrDefault();
            if (equip == null)
                return "Not Found";

            var cust = db.Customers.Where(cu => cu.CustomerCode == equip.CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            if (cust == null)
                return "";
            return cust.CompanyName;
        }

        public static string GetEquipmentType(int? ModelUID, int BranchID)
        {
            if (ModelUID == null)
                return "";
            TrackerDataContext db = new TrackerDataContext();
            var EqMod = db.EquipModels.Where(eq => eq.ModelUID == ModelUID && eq.BranchID == BranchID).FirstOrDefault();
            var et = db.EquipTypes.Where(cu => cu.EquipTypeCode == EqMod.EquipTypeCode && cu.BranchID == BranchID).FirstOrDefault();
            if (et == null)
                return "";
            return et.Name;
        }

        public static string GetLocation(int? LocationID, int BranchID)
        {
            if (LocationID == null)
                return "";
            TrackerDataContext db = new TrackerDataContext();
            var loc = db.CustomerLocations.Where(eq => eq.LocationID == LocationID && eq.BranchID == BranchID).FirstOrDefault();
            if (loc == null)
                return "";
            return loc.Location;
        }


        public static string GetAddress(int? EquipUID, int BranchID)
        {
            if (EquipUID == null)
                return "";
            if (EquipUID == 0)
                return "";
            TrackerDataContext db = new TrackerDataContext();
            var equip = db.Equipments.Where(eq => eq.EquipUID == EquipUID && eq.BranchID == BranchID).FirstOrDefault();
            if (equip == null)
                return "Not Found";
            var cust = db.Customers.Where(cu => cu.CustomerCode == equip.CustomerCode && cu.BranchID == BranchID).FirstOrDefault();
            if (cust == null)
                return "";
            return cust.PhysicalAddress;
        }

        //public AddService GetEditService(int id)
        //{
        //    Utility.Audit(HttpContext.User.Identity.Name, "Edit ServiceJob Request", id, Request);

        //    TrackerDataContext db = new TrackerDataContext();
        //    var model = new AddService();
        //    model.CustomerList = Utility.GetCustomerList();
        //    model.ChargeType = Utility.GetLabourList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
        //    model.TravelList = Utility.GetTravelList(Controllers.LoginController.BranchID(HttpContext.User.Identity.Name));
        //    model.EngineerList = Utility.GetEngineerListByBranch(LoginController.BranchID(HttpContext.User.Identity.Name));
        //    Session["NewServiceID"] = id;
        //    var ServiceData = db.ServiceJobs.Where(i => i.ServiceJobUID == id && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).ToList();
        //    model.ServiceId = id;

        //    if (ServiceData[0].ActualDateEnd.HasValue)
        //    {
        //        model.ServiceCompleteDate = ServiceData[0].ActualDateEnd.Value;
        //    }
        //    if (ServiceData[0].ActualDateStart.HasValue)
        //    {
        //        model.ServiceStartDate = ServiceData[0].ActualDateStart.Value;
        //    }
        //    if (ServiceData[0].InvoiceDate.HasValue)
        //    {
        //        model.InvoiceDate = ServiceData[0].InvoiceDate.Value;
        //    }
        //    if (ServiceData[0].DateProgrammed.HasValue)
        //    {
        //        model.ProgrammedDate = ServiceData[0].DateProgrammed.Value;
        //    }
        //    model.JobCode = Utility.GetBranchCode() + "S" + ServiceData[0].JobCode;

        //    model.IsaApprove = ServiceData[0].ApprovalRequired;
        //    model.Isorder = ServiceData[0].OrderNumberRequired;
        //    model.TechnicalContact = ServiceData[0].TechContact;
        //    model.ApprovalContact = ServiceData[0].ApprovalContact;
        //    model.BookingNotes = ServiceData[0].BookingNotes;
        //    model.Customerid = ServiceData[0].CustomerCode;
        //    model.VerbalApproval = ServiceData[0].VerbalApprovalObtained;
        //    model.IsApprovalReceived = ServiceData[0].ApprovedBy;
        //    model.OrderNo = ServiceData[0].OrderNumber;
        //    model.SpecialNotes = ServiceData[0].ServiceNotes;
        //    model.CustomerSignature = ServiceData[0].ClientSignature;
        //    model.LocationID = ServiceData[0].LocationID.ToString();
        //    model.TravelHours = ServiceData[0].TravelHours;
        //    model.Invoice = ServiceData[0].InvoiceNumber;
        //    model.Charges = ServiceData[0].Amount;
        //    model.DeptID = Convert.ToString(ServiceData[0].DeptID);
        //    model.EngineerID = ServiceData[0].EngineerID.ToString();
        //    model.DontChangeCust = ServiceData[0].NoCharge;
        //    model.PropotionalExpenses = ServiceData[0].ExpensesProportion;
        //    model.TravelTypeCode = ServiceData[0].TravelTypecode;
        //    model.HasJobInvoice = ServiceData[0].HasBeenInvoiced;
        //    model.Branchid = Convert.ToString(ServiceData[0].BranchID);
        //    model.ServiceComplete_Outstanding = ServiceData[0].CompletedOutstanding;
        //    model.ServiceComplete_BERED = ServiceData[0].CompletedBERs;
        //    model.ServiceComplete = ServiceData[0].CompletedOK;
        //    model.TravelRate = ServiceData[0].TravelChargeRate.HasValue ? ServiceData[0].TravelChargeRate.Value.ToString("C") : "$0.00";
        //    model.LabourRate = ServiceData[0].ChargeRate.HasValue ? ServiceData[0].ChargeRate.Value.ToString("C") : "$0.00";
        //    model.ChargeTypeCode = ServiceData[0].ChargeTypecode;
        //    model.LabourHours = ServiceData[0].ServiceHours;

        //    model.CostOfServicePartsForServiceJob = Math.Round(db.CostOfServicePartsForServiceJob(id, LoginController.BranchID(HttpContext.User.Identity.Name)).Value, 2);
        //    model.CostOfTotalRepairsForServiceJob = Math.Round(db.CostOfTotalRepairsForServiceJob(id, LoginController.BranchID(HttpContext.User.Identity.Name)).Value, 2);

        //    return model;
        //}

        DateTime? AddDateTime(DateTime? DateIn, DateTime? TimeIn)
        {
            if (DateIn == null)
            {
                if (TimeIn == null)
                    return null;
                return TimeIn;
            }
            else
            {
                if (TimeIn == null)
                    return DateIn;
                return Convert.ToDateTime(DateIn).Date.AddHours(Convert.ToDateTime(TimeIn).Hour).AddMinutes(Convert.ToDateTime(TimeIn).Minute);
            }
        }

        
        public ActionResult MapsCust(string CustomerCode, int BranchID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "TechMaps", 0, Request);

            ViewBag.Address = GetAddresCustomer(CustomerCode, BranchID);
            ViewBag.Address = ViewBag.Address.Replace(System.Environment.NewLine, " ");
            ViewBag.Address = ViewBag.Address.Replace("  ", " ");
            return View("Maps");
        }
        public ActionResult MapsCusts(string CustomerCodes, int BranchID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "TechMaps", 0, Request);
            string ss = "";
            char[] chr = new char[] { char.Parse(",") };
            foreach (var CustomerCode in CustomerCodes.Split(chr, StringSplitOptions.RemoveEmptyEntries).ToList())
            {
                var yy=GetAddresCustomer(CustomerCode, BranchID);
                yy= yy.Replace(System.Environment.NewLine, " ");
                yy = yy.Replace("  ", " ");
                if (ss == "")
                    ss = yy;
                else
                    ss = ss + "~" + yy;
            }
            ViewBag.Address = ss;
            return View("Maps");
        }

        public ActionResult Maps(int EquipUID,int BranchID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "TechMaps", EquipUID, Request);

            ViewBag.Address = GetAddress(EquipUID, BranchID);
            ViewBag.Address = ViewBag.Address.Replace(System.Environment.NewLine, " ");
            ViewBag.Address = ViewBag.Address.Replace("  ", " ");
            return View();
        }

        public ActionResult RepairParts(int RepairID, int BranchID,bool? Readonly)
        {
            ViewBag.Readonly = Readonly;
            List<RepairPart> ret = new List<RepairPart>();
            TrackerDataContext db = new TrackerDataContext();
            //int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            ret = db.RepairParts.Where(p => p.RepairOrServiceUID == RepairID && p.BranchID == BranchID).ToList();
            return View(ret);
        }


        public ActionResult ServiceAllWorkTimes(int ID, int BranchID,bool Repair)
        {
            //if (Repair == null)
            //    Repair = false;
            ServiceWorks ret = new ServiceWorks();
            TrackerDataContext db = new TrackerDataContext();
            ret.ServiceWorkTimes = (from sew in db.ServiceEngineerWorks
                       join eng in db.Engineers on new { EngineerID = sew.EngineerID, BranchID = sew.BranchID } equals new { EngineerID = eng.EngineerID, BranchID = eng.BranchID.GetValueOrDefault(0) }
                       join ct in db.ChargeTypes on new { ChargeTypecode = sew.ChargeTypecode, BranchID = sew.BranchID } equals new { ChargeTypecode = ct.ChargeTypecode, BranchID = ct.BranchID }
                       into gct from subct in gct.DefaultIfEmpty()
                                    where sew.Repair==Repair && sew.ServiceJobID == ID && sew.BranchID == BranchID
                       orderby eng.EngineerName
                       select new ServiceWorkTime { EndTime = sew.EndTime, StartTime = sew.StartTime, Engineer = eng.EngineerName, ChargeType = subct.ChargeType1 + " - " + ((subct.ChargeRate == null) ? "$0.00" : ("$"+subct.ChargeRate.ToString())) }
                       ).ToList();


            ret.Engineers = ret.ServiceWorkTimes.GroupBy(i=>i.Engineer).Select(i => i.Key).ToList();
            //ret.Engineers =  ( from engg in db.Engineers where engg.EngineerID== LoginController.EngineerID(User.Identity.Name) && engg.BranchID==BranchID select engg.EngineerName ).ToList();
            return View(ret);
        }
        public ActionResult ServiceTimes(int ServiceJobID, int BranchID,bool? Repair)
        {
            if (Repair == null)
                Repair = false;
            Utility.Audit(HttpContext.User.Identity.Name, "TechServiceTimes", ServiceJobID, Request);

            TrackerDataContext db = new TrackerDataContext();
            var ret = (from sew in db.ServiceEngineerWorks
                           //join eng in db.Engineers on new { EngineerID = sew.EngineerID, BranchID = sew.BranchID } equals new { EngineerID = eng.EngineerID, BranchID = eng.BranchID.GetValueOrDefault(0) }
                       join ct in db.ChargeTypes on new { ChargeTypecode = sew.ChargeTypecode, BranchID = sew.BranchID } equals new { ChargeTypecode = ct.ChargeTypecode, BranchID = ct.BranchID }
                       into gct from subct in gct.DefaultIfEmpty()
                       where sew.Repair==Repair && sew.ServiceJobID == ServiceJobID && sew.BranchID == BranchID
                       && sew.EngineerID == LoginController.EngineerID(User.Identity.Name)

                       select new ServiceWorkTime {EndTime=sew.EndTime,StartTime=sew.StartTime,Engineer="", ChargeType= ((subct==null)?"NoChargeCode":subct.ChargeType1 + " - " + ((subct.ChargeRate==null)?"$0.00": subct.ChargeRate.ToString())) }
                       ).ToList();
            return View(ret);
        }

        public ActionResult ServiceEngLock(int ServiceID, int BranchID,int Index)
        {
            ServiceEngLock ret = new ServiceEngLock();
            TrackerDataContext db = new TrackerDataContext();
            DB.Engineer englock=null;
            try
            {
                englock = (from sv in db.Services join eng in db.Engineers on sv.EngineerID equals eng.EngineerID where sv.ServiceUID == ServiceID && sv.BranchID == BranchID select eng).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ;
            }
            if (englock != null)
            {
                ret.Engineer = englock.EngineerName;
                ret.EngineerID = englock.EngineerID;
            }
            else
                ret.EngineerID = -1;

            ret.ServiceID = ServiceID;
            ret.BranchID = BranchID;
            ret.Index = Index;
            return View(ret);
        }


        public ActionResult ServiceParts(int ServiceID, int BranchID,bool Readonly)
        {
            ViewBag.Readonly = Readonly;
            Models.ServiceParts ret = new ServiceParts();
            TrackerDataContext db = new TrackerDataContext();
            ret.ServicePartsList = db.ServiceParts.Where(p => p.RepairOrServiceUID == ServiceID && p.BranchID == BranchID).ToList();
            ret.ServiceID = ServiceID;
            ret.BranchID = BranchID;
            return View(ret);
        }

        public ActionResult Repair(int id, int? BranchID)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "TechMaps", id, Request);

            var data = HttpContext.User.Identity.Name;
            ATC atc = MDS.Controllers.LoginController.IsAdmin(data);
            if (atc != ATC.Tech)
                return RedirectToAction("Edit", "Repair", new { id = id, BranchID = BranchID });
            TrackerDataContext db = new TrackerDataContext();

            RepairEquip model = new RepairEquip();
            model.Repair = db.Repairs.Where(rp => rp.RepairUID == id && rp.BranchID == BranchID).FirstOrDefault();
            model.Equipment = db.Equipments.Where(rp => rp.EquipUID == model.Repair.EquipUID && rp.BranchID == BranchID).FirstOrDefault();
            ViewBag.ConditionIDX = db.Conditions.ToList();
            ViewBag.EngineerIDX = db.Engineers.Where(ee => ee.BranchID == BranchID).ToList();
            if (model.Equipment != null)
            {
                if (model.Equipment.ModelUID != null)
                    model.EquipTypeCode = db.EquipModels.Where(em => em.ModelUID == model.Equipment.ModelUID && em.BranchID == model.Equipment.BranchID).FirstOrDefault().EquipTypeCode;
                else
                    model.EquipTypeCode = db.EquipModels.Where(em => /*em.ModelUID == se.Equipment.ModelUID &&*/ em.BranchID == model.Equipment.BranchID).FirstOrDefault().EquipTypeCode;
            model.MakeModels = (from md in db.EquipModels
                             where md.BranchID == BranchID && md.EquipTypeCode == model.EquipTypeCode
                             select new IdText { Id = md.ModelUID, Text = md.Model + " " + md.Manufacturer }).ToList();//.Take(100);

            ViewBag.LocationIDX = db.CustomerLocations.Where(cl => cl.CustomerCode == model.Equipment.CustomerCode && cl.BranchID == BranchID).ToList();
            }
            ViewBag.TravelTypecodeX = (from ct in db.TravelTypes where ct.BranchID==BranchID select new ChargeTypeList { ChargeTypeCode = ct.TravelTypeCode, ChargeType = ct.TravelType1 + " - $" + ct.TravelChargeRate.ToString() }).ToList();
            if (model.Repair.DateIn != null)
                model.Repair.TimeIn = AddDateTime(model.Repair.DateIn, model.Repair.TimeIn);
            if (model.Repair.DateOut != null)
                model.Repair.TimeOut = AddDateTime(model.Repair.DateOut, model.Repair.TimeOut);
            if (model.Repair.ResultedInRetirement)
                model.Status = "Retired";
            else if (model.Repair.RepairCompleted)
                model.Status = "Complete";
            else if (model.Repair.NotRepaired)
                model.Status = "NotRepaired";
            else
                model.Status = "Ongoing";
            //if (model.Repair.TechConfirmedStartTime == true)
            //    ;
            //else
            //    model.Repair.TimeIn = null;
            //if (model.Repair.TechConfirmedEndTime== true)
            //    ;
            //else
            //    model.Repair.TimeOut = null;

            bool JobCompleteSigatureDone = GetJobCompleteSigatureDoneRepair(model, db, LoginController.EngineerID(User.Identity.Name));
            if (!JobCompleteSigatureDone)//active job
            {
                if (model.Repair.EngineerID.Value!=MDS.Controllers.LoginController.EngineerID(User.Identity.Name))
                    model.TechClockOnPrompt = TechClockOnOffStatus.NoClock_JobCompleted;
                else
                {
                    var openSEW = db.ServiceEngineerWorks.Where(ii => ii.Repair==true && ii.ServiceJobID == id && ii.BranchID == BranchID && ii.EngineerID == LoginController.EngineerID(User.Identity.Name) && (ii.EndTime == null) && (ii.StartTime != null)).ToList();
                    if (openSEW.Count() == 0)
                        model.TechClockOnPrompt = TechClockOnOffStatus.ClockOn_NoActiveShifts;
                    else
                        model.TechClockOnPrompt = TechClockOnOffStatus.NoClockOn_ActiveShiftExists;
                }
            }
            else
            {
                model.TechClockOnPrompt = TechClockOnOffStatus.NoClock_JobCompleted;
            }

            return View(model);
        }

        public static string StatusColour(string Status)
        {
            if (Status == "Ongoing")
                return "Red";
            return "Black";
        }
        [HttpPost]
        public ActionResult Repair(RepairEquip model)
        {
            TrackerDataContext db = new TrackerDataContext();

            var moddb = db.Repairs.Where(rp => rp.RepairUID == model.Repair.RepairUID && rp.BranchID == model.Repair.BranchID).FirstOrDefault();
            moddb.TechConfirmedEquipDetails = model.Repair.TechConfirmedEquipDetails;
            //if (model.Repair.TimeIn != null)
            //{
            //    moddb.TimeIn = model.Repair.TimeIn;
            //    moddb.DateIn = model.Repair.TimeIn;
            //    moddb.TechConfirmedStartTime=true;
            //}
            //if (model.Repair.TimeOut != null)
            //{
            //    moddb.TimeOut = model.Repair.TimeOut;
            //    moddb.DateOut = model.Repair.TimeOut;
            //    moddb.TechConfirmedEndTime = true;
            //}
            moddb.Notes = model.Repair.Notes;
            moddb.TimeOut = model.Repair.TimeOut;
            moddb.DateOut = model.Repair.TimeOut;
            moddb.TravelChargeRate = model.Repair.TravelChargeRate;
            moddb.SafetyTestDone = model.Repair.SafetyTestDone;
            moddb.EngineerID = model.Repair.EngineerID;
            moddb.EquipUID = model.Repair.EquipUID;

            moddb.FaultDetails = model.Repair.FaultDetails;
            moddb.WorkDone = model.Repair.WorkDone;
            moddb.ConditionID = model.Repair.ConditionID;

            moddb.TravelHours = model.Repair.TravelHours;
            moddb.CustomerSignature = model.Repair.CustomerSignature;
            moddb.TravelChargeRate = model.Repair.TravelChargeRate;
            moddb.TravelHours = model.Repair.TravelHours;
            moddb.TravelTypecode = model.Repair.TravelTypecode;
            moddb.Notes = model.Repair.Notes;


            if (model.Status == "Retired")
            {
                moddb.ResultedInRetirement = true;
                moddb.RepairCompleted = false;
                moddb.NotRepaired = false;
            }
            else if (model.Status == "Complete")
            {
                moddb.ResultedInRetirement = false;
                moddb.RepairCompleted = true;
                moddb.NotRepaired = false;
            }
            else if (model.Status == "NotRepaired")
            {
                moddb.ResultedInRetirement = false;
                moddb.RepairCompleted = false;
                moddb.NotRepaired = true;
            }
            else
            {
                moddb.ResultedInRetirement = false;
                moddb.RepairCompleted = false;
                moddb.NotRepaired = false;
            }

            db.SubmitChanges();

            var equipdb = db.Equipments.Where(rp => rp.EquipUID == model.Repair.EquipUID && rp.BranchID == model.Repair.BranchID).FirstOrDefault();
            equipdb.LocationID = model.Equipment.LocationID;
            equipdb.SerialNumber= model.Equipment.SerialNumber;
            equipdb.BNQItemCode = model.Equipment.BNQItemCode;
            equipdb.ModelUID = model.Equipment.ModelUID;
            db.SubmitChanges();
            if ((model.Repair.CustomerSignature) && (model.Repair.TechConfirmedEquipDetails) /*&& (model.Repair.TimeOut!=null)*/ && ((model.Status.ToLower()!="ongoing")))
            {
                string domainName =System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string link = Url.Content("~/Repair/Edit?id=" + model.Repair.RepairUID.ToString()+"&BranchID="+model.Repair.BranchID.ToString());
                string Engname = "";
                if (model.Repair.EngineerID != null)
                {
                    var eng = db.Engineers.Where(i => i.EngineerID == model.Repair.EngineerID).FirstOrDefault();
                    if (eng != null)
                        Engname = eng.EngineerName;
                }
                EmailAdminTechTask(Convert.ToInt32( model.Repair.EngineerID), "Repair Job " + Utility.GetBranchCode() + "R" + model.Repair.JobCode.ToString() + " has been completed by Technician " + Engname + " and ready for admin to review and post", domainName+link);
                return RedirectToAction("Dashboard");
                // return RedirectToAction("Repair", new { id = model.Repair.RepairUID, BranchID = model.Repair.BranchID });
//                return RedirectToAction("RepairSearch", new { BranchID = model.Repair.BranchID, EngineerID = model.Repair.EngineerID, Resolved = "N" });
            }
            if (model.Command=="Dashboard")
            {
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Repair", new { id = model.Repair.RepairUID, BranchID = model.Repair.BranchID });
        }

        void EmailAdminTechTask(int EngineerID,string Task, string link)
        {
            try
            {
                TrackerDataContext db = new TrackerDataContext();

                SmtpClient client = new SmtpClient("smtp.gmail.com");

                client.Credentials = new System.Net.NetworkCredential("tim.hams@gmail.com", "#Ben123!@#");
                client.Port = 587;
                client.EnableSsl = true;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("noreply@mds.com");
                string recipients;
                if (EngineerID == -1)
                    recipients = "timtam54@hotmail.com";
                else
                    recipients = db.Engineers.Where(ul => ul.EngineerID == EngineerID).FirstOrDefault().AdminEmail;

                char[] chr = new char[] { Char.Parse(";") };
                string[] recipientArray = recipients.Split(chr, StringSplitOptions.RemoveEmptyEntries);
                foreach (string recipient in recipientArray)
                {

                mailMessage.To.Add(recipient);
                }
                mailMessage.Subject = Task;
                mailMessage.Body = Task+ " " + link;

                client.Send(mailMessage);
                

            }
            catch (Exception ex)
            {
                ;
            }
        }

        [HttpPost]
        public ActionResult Service(ServiceJobService model)
        {
            TrackerDataContext db = new TrackerDataContext();

            var moddb = db.ServiceJobs.Where(rp => rp.ServiceJobUID == model.ServiceJob.ServiceJobUID && rp.BranchID == model.ServiceJob.BranchID).FirstOrDefault();

            if (model.Services != null)
            {
                foreach (var item in model.Services)
                {

                    Service sv = db.Services.Where(ss => ss.ServiceUID == item.Service.ServiceUID && ss.BranchID == item.Service.BranchID).FirstOrDefault();


                    if (sv.EngineerID == LoginController.EngineerID(User.Identity.Name))
                    {
                        if (item.Service.SafetyTestDone)
                        {
                            if (item.ServiceFunction)
                                sv.ServiceFunctionCode = "SER";
                            else
                                sv.ServiceFunctionCode = null;

                            sv.DateServiced = item.DateServiced;
                            sv.ConditionID = item.Service.ConditionID;
                            sv.Servicable = item.Service.Servicable;
                            sv.TSPassFail = item.Service.TSPassFail;
                            sv.DataEntryComplete = item.Service.DataEntryComplete;
                            sv.Notes = item.Service.Notes;
                            sv.WorkDone = item.Service.WorkDone;
                            sv.SafetyTestDone = true;
                            //if (item.ItemNotSeen == false)
                            //    sv.ServiceFunctionCode = "SER";
                            //else
                            //    sv.ServiceFunctionCode = null;

                            sv.EquipUID = item.Service.EquipUID;
                            db.SubmitChanges();
                            //Equipment eq = db.Equipments.Where(ss => ss.EquipUID == item.Service.EquipUID && ss.BranchID == item.Equipment.BranchID).FirstOrDefault();
                           // eq.SerialNumber = item.Equipment.SerialNumber;
                            //eq.BNQItemCode = item.Equipment.BNQItemCode;
                            //eq.ModelUID = item.Equipment.ModelUID;
                        }
                        else
                            sv.SafetyTestDone = false;
                        db.SubmitChanges();

                    }

                }
            }
            //todothams
            //moddb.ActualDateStart = model.ServiceJob.ActualDateStart;
            //moddb.ActualDateEnd = model.ServiceJob.ActualDateEnd;
            //todo thams
            //thamstodo moddb.EngineerID = model.ServiceJob.EngineerID;
            moddb.TravelChargeRate = model.ServiceJob.TravelChargeRate;
            moddb.TravelHours = model.ServiceJob.TravelHours;
            moddb.TravelTypecode = model.ServiceJob.TravelTypecode;
            moddb.CustomerSignature = model.ServiceJob.CustomerSignature;

            /*moddb.Notes = model.Repair.Notes;
            moddb.SafetyTestDone = model.Repair.SafetyTestDone;
            moddb.EquipUID = model.Repair.EquipUID;
            moddb.FaultDetails = model.Repair.FaultDetails;
            moddb.WorkDone = model.Repair.WorkDone;
            moddb.ConditionID = model.Repair.ConditionID;
            */
            moddb.ServiceNotes = model.ServiceJob.ServiceNotes;
            
            if (model.Status == "CompletedBER")
            {
                moddb.CompletedBERs = true;
                moddb.CompletedOK = false;
                moddb.CompletedOutstanding = false;
            }
            else if (model.Status == "CompletedOK")
            {
                moddb.CompletedBERs = false;
                moddb.CompletedOK = true;
                moddb.CompletedOutstanding = false;
            }
            else if (model.Status == "OutstandingRepairs")
            {
                moddb.CompletedBERs = false;
                moddb.CompletedOK = false;
                moddb.CompletedOutstanding = true;
            }
            else
            {
                moddb.CompletedBERs = false;
                moddb.CompletedOK = false;
                moddb.CompletedOutstanding = false;
            }

            db.SubmitChanges();

            //var equipdb = db.Equipments.Where(rp => rp.EquipUID == model.Repair.EquipUID && rp.BranchID == model.Repair.BranchID).FirstOrDefault();
            //equipdb.LocationID = model.Equipment.LocationID;
            //db.SubmitChanges();
            if ((model.ServiceJob.CustomerSignature)  && (model.Status.ToLower() != "ongoing"))//not active time
            {
                //should check out clock off


                string domainName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string link = Url.Content("~/Service/Edit?id=" + model.ServiceJob.ServiceJobUID.ToString()+"&BranchID="+model.ServiceJob.BranchID.ToString());
                foreach (var item in model.Engineers)
                {
                    try
                    {

                        if (item == null)
                        {
                            string task = "Service Job " + Utility.GetBranchCode() + "S" + model.ServiceJob.JobCode.ToString() + " has been completed by Technician and ready for admin to review and post";

                            EmailAdminTechTask(-1, task, domainName + link);
                        }
                        else
                        {
                            string task = "Service Job " + Utility.GetBranchCode() + "S" + model.ServiceJob.JobCode.ToString() + " has been completed by Technician " + db.Engineers.Where(i=>i.EngineerID== item.EngineerID).FirstOrDefault().EngineerName + " and ready for admin to review and post";

                            EmailAdminTechTask(Convert.ToInt32(item.EngineerID), task, domainName + link);
                        }
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }
                return RedirectToAction("Dashboard");
//                return RedirectToAction("ServiceSearch", new { BranchID = model.ServiceJob.BranchID, EngineerID = model.ServiceJob.EngineerID, Resolved = "N" });
            }
            if (model.Command == "Dashboard")
            {
                return RedirectToAction("Dashboard");

            }

            return RedirectToAction("Service", new { ServiceJobUID = model.ServiceJob.ServiceJobUID, BranchID = model.ServiceJob.BranchID });
        }

//        public AddRepaire GetEditRepair(Int32 id, int branchid)
//        {
//            TrackerDataContext db = new TrackerDataContext();
//            db.CommandTimeout = 90;
//            var model = new AddRepaire();
//            var RepairData = db.Repairs.Where(i => i.RepairUID == id && i.BranchID == branchid).ToList();
//            model.Parts = Utility.GetPartList(id, branchid);
//            //          Session["Partused"] = model.Parts;
//            if (RepairData[0].DateCallReceived.HasValue)
//            {
//                model.DateInitalCall = RepairData[0].DateCallReceived.Value;
//            }

//            model.RepairId = id;
//            model.JobCode = Utility.GetBranchCode() + "R" + RepairData[0].JobCode;
//            var equip = db.Equipments.Where(eq => eq.EquipUID == RepairData[0].EquipUID).FirstOrDefault();
//            model.EquipItem = equip.BNQItemCode;
//            return model;
    
////            Session["NewRepairId"] = id;
//            model.EquipId = RepairData[0].EquipUID.Value;
//            model.SelectedEquipment = RepairData[0].EquipUID.Value.ToString();
//            if (RepairData[0].TimeCallReceived.HasValue)
//            {
//                model.TimeInitalCall = RepairData[0].TimeCallReceived.Value;
//            }
//            if (RepairData[0].DateIn.HasValue)
//            {
//                model.DateEquipRepair = RepairData[0].DateIn.Value;
//            }
//            if (RepairData[0].TimeIn.HasValue)
//            {
//                model.TimeEquipRepair = RepairData[0].TimeIn.Value;
//            }

//            model.HasItem = RepairData[0].HandoverCompleted;
//            model.Approve = RepairData[0].ApprovalRequired;
//            model.ApprovalContact = RepairData[0].ApprovalContact;
//            model.orderNumber = RepairData[0].OrderNumberRequired;
//            model.Accessories = RepairData[0].Accessories;
//            model.VerbalApproval = RepairData[0].VerbalApprovalObtained;
//            model.ApprovalReceived = RepairData[0].ApprovedBy;
//            model.OrderNo = RepairData[0].OrderNumber;
//            model.FaultDetail = RepairData[0].FaultDetails;
//            model.WorkDone = RepairData[0].WorkDone;
//            model.LabourHours = RepairData[0].RepairHours;
//            model.TravelHours = RepairData[0].TravelHours;
//            model.SafetyTestDone = RepairData[0].SafetyTestDone;
//            model.Charge = RepairData[0].Charge;
//            model.ChargePartsOnly = RepairData[0].ChargePartsOnly;
//            model.NoCharge = RepairData[0].NoCharge;
//            model.PromotionalExpenses = RepairData[0].ExpensesProportion;
//            model.AllSpecified = RepairData[0].RepairCompleted;
//            model.ResultRepair = RepairData[0].ResultedInRetirement;
//            model.ItemRepair = RepairData[0].NotRepaired;
//            model.HasJob = RepairData[0].HasBeenInvoiced;
//            model.Notes = RepairData[0].Notes;

//            model.PersonName = RepairData[0].ReceiverName;
//            model.TravelTypeCode = RepairData[0].TravelTypecode;
//            model.ChargeTypeCode = RepairData[0].ChargeTypecode;
//            model.TravelRate = RepairData[0].TravelChargeRate.HasValue ? RepairData[0].TravelChargeRate.Value.ToString("C") : "$0.00";
//            model.LabourRate = RepairData[0].ChargeRate.HasValue ? RepairData[0].ChargeRate.Value.ToString("C") : "$0.00";


//            model.Invoice = RepairData[0].InvoiceNumber;
//            if (RepairData[0].InvoiceDate.HasValue)
//            {
//                model.InvoiceDate = RepairData[0].InvoiceDate.Value;
//            }
//            model.EarthResistant = RepairData[0].EarthResistance.HasValue ? RepairData[0].EarthResistance.Value : 0.00M;
//            model.Insulation = RepairData[0].InsulationResistance.HasValue ? RepairData[0].InsulationResistance.Value : 0.00M;
//            model.LeakageCurrent = RepairData[0].LeakageCurrent.HasValue ? RepairData[0].LeakageCurrent.Value : 0.00M;

//            model.ChargesInvoice = RepairData[0].Amount;
//            model.AcctuallyWorkDone = RepairData[0].WorkDone;


//            if (RepairData[0].DateRepairFinished.HasValue)
//            {
//                model.RepairDate = RepairData[0].DateRepairFinished.Value;
//            }
//            if (RepairData[0].DateOut.HasValue)
//            {
//                model.JobCompletionDate = RepairData[0].DateOut;
//            }
//            if (RepairData[0].TimeOut.HasValue)
//            {
//                model.JobCompletionTime = RepairData[0].TimeOut;
//            }
//            if (RepairData[0].ReceiptDate.HasValue)
//            {
//                model.ReceiptDate = RepairData[0].ReceiptDate;
//            }
//            model.PersonName = RepairData[0].ReceiverName;
//            model.RetirementReportPrinted = RepairData[0].RetirementReportPrinted;
//            model.InvoiceThruServiceJob = RepairData[0].InvoiceThruServiceJob;
//            model.EngineerID = RepairData[0].EngineerID.HasValue ? RepairData[0].EngineerID.Value.ToString() : "";
//            model.ConditionID = RepairData[0].ConditionID.HasValue ? RepairData[0].EngineerID.Value.ToString() : "";
//            if (RepairData[0].ServiceJobUID != null)
//            {
//                model.AssociateServiceJob = db.ServiceJobCode(RepairData[0].ServiceJobUID, LoginController.BranchID(HttpContext.User.Identity.Name));
//            }
//            else
//            {
//                model.AssociateServiceJob = "";
//            }
//            model.ServiceJobID = RepairData[0].ServiceJobUID.HasValue ? RepairData[0].ServiceJobUID.Value : 0;
//            model.TechnicalContact = RepairData[0].TechContact;
//            db.Dispose();
//            return model;
//        }


        public static string Colour(int index)
        {
            if (index == 1)
                return "White";
            return "LightGray";
        }
        public static string FormatDate(DateTime? dt)
        {
            if (dt == null)
                return "";
            return Convert.ToDateTime(dt).ToString("dd/MM/yyyy");
        }

        public static string FormatCurrency(decimal? dt)
        {
            if (dt == null)
                return "-";
            return Convert.ToDecimal(dt).ToString("c");
        }

        
        [HttpPost]
        public ActionResult RepairSearch(RepairSearch model)
        {
            model.SelCnt = 100;
            Utility.Audit(HttpContext.User.Identity.Name, "Search Repair Submit", 0, Request);
            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            model.Branchid = LoginController.BranchID(HttpContext.User.Identity.Name);
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
            if (model.Equiptype == null)
            {
                model.Equiptype = "";
            }
            if (model.FromDate.ToShortDateString() == "1/01/0001")
            {
                model.FromDate = DateTime.Today;
            }
            if (model.ToDate.ToShortDateString() == "1/01/0001")
            {
                model.ToDate = DateTime.Today;
            }
            if (model.OutFromDate.ToShortDateString() == "1/01/0001")
            {
                model.OutFromDate = DateTime.Today;
            }
            if (model.OutToDate.ToShortDateString() == "1/01/0001")
            {
                model.OutToDate = DateTime.Today;
            }
            if (model.EngineerID == null)
            {
                model.EngineerID = "-1";
            }
            if (model.Department == null)
            {
                model.Department = "";
            }
            if (model.Locationid == null)
            {
                model.Locationid = "";
            }
            if (model.CustomerOrderNo == null)
            {
                model.CustomerOrderNo = "";
            }
            if (model.RepairJob == null)
            {
                model.RepairJob = "";
            }

            if (model.ServiceJobID == null)
            {
                model.ServiceJobID = "-1";
            }
            if (model.SelectedEquipment == null)
            {
                model.SelectedEquipment = "-1";
            }

            if (model.Resolved == null)
            {
                model.Resolved = "";
            }
            if (model.Complete == null)
            {
                model.Complete = "";
            }

            if (model.Customerid != null)
            {
                string Customer = model.Customerid;
                string Equip = model.Equiptype;
                var FromDate = model.FromDate;
                var ToDate = model.ToDate;
                var Location = model.Locationid;
                var OutFromDate = model.OutFromDate;
                var OutToDate = model.OutToDate;
                var Customersite = model.Department;
                var Engineer = Convert.ToInt32(model.EngineerID);
                var Branch = model.Branchid;
                var CustomerOrderNo = model.CustomerOrderNo;
                var RepairJob = model.RepairJob;
                var Repair = model.Resolved;
                var Handover = model.Complete;
                var ServiceJobID = Convert.ToInt32(model.ServiceJobID);
                var EquipID = Convert.ToInt32(model.SelectedEquipment);
                Session["R_Customer"] = Customer;
                Session["R_CustSite"] = Customersite;
                Session["R_Equip"] = Equip;
                Session["R_Engineer"] = Engineer;
                Session["R_Location"] = Location;
                Session["R_WorkOrderResolved"] = model.Resolved;
                Session["R_WorkOrderComplete"] = model.Complete;
                TrackerDataContext db = new TrackerDataContext();
                db.CommandTimeout = 120;
                if (Location == "--Location--")
                    Location = "";
                if (Customersite == "--Select Customer Site--")
                    Customersite = "";
                var s = db.RepairsSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), Customer, Equip,model.Branchid, Convert.ToChar(Repair), Convert.ToChar(Handover), ServiceJobID, EquipID, Customersite, Location, Engineer, CustomerOrderNo, RepairJob, OutFromDate, OutToDate, model.DateInFilter, model.DateOutFilter, model.SelCnt).Select(i => new RepairSearchList
                {
                    Customer = i.Customer,
                    RepairUID = i.RepairUID,
                    DateIn = i.DateIn.HasValue ? i.DateIn.Value : (DateTime?)null,
                    DateOut = i.DateOut.HasValue ? i.DateOut.Value : (DateTime?)null,
                    Equipment = i.EquipmentType,
                    OrderNumber = i.OrderNumber,
                    JobCode = i.JObCode,
                    Engineer = i.EngineerName,
                    RepairCompleted = i.RepairCompleted,
                    TotalCharge = Convert.ToDecimal(i.RepairTravelExpenseCost) + i.PartsCost,
                    FaultDetails = i.FaultDetails,
                    WorkDone = i.WorkDone,
                    RepairTravelExpenseCost = i.RepairTravelExpenseCost,
                    PartsCost = i.PartsCost,
                    WarrantyExpirationDate = i.WarrantyExpirationDate

                }).ToList();
                model.Repairs = s;
            }
            return View(model);
        }

        public ActionResult RepairSearch(int? BranchID,string Customerid,int? EquipID, int? EngineerID,string Resolved)
        {
            var model = new RepairSearch();
            model.CustomerList = Utility.GetCustomerList();
            model.EquipTypeList = Utility.GetEquipTypeList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();
            model.EngineerList = Utility.GetEngineerListByBranchId();
            model.BranchList = Utility.GetBranchList();
            if (BranchID == null)
                model.Branchid = Controllers.LoginController.BranchID(HttpContext.User.Identity.Name);
            else
                model.Branchid = Convert.ToInt32( BranchID);
            model.Cnt = new List<Int32>();
            model.Cnt.Add(100);
            model.Cnt.Add(200);
            model.Cnt.Add(500);
            model.Cnt.Add(1000);
            model.Cnt.Add(2000);
            model.Cnt.Add(5000);
            model.Cnt.Add(10000);
            model.SelCnt = 100;
            TrackerDataContext db = new TrackerDataContext();
            db.CommandTimeout = 120;

            if (EquipID != null)
                model.SelectedEquipment = EquipID.ToString();
            else
                model.SelectedEquipment = "-1";
            if (Session["R_Equip"] != null)
            {
                model.Equiptype = Session["R_Equip"].ToString();
            }
            else
            {
                model.Equiptype = "";
            }
            Session["R_Engineer"] = EngineerID;
            if (Session["R_Engineer"] != null)
            {
                model.EngineerID = Session["R_Engineer"].ToString();
            }
            else
            {
                model.EngineerID = "";
            }
 //           string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            if (Customerid != null)
                Session["R_Customer"] = Customerid;
            //if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            //{
            //    model.Customerid = Cust;
            //}
            //else 
            if (Session["R_Customer"] != null)
            {
                model.Customerid = Session["R_Customer"].ToString();
            }
            else
            {
                model.Customerid = "";
            }
            if (Session["R_CustSite"] != null)
            {
                model.Department = Session["R_CustSite"].ToString();
            }
            else
            {
                model.Department = "";
            }
            if (Session["R_Location"] != null)
            {
                model.Locationid = Session["R_Location"].ToString();
            }
            else
            {
                model.Locationid = "";
            }
            if (Resolved != null)
                Session["R_WorkOrderResolved"] = Resolved;
            if (Session["R_WorkOrderResolved"] != null)
            {
                model.Resolved = Session["R_WorkOrderResolved"].ToString();
            }
            else
            {
                //if (!MDS.Controllers.LoginController.AllCustomers(@User.Identity.Name))
                //    model.Resolved = "R";
                //else
                //    model.Resolved = "N";
                model.Resolved = "N";
            }
            if (Session["R_WorkOrderComplete"] != null)
            {
                model.Complete = Session["R_WorkOrderComplete"].ToString();
            }
            else
            {
                model.Complete = "E";
            }
            int EngID = -1;
            if (model.EngineerID != "")
                EngID = Convert.ToInt32(model.EngineerID);

            if (model.Locationid == "--Location--")
                model.Locationid = "";
            if (model.Department == "--Select Customer Site--")
                model.Department = "";
            var s = db.RepairsSearch(Convert.ToDateTime("2000/1/1"), Convert.ToDateTime("2100/1/1"), model.Customerid, model.Equiptype, LoginController.BranchID(HttpContext.User.Identity.Name), Convert.ToChar(model.Resolved), Convert.ToChar(model.Complete), -1, Convert.ToInt32(model.SelectedEquipment), model.Department, model.Locationid, EngID, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false, 100).Select(i => new RepairSearchList
            {
                Customer = i.Customer,
                RepairUID = i.RepairUID,
                DateIn = i.DateIn.HasValue ? i.DateIn.Value : (DateTime?)null,
                DateOut = i.DateOut.HasValue ? i.DateOut.Value : (DateTime?)null,
                Equipment = i.EquipmentType,
                OrderNumber = i.OrderNumber,
                JobCode = i.JObCode,
                Engineer = i.EngineerName,
                RepairCompleted = i.RepairCompleted,
                TotalCharge = Convert.ToDecimal(i.RepairTravelExpenseCost) + i.PartsCost,
                FaultDetails = i.FaultDetails,
                WorkDone = i.WorkDone,
                RepairTravelExpenseCost = i.RepairTravelExpenseCost,
                PartsCost = i.PartsCost,
                WarrantyExpirationDate = i.WarrantyExpirationDate

            }).ToList();
            model.Repairs = s;
            return View(model);
        }

        public ActionResult DashBoard()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "TechDashboard", 0, Request);

            TrackerDataContext db = new TrackerDataContext();
            string prefix=Utility.GetBranchCode();
            int EngineerID = LoginController.EngineerID(User.Identity.Name);
            TechDashboard td = new TechDashboard();
            //td.RepairSummary = (from rp in db.Repairs join eq in db.Equipments on new IDBranch {ID= Convert.ToInt32(rp.EquipUID), BranchID=rp.BranchID } equals new IDBranch {ID= eq.EquipUID, BranchID= eq.BranchID }
            //                         join cu in db.Customers on new { eq.CustomerCode, eq.BranchID } equals new {cu.CustomerCode, cu.BranchID }
            //                         where rp.EngineerID == EngineerID
            //                        && ((rp.ResultedInRetirement==false && rp.RepairCompleted == false && rp.NotRepaired == false)
            //                        || (!rp.CustomerSignature))
            //                        group rp by new { ScheduledDate=rp.DateIn.GetValueOrDefault(DateTime.Now.Date).Date, CompanyName = cu.CompanyName, CustomerID = cu.CustomerCode,BranchID=rp.BranchID } into g
            //                    select new CustomerCount { ScheduledDate=g.Key.ScheduledDate, FirstID = g.First().RepairUID, EngineerID = EngineerID, BranchID=g.Key.BranchID,  Customer=g.Key.CompanyName, CustomerID = g.Key.CustomerID, Cnt=g.Count() }).ToList();

            td.RepairSummary = (from rp in db.Repairs
                                join eq in db.Equipments on new IDBranch { ID = Convert.ToInt32(rp.EquipUID), BranchID = rp.BranchID } equals new IDBranch { ID = eq.EquipUID, BranchID = eq.BranchID }
                                join cu in db.Customers on new { eq.CustomerCode, eq.BranchID } equals new { cu.CustomerCode, cu.BranchID }
                                where rp.EngineerID == EngineerID
                               && ((rp.ResultedInRetirement == false && rp.RepairCompleted == false && rp.NotRepaired == false)
                               //|| (!rp.CustomerSignature)
                               )
                                //group rp by new { ScheduledDate = rp.DateIn.GetValueOrDefault(DateTime.Now.Date).Date, CompanyName = cu.CompanyName, CustomerID = cu.CustomerCode, BranchID = rp.BranchID } into g
                                select new CustomerCount { MapAddress=cu.PhysicalAddress, JobNo= prefix+"R" +rp.JobCode.ToString(), ScheduledDate = ((rp.DateIn==null)?rp.DateCallReceived:rp.DateIn.Value.Date), ID = rp.RepairUID, EngineerID = EngineerID, BranchID = rp.BranchID, Customer = cu.CompanyName, CustomerID = cu.CustomerCode/*, Cnt = g.Count()*/ }).ToList();

            var RepDte = (from rd in td.RepairSummary group rd by rd.ScheduledDate into rdg select rdg.Key).ToList();
            
            foreach (var dte in RepDte)
            {
                var repdte= (from tdr in td.RepairSummary where tdr.ScheduledDate.Value.Date == dte.Value.Date select tdr).ToList();
                string cuarr = "";
                foreach (var rp in repdte)
                {
                    if (cuarr == "")
                        cuarr = rp.CustomerID;
                    else
                        cuarr = cuarr + "," + rp.CustomerID;
                }
                foreach (var rp in repdte)
                {
                    rp.CustArr = cuarr;
                }

            }

            td.ServiceSummary = (from sj in db.ServiceJobs //on new IDBranch { ID = Convert.ToInt32(rp.ServiceJobUID), BranchID = rp.BranchID } equals new IDBranch { ID = Convert.ToInt32(sj.ServiceJobUID), BranchID = sj.BranchID }
                                 //join eq in db.Equipments on new IDBranch { ID = Convert.ToInt32(rp.EquipUID), BranchID = rp.BranchID } equals new IDBranch { ID = eq.EquipUID, BranchID = eq.BranchID }
                                 join cu in db.Customers on new { sj.CustomerCode, sj.BranchID } equals new { cu.CustomerCode, cu.BranchID }
                                 join sje in db.ServiceJobEngineers on new { ServiceJobUID = sj.ServiceJobUID, BranchID = sj.BranchID } equals new { ServiceJobUID = sje.ServiceJobID, BranchID = sje.BranchID }

                                 where sje.EngineerID == EngineerID
                                && (sj.CompletedOK == false && sj.CompletedOutstanding == false && sj.CompletedBERs == false)
                                //|| (!sj.CustomerSignature)
                                 //group sj by new { ScheduledDate = sj.DateProgrammed.GetValueOrDefault(DateTime.Now.Date).Date, CompanyName = cu.CompanyName, CustomerID = cu.CustomerCode, BranchID = sj.BranchID } into g
                                 select new CustomerCount {MapAddress=cu.PhysicalAddress, JobNo = prefix+"S"+ sj.JobCode.ToString(),  ScheduledDate = sj.DateProgrammed, ID = sj.ServiceJobUID, EngineerID = EngineerID, BranchID = sj.BranchID, Customer = cu.CompanyName, CustomerID = cu.CustomerCode  }).ToList();
            foreach (var item in td.ServiceSummary)
            {
                if (item.ScheduledDate == null)
                {
                    var xx = db.ServiceJobs.Where(i => i.BranchID == item.BranchID && i.ServiceJobUID == item.ID).FirstOrDefault();
                    xx.DateProgrammed = DateTime.Now;
                    db.SubmitChanges();

                    item.ScheduledDate = DateTime.Now;
                }
            }

            var SrvDte = (from rd in td.ServiceSummary group rd by rd.ScheduledDate into rdg select rdg.Key).ToList();

            foreach (var dte in SrvDte)
            {

                var repdte = (from tdr in td.ServiceSummary where tdr.ScheduledDate.Value.Date == dte.Value.Date select tdr).ToList();
                string cuarr = "";
                foreach (var rp in repdte)
                {
                    if (cuarr == "")
                        cuarr = rp.CustomerID;
                    else
                        cuarr = cuarr + "," + rp.CustomerID;
                }
                foreach (var rp in repdte)
                {
                    rp.CustArr = cuarr;
                }

            }


            return View(td);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ServiceSearch(ServiceSearch model)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Search ServiceJob Request", 0, Request);

            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerList();
            model.LocationList = Utility.GetLocationList();
            model.CustomerSiteList = Utility.GetCustomerSitesList();

            model.BranchList = Utility.GetBranchList();
            //model.Branchid = model.Branchid;
            int? EquipmentID=null;
            if ((model.SelectedEquipment != "") && (model.SelectedEquipment != null))
                EquipmentID = Convert.ToInt32(model.SelectedEquipment);
            int? BranchID = null;
            if (model.Branchid != 0)
                BranchID = model.Branchid;
            int? EngineerID = null;
            string Brcd=Utility.GetBranchCode();
            if (model.EngineerID != "")
                EngineerID =Convert.ToInt32( model.EngineerID);
            TrackerDataContext db = new TrackerDataContext();
            var Sjwo = (from sj in db.ServiceJobs
                        join sw in db.Services on new { ServiceJobUID = sj.ServiceJobUID, BranchID = sj.BranchID } equals new { ServiceJobUID = Convert.ToInt32(sw.ServiceJobUID.GetValueOrDefault(0)), BranchID = sw.BranchID }
                        join cu in db.Customers on new { CustomerCode = sj.CustomerCode, BranchID = sj.BranchID } equals new { CustomerCode = cu.CustomerCode, BranchID = cu.BranchID }
                        join sje in db.ServiceJobEngineers on new { ServiceJobUID = sj.ServiceJobUID, BranchID = sj.BranchID } equals new { ServiceJobUID = sje.ServiceJobID, BranchID = sje.BranchID }
                        join en in db.Engineers on new { EngineerID = sje.EngineerID, BranchID = sje.BranchID } equals new { EngineerID = en.EngineerID, BranchID = en.BranchID.GetValueOrDefault(0) }
                        join eq in db.Equipments on new { EquipUID = sw.EquipUID.GetValueOrDefault(0), BranchID = sw.BranchID } equals new { EquipUID = eq.EquipUID, BranchID = eq.BranchID }
                        join em in db.EquipModels on new { ModelUID = eq.ModelUID.GetValueOrDefault(0), BranchID = eq.BranchID } equals new { ModelUID = em.ModelUID, BranchID = em.BranchID }
                        join et in db.EquipTypes on new { EquipTypeCode = em.EquipTypeCode, BranchID = em.BranchID } equals new { EquipTypeCode = et.EquipTypeCode, BranchID = et.BranchID }
                        join br in db.Branches on sj.BranchID equals br.BranchID

                        where (sw.BranchID ==  BranchID || BranchID == null) && (sj.CustomerCode == model.Customerid || model.Customerid == null) && (sje.EngineerID == EngineerID || EngineerID == 0)
                        && (sw.EquipUID == EquipmentID || EquipmentID == null)
                        orderby sj.ServiceJobUID descending
                        select new ServicesearchList
                        {
                            Branch=br.BranchName,
                            ServiceJobUID = sj.ServiceJobUID.ToString(),
                            Customer = cu.CompanyName,
                            JobCode = Brcd+ "S" + sj.JobCode.ToString(),
                            DateStart = null,// todothams sj.ActualDateStart.HasValue ? sj.ActualDateStart.Value : (DateTime?)null,
                            DateProgrammed = sj.DateProgrammed.HasValue ? sj.DateProgrammed.Value : (DateTime?)null,
                            EngineerName = en.EngineerName,
                            CustomerSite = "CustomerSite",
                            BranchID = sw.BranchID,
                            EquipmentSerialNumber = eq.SerialNumber,
                            EquipmentModel = em.Model,
                            EquipmentType = et.Name

                        }).Take(100).ToList();
            model.Service = Sjwo;
            return View(model);

            //string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            //model.Cnt = new List<Int32>();
            //model.Cnt.Add(100);
            //model.Cnt.Add(200);
            //model.Cnt.Add(400);
            //model.Cnt.Add(500);
            //model.Cnt.Add(800);
            //model.Cnt.Add(1000);
            //model.Cnt.Add(2000);
            //if ((!Cust.Contains("@")) && (Cust != "Admin") && (Cust != "Tech"))
            //{
            //    Session["S_Cust"] = Cust;
            //    model.Customerid = Cust;
            //    model.ServiceWork = "C";
            //}
            //else
            //{
            //if (model.Customerid == null)
            //    {
            //        model.Customerid = "";
            //    }
            //    if (model.ServiceWork == null)
            //    {
            //        model.ServiceWork = "";
            //    }
            ////}
            //if (model.EngineerID == null)
            //{
            //    model.EngineerID = "-1";
            //}

            //if (model.Department == null)
            //{
            //    model.Department = "";
            //}
            //if (model.Locationid == null)
            //{
            //    model.Locationid = "";
            //}
            //if (model.FromDate.ToShortDateString() == "1/01/0001")
            //{
            //    model.FromDate = new DateTime(2000,1,1);
            //}
            //if (model.ToDate.ToShortDateString() == "1/01/0001")
            //{
            //    model.ToDate = DateTime.Today.AddYears(10);
            //}
            //if (model.OutFromDate.ToShortDateString() == "1/01/0001")
            //{
            //    model.OutFromDate = DateTime.Today;
            //}
            //if (model.OutToDate.ToShortDateString() == "1/01/0001")
            //{
            //    model.OutToDate = DateTime.Today;
            //}
            //if (model.ServiceJob == null)
            //{
            //    model.ServiceJob = "";
            //}
            //if (model.CustomerOrderNo == null)
            //{
            //    model.CustomerOrderNo = "";
            //}

            //if (model.Invoice == null)
            //{
            //    model.Invoice = "";
            //}
            //var Customer = model.Customerid;
            //if (Customer == "-1")
            //    Customer = "";
            //var Engineerid = model.EngineerID;
            //var CustDepartment = model.Department;
            //var FromDate = model.FromDate;
            //var ToDate = model.ToDate;
            //var OutDateFrom = model.OutFromDate;
            //var OutDateIn = model.OutToDate;
            //var Location = model.Locationid;
            //var ServiceJob = model.ServiceJob;

            //var CustomerOrderNo = model.CustomerOrderNo;
            //var ServiceWork = model.ServiceWork;

            //var Invoice = model.Invoice;
            //Session["S_Cust"] = Customer;
            //Session["S_Custdep"] = CustDepartment;
            //Session["S_ServiceWorkComplete"] = model.ServiceWork;
            //TrackerDataContext db = new TrackerDataContext();

            //if (Location == "--Location--")
            //    Location = "";

            //if (CustDepartment == "--Select Customer Site--")
            //    CustDepartment = "";

            ////var servicedata = db.ServiceJobSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), model.Customerid, model.Department, EngineerID, model.Branchid, model.ServiceWork, "E", model.Locationid, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false, false, 100).Select(i => new ServicesearchList

            //var servicedata = db.ServiceJobSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), Customer, CustDepartment, Convert.ToInt16(Engineerid), model.Branchid, ServiceWork, "E", Location, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false, false, 100).Select(i => new ServicesearchList


            ////var servicedata = db.ServiceJobSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), Customer, CustDepartment, Convert.ToInt16(Engineerid), model.Branchid, ServiceWork, Invoice, Location, ServiceJob, CustomerOrderNo, FromDate, ToDate, model.DateInFilter, model.DateOutFilter, false, 100).Select(i => new ServicesearchList
            //{
            //    ServiceJobUID = i.ServiceJobUID.ToString(),
            //    Customer = i.Customer,
            //    JobCode = i.JobCode,
            //    DateStart = i.DateStart.HasValue ? i.DateStart.Value : (DateTime?)null,
            //    DateProgrammed = i.DateProgrammed.HasValue ? i.DateProgrammed.Value : (DateTime?)null,
            //    EngineerName = i.Engineername,
            //    CustomerSite = i.CustomerSite

            //}).ToList();
            //model.Service = servicedata;

            //return View(model);


        }
        [HttpPost]
        public ActionResult EquipmentExpiry(EquipExpiryHeader ret)
        {
            GetResults(ret);
                        ret.ProgrammedDate = DateTime.Now.AddDays(1);
            ret.ProgrammedStartTime = DateTime.Now.Date.AddHours(9);
            ret.ProgrammedEndTime = DateTime.Now.Date.AddHours(11);

            return View(ret);
        }

        public ActionResult EquipmentExpiry()
        {
            EquipExpiryHeader ret = new EquipExpiryHeader();
            ret.FromDate = DateTime.Now.AddDays(-7);
            ret.ToDate = DateTime.Now.AddDays(40);
            ret.ProgrammedDate = DateTime.Now.AddDays(1);
            ret.ProgrammedStartTime = DateTime.Now.Date.AddHours(9);
            ret.ProgrammedEndTime = DateTime.Now.Date.AddHours(11);
            ret.BranchID = Controllers.LoginController.BranchID(HttpContext.User.Identity.Name);

            GetResults(ret);
            return View(ret);
        }

        private static void GetResults(EquipExpiryHeader ret)
        {TrackerDataContext db = new TrackerDataContext();

            var branch = db.Branches.Where(i => i.BranchID == ret.BranchID).FirstOrDefault();

            ret.Results = (from eq in db.Equipments
                           join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                           join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                           join tp in db.EquipTypes on em.EquipTypeCode equals tp.EquipTypeCode
                           where tp.BranchID == ret.BranchID && eq.BranchID == ret.BranchID && em.BranchID == ret.BranchID && cu.BranchID == ret.BranchID
                           && eq.CurrentlyServicedByBNQ == true && eq.InService == true && eq.WarrantyExpirationDate >= ret.FromDate && eq.WarrantyExpirationDate <= ret.ToDate
                           select new EquipExpiry { EquipID = eq.EquipUID, CustomerCode = cu.CustomerCode, Type = tp.Name, Customer = cu.CompanyName, BNQItemCode = eq.BNQItemCode, SerialNumber = eq.SerialNumber, Model = em.Model, WarrantyExpirationDate = Convert.ToDateTime(eq.WarrantyExpirationDate) }
                       ).ToList();
            foreach (EquipExpiry item in ret.Results)
            {
                int EquipmentID = item.EquipID;
                var sjs = (from sv in db.Services
                           join sj in db.ServiceJobs on sv.ServiceJobUID equals sj.ServiceJobUID
                           where sv.EquipUID == EquipmentID
                           orderby sj.DateProgrammed descending
                           select sj).Take(1).FirstOrDefault();
                if (sjs == null)
                {
                    ;
                }
                else
                {
                    item.ServiceJobID = sjs.ServiceJobUID;
                    if ((sjs.CompletedBERs) || (sjs.CompletedOK) || (sjs.CompletedOutstanding))
                        item.ServiceJobComplete = true;
                    else
                        item.ServiceJobComplete = false;
                    item.ServiceJobNo = branch.PrefixCode + "S" + sjs.JobCode;
                    item.ServiceJobProgrammed = sjs.DateProgrammed;
                }
            }
        }

        public ActionResult ServiceSearch(int? BranchID, string Customerid,int? EngineerID,int? EquipID,string Resolved)
        {
            TrackerDataContext db = new TrackerDataContext();
            ServiceSearch model = new ServiceSearch();
            model.CustomerList = Utility.GetCustomerListByBranchId();
            model.EngineerList = Utility.GetEngineerListByBranchId();
         //   model.BranchList = Utility.GetBranchList();
            if (BranchID!=null)
                model.Branchid =Convert.ToInt32( BranchID);// LoginController.BranchID(HttpContext.User.Identity.Name);
            else
                model.Branchid= LoginController.BranchID(HttpContext.User.Identity.Name);
            if (Customerid != null)
                model.Customerid = Customerid;// LoginController.BranchID(HttpContext.User.Identity.Name);
            if (EngineerID != null)
                model.EngineerID = EngineerID.ToString();// LoginController.BranchID(HttpContext.User.Identity.Name);
            if (Resolved != null)
                model.ServiceWork = Resolved;
            if (EquipID != null)
            {
                model.SelectedEquipment = EquipID.ToString();// LoginController.BranchID(HttpContext.User.Identity.Name);
                var equiprow = db.Equipments.Where(i => i.EquipUID == Convert.ToInt32(EquipID) && i.BranchID==BranchID).FirstOrDefault();
                if (equiprow == null)
                    model.EquipmentData = "not found";
                else
                {
                    var em = db.EquipModels.Where(i => i.ModelUID == equiprow.ModelUID && i.BranchID == BranchID).FirstOrDefault();
                    if (em == null)
                        model.EquipmentData = "Serial Number:" + equiprow.SerialNumber;
                    else
                    {
                        var EquipType = db.EquipTypes.Where(i => i.EquipTypeCode == em.EquipTypeCode && i.BranchID == BranchID).FirstOrDefault();
                        if (EquipType == null)
                            model.EquipmentData = em.Model + ", Serial Number:" + equiprow.SerialNumber;
                        else
                            model.EquipmentData = EquipType.Name + ","+ em.Model + ", Serial Number:" + equiprow.SerialNumber;
                    }
                }
            }
            string brcd = Utility.GetBranchCode();
            Utility.Audit(HttpContext.User.Identity.Name, "All ServiceJob Request", 0, Request);
            var Sjwo = (from sj in db.ServiceJobs join sw in db.Services on new { ServiceJobUID=sj.ServiceJobUID, BranchID=sj.BranchID } equals new { ServiceJobUID=Convert.ToInt32( sw.ServiceJobUID.GetValueOrDefault(0)), BranchID=sw.BranchID } 
                        join cu in db.Customers on new { CustomerCode = sj.CustomerCode, BranchID = sj.BranchID } equals new { CustomerCode = cu.CustomerCode, BranchID = cu.BranchID }
                        join sje in db.ServiceJobEngineers on new { ServiceJobUID=sj.ServiceJobUID, BranchID=sj.BranchID } equals new { ServiceJobUID = sje.ServiceJobID, BranchID = sje.BranchID }
                        join en in db.Engineers on new { EngineerID =  sje.EngineerID, BranchID=sje.BranchID } equals new { EngineerID = en.EngineerID, BranchID = en.BranchID.GetValueOrDefault(0) }
                        join eq in db.Equipments on new { EquipUID = sw.EquipUID.GetValueOrDefault(0), BranchID = sw.BranchID } equals new { EquipUID = eq.EquipUID, BranchID = eq.BranchID }
                        join em in db.EquipModels on new { ModelUID = eq.ModelUID.GetValueOrDefault(0), BranchID=eq.BranchID } equals new {ModelUID = em.ModelUID,BranchID = em.BranchID }
                        join et in db.EquipTypes on new { EquipTypeCode = em.EquipTypeCode, BranchID=em.BranchID } equals new {EquipTypeCode = et.EquipTypeCode,BranchID = et.BranchID   }
                        join br in db.Branches on sj.BranchID equals br.BranchID
                        where (sw.BranchID == BranchID || BranchID==null) && (sj.CustomerCode == Customerid || Customerid==null) && (sje.EngineerID == EngineerID || EngineerID == null)
                        && (sw.EquipUID == EquipID || EquipID==null)
                        orderby sj.ServiceJobUID descending

                        select new ServicesearchList { Branch=br.BranchName, ServiceJobUID =sj.ServiceJobUID.ToString(),
                Customer = cu.CompanyName,
                JobCode = brcd+"S"+sj.JobCode.ToString(),
                DateStart =null,//todothams sj.DateProgrammed.HasValue ? sj.DateProgrammed.Value : (DateTime?)null,
                DateProgrammed = sj.DateProgrammed.HasValue ? sj.DateProgrammed.Value : (DateTime?)null,
                EngineerName = en.EngineerName,
                CustomerSite = "CustomerSite",
                BranchID=sw.BranchID,

                EquipmentSerialNumber=eq.SerialNumber,
                            EquipmentModel = em.Model,
                                            EquipmentType = et.Name

                        }).Take(100).ToList();
            model.Service = Sjwo;
            return View(model);

    
            //if (EngineerID ==null)
            //    EngineerID=-1;

            //ServiceSearch model = new ServiceSearch();
            //model.CustomerList = Utility.GetCustomerListByBranchId();
            //model.EngineerList = Utility.GetEngineerListByBranchId();
            //model.BranchList = Utility.GetBranchList();
            //if (BranchID == null)
            //    model.Branchid = Controllers.LoginController.BranchID(HttpContext.User.Identity.Name);
            //else
            //    model.Branchid = Convert.ToInt32(BranchID);


            //model.Invoice = "E";

            ////string Cust = LoginController.AdminTechCustomer(User.Identity.Name);
            //if (Session["S_ServiceWorkComplete"] != null)
            //{
            //    model.ServiceWork = Session["S_ServiceWorkComplete"].ToString();
            //}
            //else
            //{
            //    model.ServiceWork = "I"; //model.ServiceWork = "";
            //}

            //if (Customerid != null)
            //{
            //    Session["S_Cust"] = Customerid;
            //}
            //if (Session["S_Cust"] != null)
            //{
            //    model.Customerid = Session["S_Cust"].ToString();
            //}
            //else
            //{
            //    model.Customerid = "";
            //}

            //if (Session["S_Custdep"] != null)
            //{
            //    model.Department = Session["S_Custdep"].ToString();
            //}
            //else
            //{
            //    model.Department = "";
            //}

            //TrackerDataContext db = new TrackerDataContext();
            //db.CommandTimeout = 90;
            //if (model.Locationid == null)
            //    model.Locationid = "";
            //if (model.Locationid == "--Location--")
            //    model.Locationid = "";
            //if (model.Department == "--Select Customer Site--")
            //    model.Department = "";


            //var servicedata = db.ServiceJobSearch(Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), model.Customerid, model.Department, EngineerID, model.Branchid, model.ServiceWork, "E", model.Locationid, "", "", Convert.ToDateTime("1900/1/1"), Convert.ToDateTime("2100/1/1"), false, false, false, 100).Select(i => new ServicesearchList
            //{
            //    ServiceJobUID = i.ServiceJobUID.ToString(),
            //    Customer = i.Customer,
            //    JobCode = i.JobCode,
            //    DateStart = i.DateStart.HasValue ? i.DateStart.Value : (DateTime?)null,
            //    DateProgrammed = i.DateProgrammed.HasValue ? i.DateProgrammed.Value : (DateTime?)null,
            //    EngineerName = i.Engineername,
            //    CustomerSite = i.CustomerSite
            //}).ToList();
            //model.Service = servicedata;
            //return View(model);
        }
    }
}
 