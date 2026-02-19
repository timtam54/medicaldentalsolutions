<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RepairDetailRpt.aspx.cs" Inherits="MDS.Reports.RepairDetailRpt" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Repair Detail Report</title>
    <script src="../Scripts/Kendo/jquery.min.js"></script>
    <script src="../Scripts/Kendo/kendo.all.min.js"></script>   
</head>
<body>
    <form id="form1" runat="server">
          <div>
            <table style="width:100%">
                <tr  style="width:100%">
                    <td style="text-align:left">
            <b>Print pdf:</b><asp:ImageButton ID="pdfDownload" runat="server" ImageUrl="~/Reports/pdf.jpg" OnClick="pdfDownload_Click" />
                </td>
                    <td>
                    <a style="text-align:right"  href="javascript: window.close();" ><img style="text-align:right" src="close.jpg" /></a>  
                </td>
                </tr>
                </table>

        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" Width="100%" Height="800px" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportEmbeddedResource="MDS.Reports.RepairDetRpt.rdlc" ReportPath="Reports/RepairDetRpt.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>   
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="RepairReport"  OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:QueryStringParameter Name="repairID" DefaultValue="-2127048373" QueryStringField="repairID" Type="Int32" />
                <asp:QueryStringParameter Name="branchID" DefaultValue="1" QueryStringField="branchID" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
                     <asp:TextBox ID="footfloat" Width="0" Height="0" runat="server"></asp:TextBox>   
 <script src="ReportViewerVertSize.js" type="text/javascript"></script>

        </div>
    </form>
</body>
</html>
