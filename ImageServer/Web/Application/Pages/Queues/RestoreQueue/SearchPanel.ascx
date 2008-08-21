<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.SearchPanel" %>

<%@ Register Src="RestoreQueueItemList.ascx" TagName="RestoreQueueItemList" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>

            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right" VerticalAlign="Bottom" >
                    
                       <table cellpadding="0" cellspacing="0"  width="100%">
                            <!-- need this table so that the filter panel container is fit to the content -->
                            <tr>
                                <td align="left">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>

                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Patient Name" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Patient Name" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Patient ID" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Patient Id" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="Schedule" CssClass="SearchTextBoxLabel" EnableViewState="false"/>
                                                <asp:LinkButton ID="ClearScheduleDateButton" runat="server" Text="X" CssClass="SmallLink"/><br />
                                                <ccUI:TextBox ID="ScheduleDate" runat="server" CssClass="SearchTextBox" ReadOnly="true" ToolTip="Search the list by Schedule Date [dd/mm/yyyy]" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Status" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList" ToolTip="Search the list by Status" />
                                            </td>                
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><asp:ImageButton ID="SearchButton" runat="server" SkinID="SearchButton" OnClick="SearchButton_Click" /></asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <aspAjax:CalendarExtender ID="ScheduleDateCalendarExtender" runat="server" TargetControlID="ScheduleDate"
                            CssClass="Calendar">
                        </aspAjax:CalendarExtender>

                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="DeleteItemButton" runat="server" SkinID="DeleteButton" OnClick="DeleteItemButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:RestoreQueueItemList id="RestoreQueueItemList" runat="server" Height="500px"></localAsp:RestoreQueueItemList></td></tr>
                                <tr><td style="border-top: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerBottom" runat="server" /></td></tr>                    
                            </table>                        
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>

        <ccAsp:MessageBox ID="MessageBox" runat="server" />    
        <ccAsp:MessageBox ID="RestoreMessageBox" runat="server" />    
    </ContentTemplate>
</asp:UpdatePanel>