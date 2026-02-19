using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class ServicePrevMaintenanceCheck : System.Web.UI.Page
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
                ReportViewer1.Height = 2000;
                if (ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response, AddToSession_PurgeFromSession_No.PurgeFromSession, Session) == null)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "close1", "<script language='Javascript'>alert('close this page');</script>");
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "close2", "<script language='Javascript'>myWindowPM.close();</script>");
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "close3", "<script language='Javascript'>this.documents[0].close();</script>");
                }
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
            IQueryable<ServicesJobServicesEquipResult> ret = context.ServicesJobServicesEquip(ServiceJobUID, BranchID).AsQueryable();
            return ret;
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
            ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);
        }

        public void Unnamed_SubreportProcessing(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
        {
            int serviceuid =Convert.ToInt32( e.Parameters["serviceuid"].Values[0]);
            int BranchID = Convert.ToInt32(Request.QueryString["BranchID"]);
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            var ServicesJobServicesEquipParts = context.ServicesJobServicesEquipPartsNew(serviceuid, BranchID).AsQueryable();
            e.DataSources.Add(new ReportDataSource("DataSetSub", ServicesJobServicesEquipParts));
        }
    }
}