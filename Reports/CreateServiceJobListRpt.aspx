<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateServiceJobListRpt.aspx.cs" Inherits="MDS.Reports.CreateServiceJobListRpt" %>

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
            <table style="width: 100%">
                <tr style="width: 100%">
                    <td style="text-align: left">
                        <b>Print pdf:</b><asp:ImageButton ID="pdfDownload" runat="server" ImageUrl="~/Reports/pdf.jpg" OnClick="pdfDownload_Click" Style="height: 27px" />
                    </td>
                    <td>
                        <a style="text-align: right" href="javascript:window.close();">
                            <img style="text-align: right" src="close.jpg" /></a>
                    </td>
                </tr>
            </table>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" Width="100%" Height="300px" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
                <LocalReport ReportEmbeddedResource="MDS.Reports.CreateServiceJobListRpt.rdlc" ReportPath="Reports/CreateServiceJobListRpt.rdlc">
                    <DataSources>
                        <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                    </DataSources>
                </LocalReport>
            </rsweb:ReportViewer>
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="CreateServiceJobSearch" OldValuesParameterFormatString="original_{0}">
                <SelectParameters>
                    <asp:QueryStringParameter Name="dueOption" QueryStringField="dueOption" Type="String" />
                    <asp:QueryStringParameter Name="monthYear" QueryStringField="monthYear" Type="Int32" />
                    <asp:QueryStringParameter Name="customerID" QueryStringField="customerID" Type="String" />
                    <asp:QueryStringParameter Name="BranchID" QueryStringField="BranchID" Type="Int32" />
                    <asp:QueryStringParameter Name="equipmentID" QueryStringField="equipmentID" Type="Int32" />
                    <asp:QueryStringParameter Name="department" QueryStringField="department" Type="String" />
                    <asp:QueryStringParameter Name="location" QueryStringField="location" Type="String" />
                    <asp:QueryStringParameter Name="Count" QueryStringField="Count" Type="Int32" />

                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:TextBox ID="footfloat" Width="0" Height="0" runat="server"></asp:TextBox>
            <script src="ReportViewerVertSize.js" type="text/javascript"></script>

        </div>
    </form>
</body>
</html>
