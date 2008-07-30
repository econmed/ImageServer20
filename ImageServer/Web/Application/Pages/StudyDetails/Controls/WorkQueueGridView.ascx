<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.WorkQueueGridView" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <ccUI:GridView ID="StudyWorkQueueGridView" runat="server" 
                       AutoGenerateColumns="False" CssClass="GlobalGridView" 
                       CellPadding="0" CaptionAlign="Top" Width="100%" 
                       OnPageIndexChanged="StudyWorkQueueGridView_PageIndexChanged" 
                       OnPageIndexChanging="StudyWorkQueueGridView_PageIndexChanging" SelectionMode="Disabled"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                        <Columns>
                            <asp:BoundField DataField="WorkQueueTypeEnum" HeaderText="Type">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="ScheduledTime" HeaderText="Schedule">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="WorkQueuePriorityEnum" HeaderText="Priority">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="WorkQueueStatusEnum" HeaderText="Status">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="ProcessorID" HeaderText="Processing Server">
                                <HeaderStyle wrap="False" />  
                            </asp:BoundField>
                            <asp:BoundField DataField="FailureDescription" HeaderText="Notes">
                                <HeaderStyle wrap="False" />  
                            </asp:BoundField>                            
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell>Type</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Priority</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Status</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Processing Server</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Notes</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="6" Height="50" HorizontalAlign="Center">
                                        <asp:panel runat="server" CssClass="GlobalGridViewEmptyText">No Work Queue items for this study.</asp:panel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>   
    </ContentTemplate>
</asp:UpdatePanel>
