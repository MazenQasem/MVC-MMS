<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, 
    PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>



<%@ Import Namespace="MMS2" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.Entity" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Reflection" %>

<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="Microsoft.Reporting.WebForms" %>
<%@ Import Namespace="System.Security" %>
<%@ Import Namespace="System.Security.Permissions" %>


<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>ReportViwer in MVC4 Application</title>

    <script runat="server">
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                                                                                                                                                                                                                                                                                                                  
                PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
                ReportViewer1.Reset();
                ReportViewer1.LocalReport.ReportPath = HttpContext.Current.Server.MapPath("~/ReportsRDLC/ItemStockBySupplier.rdl");
                ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(permissions);
                                 ReportViewer1.LocalReport.SetParameters(new ReportParameter("OptionTitle", "21"));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("SupplierTitle", "212"));
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", MISReptFun.SP_TO_DataSet(1268).Tables[0]));
                 





                ReportViewer1.DataBind();
                                 Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                byte[] bytes;
                bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType,
                out encoding, out filenameExtension, out streamids, out warnings);
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "Application/pdf";
                Response.AddHeader("content-length", bytes.Length.ToString());
                Response.BinaryWrite(bytes);


            }
        }

    </script>

</head>

<html>
<body>

    <%--  <form runat="server" id="Form1">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div>

            <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="false"
                SizeToReportContent="true" PageCountMode="Actual" Width="626px">
            </rsweb:ReportViewer>
        </div>
    </form>--%>
    <form id="Form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server"
            AsyncRendering="false"
            SizeToReportContent="true"
            Style="overflow: visible !important;"
            Height="100%" ShowFindControls="False"
            ShowRefreshButton="False"
            ShowBackButton="False">
        </rsweb:ReportViewer>
    </form>

</body>
</html>


