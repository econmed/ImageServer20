<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsTabs" %>

<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="localAsp" %>
<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="localAsp" %>
<%@ Register Src="WorkQueueGridView.ascx" TagName="WorkQueueGridView" TagPrefix="localAsp" %>
<%@ Register Src="FileSystemQueueGridView.ascx" TagName="FileSystemQueueGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyStorageView.ascx" TagName="StudyStorageView" TagPrefix="localAsp" %>

<aspAjax:TabContainer ID="StudyDetailsTabContainer" runat="server" ActiveTabIndex="0"
    CssClass="TabControl" Width="100%">
    <aspAjax:TabPanel ID="StudyDetailsTab" HeaderText="<%$Resources: Titles, StudyDetails %>" runat="server">
        <ContentTemplate>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="8" cellspacing="0" style="background-color: #B8D8EE;">
                            <tr><td>
                                <localAsp:StudyDetailsView ID="StudyDetailsView" runat="server" />
                            </td></tr>
                        </table>                        
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </ContentTemplate>
    </aspAjax:TabPanel>
        <aspAjax:TabPanel ID="SeriesDetailsTab" HeaderText="<%$Resources: Titles, SeriesDetails %>" runat="server">
        <ContentTemplate>
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="4" cellspacing="0" style="background-color: #B8D8EE;">
                            <tr>
                                <td>
                                    <div style="padding-top: 5px; padding-left: 1px;" /><ccUI:ToolbarButton runat="server" ID="ViewSeriesButton" SkinID="ViewDetailsButton" /></div></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <localAsp:SeriesGridView ID="SeriesGridView" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                 </asp:TableRow>
            </asp:Table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="WorkQueueTab" HeaderText="<%$Resources: Titles, WorkQueue %>" runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0" style="background-color: #B8D8EE;">
                <tr>
                    <td>
                        <localAsp:WorkQueueGridView ID="WorkQueueGridView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="FileSystemQueueTab" HeaderText="<%$Resources: Titles, FileSystemQueue %>" runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0" style="background-color: #B8D8EE;">
                <tr>
                    <td>
                        <localAsp:FileSystemQueueGridView ID="FSQueueGridView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="TabPanel1" HeaderText="<%$Resources: Titles, StudyStorage %>" runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0" style="background-color: #B8D8EE;">
                <tr>
                    <td>
                        <localAsp:StudyStorageView ID="StudyStorageView" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
    <aspAjax:TabPanel ID="TabPanel2" HeaderText="<%$Resources: Titles, Archive %>" runat="server">
        <ContentTemplate>
            <table width="100%" cellpadding="8" cellspacing="0" style="background-color: #B8D8EE;">
                <tr>
                    <td>

                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </aspAjax:TabPanel>
</aspAjax:TabContainer>
