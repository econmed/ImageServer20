<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Login._Default" %>
<%@ Import namespace="ClearCanvas.ImageServer.Common"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="ChangePasswordDialog.ascx" TagName="ChangePasswordDialog" TagPrefix="localAsp" %>
<%@ Register Src="PasswordExpiredDialog.ascx" TagName="PasswordExpiredDialog" TagPrefix="localAsp" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link id="Link1" rel="shortcut icon" type="image/ico" runat="server" href="~/Images/favicon.ico" />
</head>
<body class="LoginBody">
    
    <form runat="server">

    <ccAsp:CheckJavascript runat="server" ID="CheckJavascript" />            

    <asp:ScriptManager ID="GlobalScriptManager" runat="server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true">
    </asp:ScriptManager>    
                
    <asp:Panel ID="LoginSplash" DefaultButton="LoginButton" runat="server">

        <div id="VersionInfoPanel" style="text-align: right">
            <table cellpadding="1">
            <tr><td align="right">Version:</td><td align="left"><%= String.IsNullOrEmpty(ServerPlatform.VersionString) ? "Unknown" : ServerPlatform.VersionString%></td></tr>
            <tr><td align="right">Mode:</td><td><%= EnterpriseMode ? "Enterprise" : "Stand-alone"%></td></tr>
            </table>
        </div>            

    
        <div id="LoginCredentials">
        
        <table>      
            <tr>
            <td align="right">User ID:</td>
            <td align="right"><asp:TextBox runat="server" ID="UserName" Width="100" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr>
            <tr>
            <td align="right">Password:</td>
            <td align="right"><asp:TextBox runat="server" ID="Password" TextMode="Password" Width="100" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr> 
            <tr>
                <td colspan="2" align="right"><asp:Button runat="server" ID="LoginButton" OnClick="LoginClicked"  Text="Login" CssClass="LoginButton"/></td>
            </tr>               
            <tr>
                <td colspan="2" align="right" ><asp:LinkButton ID="LinkButton1" runat="server" CssClass="LoginLink" OnClick="ChangePassword">Change Password</asp:LinkButton></td>            
            </tr>
        </table>
          
        </div>
        
                        <asp:Panel CssClass="LoginErrorMessagePanel" runat="server" ID="ErrorMessagePanel" 
                        Visible='<%# !String.IsNullOrEmpty(Page.Request.QueryString["error"]) %>'>
                        <asp:Label runat="server" ID="ErrorMessage" ForeColor="red" Text='<%# Page.Request.QueryString["error"] %>' />
        </asp:Panel>  
                        
            
    </asp:Panel>       
    

    
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <localAsp:ChangePasswordDialog runat="server" id="ChangePasswordDialog" />
            <localAsp:PasswordExpiredDialog runat="server" id="PasswordExpiredDialog" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    </form>
</body>
</html>
