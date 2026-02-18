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
    public class DashboardController : BaseController
    {
        static bool ArrayEqual(List<int> l1, List<int> l2)
        {
            if (l2.Count() != l2.Count())
                return false;
            foreach (int i1 in l1)
            {
                if (!l2.Contains(i1))
                    return false;
            }
            foreach (int i2 in l2)
            {
                if (!l1.Contains(i2))
                    return false;
            }

            return true;
        }
        public class Colours
        {
            public string Fore { get; set; }
            public string Back { get; set; }
        }

        public static Colours ForeBack(Booking rep)
        {
            Colours col = new Colours();
            if (rep.RepairOrService == "Repair")
            {
                if (!rep.Complete)
                {
                    col.Back= "LightGreen";
                    col.Fore = "Black";
                }
                else
                {
                    col.Back = "Silver";
                    col.Fore = "Black";
                }
            }
            else
            {
                if (!rep.Complete)
                {
                    col.Back = "LightBlue";
                    col.Fore = "Black";
                }
                else
                {
                    col.Back = "gainsboro";
                    col.Fore = "Black";
                }
            }
            return col;
        }
        public static string BookingCustomer(Booking rep)
        {
            if (rep.EquipUID != null)
            {
                return MDS.Controllers.TechController.GetCustomer(rep.EquipUID, rep.BranchID);
            }
            else
            {
                return MDS.Controllers.TechController.GetCustomerbyCustCode(rep.CustomerCode, rep.BranchID);
            }
           // return "No Found";
        }
        public static int GetSpan(int Hour, int Minute, int HalfHours, List<int> IDs, List<Booking> Bookings,List<int> Hours,DateTime Date,int EngineerID)
        {
            int Loop = 0;
            for (int hh = Hour; hh < Hours.Max(); hh++)
            {
                if (Minute == 0)
                {
                    
                    var reps = Bookings.Where(rp => /*(rp.ID != ID) && */((rp.ScheduledStart.Value.Hour * 100 + rp.ScheduledStart.Value.Minute) <= (hh * 100 + Minute)) && ((rp.ScheduledEnd.Value.Hour * 100 + rp.ScheduledEnd.Value.Minute) > (hh * 100 + Minute)) && rp.ScheduledStart.Value.Date == Date && rp.EngineerID == EngineerID).ToList();
                    if (ArrayEqual(reps.Select(i=>i.ID).ToList(),IDs.ToList()))
                    //if (reps.Count() == 0)
                    {
                        Loop++;
                    }
                    else
                        return Loop;//clash
                    if (Loop >= HalfHours)
                        return HalfHours;
                    Minute = 30;
                }
                //if min = 30
                {
                    var reps = Bookings.Where(rp => /*(rp.ID != ID) &&*/ ((rp.ScheduledStart.Value.Hour * 100 + rp.ScheduledStart.Value.Minute) <= (hh * 100 + Minute)) && ((rp.ScheduledEnd.Value.Hour * 100 + rp.ScheduledEnd.Value.Minute) > (hh * 100 + Minute)) && rp.ScheduledStart.Value.Date == Date && rp.EngineerID == EngineerID).ToList();
                    //if (reps.Count() == 0)
                    if (ArrayEqual(reps.Select(i => i.ID).ToList(), IDs.ToList()))
                    {
                        Loop++;
                    }
                    else
                        return Loop;//clash
                    if (Loop >= HalfHours)
                        return HalfHours;
                    Minute = 0;
                }
            }
            return Loop;
        }

        [Authorize]
        public ActionResult CustomersUnscheduledServicesDueThisMonth()
        {
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            int Year12Month = DateTime.Now.Year * 12 + DateTime.Now.Month;
            TrackerDataContext db = new TrackerDataContext();

            var x = (from eq in db.Equipments
                     join ns in db.NextServices on new { eq.EquipUID, eq.BranchID } equals new { ns.EquipUID, ns.BranchID }
                     join cu in db.Customers on new { eq.CustomerCode, eq.BranchID } equals new { cu.CustomerCode, cu.BranchID }  // and branch
                     join em in db.EquipModels on new { ModelUID = eq.ModelUID, BranchID = eq.BranchID } equals new { ModelUID = (int?)em.ModelUID, BranchID = em.BranchID } //into ps from emx in ps.DefaultIfEmpty() 
                     join et in db.EquipTypes on new { em.EquipTypeCode, em.BranchID } equals new { et.EquipTypeCode, et.BranchID }
                     where ns.BranchID == BranchID
                    && (ns.NextServiceYear * 12 + ns.NextServiceMonth) == Year12Month
                     && eq.InService == true && eq.CurrentlyServicedByBNQ == true
                     select new EquipmentCustNextServiceList
                     {
                         NextServiceMonth = ns.NextServiceMonth,
                         NextServiceYear = ns.NextServiceYear,
                         EquipID = eq.EquipUID,
                         EquipDesc =    /*et.Name + " " + em.Model+" " + */ /*ISNULL(dbo.CustomerLocation.Location,'') +*/ "  Serial #: " + eq.SerialNumber,
                         Customer = cu.CompanyName,
                         CustomerCode = cu.CustomerCode,
                         WarrantyExpirationDate = eq.WarrantyExpirationDate
                     ,
                         MDSItemNo = eq.BNQItemCode,
                         Model = em.Model,
                         EquipementType = et.Description,
                         ModelUID = em.ModelUID
                     }).ToList();



            //var x = from eq in db.Equipments
            //        join ns in db.NextServices on new { eq.EquipUID, eq.BranchID } equals new { ns.EquipUID, ns.BranchID }
            //        join cu in db.Customers on eq.CustomerCode equals cu.CustomerCode
            //        join em in db.EquipModels on new { ModelUID = eq.ModelUID, BranchID = eq.BranchID } equals new { ModelUID = (int?)em.ModelUID, BranchID = em.BranchID } into ps
            //        from emx in ps.DefaultIfEmpty()
            //        join et in db.EquipTypes on new { emx.EquipTypeCode, emx.BranchID } equals new { et.EquipTypeCode, et.BranchID }
            //        where ns.BranchID == BranchID
            //        && (ns.NextServiceYear * 12 + ns.NextServiceMonth) == Year12Month
            //        && eq.InService == true && eq.CurrentlyServicedByBNQ == true
            //        select new EquipmentCustNextServiceList
            //        {
            //            NextServiceMonth = ns.NextServiceMonth,
            //            NextServiceYear = ns.NextServiceYear,
            //            EquipID = eq.EquipUID,
            //            EquipDesc =	/*et.Name + " " + em.Model+" " + */ /*ISNULL(dbo.CustomerLocation.Location,'') +*/ "  Serial #: " + eq.SerialNumber,
            //            Customer = cu.CompanyName,
            //            CustomerCode = cu.CustomerCode,
            //            WarrantyExpirationDate = eq.WarrantyExpirationDate,
            //            MDSItemNo = eq.BNQItemCode,
            //            Model = emx.Model,
            //            EquipementType = et.Description,
            //            ModelUID=emx.ModelUID
            //        };
            Utility.Audit(HttpContext.User.Identity.Name, "Customer Unscheduled Services die this Month", 0, Request);
            return View(x);

            //              select @Cnt=count(CustomerCode) from
            //(SELECT CustomerCode FROM Equipment INNER JOIN NextService ON Equipment.EquipUID = NextService.EquipUID and Equipment.BranchID=NextService.BranchID
            //WHERE (NextServiceYear*12 + NextServiceMonth) = Year(GETDATE()) * 12 + Month(GETDATE())  and Equipment.BranchID=@BranchID
            //group by customerCode) as det

        }
        
        [Authorize]
        public ActionResult ContractsDueRenewalThisMonth()
        {
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            TrackerDataContext db = new TrackerDataContext();
            DateTime StartOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var custor = from cust in db.Customers
                         where cust.BranchID == BranchID && cust.ContractRenewalDate > StartOfMonth && cust.ContractRenewalDate < StartOfMonth.AddMonths(1)
                         select new CustomerForm { CustomerCode = cust.CustomerCode, CompanyName = cust.CompanyName, ContractRenewalDate = cust.ContractRenewalDate };
            Utility.Audit(HttpContext.User.Identity.Name, "Contracts Due for Renewal This month", 0, Request);
            return View(custor);
            
                        // SELECT @Cnt=Count(*) FROM Customer WHERE Month(ContractRenewalDate) = Month(getdate()) AND Year(ContractRenewalDate) = Year(GETDATE())  and branchid=@BranchID
 
              return View();
        }

        [Authorize]
        public ActionResult ContractsOverdueRenewal()
        {
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            TrackerDataContext db = new TrackerDataContext();
             DateTime StartOfMonth=new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
             var custor = from cust in db.Customers
                          where cust.BranchID == BranchID && cust.ContractRenewalDate < StartOfMonth
                          select new CustomerForm { CustomerCode = cust.CustomerCode,CompanyName=cust.CompanyName,ContractRenewalDate=cust.ContractRenewalDate};
            Utility.Audit(HttpContext.User.Identity.Name, "Contracts Overdue Renewal", 0, Request);
            return View(custor);
        }

        [Authorize]
        public ActionResult CustUnschedServicesBeforeMonth()
        {
            int BranchID=LoginController.BranchID(HttpContext.User.Identity.Name);
            int Year12Month = DateTime.Now.Year*12+DateTime.Now.Month;
            TrackerDataContext db = new TrackerDataContext();
            var x = (from eq in db.Equipments
                    join ns in db.NextServices on new { eq.EquipUID, eq.BranchID } equals new { ns.EquipUID, ns.BranchID }
                    join cu in db.Customers on new { eq.CustomerCode, eq.BranchID } equals new { cu.CustomerCode, cu.BranchID } into cul  from cu in cul.DefaultIfEmpty()
                     join em in db.EquipModels on new {ModelUID= eq.ModelUID, BranchID =eq.BranchID } equals new {ModelUID= (int?)em.ModelUID, BranchID =em.BranchID } into eml from em in eml.DefaultIfEmpty() 
                    join et in db.EquipTypes on  new { em.EquipTypeCode, em.BranchID } equals new { et.EquipTypeCode, et.BranchID } into etl from et in etl.DefaultIfEmpty()
                     where ns.BranchID == BranchID
                   && (ns.NextServiceYear * 12 + ns.NextServiceMonth) < Year12Month
                    && eq.InService == true && eq.CurrentlyServicedByBNQ == true
                    select new EquipmentCustNextServiceList {NextServiceMonth=ns.NextServiceMonth,NextServiceYear=ns.NextServiceYear,EquipID=eq.EquipUID,EquipDesc=	/*et.Name + " " + em.Model+" " + */ /*ISNULL(dbo.CustomerLocation.Location,'') +*/ "  Serial #: " + eq.SerialNumber, 
                    Customer=cu.CompanyName,CustomerCode=cu.CustomerCode,WarrantyExpirationDate=eq.WarrantyExpirationDate
                    ,MDSItemNo=eq.BNQItemCode,Model=em.Model,EquipementType=et.Description,ModelUID=em.ModelUID
                    }).ToList();
            Utility.Audit(HttpContext.User.Identity.Name, "Cust Unsched Services Before Month", 0, Request);
            return View(x);
        }

        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == "Number of customers with unscheduled services that were due BEFORE this month")
                return RedirectToAction("CustUnschedServicesBeforeMonth");
            if (id == "Number of contracts overdue for renewal (should've been done prior to this month)")
                return RedirectToAction("ContractsOverdueRenewal");
            if (id == "Number of additional contracts due for renewal this month")
                return RedirectToAction("ContractsDueRenewalThisMonth");//
            if (id == "Number of customers with unscheduled services that are due THIS calendar month")
                return RedirectToAction("CustomersUnscheduledServicesDueThisMonth");//

            return View();
        }


        public ActionResult Test()
        {
            EquipmentCustNextServiceList tst = new EquipmentCustNextServiceList();
            tst.CurrentlyServicedByMDS = "bob";
            tst.Customer = "bob";
            tst.CustomerCode = "bob";
            tst.EquipDesc = "bob";
            tst.EquipementType = "bob";
            tst.EquipID = 0;
            tst.InService = "Y";
            tst.MDSItemNo = "12";
            tst.Model = "12";
            tst.ModelUID = 12;
            tst.NextServiceMonth = 1;
            tst.NextServiceYear = 2;
            tst.SerialNumber = "123";

            List<EquipmentCustNextServiceList> tsts = new List<EquipmentCustNextServiceList>();
            tsts.Add(tst);

            return View(tsts);
        }

        TrackerDataContext db = new TrackerDataContext();
        [Authorize]
        public ActionResult Scheduler()
        {
            Diary ret = GetSchedulerData(DateTime.Now.Date.AddDays(-1), DateTime.Now.Date.AddDays(6), null);
            return View(ret);
        }

        [Authorize]
        public ActionResult TechScheduler(int EngineerID)
        {
            
            Diary ret = GetSchedulerData(DateTime.Now.Date.AddDays(-8), DateTime.Now.Date.AddDays(27),EngineerID);
            return View(ret);
        }
        [HttpPost]

        public ActionResult TechScheduler(Diary model)
        {
            Diary ret = GetSchedulerData(model.FromDate, model.ToDate, MDS.Controllers.LoginController.EngineerID(User.Identity.Name));
            return View(ret);
        }

        private Diary GetSchedulerData(DateTime FromDate,DateTime ToDate, int? EngineerID)
        {
            Diary ret = new Diary();
            ret.FromDate = FromDate;
            ret.ToDate = ToDate;
            ret.Days = new List<DateTime>();
            for (DateTime i = FromDate; i < ToDate; i = i.AddDays(1))
            {
                ret.Days.Add(i);
            }
            int BranchID = LoginController.BranchID(HttpContext.User.Identity.Name);
            FixRepServEndDateTime(FromDate, ToDate);//, EngineerID, ret);

            if (EngineerID == null)
            {
                var reng = (from eng in db.Engineers join rep in db.Repairs on eng.EngineerID equals rep.EngineerID where rep.BranchID == BranchID && rep.DateIn >= FromDate && rep.DateIn <= ToDate group eng by new Eng { EngineerID = eng.EngineerID, EngineerName = eng.EngineerName } into e select new Eng { EngineerID = e.Key.EngineerID, EngineerName = e.Key.EngineerName }).ToList();
                var Seng = (from eng in db.Engineers join sje in db.ServiceJobEngineers on eng.EngineerID equals sje.EngineerID join sj in db.ServiceJobs on sje.ServiceJobID equals sj.ServiceJobUID where sj.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name) && sj.DateProgrammed >= FromDate && sj.DateProgrammed <= ToDate group eng by new Eng { EngineerID = eng.EngineerID, EngineerName = eng.EngineerName } into e select new Eng { EngineerID = e.Key.EngineerID, EngineerName = e.Key.EngineerName }).ToList();
                foreach (var item in reng)
                {
                    if (!Seng.Select(i => i.EngineerID).Contains(item.EngineerID))
                        Seng.Add(item);
                }
                ret.Engineers = Seng;
                Eng ueng = new Eng();
                ueng.EngineerID = 0;
                ueng.EngineerName = "Unassigned";
                //ueng.BranchID = BranchID;
                ret.Engineers.Add(ueng);
            }
            else
                ret.Engineers = (from eng in db.Engineers where eng.EngineerID == Convert.ToInt32(EngineerID) select new Eng { EngineerID = eng.EngineerID, EngineerName = eng.EngineerName }).ToList();


            ret.Hours = new List<int>();
            for (int i = 6; i <= 18; i++)
                ret.Hours.Add(i);
            ret.Minutes = new List<int>();
            ret.Minutes.Add(0);
            ret.Minutes.Add(30);

            List<Repair> Repairs;
            if (EngineerID == null)
                Repairs = db.Repairs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name) /*ret.Engineers.Select(e=>e.EngineerID).ToList().Contains(Convert.ToInt32( i.EngineerID))*/ && i.DateIn >= FromDate && i.DateIn <= ToDate).ToList();
            else
                Repairs = db.Repairs.Where(i => i.EngineerID == Convert.ToInt32(EngineerID) && i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name) /*ret.Engineers.Select(e=>e.EngineerID).ToList().Contains(Convert.ToInt32( i.EngineerID))*/ && i.DateIn >= FromDate && i.DateIn <= ToDate).ToList();

            ret.Bookings = new List<Booking>();

            foreach (var item in Repairs)
            {
                if (item.RepairUID == 2146629094)
                    item.RepairUID = 2146629094;
                Booking bk = new Booking();
                bk.RepairOrService = "Repair";
                bk.ID = item.RepairUID;
                bk.Code = item.JobCode;
                bk.EngineerID = item.EngineerID;
                bk.BranchID = item.BranchID;
                bk.ScheduledStart = item.DateIn;
                bk.ScheduledEnd = item.DateOut;
                bk.EquipUID = item.EquipUID;
                bk.Complete = item.RepairCompleted;
                ret.Bookings.Add(bk);
            }

            List<Booking> sjbooks;
            if (EngineerID == null)
            {
                sjbooks = (from sj in db.ServiceJobs
                           join sje in db.ServiceJobEngineers on new { ServiceJobUID = sj.ServiceJobUID, BranchID = sj.BranchID } equals new { ServiceJobUID = sje.ServiceJobID, BranchID = sje.BranchID }
                           where (sj.DateProgrammed >= FromDate && sj.DateProgrammed <= ToDate) && sj.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)
                           select new Booking
                           {
                               ID = sj.ServiceJobUID,
                               Code=sj.JobCode,
                               EngineerID = sje.EngineerID,
                               BranchID = sj.BranchID,
                               ScheduledStart = sj.DateProgrammed,
                               ScheduledEnd = sj.DateProgrammedEnd,
                               CustomerCode = sj.CustomerCode,
                               RepairOrService = "Service",
                               Complete = ((sj.CompletedBERs) || (sj.CompletedOK) || (sj.CompletedOutstanding))

                           }).ToList();

                //ret.Bookings = (from sj in db.ServiceJobs
                //                join sje in db.ServiceJobEngineers on new { ServiceJobUID = sj.ServiceJobUID, BranchID = sj.BranchID } equals new { ServiceJobUID = sje.ServiceJobID, BranchID = sje.BranchID }
                //              sj.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)
                //               select new Booking
                //               {
                //                   ID = sj.ServiceJobUID,
                //                   EngineerID = sje.EngineerID,
                //                   BranchID = sj.BranchID,
                //                   ScheduledStart = sj.DateProgrammed,
                //                   ScheduledEnd = sj.DateProgrammedEnd,
                //                   CustomerCode = sj.CustomerCode,
                //                   RepairOrService = "Service",
                //                   Complete = ((sj.CompletedBERs) || (sj.CompletedOK) || (sj.CompletedOutstanding))
                //               }).ToList();

            }
            else
            {
                sjbooks = (from sj in db.ServiceJobs
                           join sje in db.ServiceJobEngineers on new { ServiceJobUID = sj.ServiceJobUID, BranchID = sj.BranchID } equals new { ServiceJobUID = sje.ServiceJobID, BranchID = sje.BranchID }
                           where (sj.DateProgrammed >= FromDate && sj.DateProgrammed <= ToDate) && sje.EngineerID == Convert.ToInt32(EngineerID) && sj.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)
                           select new Booking
                           {
                               ID = sj.ServiceJobUID,
                               Code=sj.JobCode,
                               EngineerID = sje.EngineerID,
                               BranchID = sj.BranchID,
                               ScheduledStart = sj.DateProgrammed,
                               ScheduledEnd = sj.DateProgrammedEnd,
                               CustomerCode = sj.CustomerCode,
                               RepairOrService = "Service",
                               Complete = ((sj.CompletedBERs) || (sj.CompletedOK) || (sj.CompletedOutstanding))

                           }).ToList();
            }
            foreach (var item in sjbooks)
            {
                ret.Bookings.Add(item);
            }

            return ret;
        }

        private void FixRepServEndDateTime(DateTime FromDate, DateTime ToDate)//, int? EngineerID, Diary ret)
        {
            //ammend repairs where dateprogrammed no end
            {
                var repstimeoutfix = (from rp in db.Repairs where (rp.TimeOut == null || rp.DateOut == null) && rp.DateIn >= FromDate && rp.DateIn <= ToDate select rp).ToList();
                foreach (var rpitem in repstimeoutfix)
                {
                    if (rpitem.DateIn.Value.Hour == 0)
                    {
                        rpitem.DateIn = rpitem.DateIn.Value.Date.AddHours(8);
                        rpitem.TimeIn = rpitem.DateIn;
                    }
                    rpitem.DateOut = rpitem.DateIn.Value.AddHours(2);
                    rpitem.TimeOut = rpitem.DateOut;
                    db.SubmitChanges();
                }

            }

            //ammend repairs where dateprogrammed no end
            {
                var repstimeoutfix = (from rp in db.Repairs where (rp.DateIn == rp.DateOut || rp.TimeIn == rp.TimeOut) && rp.DateIn >= FromDate && rp.DateIn <= ToDate select rp).ToList();
                foreach (var rpitem in repstimeoutfix)
                {
                    
                    {
                        rpitem.TimeIn = rpitem.DateIn;
                        rpitem.DateOut = rpitem.DateIn.Value.Date.AddHours(1);
                        rpitem.TimeOut = rpitem.DateOut;

                    }
                    rpitem.DateOut = rpitem.DateIn.Value.AddHours(2);
                    rpitem.TimeOut = rpitem.DateOut;
                    db.SubmitChanges();
                }

            }

            {
                var repstimeoutfix = (from rp in db.Repairs where (rp.DateIn.Value.Hour==0  || rp.TimeIn.Value.Hour == 0) && rp.DateIn >= FromDate && rp.DateIn <= ToDate select rp).ToList();
                foreach (var rpitem in repstimeoutfix)
                {
                    {
                        rpitem.DateIn = rpitem.DateIn.Value.AddHours(8);
                        rpitem.TimeIn = rpitem.DateIn;
                        rpitem.DateOut = rpitem.DateOut.Value.AddHours(8);
                        rpitem.TimeOut = rpitem.DateOut;

                    }
                    rpitem.DateOut = rpitem.DateIn.Value.AddHours(2);
                    rpitem.TimeOut = rpitem.DateOut;
                    db.SubmitChanges();
                }

            }



            //ammend service jobs where dateprogrammed no end
            {
                var sjnoprogenddate = (from sj in db.ServiceJobs where sj.DateProgrammedEnd == null && sj.DateProgrammed >= FromDate && sj.DateProgrammed <= ToDate select sj).ToList();
                foreach (var sjitem in sjnoprogenddate)
                {
                    if (sjitem.DateProgrammed.Value.Hour == 0)

                        sjitem.DateProgrammed = sjitem.DateProgrammed.Value.Date.AddHours(8);
                    sjitem.DateProgrammedEnd = sjitem.DateProgrammed.Value.AddHours(2);
                    db.SubmitChanges();
                }
            }

            {
                var sjnoprogenddate = (from sj in db.ServiceJobs where sj.DateProgrammedEnd == sj.DateProgrammed && sj.DateProgrammed >= FromDate && sj.DateProgrammed <= ToDate select sj).ToList();
                foreach (var sjitem in sjnoprogenddate)
                {
                    //if (sjitem.DateProgrammed.Value.Hour == 0)

                     //   sjitem.DateProgrammed = sjitem.DateProgrammed.Value.Date.AddHours(8);
                    sjitem.DateProgrammedEnd = sjitem.DateProgrammed.Value.AddHours(1);
                    db.SubmitChanges();
                }
            }
            {
                var sjnoprogenddate = (from sj in db.ServiceJobs where sj.DateProgrammed.Value.Hour==0 && sj.DateProgrammed >= FromDate && sj.DateProgrammed <= ToDate select sj).ToList();
                foreach (var sjitem in sjnoprogenddate)
                {
                    sjitem.DateProgrammed = sjitem.DateProgrammed.Value.AddHours(8);
                    sjitem.DateProgrammedEnd = sjitem.DateProgrammedEnd.Value.AddHours(8);
                    db.SubmitChanges();
                }
            }

        }

        [Authorize]
        public ActionResult Index()
        {
            var model = new StatusSummary();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.ContractsOverDueAndUnscheduledServices(LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new StatusSummaryList
            {
                InformationTitle = i.InformationTitle,
                StatusCount = i.Cnt

            }).ToList();
            model.Status = s;
            
            Utility.Audit(HttpContext.User.Identity.Name,"Dashboard",0,Request);
            return View(model);
        }

    }
}
