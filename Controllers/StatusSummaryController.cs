using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDS.DB;
using MDS.Models;
using MDS.Helper;
namespace MDS.Controllers
{
    public class StatusSummaryController : BaseController
    {
        //
        // GET: /StatusSummary/
          [Authorize]
        public ActionResult Index()
        {
            var model = new StatusSummary();
            TrackerDataContext db = new TrackerDataContext();
            var s = db.ContractsOverDueAndUnscheduledServices(LoginController.BranchID(HttpContext.User.Identity.Name)).Select(i => new StatusSummaryList
            {
                InformationTitle=i.InformationTitle,
                StatusCount=i.Cnt
            }).ToList();
            model.Status = s;
            return View(model);
        }

    }
}
