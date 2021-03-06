<%@ Import Namespace="ClearCanvas.ImageServer.Core.Edit" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Common.CommandProcessor" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistoryDetailsColumn.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditHistoryDetailsColumn" %>

<script type="text/javascript">

    $(document).ready(function() {
        $("#<%=HistoryDetailsPanel.ClientID%>").hide();
        $("#<%=ShowHideDetails.ClientID%>").click(function() {
            if ($("#<%=ShowHideDetails.ClientID%>").text() == "[Show Details]") {
                $("#<%=HistoryDetailsPanel.ClientID%>").show();
                $("#<%=ShowHideDetails.ClientID%>").text("[Hide Details]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "bold");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "5px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");                
            } else {
                $("#<%=HistoryDetailsPanel.ClientID%>").hide();
                $("#<%=ShowHideDetails.ClientID%>").text("[Show Details]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "normal");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "0px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");                
            }
            return false;
        });
    });

</script>

<div id="SummaryPanel" runat="server">
    <%# String.Format("{0} by {1}", HtmlUtility.GetEnumInfo(EditHistory.EditType).LongDescription, EditHistory.UserId ?? "Unknown")%>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[Show Details]</a>
</div>

<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                Reason:
            </td>
            <td align="left">
                <%= GetReason(EditHistory.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                Comment:
            </td>
            <td align="left">
                <%= GetComment(EditHistory.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel" style="padding-top: 8px;">
                Changes
            </td>
            <td align="left">
            </td>
        </tr>
    </table>
    <div style="border-bottom: dashed 1px #999999; margin-top: 3px;">
    </div>

                    <% if (EditHistory.UpdateCommands == null || EditHistory.UpdateCommands.Count == 0)
                       {%>
    <table class="ChangeHistorySummary" width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td>
                    <pre style="padding-left: 10px">Study was not changed.</pre>
            </td>
        </tr>
   </table>
                    <%}
                       else
                       {%>
                    <div style="padding: 2px;">                       
                    <table width="100%" cellspacing="0" >
                        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
                            <td>
                                <b>Tag</b>
                            </td>
                            <td>
                                <b>Original Value</b>
                            </td>
                            <td>
                                <b>New Value</b>
                            </td>
                        </tr>
                        <%{
                              foreach (BaseImageLevelUpdateCommand cmd in EditHistory.UpdateCommands)
                              {
                                  IUpdateImageTagCommand theCmd = cmd as IUpdateImageTagCommand;
                                  if (theCmd != null)
                                  { %><tr style="background: #fefefe">
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.TagPath.Tag) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.OriginalValue) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.Value!=null? theCmd.UpdateEntry.Value.ToString(): "") %></pre>
                                                      </td>
                                                  </tr>
                                <%} %>
                            <%}%>
                        <%}%>
                    </table>
                    </div>
                    <%}%>
</div>
