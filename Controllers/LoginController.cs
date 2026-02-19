using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDS.DB;
using MDS.Models;
using MDS.Helper;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace MDS.Controllers
{
    public enum ATC
    {
        Admin,
        Tech,
        Customer,
        None
    }

    public class LoginController : Controller
    {
        [Authorize]

        public ActionResult PDFReportUserDetails(string UserEmail, int Period)
        {

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Login/AuditLogDetail.rdlc");

            return Renderdoc(localReport, Models.UsersAuditDetailDatas.GetDetailUserEmail(UserEmail, Period).ToList()); //, ds);
        }
        [Authorize]

        public ActionResult PDFReportUser(string UserEmail, int Period)
        {

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Login/UsersHitsPeriodRpt.rdlc");

            return Renderdoc(localReport, Models.UsersHitsPeriodDatas.GetUser(UserEmail, Period).ToList()); //, ds);
        }
        [Authorize]

        public ActionResult PDFAuditDetail(int Period)
        {

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Login/AuditLogDetail.rdlc");
            string Email = MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name);
            return Renderdoc(localReport, Models.UsersAuditDetailDatas.GetDetailUser(Email, Period).ToList(), true); //, ds);
        }

        public ActionResult PDFAuditDetailBranch(string BranchName, int Period)
        {

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Login/AuditLogDetail.rdlc");
            //string Email = MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name);
            return Renderdoc(localReport, Models.UsersAuditDetailDatas.GetDetailBranch(BranchName, Period).ToList(), true); //, ds);
        }


        void SendMail(string recipient, string subject, string body)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");

            client.Credentials = new NetworkCredential("your-email@example.com", "");
            client.Port = 587;
            client.EnableSsl = true;
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@mds.com");
            mailMessage.To.Add(recipient);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            client.Send(mailMessage);
        }

        [Authorize]
        public ActionResult PDFReport(int Period)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Login/UsersHitsPeriodRpt.rdlc");
            string Email = MDS.Controllers.LoginController.AdminTechCustomer(User.Identity.Name);

            return Renderdoc(localReport, Models.UsersHitsPeriodDatas.GetAll(Email, Period).ToList()); //, ds);
        }

        [Authorize]
        public ActionResult PDFReportBranch(string BranchName, int Period)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Login/UsersHitsPeriodRpt.rdlc");
            return Renderdoc(localReport, Models.UsersHitsPeriodDatas.GetBranch(BranchName, Period).ToList()); //, ds);
        }

        private ActionResult Renderdoc(LocalReport localReport, System.Collections.IEnumerable dt, bool Landscape = false)
        {
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dt);

            localReport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo;
            if (Landscape)
            {
                deviceInfo =
    "<DeviceInfo>" +
    "  <OutputFormat>PDF</OutputFormat>" +
    "  <PageWidth>29.7cm</PageWidth>" +
    "  <PageHeight>21cm</PageHeight>" +
    "  <MarginTop>1cm</MarginTop>" +
    "  <MarginLeft>1cm</MarginLeft>" +
    "  <MarginRight>1cm</MarginRight>" +
    "  <MarginBottom>1cm</MarginBottom>" +
    "</DeviceInfo>";
            }
            else
            {
                deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.2in</MarginTop>" +
                "  <MarginLeft>0.2in</MarginLeft>" +
                "  <MarginRight>0.2in</MarginRight>" +
                "  <MarginBottom>0.2in</MarginBottom>" +
                "</DeviceInfo>";
            }
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            FileContentResult ff = File(renderedBytes, mimeType);

            return ff;
        }




        public static Int32 BranchID(string HttpContextUserIdentityName)
        {
            try
            {
                char cc = Convert.ToChar(",");
                string[] identity = HttpContextUserIdentityName.Split(new char[] { cc }, StringSplitOptions.RemoveEmptyEntries);
                return Convert.ToInt32(identity[0]);
            }
            catch (Exception ex)
            {
                return 4;
            }
        }

        public static bool IsSuperUser(string HttpContextUserIdentityName)
        {
            char cc = Convert.ToChar(",");
            string[] identity = HttpContextUserIdentityName.Split(new char[] { cc }, StringSplitOptions.RemoveEmptyEntries);
            if (identity[2] == "A")
                return true;
            return false;
        }

        public static bool IsAudit(string HttpContextUserIdentityName)
        {
            if (AdminTechCustomer(HttpContextUserIdentityName).ToLower().Contains("jane"))
                return true;
            if (AdminTechCustomer(HttpContextUserIdentityName).ToLower().Contains("tim"))
                return true;
            if (AdminTechCustomer(HttpContextUserIdentityName).ToLower().Contains("ren"))
                return true;
            return false;
        }


        public static ATC IsAdmin(string HttpContextUserIdentityName)
        {
            if (HttpContextUserIdentityName == "")
                return ATC.Admin;
            char cc = Convert.ToChar(",");
            string[] identity = HttpContextUserIdentityName.Split(new char[] { cc }, StringSplitOptions.RemoveEmptyEntries);
            if (identity.Count() < 3)
                return ATC.None;
            if (identity[2].ToString().ToLower() == "a")
                return ATC.Admin;
            if (identity[2].ToString().ToLower() == "u")
                return ATC.Admin;
            if (identity[2].ToString().ToLower() == "t")
                return ATC.Tech;
            if (identity[2].ToString().ToLower() == "c")
                return ATC.Customer;
            return ATC.Admin;


        }
        public static string AdminTechCustomer(string HttpContextUserIdentityName)
        {
            if (HttpContextUserIdentityName == "")
                return "Unknown";
            char cc = Convert.ToChar(",");
            string[] identity = HttpContextUserIdentityName.Split(new char[] { cc }, StringSplitOptions.RemoveEmptyEntries);
            return identity[1];
        }

        public static int EngineerID(string HttpContextUserIdentityName)
        {
            if (HttpContextUserIdentityName == "")
                return -1;
            char cc = Convert.ToChar(",");
            string[] identity = HttpContextUserIdentityName.Split(new char[] { cc }, StringSplitOptions.RemoveEmptyEntries);
            if (identity.Length == 4)
                return Convert.ToInt32(identity[3]);
            return -1;
        }


        public static string GetSiteRootUrl(HttpRequestBase Request)
        {
            string protocol;

            if (Request.IsSecureConnection)
                protocol = "https";
            else
                protocol = "http";

            System.Text.StringBuilder uri = new System.Text.StringBuilder(protocol + "://");

            string hostname = Request.Url.Host;

            uri.Append(hostname);

            int port = Request.Url.Port;

            if (port != 80 && port != 443)
            {
                uri.Append(":");
                uri.Append(port.ToString());
            }

            return uri.ToString() + Request.ApplicationPath;
        }

        public ActionResult RetrievePassword()
        {
            Login ll = new Login();
            ll.Password = "Unknown";
            return View(ll);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetrievePassword(Login model)
        {
            TrackerDataContext db = new TrackerDataContext();
            var login = db.UserLogins.Where(i => i.Email == model.UserName).FirstOrDefault();
            if (login == null)
            {
                ModelState.AddModelError("", "This Username is not found in the Userlist");
                return View(model);
            }
            SendMail(model.UserName, "MDS - Password Recovery", "Your password is " + login.Password);

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            // SECURITY: Removed password-in-query-string login - credentials should only be sent via POST
            if (ControllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var data = HttpContext.User.Identity.Name;
                ATC atc = IsAdmin(data);
                if (atc == ATC.None)
                    return View();

                if (atc == ATC.Customer)
                    return RedirectToDefault("C");
                else if (atc == ATC.Admin)
                    return RedirectToDefault("A");
                else
                    return RedirectToDefault("T");
            }
            else
            {
                return View();
            }
        }

        static string _AdminHomePageMethod;

       public  static string AdminHomePageMethod
        {
            get
            {
                if ((_AdminHomePageMethod==null) || (_AdminHomePageMethod==""))
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    _AdminHomePageMethod = appSettings["AdminHomePageMethod"].ToString();
                }
                return _AdminHomePageMethod;
            }
        }
        private ActionResult RedirectToDefault(string AdminTechCust)
        {
            if (AdminTechCust == "C")
                return RedirectToAction("Index", "Equipment");
            else if (AdminTechCust == "A")
            {
                var appSettings = ConfigurationManager.AppSettings;
                string controller = appSettings["AdminHomePageController"].ToString();
                return RedirectToAction(AdminHomePageMethod, controller);

            }
            else//tech 
                return RedirectToAction("Dashboard", "Tech");
        }

        public static bool AllCustomers(string HttpContextUserIdentityName)
        {

            string Cust = AdminTechCustomer(HttpContextUserIdentityName);
            if (Cust == "Admin")
                return true;
            if (Cust.Contains("@"))
                return true;
            if (Cust == "Tech")
                return true;
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Login model)
        {
            //if (HttpContext.User.Identity.Name!="")
            Utility.Audit(HttpContext.User.Identity.Name, "Login " + model.UserName, 0, Request);
            var username = model.UserName;
            var password = model.Password;
            return Login(model, username, password);
        }

        [Authorize]

        public ActionResult UserLogin()
        {
            TrackerDataContext db = new TrackerDataContext();
            DB.UserLogin ul = db.UserLogins.Where(i => i.Email == MDS.Controllers.LoginController.AdminTechCustomer(@User.Identity.Name)).FirstOrDefault();

            return View(ul);
        }

        public ActionResult Select(string UserName, int BranchID)
        {
            TrackerDataContext db = new TrackerDataContext();
            Utility.Audit(HttpContext.User.Identity.Name, "Select Branch " + UserName, BranchID, Request);
            var userLogin = db.UserLogins.Where(i => i.Email == UserName).FirstOrDefault();
            if (userLogin == null)
            {
                throw new Exception("User not found");
            }
            var eng = db.Engineers.Where(i => i.UserName == UserName && i.BranchID==BranchID).FirstOrDefault();
            //FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + UserName + "," + Admin+ "," + userLogin.EngineerID.ToString(), true);
            if (eng != null)//.AdminUserTech.ToString() == "T")
            {
                FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + UserName + ",T," + eng.EngineerID, true);
                return RedirectToDefault("T");
            }
            else
            {
                if (userLogin.Admin==true)
                    FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + UserName + ",A", true);
else
                    FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + UserName + ",U", true);
                return RedirectToDefault("A");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(DB.UserLogin ulEdit)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Update Password " + ulEdit.Email, 0, Request);

            TrackerDataContext db = new TrackerDataContext();
            DB.UserLogin ul = db.UserLogins.Where(i=>i.Email== ulEdit.Email).FirstOrDefault();
            ul.Password = ulEdit.Password;
            db.SubmitChanges();
            return View(ul);
        }
    //    [Authorize]
        public ActionResult Branches(string Email)
        {
            TrackerDataContext db = new TrackerDataContext();
            var userbranches = (from ub in db.UserBranches
                               join br in db.Branches on ub.BranchID equals br.BranchID
                               join us in db.UserLogins on ub.UserEmail equals us.Email
                               where ub.UserEmail == Email
                               select new UserBranchList {UserName=Email,BranchID= br.BranchID,BranchName=br.BranchName,Admin=us.Admin }).ToList();
            return View(userbranches); 
        }


        [Authorize]
        public ActionResult  ClearAudit()
        {
            TrackerDataContext db = new TrackerDataContext();
            db.ClearAudit();
            //exec ClearAudit
            return RedirectToAction("Audit");
        }

        [Authorize]
        public ActionResult Audit()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Audit", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            AuditSelect AS = new AuditSelect();
            string Email = MDS.Controllers.LoginController.AdminTechCustomer(HttpContext.User.Identity.Name);
            var branches = (from br in db.Branches
                            join ub in db.UserBranches on br.BranchID equals ub.BranchID
                            where ub.UserEmail == Email
                            && ub.Audit == true
                            select br).ToList();

            AS.branches = branches;
            var users = (from au in db.Audits
                         where branches.Select(i=>i.BranchName).Contains(au.Branch)
                         group au by new { Email = au.UserName } into augr
                         select new { Email = augr.Key.Email }
                         ).ToList();
            AS.users = users.Select(i => i.Email).ToList();
            return View(AS);
        }

        [Authorize]
        public ActionResult AllUsers()
        {
            Utility.Audit(HttpContext.User.Identity.Name, "All User logins", 0, Request);

            TrackerDataContext db = new TrackerDataContext();
            var users = db.UserLogins.ToList();
            foreach (var user in users)
            {
                var userbranch =(from ub in db.UserBranches
                                 join br in db.Branches on ub.BranchID equals br.BranchID
                                where ub.UserEmail== user.Email
                                select br.BranchName).ToList();
                user.Password = string.Join(",", userbranch);
            }
            return View(users);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDetail(UserDetailBranches us)
        {

            TrackerDataContext db = new TrackerDataContext();
            if (us.New)
            {
                DB.UserLogin user = new DB.UserLogin();
                user.Email = us.Email;
                user.Password = us.Password;
                //if (db.Engineers.Where(i => i.UserName == us.Email).Count() > 0)
                //    user.AdminUserTech = "T";
                //else if (user.Admin)
                //    user.AdminUserTech = "A";// Convert.ToChar(us.AdminUserTech);
                //else
                //    user.AdminUserTech = "U";
                    user.Admin = us.Admin;
                db.UserLogins.InsertOnSubmit(user);
                db.SubmitChanges();
            }
            else
            {
                DB.UserLogin user = db.UserLogins.Where(i=>i.Email==us.Email).FirstOrDefault();
           //     user.Email = us.Email;
                user.Password = us.Password;

               user.Admin= us.Admin;
                //db.UserLogins.InsertOnSubmit(user);
                db.SubmitChanges();
            }
            var selectedBranches = db.UserBranches.Where(i => i.UserEmail == us.Email).ToList();
            if (selectedBranches != null)
            {
                foreach (DB.UserBranch item in selectedBranches)
                {
                    if (!us.SelectedBranch.Contains(item.BranchID))
                        db.UserBranches.DeleteOnSubmit(item);
                }
            }
            foreach (var BranchID in  us.SelectedBranch)
            {
                if (selectedBranches==null)
                {
                    DB.UserBranch newub = new UserBranch();
                    newub.BranchID = BranchID;
                    newub.UserEmail = us.Email;
                    if (us.AuditBranch.Contains(BranchID))
                        newub.Audit = true;

                    db.UserBranches.InsertOnSubmit(newub);
                }
                else if (!selectedBranches.Select(i => i.BranchID).ToList().Contains(BranchID))
                {
                    DB.UserBranch newub = new UserBranch();
                    newub.BranchID = BranchID;
                    newub.UserEmail = us.Email;
                    if (us.AuditBranch != null)
                    {
                        if (us.AuditBranch.Contains(BranchID))
                            newub.Audit = true;
                    }
                    db.UserBranches.InsertOnSubmit(newub);
                }
                else
                {
                    if (us.AuditBranch != null)
                    {
                        bool Audit = us.AuditBranch.Contains(BranchID);
                        var xx = db.UserBranches.Where(i => i.BranchID == BranchID && i.UserEmail == us.Email).FirstOrDefault();
                        xx.Audit = Audit;
                    }
                }
            }

            db.SubmitChanges();
            return RedirectToAction("AllUsers");
            //return View(us);
        }
        [Authorize]
        public ActionResult UserDetail(string UserName)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "Edit User Details / Branch Access", 0, Request);
            TrackerDataContext db = new TrackerDataContext();
            UserDetailBranches us = new UserDetailBranches();
            if (UserName == "New")
                us.New = true;
            else
            {
                us.New = false;
                var user = db.UserLogins.Where(i => i.Email == UserName).FirstOrDefault();
                us.Email = user.Email;
                //if (user.AdminUserTech == null)
                //    us.AdminUserTech = "Ü";
                //else
                //    us.AdminUserTech =Convert.ToString(  user.AdminUserTech);
                us.Admin = user.Admin;
                us.Password = user.Password;
            }
            var allbr= db.Branches;
            us.branches = new List<UserBranchList>();
            us.SelectedBranch = db.UserBranches.Where(i=>i.UserEmail==UserName).Select(i => i.BranchID).ToList();
            us.AuditBranch = db.UserBranches.Where(i => i.UserEmail == UserName && i.Audit==true).Select(i => i.BranchID).ToList();
            foreach (var item in allbr)
            {
                UserBranchList ub = new UserBranchList();
                ub.BranchID = item.BranchID;
                ub.BranchName = item.BranchName;
                if (us.SelectedBranch.Contains(item.BranchID))
                    ub.Selected = true;
                if (us.AuditBranch.Contains(item.BranchID))
                    ub.Audit = true;
                us.branches.Add(ub);
            }
         
            return View(us);
        }
        static string _CompanyName;
        public static string CompanyName
        {
            get
            {
                if (_CompanyName==null)
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    _CompanyName = appSettings["CompanyName"].ToString();
                }
                return _CompanyName;
            }
        }
        private ActionResult Login(Login model, string username, string password)
        {
            Utility.Audit(HttpContext.User.Identity.Name, "User Login " + username, 0, Request);

            Session.Abandon();
            FormsAuthentication.SignOut();
            TrackerDataContext db = new TrackerDataContext();
            var userLogin = db.UserLogins.Where(i => i.Email == username && i.Password == password).FirstOrDefault();
            if (userLogin != null)
            {
                int BranchID;
                //string AdminUserTech = userLogin.AdminUserTech.ToString();
                List<UserBranch> Branches = db.UserBranches.Where(i => i.UserEmail == username && i.DefaultBranch == true).ToList();
                if (Branches.Count == 1)
                    BranchID = Branches.FirstOrDefault().BranchID;
                else
                {
                    Branches = db.UserBranches.Where(i => i.UserEmail == username).ToList();
                    if (Branches.Count == 0)
                        throw new Exception("No Branches Assigned");
                    else
                        BranchID = Branches.FirstOrDefault().BranchID;

                }
                var eng = db.Engineers.Where(i => i.UserName == username && i.BranchID == BranchID).FirstOrDefault();

                if (eng != null)//.AdminUserTech.ToString() == "T")
                {
                    FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + username + ",T," + eng.EngineerID, true);
                    return RedirectToDefault("T");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(BranchID.ToString() + "," + username + "," + ((userLogin.Admin==true)?"A":"U"), true);
                    return RedirectToDefault("A");
                }
            }
            else
            {
                ;// var Help = db.UserLogins.Where(i => i.Email == username).FirstOrDefault().Password;

            }

            //var isLogin = db.Branches.Where(i => i.BranchName == username && i.Password == password).FirstOrDefault();
            //if (isLogin != null)
            //{
            //    FormsAuthentication.SetAuthCookie(isLogin.BranchID.ToString() + ",Admin, false", true);
            //    return RedirectToAction("Index", "Dashboard");
            //}
            var isLoginRO = db.Branches.Where(i => i.BranchName == username && i.PasswordROTech == password).FirstOrDefault();
            if (isLoginRO != null)
            {
                Session["BranchID"] = isLoginRO.BranchID.ToString();
                FormsAuthentication.SetAuthCookie(isLoginRO.BranchID.ToString() + "," + username + ",TO,", true);
                return RedirectToAction("Index", "Equipment");
            }
            var isLoginCustomer = db.Customers.Where(i => i.CustomerCode + i.BranchID.ToString() == username && i.OnLinePassword == password).FirstOrDefault();
            if (isLoginCustomer != null)
            {
                Session["BranchID"] = isLoginCustomer.BranchID.ToString();
                FormsAuthentication.SetAuthCookie(isLoginCustomer.BranchID.ToString() + "," + isLoginCustomer.CustomerCode+",C", true);
  
                return RedirectToAction("Index", "Repair");
            }
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            if (model==null)
                return View();
            return View(model);
        }

        static List<Branch> _Branchs;

        public static List<Branch> Branchs
        {
            get
            {
                if (_Branchs==null)
                {
                    TrackerDataContext db = new TrackerDataContext();
                    _Branchs = db.Branches.ToList();
                }
                return _Branchs;
            }
        }

        public static string BranchName(string HttpContextUserIdentityName)
        {
                var br=Branchs.Where(i => i.BranchID == LoginController.BranchID(HttpContextUserIdentityName)).FirstOrDefault();
                if (br == null)
                    return "Not found";
                return br.BranchName;
        }
        [Authorize]
        public virtual ActionResult Details()
        {
            //var login = new Branch();
            //if (Request.IsAuthenticated)
            //{
            //    login = Branchs.Where(i => i.BranchID == LoginController.BranchID(HttpContext.User.Identity.Name)).FirstOrDefault();
            //}
            var userDetails = new LoginDetail
            {
                BranchName = Request.IsAuthenticated ? BranchName(HttpContext.User.Identity.Name) : null,
                LoggedOn = Request.IsAuthenticated
            };

            return PartialView(userDetails);
        }

        [Authorize]
        public virtual ActionResult LogOff()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            cookie.HttpOnly = true;
            cookie.Secure = true;
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

    }
}
