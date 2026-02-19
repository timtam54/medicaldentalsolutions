using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class UsersHitsPeriodData
    {
        public UsersHitsPeriodData()
        {

        }
        public string UserName { get; set; }
        public int Hits { get; set; }
        public int Hours { get; set; }

        public DateTime Period { get; set; }


    }


    public class UsersHitsPeriodDatas
    {
        public static List<UsersHitsPeriodData> GetBranch(string BranchName,int Period)
        {
            DB.TrackerDataContext db = new DB.TrackerDataContext();


            var lreturn = (from aud in db.Audits
                           where aud.Branch==BranchName
                           && aud.ActivityDteTme > UsersAuditDetailDatas.FromDate(Period)
                           group aud by new { User = aud.UserName, Date = aud.ActivityDteTme.Value.Date, Hour = aud.ActivityDteTme.Value.Hour } into g
                           select new UsersHitsPeriodData { UserName = g.Key.User, Period = g.Key.Date, Hours = g.Key.Hour, Hits = g.Count() }).ToList();


            foreach (var item in lreturn)
            {
                item.Period = item.Period.Date.AddHours(item.Hours);
            }
            return lreturn.ToList();
        }

        public static List<UsersHitsPeriodData> GetAll(string UserEmail,int Period)
        {
            DB.TrackerDataContext db = new DB.TrackerDataContext();

            var branches = (from br in db.Branches
                            join ub in db.UserBranches on br.BranchID equals ub.BranchID
                            where ub.UserEmail == UserEmail
                            && ub.Audit == true
                            select br).ToList();

            var lreturn = (from aud in db.Audits
                          where branches.Select(i => i.BranchName).Contains(aud.Branch)
                          && aud.ActivityDteTme > UsersAuditDetailDatas.FromDate(Period)
                           group aud by new { User = aud.UserName, Date = aud.ActivityDteTme.Value.Date, Hour = aud.ActivityDteTme.Value.Hour } into g
                          select new UsersHitsPeriodData { UserName = g.Key.User, Period = g.Key.Date, Hours=g.Key.Hour, Hits = g.Count() }).ToList();


            foreach (var item in lreturn)
            {
                item.Period = item.Period.Date.AddHours(item.Hours);
            }
            return lreturn.ToList();
        }
        public static List<UsersHitsPeriodData> GetUser(string UserEmail,int Period)
        {
            DB.TrackerDataContext db = new DB.TrackerDataContext();
            //var branches = (from br in db.Branches
            //                join ub in db.UserBranches on br.BranchID equals ub.BranchID
            //                where ub.UserEmail == UserEmail
            //                && ub.Audit == true
            //                select br).ToList();


            var lreturn = (from aud in db.Audits
                          where aud.UserName==UserEmail
                          && aud.ActivityDteTme> UsersAuditDetailDatas.FromDate(Period)
                           group aud by new { User = aud.Page, Date = aud.ActivityDteTme.Value.Date,Hour = aud.ActivityDteTme.Value.Hour } into g
                          select new UsersHitsPeriodData { UserName = g.Key.User,Hours=g.Key.Hour, Period = g.Key.Date, Hits = g.Count() }).ToList();
            foreach (var item in lreturn)
            {
                item.Period = item.Period.Date.AddHours(item.Hours);
            }

            return lreturn.ToList();
        }
    }
}