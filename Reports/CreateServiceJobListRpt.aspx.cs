using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class CreateServiceJobListRpt : System.Web.UI.Page
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
                ReportViewer1.Height = 2000;// Convert.ToInt32(xx) - 70;
                if (Request.QueryString["Excel"] == null)
                {
                    ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);
                }
                else
                {
                    ReportGlobal.CreateDownloadExcel( Server, ReportViewer1, Response);
                }
            }
        }
        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
        public static IQueryable<NextServiceSearchResult> CreateServiceJobSearch(string dueOption, int monthYear, string customerID, int BranchID,int equipmentID, string department, string location,int Count)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            if (location == "--Location--")
                location = "";
            if (department == "--Select Customer Site--")
                department = "";

            return context.NextServiceSearch(dueOption, customerID, department, location, equipmentID, BranchID,monthYear,Count).AsQueryable();
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
           // string filename = "CreateServiceJobList.pdf";
            ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);

        }
    }
}