using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{

    public class UsersAuditDetailDatas
    {

     public   static DateTime FromDate(int Period)
        {
            if (Period == 1)
                return DateTime.Now.AddDays(-2);
            if (Period == 2)
                return DateTime.Now.AddDays(-7);
            if (Period == 3)
                return DateTime.Now.AddHours(-24);
            if (Period == 4)
                return DateTime.Now.AddDays(-28);
            return DateTime.Now;
        }

        public static List<DB.Audit> GetDetailBranch(string BranchName,int Period)
        {
            DB.TrackerDataContext db = new DB.TrackerDataContext();
            //var Branches = (from ub in db.UserBranches
            //                join br in db.Branches on ub.BranchID equals br.BranchID
            //                where ub.UserEmail == Email
            //                select br.BranchName).ToList();
            var lreturn = from aud in db.Audits
                          where aud.ActivityDteTme > FromDate(Period)// DateTime.Now.AddDays(-2)
                          && aud.Branch== BranchName
                          orderby aud.ActivityDteTme
                          select aud;
            //                          group aud by new { User = aud.UserName, Date = aud.ActivityDteTme.Value.Date } into g
            //                        select new UsersHitsPeriodData { UserName = g.Key.User, Period = g.Key.Date, Hits = g.Count() };

            return lreturn.ToList();
        }
        public static List<DB.Audit> GetDetailUserEmail(string UserEmail, int PeriodID)
        {
            DB.TrackerDataContext db = new DB.TrackerDataContext();
            var lreturn = from aud in db.Audits
                          where aud.ActivityDteTme > FromDate(PeriodID)
                          && aud.UserName==UserEmail
                          orderby aud.ActivityDteTme
                          select aud;

            return lreturn.ToList();
        }

        public static List<DB.Audit> GetDetailUser(string Email,int PeriodID)
        {
            DB.TrackerDataContext db = new DB.TrackerDataContext();
            var Branches = (from ub in db.UserBranches
                            join br in db.Branches on ub.BranchID equals br.BranchID
                            where ub.UserEmail == Email
                            select br.BranchName).ToList();
            var lreturn = from aud in db.Audits
                          where aud.ActivityDteTme > FromDate(PeriodID)
                          && Branches.Contains( aud.Branch)
                          orderby aud.ActivityDteTme
                        select aud;
//                          group aud by new { User = aud.UserName, Date = aud.ActivityDteTme.Value.Date } into g
  //                        select new UsersHitsPeriodData { UserName = g.Key.User, Period = g.Key.Date, Hits = g.Count() };

            return lreturn.ToList();
        }
    }
}