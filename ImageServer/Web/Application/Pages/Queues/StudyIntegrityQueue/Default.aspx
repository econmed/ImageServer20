<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.Default"
    Title="Study Integrity Queue | ClearCanvas ImageServer" %>

<%@ Register Src="ReconcileDialog.ascx" TagName="ReconcileDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">Study Integrity Queue</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ccAsp:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" Visible="true" />
                <localAsp:ReconcileDialog ID="ReconcileDialog" runat="server" /> 
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>    
</asp:Content>
