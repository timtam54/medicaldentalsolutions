using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class RepairListRpt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportParameter rp = new ReportParameter("CompanyName", MDS.Controllers.LoginController.CompanyName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });


                ReportViewer1.LocalReport.Refresh();
                ClientScript.RegisterClientScriptBlock(this.GetType(), "print", "<script language='Javascript'>document.forms[0].submit();</script>");
            }
            else
            {
                string xx = footfloat.Text;
                ReportViewer1.Height = 2000;
                //return;
                if (Request.QueryString["Excel"] == null)
                {
                   // string filename = "RepairResultsList_" + Request.QueryString["BranchID"].ToString() + ".pdf";
                    ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);
                  //  Response.Redirect("temppdfs/" + filename);
                }
                else
                {
                   // string filename = "RepairResultsList_" + Request.QueryString["BranchID"].ToString() + ".xls";
                    ReportGlobal.CreateDownloadExcel(Server, ReportViewer1, Response);
                   // Response.Redirect("temppdfs/" + filename);
                }
            }
        }

        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
      //  public static List<RepairsSearchResult> RepairsSearch(DateTime dateInFrom, DateTime dateInTo, string customerID, string equipmentType, int BranchID, char resolved_NotResolved_Either, char handoverCompleted_Incomplete_Either, int serviceJobID, int equipmentID, string department, string location, int engineerID, string custOrderNo, string repairJobNo, DateTime dateOutFrom, DateTime dateOutTo, bool DateInFilter, bool DateOutFilter, int Cnt)
       public static IQueryable<RepairsSearchResult> RepairsSearch(DateTime dateInFrom, DateTime dateInTo, string customerID, string equipmentType, int BranchID, char resolved_NotResolved_Either, char handoverCompleted_Incomplete_Either, int serviceJobID, int equipmentID, string department, string location, int engineerID, string custOrderNo, string repairJobNo, DateTime dateOutFrom, DateTime dateOutTo, bool DateInFilter, bool DateOutFilter, int Cnt)
        {
            //RepairsSearchResult x = new RepairsSearchResult();
            //List<RepairsSearchResult> l = new List<RepairsSearchResult>();
            //l.Add(x);
            //return l.AsQueryable();
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            if (location == "--Location--")
                location = "";
            if (department == "--Select Customer Site--")
                department = ""; 
            return context.RepairsSearch(dateInFrom, dateInTo, customerID, equipmentType, BranchID, resolved_NotResolved_Either, handoverCompleted_Incomplete_Either, serviceJobID, equipmentID, department, location, engineerID, custOrderNo, repairJobNo, dateOutFrom, dateOutTo, DateInFilter, DateOutFilter, Cnt).AsQueryable();
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
           // string filename = "RepairResultsList.pdf";
            ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);


        }
    }
}