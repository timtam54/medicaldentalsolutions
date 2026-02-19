using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class ServiceWorkOrderDetail : System.Web.UI.Page
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
             //   string filename = "ServiceWorkOrderDetail_" + Request.QueryString["branchID"].ToString() + "_" + Request.QueryString["serviceID"].ToString() + ".pdf";
               ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);
             //  Response.Redirect("temppdfs/" + filename);
            }

        }

        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }

        public static IQueryable<ServiceWorkOrderDetailResult> ServiceReport(Int32 serviceID, Int32 branchID)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            
            return context.ServiceWorkOrderDetail(serviceID, branchID).AsQueryable();
        }


        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
           // string filename = "ServiceWorkOrderDetail.pdf";
            ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);

        }

    }
}