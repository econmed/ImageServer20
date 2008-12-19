<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.Default"
    Title="Alerts | Admin | ClearCanvas ImageServer" %>

<%@ Register Src="AlertsPanel.ascx" TagName="AlertsPanel" TagPrefix="localAsp" %>
<%@ Register Src="~/Pages/Admin/Alerts/DeleteAlertDialog.ascx" TagName="DeleteDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">Alerts</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <localAsp:AlertsPanel runat="server" ID="AlertsPanel" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>    
    <localAsp:DeleteDialog ID="DeleteAlertDialog" runat="server"  />
    <localAsp:DeleteDialog ID="DeleteAllAlertsDialog" runat="server" />
</asp:Content>