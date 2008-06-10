<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoRouteWorkQueueDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.AutoRouteWorkQueueDetailsView" %>

<ccAsp:SectionPanel ID="AutoRouteInfoSectionPanel" runat="server" HeadingText="Auto-Route" HeadingCSS="CSSDefaultSectionHeading">
<SectionContentTemplate>
    <asp:DetailsView ID="AutoRouteDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4"
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="100%">
    <Fields>
        <asp:TemplateField HeaderText="Type">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Type" runat="server"  Text='<%# Eval("Type.Description") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Destination">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Destination" runat="server" Text='<%# Eval("DestinationAE") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>  
        <asp:TemplateField HeaderText="Study Instance UID">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="StudyInstanceUid" runat="server" Text='<%# Eval("StudyInstanceUid") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>      
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>
</SectionContentTemplate>
</ccAsp:SectionPanel>

<ccAsp:SectionPanel ID="GeneralInfoSectionPanel" runat="server" HeadingText="General Information" HeadingCSS="CSSDefaultSectionHeading">
<SectionContentTemplate>
    <asp:DetailsView ID="GeneralInfoDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4"
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="100%">
    <Fields>
    
        <asp:TemplateField HeaderText="Scheduled Date/Time">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ScheduledDateTime" runat="server"  Value='<%# Eval("ScheduledDateTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Expiration Date/Time">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ExpirationTime" runat="server"  Value='<%# Eval("ExpirationTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Status">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Status" runat="server" Text='<%# Eval("Status.Description") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Priority">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Priority" runat="server" Text='<%# Eval("Priority.Description") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="Processing Server" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="ServerDescription" />
                
        <asp:TemplateField HeaderText="Patient Name">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />        
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("Study.PatientName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Patient ID">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />        
            <ItemTemplate>
                <asp:Label runat="server" ID="PatientID" Text='<%# Eval("Study.PatientId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
       
        <asp:BoundField HeaderText="Failure Count" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="FailureCount" />
        <asp:BoundField HeaderText="Failure Description" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="FailureDescription" />
        <asp:BoundField HeaderText="Number of Instances Pending" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="NumInstancesPending" />
        <asp:BoundField HeaderText="Number of Series Pending" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="NumSeriesPending" />
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>
</SectionContentTemplate>
</ccAsp:SectionPanel>



