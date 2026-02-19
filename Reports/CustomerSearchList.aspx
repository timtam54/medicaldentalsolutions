<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerSearchList.aspx.cs" Inherits="MDS.Reports.CustomerSearchList" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="../Scripts/Kendo/jquery.min.js"></script>
    <script src="../Scripts/Kendo/kendo.all.min.js"></script>   
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <table style="width:100%">
                <tr  style="width:100%">
                    <td style="text-align:left">
            <b>Print pdf:</b><asp:ImageButton ID="pdfDownload" runat="server" ImageUrl="~/Reports/pdf.jpg" OnClick="pdfDownload_Click" style="height: 27px" />
                </td>
                    <td>
                    <a style="text-align:right"  href="javascript: window.close();" ><img style="text-align:right" src="close.jpg" /></a>  
                </td>
                </tr>
        </table>


        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" Width="100%" Height="300px" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportEmbeddedResource="MDS.Reports.CustomerResultsList.rdlc" ReportPath="Reports/CustomerResultsList.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>   
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="CustomerSearch"  OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:QueryStringParameter Name="companyNameContains"  QueryStringField="companyNameContains" Type="String" />
                <asp:QueryStringParameter Name="customerCodeContains"  QueryStringField="customerCodeContains" Type="String" />
                    <asp:QueryStringParameter Name="companyNameStarts"  QueryStringField="companyNameStarts" Type="String" />
                    <asp:QueryStringParameter Name="contractsOverDueOrDueThisMonth"  QueryStringField="contractsOverDueOrDueThisMonth" Type="Boolean" />
                <asp:QueryStringParameter Name="BranchID"  QueryStringField="BranchID" Type="Int32" />
                       </SelectParameters>
        </asp:ObjectDataSource>
                       <asp:TextBox ID="footfloat" Width="0" Height="0" runat="server"></asp:TextBox>   
 <script src="ReportViewerVertSize.js" type="text/javascript"></script>

        </div>
    </form>
</body>
</html>
