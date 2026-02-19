using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class ServicePrevMaintenance : System.Web.UI.Page
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
               // string filename = "ServicePreventativeMaintenance_" + Request.QueryString["ServiceJobUID"].ToString() + "_" + Request.QueryString["BranchID"].ToString() + ".pdf";
                ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response, AddToSession_PurgeFromSession_No.No, Session);
               // Response.Redirect("temppdfs/" + filename);
            }

        }

        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
        public static IQueryable<ServicesJobServicesEquipResult> ServicesJobServicesEquip(Int32 ServiceJobUID, int BranchID)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            
            return context.ServicesJobServicesEquip(ServiceJobUID, BranchID).AsQueryable();
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
            //string filename = "ServicePreventativeMaintenance.pdf";
            ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);

        }

    }
}