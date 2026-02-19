using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MDS.Reports
{
    public partial class ServiceWorkOrderList : System.Web.UI.Page
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
                ReportViewer1.Height = 3000;// Convert.ToInt32(xx) - 70;
                if (Request.QueryString["Excel"] == null)
                {
                   // string filename = "ServiceWorkOrderList_" + Request.QueryString["BranchID"].ToString() + ".pdf";
                    ReportGlobal.CreateDownloadFile(Server, ReportViewer1, Response);
                   // Response.Redirect("temppdfs/" + filename);
                }
                else
                {
                  //  string filename = "ServiceWorkOrderList_" + Request.QueryString["BranchID"].ToString() + ".xls";
                    ReportGlobal.CreateDownloadExcel(Server, ReportViewer1, Response);
                  //  Response.Redirect("temppdfs/" + filename);
                }
            }
        }

        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = this.GetType().AssemblyQualifiedName;
        }
        public static IQueryable<ServiceWorkOrderSearchResult> ServiceWorkOrderSearch(string customerID, string equipmentType, int BranchID, bool dataEntryIncompleteOnly, int serviceJobID, int equipmentID, string department, string location, int engineerID, DateTime servicesAfterDate, bool serviceAfterDateFilter,int Cnt)
        {
            ReportsDBDataContext context = new ReportsDBDataContext();
            context.CommandTimeout = 120;
            if (location == "--Location--")
                location = "";
            if (department == "--Select Customer Site--")
                department = "";
            return context.ServiceWorkOrderSearch(equipmentType, serviceJobID, customerID, department, engineerID, BranchID, dataEntryIncompleteOnly, servicesAfterDate, location, equipmentID,serviceAfterDateFilter,Cnt).AsQueryable();
        }

        protected void pdfDownload_Click(object sender, ImageClickEventArgs e)
        {
           // string filename = "ServiceWorkOrderList.pdf";
            ReportGlobal.CreateDownloadFile( Server, ReportViewer1, Response);

        }

    }
}