<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    Codebehind="ServerRulePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerRules.ServerRulePage"
    Title="Configure Server Rules" ValidateRequest="false" %>

<%@ Register Src="AddEditServerRuleDialog.ascx" TagName="AddEditServerRuleDialog" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Server Rules</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="ServerRulePageUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ccAsp:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                <ccAsp:ConfirmationDialog ID="ConfirmDialog" runat="server" />
                <localAsp:AddEditServerRuleDialog ID="AddEditServerRuleControl" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
