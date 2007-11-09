<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ImageServerWebApplication.Admin.Configuration.AddDeviceDialog" Codebehind="AddDeviceDialog.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

<asp:Panel ID="DialogPanel" runat="server"
                 CssClass="PopupWindow" Width="442px" style="display:none">
                <asp:Panel ID="TitleBarPanel" runat="server" CssClass="PopupWindowTitleBar"
                    Width="100%">
                    <table style="width: 100%">
                        <tr>
                            <td valign="middle">
                <asp:Label ID="TitleLabel" runat="server" Text="Add Device" Width="100%" EnableViewState="False"></asp:Label></td>
                        </tr>
                    </table>
                </asp:Panel>
                <div class="PopupWindowBody" style="vertical-align: top; text-align: center">
                        <br />
                        <table id="TABLE1" runat="server" cellspacing="4" style="width: 90%; text-align: left">
                            <tr>
                                <td style="width: 162px">
                                AE Title</td>
                                <td colspan="3">
                                    <asp:TextBox ID="AETitleTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 29px">
                                IP Address</td>
                                <td colspan="3" style="height: 29px">
                                    <asp:TextBox ID="IPAddressTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                                <td style="width: 3px; height: 29px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px">
                                Port</td>
                                <td colspan="3">
                                    <asp:TextBox ID="PortTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px">
                                Description</td>
                                <td colspan="3">
                                    <asp:TextBox ID="DescriptionTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                Server Partition</td>
                                <td align="left" colspan="3" style="height: 24px">
                                    <asp:DropDownList ID="ServerPartitionDropDownList" runat="server"  DataTextField="Description"
                                        DataValueField="Key" Width="100%">
                                    </asp:DropDownList></td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 22px;">
                                </td>
                                <td style="width: 128px; height: 22px;">
                                    <asp:CheckBox ID="ActiveCheckBox" runat="server" Text="Active" />
                                </td>
                                <td style="width: 128px; height: 22px;">
                                    <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="DHCP" /></td>
                                <td align="left" style="height: 22px">
                                    &nbsp;
                                </td>
                                <td style="width: 3px; height: 22px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                </td>
                                <td style="width: 128px; height: 24px">
                                </td>
                                <td style="width: 128px; height: 24px">
                                </td>
                                <td align="left" style="height: 24px">
                                </td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                    &nbsp;</td>
                                <td style="width: 128px; height: 24px">
                                    <asp:Button ID="OKButton" runat="server" Text="Add" Width="77px" OnClick="OKButton_Click" /></td>
                                <td style="width: 128px; height: 24px">
                                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
                                <td align="left" style="height: 24px">
                                </td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                        </table>
                    <br />
                        <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="36px" style="z-index: 100; left: 16px; position: absolute; top: 260px">
                        </asp:Panel>
                </div>
            </asp:Panel>
            <cc1:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="MyStupidExtender"  runat="server" Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" BackgroundCssClass ="modalBackground">
            </cc1:ModalPopupExtender>
        <cc1:ToggleButtonExtender ID="ToggleButtonExtender1" runat="server" CheckedImageUrl="~/images/checked_tall.gif"
            ImageHeight="16" ImageWidth="12" TargetControlID="ActiveCheckBox" UncheckedImageUrl="~/images/unchecked_tall.gif">
        </cc1:ToggleButtonExtender>
        <cc1:ToggleButtonExtender ID="ToggleButtonExtender2" runat="server" CheckedImageUrl="~/images/checked_tall.gif"
            ImageHeight="16" ImageWidth="12" TargetControlID="DHCPCheckBox" UncheckedImageUrl="~/images/unchecked_tall.gif">
        </cc1:ToggleButtonExtender>
    </ContentTemplate>
</asp:UpdatePanel>
