<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewerPage.aspx.cs" Inherits="SSRSPro.ReportViewerPage" Async="true" ValidateRequest="false" EnableEventValidation="false"%>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" /> 
    <title></title>
        <style type="text/css">
        html, body, form { width: 100%; height: 100%; margin: 0; padding: 0 }
    </style>
   
</head>
<body>
    <div id="ErrorText" style="display:none"></div>
    <asp:Panel ID="Panel1" runat="server" style="height:100%;width:100%">
    <form id="Form1" runat="server" style="height:100%;width:100%" >        
        <div id="ViewerContainer" runat="server">  
            <asp:ScriptManager runat="server"  AsyncPostBackTimeOut="56000"></asp:ScriptManager>  
            <asp:hiddenfield id="ParameterValuesPost" value=""  runat="server"/>
            <asp:hiddenfield id="OldParameterValuesPost" value="NotEmpty"  runat="server"/>
            <asp:hiddenfield id="ReportPathPost" value=""  runat="server"/>
            <asp:hiddenfield id="DrillThroughParams" value=""  runat="server"/>
            
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Local" Height="100%" Width="100%" AsyncRendering="true" SizeToReportContent="true" EnableTelemetry="false"  ShowToolBar ="true" > </rsweb:ReportViewer>
        </div>
    </form>
    </asp:Panel>

</body>
</html>
