using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class ServiceJobList : System.Web.UI.Page
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
             //   return;
                string xx = footfloat.Text;
                ReportViewer1.Height = 2500;// Convert.ToInt32(xx) - 70;
                if (Request.QueryString["Excel"] == null)
                {
                  //  string filename = "ServiceJobList_" + Request.QueryString["BranchID"].ToString() + ".pdf";
                    ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);
                    //Response.Redirect("temppdfs/" + filename);
                }
                else
                {
                   // string filename = "ServiceJobList_" + Request.QueryString["BranchID"].ToString() + ".xls";
                    ReportGlobal.CreateDownloadExcel( Server, ReportViewer1, Response);
                    //Response.Redirect("temppdfs/" + filename);
                }
            }
        }

        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
        public static IQueryable<ServiceJobSearchResult> ServiceJobSearch(DateTime dateInFrom, DateTime dateInTo, string customerID, string completeIncomplete, int BranchID, string Invoice, string serviceJobNo, string department, string location, int engineerID, string custOrderNo, DateTime dateOutFrom, DateTime dateOutTo, bool DateInFilter, bool DateOutFilter,bool Excel,int Cnt)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            if (location == "--Location--")
                location = "";
            if (department == "--Select Customer Site--")
                department = "";
            var x= context.ServiceJobSearch(dateInFrom, dateInTo, customerID, department, engineerID, BranchID, completeIncomplete, Invoice, location, serviceJobNo, custOrderNo, dateOutFrom, dateOutTo, DateInFilter, DateOutFilter,Excel,Cnt).AsQueryable();
            return x;
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
          //  string filename = "ServiceJobList.pdf";
            ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);

        }

    }
}