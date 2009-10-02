<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThrottleSettingsTab.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.ThrottleSettingsTab" %>

<script type="text/javascript">
    Sys.Application.add_load(page_load);
    function page_load() {
        var limitedCB = $("#<%= LimitedCheckBox.ClientID %>");
        var unlimitedCB = $("#<%= UnlimitedCheckBox.ClientID %>");
        var textInput = $("#<%=MaxConnectionTextBox.ClientID %>");

        if (unlimitedCB.is(":checked")) {
            textInput.hide();
        }

        unlimitedCB.click(function(ev) {
            $("#<%=MaxConnectionTextBox.ClientID %>").hide();
            $("#<%=InvalidRangeIndicator.ClientID %>").hide();
            $("#<%=MaxConnectionTextBox.ClientID %>").val("");
        });

        limitedCB.click(function(ev) {
            var textInput = $("#<%=MaxConnectionTextBox.ClientID %>");
            textInput.show();
            textInput.focus();
            textInput.select();
        });
    }
</script>

<asp:Panel runat="server" CssClass="DeviceSettingThrottleTab-GroupPanel">
    <div class="DialogMessagePanel" style="width: 460px;">
        Specify the maximum number of simultaneous connections Image Server can initiate
        for this device.
    </div>
    <table width="100%">
        <tr>
            <td>
                <span style="white-space: nowrap">Number of Connections Allowed:</span>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:RadioButton runat="server" ID="UnlimitedCheckBox" GroupName="MaxConnection"
                                Text="Unlimited" />
                        </td>
                        <td>
                            <asp:RadioButton runat="server" ID="LimitedCheckBox" GroupName="MaxConnection" Text="Limited" />
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" Width="20" ID="MaxConnectionTextBox" CssClass="DialogTextBox"
                                            ValidationGroup="ThrottleSettingsValidationGroup"></asp:TextBox>
                                    </td>
                                    <td>
                                        <ccAsp:InvalidInputIndicator ID="InvalidRangeIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
<ccValidator:RangeValidator ID="MaxConnectionTextBoxValidator" runat="server" ConditionalCheckBoxID="LimitedCheckBox"
    ControlToValidate="MaxConnectionTextBox" InvalidInputIndicatorID="InvalidRangeIndicator"
    ValidationGroup="ThrottleSettingsValidationGroup" Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"
    MinValue="1" MaxValue="100" Text="The value must be between 1 and 100"></ccValidator:RangeValidator>
