<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.Default"
%>

<%@ Register Src="DeletedStudiesSearchPanel.ascx" TagName="DeletedStudiesSearchPanel" TagPrefix="localAsp" %>
<%@ Register Src="DeletedStudyDetailsDialog.ascx" TagName="DetailsDialog" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,DeletedStudies%>" /></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <localAsp:DeletedStudiesSearchPanel ID="SearchPanel" runat="server"/>
            </asp:Panel>
            
        </ContentTemplate>
      
    </asp:UpdatePanel>
    
      <localAsp:DetailsDialog runat="server" ID="DetailsDialog" />
      <ccAsp:MessageBox runat="server" ID="DeleteConfirmMessageBox" 
                Title="Please Confirm" 
                Message="Are you sure you want to delete this record?<BR>You will have to delete all backup files manually." 
                MessageType="OKCANCEL" />
</asp:Content>

