using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class ServiceJobPartsList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.LocalReport.Refresh();
                ClientScript.RegisterClientScriptBlock(this.GetType(), "print", "<script language='Javascript'>document.forms[0].submit();</script>");
            }
            else
            {
                string xx = footfloat.Text;
                ReportViewer1.Height = 2000;// Convert.ToInt32(xx) - 70;
               // string filename = "ServiceJobPartsList_" + Request.QueryString["ServiceJobUID"].ToString() + ".pdf";
                ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);
               // Response.Redirect("temppdfs/" + filename);
            }

        }
        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
        public static IQueryable<ServiceJobPartsListResult> ServiceJobPartsReport(Int32 ServiceJobUID, Int32 BranchID)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            
            return context.ServiceJobPartsList(ServiceJobUID, BranchID).AsQueryable();
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
          //  string filename = "ServiceJobPartsList.pdf";
            ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);

        }
    }
}