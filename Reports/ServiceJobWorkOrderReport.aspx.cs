using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace MDS.Reports
{
    public partial class ServiceJobWorkOrderReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.EnableExternalImages = true;
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
                //if (Request.QueryString["CacheCombine"].ToString() == "True")
                //{
                //    if (ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response, AddToSession_PurgeFromSession_No.AddToSession, Session) == null)
                //    {
                //        ClientScript.RegisterClientScriptBlock(this.GetType(), "close", "<script language='Javascript'>myWindowWO.close();</script>");
                //    }
                //}
                //else
                    ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response, AddToSession_PurgeFromSession_No.No);

             //   Response.Redirect("temppdfs/" + filename);
            }
        }
        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
        public static IQueryable<ServiceWorkOrderRptResult> ServiceWorkOrderRpt(Int32 ServiceJobUID, int BranchID)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            
            return context.ServiceWorkOrderRpt(ServiceJobUID,BranchID).AsQueryable();
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
            //string filename = "ServiceJobWorkOrder.pdf";
            ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);

        }

    }
}