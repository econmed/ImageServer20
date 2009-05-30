<%@ Import namespace="ClearCanvas.ImageServer.Core.Validation"%>
<%@ Import namespace="ClearCanvas.ImageServer.Services.WorkQueue"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueAlertContextDataView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.WorkQueueAlertContextDataView" %>

<%  WorkQueueAlertContextData data = this.Alert.ContextData as WorkQueueAlertContextData;
    String viewWorkQueueUrl = HtmlUtility.ResolveWorkQueueDetailsUrl(Page, data.WorkQueueItemKey);
    String viewStudyUrl = data.StudyInfo != null? HtmlUtility.ResolveStudyDetailsUrl(Page, data.StudyInfo.ServerAE, data.StudyInfo.StudyInstaneUid):null;
    
%>


<div >
<table class="WorkQueueAlertStudyTable">
<% if (data.StudyInfo!=null) { %>
<tr><td>Partition</td><td><%= data.StudyInfo.ServerAE %></td></tr>
<tr><td>Patient's Name</td><td><pre><%= data.StudyInfo.PatientsName%></pre></td></tr>
<tr><td>Patient ID</td><td><pre><%= data.StudyInfo.PatientsId %></pre></td></tr>
<tr><td>Study Instance UID</td><td><%= data.StudyInfo.StudyInstaneUid%></td></tr>
<tr><td>Accession #</td><td><pre><%= data.StudyInfo.AccessionNumber%></pre></td></tr>
<tr><td>Study Date</td><td><pre><%= data.StudyInfo.StudyDate%></pre></td></tr>
<%} else {%>
<tr><td>Study</td><td>N/A</td></tr>
<%} %>

</table>

<table>
    <tr>
        <% if (data.StudyInfo!=null){%>
        <td style="border-bottom:none; background-color:Transparent"><a href='<%=viewStudyUrl%>' target="_blank">View Study</a></td>
        <td style="border-bottom:none; background-color:Transparent">|</td>
        <%}%>
        <td style="border-bottom:none; background-color:Transparent"><a href='<%= viewWorkQueueUrl %>' target="_blank">View Work Queue</a></td>
    </tr>
</table>

</div>
