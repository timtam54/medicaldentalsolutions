using MDS.DB;
using MDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MDS.Controllers
{
    public class MobileController : Controller
    {
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }

        static TrackerDataContext db = new TrackerDataContext();
        
        public static string GetEquipModel(int? ModelID)
        {
            if (ModelID==null)
                return "Unknown";

            var em = db.EquipModels.Where(ii => ii.ModelUID == ModelID).FirstOrDefault();
            if (em == null)
                return "Unknown";
            return em.Model + " "+em.Manufacturer ;
        }

        public static string GetEquipType(int? ModelID)
        {
            if (ModelID == null)
                return "Unknown";

            var em = db.EquipModels.Where(ii => ii.ModelUID == ModelID).FirstOrDefault();
            if (em == null)
                return "Unknown";

            var et = db.EquipTypes.Where(ii => ii.EquipTypeCode == em.EquipTypeCode).FirstOrDefault();
            if (et == null)
                return "Unknown";

            return et.Description + " " + et.Name;

        }

        public static string GetVendor(int? VID)
        {
            if (VID == null)
                return "Not specified";
            if (VID == 0)
                return "Not specified";

            return db.Vendors.Where(ii => ii.VendorUID == VID).FirstOrDefault().CompanyName;
        }

        public static string GetServiceArea(int? SAID)
        {
            if (SAID == null)
                return "Not specified";
            return db.ServiceAreas.Where(ii => ii.ServiceAreaUID == SAID).FirstOrDefault().AreaDescription;
        }
        public static string EngineerName(string MobLogin)
        {
            char cc = Convert.ToChar(",");
            string[] identity = MobLogin.Split(new char[] { cc }, StringSplitOptions.RemoveEmptyEntries);
            var eng= identity[1].ToString();

            int index = eng.IndexOf("@");
            if (index > 0)
                return eng.Substring(0, index);
            return eng;

        }
        public static string EquipmentDesc(int ID)
        {
            var eqdesc = (from eq in db.Equipments
                          join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                          join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                          where eq.EquipUID == ID// (((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber)).Contains(searchString)
                          select ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber)).FirstOrDefault();

            return eqdesc;

        }

        public JsonResult EquipSearch(string searchString)
        {
            //RM 30/11/18 - search was changed to include rank, since it was impossible to just pick 'Acropora GENUS' 
            List<MDS.Models.EqSearch> taxon = (from eq in db.Equipments
                                               join rp in db.Repairs on eq.EquipUID equals rp.EquipUID
                                               join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                               join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                                               where (((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber)).Contains(searchString)
                                               && rp.EngineerID == LoginController.EngineerID(User.Identity.Name)
                                               select new MDS.Models.EqSearch { ID = eq.EquipUID, Desc = ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber), Ord = 1 }
                         ).Take(4).ToList();
            List<MDS.Models.EqSearch> res;
            if (taxon.Count() < 4)
            {
                List<MDS.Models.EqSearch> taxon2 = (from eq in db.Equipments
                                                   join rp in db.Services on eq.EquipUID equals rp.EquipUID
                                                   join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                                   join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                                                   where (((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber)).Contains(searchString)
                                                   && rp.EngineerID == LoginController.EngineerID(User.Identity.Name)
                                                   select new MDS.Models.EqSearch { ID = eq.EquipUID, Desc = ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber), Ord = 1 }
                     ).Take(4 - taxon.Count()).ToList();
                //                         .ToList();
                res = taxon.Union(taxon2).ToList();

            }
            else
                res = taxon;
            //.OrderBy(i => i.Ord).ThenBy(i => i.Desc).Take(4).ToList();
            foreach (var item in res)
            {
                if (item.Desc.Length > 30)
                    item.Desc = item.Desc.Substring(0, 30) + ".";
            }
            return Json(new { results = res });
        }

        public JsonResult Search(string searchString)
        {
            TrackerDataContext dbs = new TrackerDataContext();
            //RM 30/11/18 - search was changed to include rank, since it was impossible to just pick 'Acropora GENUS' 
            List< MDS.Models.EqSearch> taxon = (from eq in dbs.Equipments
                         join rp in dbs.Repairs on eq.EquipUID equals rp.EquipUID
                         join em in dbs.EquipModels on eq.ModelUID equals em.ModelUID
                         join cu in dbs.Customers on eq.CustomerCode equals cu.CustomerCode
                         where (((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber)).Contains(searchString)
                         && rp.EngineerID == LoginController.EngineerID(User.Identity.Name)
                         select new MDS.Models.EqSearch { ID = eq.EquipUID, Desc = ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber), Ord = 1 }
                         ).Take(4).ToList();
            List<MDS.Models.EqSearch> res;
            if (taxon.Count() < 4)
            {
                List<MDS.Models.EqSearch> cu = (from rp in dbs.Repairs
                                                join br in dbs.Branches on rp.BranchID equals br.BranchID
                                                where rp.EngineerID == LoginController.EngineerID(User.Identity.Name)
                                               && (br.PrefixCode + "R" + ((rp.JobCode == null) ? "" : Convert.ToInt32(rp.JobCode).ToString())).Contains(searchString)
                                                select new MDS.Models.EqSearch { ID = rp.RepairUID, Desc = br.PrefixCode + "R" + ((rp.JobCode == null) ? "" : Convert.ToInt32(rp.JobCode).ToString()), Ord = 2 }
                 ).Take(4 - taxon.Count()).ToList();
                res = taxon.Union(cu).ToList();
            }
            else
                res = taxon;
            foreach (var item in res)
            {
                if (item.Desc.Length > 30)
                    item.Desc = item.Desc.Substring(0, 30) + ".";
            }
            return Json(new { results = res });
        }

        [Authorize]
        public ActionResult DashboardServiceJobs()
        {

            List<MobServiceJobs> SWO = (from sj in db.ServiceJobs
                                        join ss in db.Services on sj.ServiceJobUID equals ss.ServiceJobUID  
                                    //join eq in db.Equipments on rp.EquipUID equals eq.EquipUID
                                    //join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                        join cu in db.Customers on sj.CustomerCode equals cu.CustomerCode
                                        join sje in db.ServiceJobEngineers on sj.ServiceJobUID equals sje.ServiceJobID
                                    join br in db.Branches on sj.BranchID equals br.BranchID
                                    where sje.EngineerID == LoginController.EngineerID(User.Identity.Name)
                                    //&& sj.ActualDateStart > DateTime.Now.AddMonths(-12)
                                    orderby sj.DateProgrammed descending
                                    select new MobServiceJobs
                                    {
                                        ServiceJobID = sj.ServiceJobUID,
                                        JobNo = br.PrefixCode + "R" + ((sj.JobCode == null) ? "" : Convert.ToInt32(sj.JobCode).ToString()),
                                     //   Amount = rp.Amount,
                                        Customer = cu.CompanyName,
                                        //EquipmentID = Convert.ToInt32(rp.EquipUID),
                                        DateStart = null,//sj.ActualDateStart
                                        //,Equipment = ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber),
                                        //Fault = rp.FaultDetails
                                    }).ToList();
            MobServiceJob ret = new MobServiceJob();
            ret.sjs = SWO;
            return View(ret);
        }


        [Authorize]
        public ActionResult DashBoardRepairs()
        {

            List<MobRepairs> SWO = (from rp in db.Repairs
                                     join eq in db.Equipments on rp.EquipUID equals eq.EquipUID
                                     join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                   //  join sj in db.ServiceJobs on rp.ServiceJobUID equals sj.ServiceJobUID
                                     join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                                     join br in db.Branches on eq.BranchID equals br.BranchID
                                     where rp.EngineerID == LoginController.EngineerID(User.Identity.Name)  && rp.DateIn > DateTime.Now.AddMonths(-12)
                                     orderby rp.DateIn descending
                                     select new MobRepairs { RepairID=rp.RepairUID,  JobNo= br.PrefixCode+ "R" + ((rp.JobCode==null)?"":Convert.ToInt32(rp.JobCode).ToString() ) ,Amount =rp.Amount, Customer = cu.CompanyName, EquipmentID = Convert.ToInt32(rp.EquipUID), DateServiced = rp.DateIn,
                                         Equipment = ((em.EquipTypeCode==null)?"NA":em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode== null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber), Fault = rp.FaultDetails }).ToList();
            MobRepair ret = new MobRepair();
            ret.reps = SWO;
            return View(ret);
        }
        [HttpPost]
        public ActionResult DashBoardRepairs(MobRepair ret)
        {
            if (ret.Typ == "1")
            {
                List<MobRepairs> SWO = (from rp in db.Repairs
                                        join eq in db.Equipments on rp.EquipUID equals eq.EquipUID
                                        join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                        //  join sj in db.ServiceJobs on rp.ServiceJobUID equals sj.ServiceJobUID
                                        join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                                        join br in db.Branches on eq.BranchID equals br.BranchID
                                        where rp.EngineerID == LoginController.EngineerID(User.Identity.Name)
                                        && eq.EquipUID == Convert.ToInt32(ret.ID)
                                        //&& rp.DateIn > DateTime.Now.AddMonths(-12)
                                        orderby rp.DateIn descending
                                        select new MobRepairs
                                        {
                                            RepairID = rp.RepairUID,
                                            JobNo = br.PrefixCode + "R" + ((rp.JobCode == null) ? "" : Convert.ToInt32(rp.JobCode).ToString()),
                                            Amount = rp.Amount,
                                            Customer = cu.CompanyName,
                                            EquipmentID = Convert.ToInt32(rp.EquipUID),
                                            DateServiced = rp.DateIn,
                                            Equipment = ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber),
                                            Fault = rp.FaultDetails
                                        }).ToList();
                ret.reps = SWO;
                return View(ret);
            }
            if (ret.Typ == "2")
            {
                List<MobRepairs> SWO = (from rp in db.Repairs
                                        join eq in db.Equipments on rp.EquipUID equals eq.EquipUID
                                        join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                                        //  join sj in db.ServiceJobs on rp.ServiceJobUID equals sj.ServiceJobUID
                                        join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
                                        join br in db.Branches on eq.BranchID equals br.BranchID
                                        where rp.EngineerID == LoginController.EngineerID(User.Identity.Name)
                                        && rp.RepairUID == Convert.ToInt32(ret.ID)
                                        //&& rp.DateIn > DateTime.Now.AddMonths(-12)
                                        orderby rp.DateIn descending
                                        select new MobRepairs
                                        {
                                            RepairID = rp.RepairUID,
                                            JobNo = br.PrefixCode + "R" + ((rp.JobCode == null) ? "" : Convert.ToInt32(rp.JobCode).ToString()),
                                            Amount = rp.Amount,
                                            Customer = cu.CompanyName,
                                            EquipmentID = Convert.ToInt32(rp.EquipUID),
                                            DateServiced = rp.DateIn,
                                            Equipment = ((em.EquipTypeCode == null) ? "NA" : em.EquipTypeCode) + ' ' + ((em.Model == null) ? "NA" : em.Model) + ' ' + ((cu.CustomerCode == null) ? "NA" : cu.CustomerCode) + ' ' + ((eq.SerialNumber == null) ? "NA" : eq.SerialNumber),
                                            Fault = rp.FaultDetails
                                        }).ToList();
                ret.reps = SWO;
                return View(ret);
            }
            return View(ret);

        }

        [Authorize]
        public ActionResult Repair(int RepairID)
        {
            MobRep ret = new MobRep();
            ret.rep = db.Repairs.Where(ii => ii.RepairUID == RepairID).FirstOrDefault();
            ret.parts = (from pt in db.Parts join rp in db.RepairParts on pt.ID equals rp.PartID where rp.RepairOrServiceUID == RepairID select pt).ToList();
            return View(ret);
        }

        [Authorize]
        public ActionResult Equipment(int EquipmentID)
        {
            MobEquip ret = GetEquipObj(EquipmentID);
            return View(ret);

        }

        private MobEquip GetEquipObj(int EquipmentID)
        {
            MobEquip ret = new MobEquip();
            ret.Equip = db.Equipments.Where(ii => ii.EquipUID == EquipmentID).FirstOrDefault();
            ret.Repairs = db.Repairs.Where(ii => ii.EquipUID == EquipmentID).ToList();
            ret.BranchPrefix = db.Branches.Where(ii => ii.BranchID == MDS.Controllers.LoginController.BranchID(User.Identity.Name)).FirstOrDefault().PrefixCode;
            ret.Services = (from sj in db.ServiceJobs join ss in db.Services on sj.ServiceJobUID equals ss.ServiceJobUID where ss.EquipUID == EquipmentID select sj).ToList();
            return ret;
        }

        [HttpPost]
        public ActionResult Equipment(MobEquip eqin)
        {
            MobEquip ret = GetEquipObj(eqin.Equip.EquipUID);
            return View(ret);

        }


        [Authorize]
        public ActionResult DashBoard()
        {

            List<MobServices> SWO = (from wo in db.Services join eq in db.Equipments on wo.EquipUID equals eq.EquipUID
                       join em in db.EquipModels on eq.ModelUID equals em.ModelUID
                       join sj in db.ServiceJobs on wo.ServiceJobUID equals sj.ServiceJobUID
                       join cu in db.Customers on sj.CustomerCode equals cu.CustomerCode
                       join br in db.Branches on sj.BranchID equals br.BranchID
                       where wo.EngineerID ==LoginController.EngineerID(User.Identity.Name) && wo.DateServiced>DateTime.Now.AddMonths(-1)
                       orderby wo.DateServiced descending
                       select new MobServices {JobCode=br.PrefixCode+ sj.JobCode.ToString(),Customer=cu.CompanyName, EquipmentID=Convert.ToInt32( wo.EquipUID), DateServiced=wo.DateServiced, EquipmentSerialNo=eq.SerialNumber, EquipmentModel=em.Model,WorkDone=wo.WorkDone }  ).ToList();
        //    MobTechLogin ret = new MobTechLogin();
            //var vtp = db.Engineers.Where(ii => ii.Password != null).ToList();
            //ViewBag.EngineerIDX = vtp;
            return View(SWO);
        }

        //public ActionResult TechLogin()
        //{
        //    MobTechLogin ret = new MobTechLogin();
        //    var vtp = db.Engineers.Where(ii=>ii.Password!=null).ToList();
        //    ViewBag.EngineerIDX = vtp;
        //    return View(ret);
        //}

        //[HttpPost]
        //public ActionResult TechLogin(MobTechLogin ret)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var vtp = db.Engineers.Where(ii => ii.Password != null).ToList();

        //        ViewBag.EngineerIDX = vtp;

        //        return View(ret);
        //    }
        //    //MobTechLogin ret = new MobTechLogin();
        //    var eng= db.Engineers.Where(ii => ii.EngineerID == ret.EngineerID).FirstOrDefault();
        //    if (eng.Password == ret.Password)
        //    {
        //        FormsAuthentication.SetAuthCookie(eng.EngineerID.ToString() + "," + eng.EngineerName, true);
        //        return RedirectToAction("Dashboard");

        //    }
        //    return View(ret);
        //}

    }
}
 