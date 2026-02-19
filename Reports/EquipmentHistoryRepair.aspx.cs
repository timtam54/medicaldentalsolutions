using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class EquipmentHistoryRepair : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportParameter rp = new ReportParameter("CompanyName", MDS.Controllers.LoginController.CompanyName);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });

            }
            else
            {
                string xx = footfloat.Text;
                ReportViewer1.Height = 2500;// Convert.ToInt32(xx) - 70;
            }
            ReportViewer1.Drillthrough += ReportViewer1_Drillthrough;
        }

        void ReportViewer1_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e)
        {
            Object oo = getValue(e, "RepairUID");
            Object BID = getValue(e, "BranchID");
            
            Response.Redirect("RepairDetailRpt.aspx?repairid=" + oo.ToString() + "&branchid=" + BID.ToString());
        }

        public static object getValue(Microsoft.Reporting.WebForms.DrillthroughEventArgs e, string Name)
        {
            try
            {
                Microsoft.Reporting.WebForms.LocalReport lr = (Microsoft.Reporting.WebForms.LocalReport)e.Report;
                foreach (Microsoft.Reporting.WebForms.ReportParameter d in lr.OriginalParametersToDrillthrough)
                {
                    if (d.Name.ToLower() == Name.ToLower())
                        return d.Values[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                // data.LogError(ex, "DebtorsBatchTrans.aspx.cs.getvalue");
                return null;
            }
        }


        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }

    public static IQueryable<RepairsSearchResult> RepairsSearch(int BranchID,int equipmentID)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            
            char E = Convert.ToChar("E");

            return context.RepairsSearch(new DateTime(2000, 1, 1), DateTime.Now, "", "", BranchID, E, E, -1, equipmentID, "", "", -1, "", "", new DateTime(2000, 1, 1), DateTime.Now, false,false,1000).AsQueryable();
        }

    protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
    {
       // string filename = "EquipmentHistoryRepairs.pdf";
        ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);


    }
    }
}