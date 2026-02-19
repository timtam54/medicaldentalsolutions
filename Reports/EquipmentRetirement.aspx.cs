using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class EquipmentRetirement : System.Web.UI.Page
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
                ReportViewer1.Height = 1800;// Convert.ToInt32(xx) - 70;
              //  string filename = "EquipmentRetirement_" + Request.QueryString["equipmentID"].ToString() + "_" + Request.QueryString["BranchID"].ToString()+ ".pdf";
                ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);
               // Response.Redirect("temppdfs/" + filename);
            }
        }


        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }

        public static IQueryable<EquipmentRetirementDetailsResult> EquipmentRetirementDetails(int BranchID, int equipmentID)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            
            return context.EquipmentRetirementDetails(equipmentID,BranchID).AsQueryable();
        }

    protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
    {
      //  string filename = "EquipmentRetirement_" + Request.QueryString["equipmentID"].ToString() + "_" + Request.QueryString["BranchID"].ToString() + ".pdf";
        ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);


    }
    }
}