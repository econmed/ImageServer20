<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.ReconcileDialog"
    Codebehind="ReconcileDialog.ascx.cs" %>


<ccAsp:ModalDialog ID="ReconcileItemModalDialog" runat="server" Width="900px" Title='<%$ Resources:Titles, ReconcileStudyDialog %>'>
    <ContentTemplate> 
        <asp:Panel CssClass="StudyDetailsMessage" runat="server" ID="MessagePanel" Visible="false">
                <asp:Label ID="AlertMessage" runat="Server" Text="" />
            </asp:Panel>
        <aspAjax:TabContainer runat="server" ID="TabContainer"  Width="950px" ActiveTabIndex="0" CssClass="DialogTabControl">
            <aspAjax:TabPanel runat="server" id="OverviewTab" HeaderText="Overview" Height="100%" CssClass="DialogTabControl">
                <ContentTemplate>                    
            <div class="ReconcilePanel">
                <asp:Table runat="server">
                <asp:TableRow CssClass="ReconcileHeaderRow">
                    <asp:TableCell >Existing Study</asp:TableCell>
                    <asp:TableCell CssClass="Separator"><asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="1px" /></asp:TableCell>
                    <asp:TableCell><span class="ConflictingStudyTitle">Conflicting Study</span></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="3"><div class="StudyInstanceUIDMessage">Study Instance UID: <asp:Label ID="StudyInstanceUIDLabel" runat="server" Text='<%# ReconcileDetails.StudyInstanceUid %>'></asp:Label></div></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell>
                        <asp:Table runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <div class="StudyInformation">
                                     <table>
                                        <tr>
                                            <td width="130px" class="DialogLabelBackground"><asp:Label runat="server" CssClass="DialogTextBoxLabel">Patient Name</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="PreformattedLabel1" CssClass="StudyField"  Text='<%# ReconcileDetails.ExistingStudy.Patient.Name %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label321" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientID" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.PatientID %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label322" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientBirthDate" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.BirthDate %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label323" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ExistingAccessionNumber" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.AccessionNumber %>' /></td>
                                        </tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label324" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                            <td>
                                                <table cellpadding="0" cellspacing="0">
                                                    <tr>
														<td><asp:Label ID="ExistingPatientSex" runat="server" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.Sex %>'></asp:Label></td>
                                                        <td><ccAsp:InvalidInputIndicator ID="InvalidInputIndicator1" runat="server" SkinID="InvalidInputIndicator" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>                                                        
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label325" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientIssuerOfPatientID" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.IssuerOfPatientID %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label7" runat="server" CssClass="DialogTextBoxLabel">Study Date</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ExistingStudyDate" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.StudyDate %>' /></td>
                                        </tr>
                                    </table>
                                    </div>
                                    <table cellpadding="0" cellspacing="0" width="100%"><tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr></table>
                                    <div class="SeriesInformation">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr><td style="padding: 0px 12px 0px 4px;">
                                    <div class="ReconcileGridViewPanel">
                                        <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ExistingPatientSeriesGridView" width="440px" AutoGenerateColumns="false">
                                            <Columns>
						                        <asp:BoundField HeaderText="Number" DataField="SeriesNumber" />
						                        <asp:BoundField HeaderText="Description" DataField="Description" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
						                        <asp:BoundField HeaderText="Modality" DataField="Modality" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />                    
						                        <asp:BoundField HeaderText="Instances" DataField="NumberOfInstances" />
						                    </Columns>
						                    <RowStyle CssClass="ReconcileSeriesGridViewRow" />
                    						<HeaderStyle CssClass="ReconcileSeriesGridViewHeader" />
                                        </asp:GridView>
                                    </div>
                                        </td></tr>
                                    </table>
                                    </div>
                               </asp:TableCell>
                            </asp:TableRow>
                         </asp:Table>
                     </asp:TableCell>
                    <asp:TableCell CssClass="Separator"><asp:Image ID="Image2" runat="server" SkinID="Spacer" Width="2px" /></asp:TableCell>
                    <asp:TableCell>
                        <asp:Table runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <div class="StudyInformation">
                                     <table>
                                        <tr>
                                            <td width="130px" class="DialogLabelBackground"><asp:Label runat="server" CssClass="DialogTextBoxLabel">Patient Name</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ConflictingNameLabel" Width="300px" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingStudyInfo.Patient.Name)? "Not Specified": ReconcileDetails.ConflictingStudyInfo.Patient.Name%>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label1" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIDLabel" CssClass="StudyField" Text='<%# ReconcileDetails.ConflictingStudyInfo.Patient.PatientID %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label2" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientBirthDate" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingStudyInfo.Patient.BirthDate)? "Not Specified":ReconcileDetails.ConflictingStudyInfo.Patient.BirthDate %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ConflictingAccessionNumberLabel" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingStudyInfo.AccessionNumber)?"Not Specified":ReconcileDetails.ConflictingStudyInfo.AccessionNumber %>' /></td>
                                         </tr>
                                         <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                            <td>
                                                <table cellpadding="0" cellspacing="0">
                                                    <tr><td><asp:textbox ID="ConflictingPatientSex" runat="server" CssClass="StudyInfoField" BorderWidth="0" ReadOnly="true" Width="95" ValidationGroup="ReconcileValidationGroup" BorderStyle="None" BackColor="Transparent" Font-Size="14px" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingStudyInfo.Patient.Sex)?"Not Specified":ReconcileDetails.ConflictingStudyInfo.Patient.Sex %>'></asp:textbox></td>
                                                    <td><ccAsp:InvalidInputIndicator ID="UnknownSex" runat="server" SkinID="InvalidInputIndicator" />
                                                        <ccValidator:RegularExpressionFieldValidator ID="PatientSexValidator" runat="server" 
                                                            ControlToValidate="ConflictingPatientSex" ValidationGroup="ReconcileValidationGroup" InvalidInputIndicatorID="UnknownSex"
                                                            ValidationExpression="M$|m$|F$|f$|O$" Text="The value used for Merge Study<br/>will be Other (O)." Display="None">
                                                        </ccValidator:RegularExpressionFieldValidator>
                                                    </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIssuerOfPatientID" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingStudyInfo.Patient.IssuerOfPatientID)? "Not Specified":ReconcileDetails.ConflictingStudyInfo.Patient.IssuerOfPatientID %>' /></td>
                                        </tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label6" runat="server" CssClass="DialogTextBoxLabel">Study Date</asp:Label></td>
                                            <td><ccUI:PreformattedLabel runat="server" ID="ConflictingStudyDate" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingStudyInfo.StudyDate)?"Not Specified":ReconcileDetails.ConflictingStudyInfo.StudyDate %>' /></td>
                                        </tr>
                                    </table>
                                    </div>
                                    <table cellpadding="0" cellspacing="0" width="100%"><tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr></table>
                                    <div class="SeriesInformation">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr><td style="padding: 0px 12px 0px 4px;">
                                    <div class="ReconcileGridViewPanel">
                                        <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ConflictingPatientSeriesGridView" width="440px" AutoGenerateColumns="false">
                                            <Columns>
						                        <asp:BoundField HeaderText="Number" DataField="SeriesNumber" />
						                        <asp:BoundField HeaderText="Description" DataField="Description" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
						                        <asp:BoundField HeaderText="Modality" DataField="Modality" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />                    
						                        <asp:BoundField HeaderText="Instances" DataField="NumberOfInstances" />
						                    </Columns>
						                    <RowStyle CssClass="ReconcileSeriesGridViewRow" />
                    						<HeaderStyle CssClass="ReconcileSeriesGridViewHeader" />
                                        </asp:GridView>
                                    </div>
                                        </td></tr>
                                    </table>
                                    </div>
                               </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" ID="OptionRow">
                                <asp:TableCell style="padding: 0px 10px 10px 10px;">
                                    <table cellpadding="0" cellspacing="0" width="100%" class="ReconcileButtonsTable">
                                        <tr><td style="padding-left: 5px; padding-top: 5px;"><asp:radiobutton runat="server" ID="MergeUsingExistingStudy" Text=" Merge Using Existing Study" GroupName="ReconcileStudy" Checked="true"/></td><td><asp:radiobutton runat="server" ID="CreateNewStudy" Text=" Create New Study" GroupName="ReconcileStudy" CssClass="ReconcileRadioButton"/></td></tr>
                                        <tr><td style="padding-left: 5px;"><asp:radiobutton runat="server" ID="MergeUsingConflictingStudy" Text=" Merge Using Conflicting Study" GroupName="ReconcileStudy"/></td><td><asp:radiobutton runat="server" ID="DiscardStudy" Text=" Discard Study" GroupName="ReconcileStudy"/></td></tr>
                                        <tr><td style="padding-left: 5px;"></td><td><asp:radiobutton runat="server" ID="IgnoreConflict" Text=" Ignore (Process As Is)" GroupName="ReconcileStudy"/></td></tr>
                                        <tr><td colspan="2">&nbsp;</td></tr>
                                    </table>                                        
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>                                    
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            </div>
        </ContentTemplate>
            </aspAjax:TabPanel>
            <aspAjax:TabPanel runat="server" id="DetailsTab" HeaderText="Additional Info">
                <ContentTemplate>
                    <asp:Panel ID="Panel4" runat="server" Height="100%">
                        <asp:Panel ID="Panel5" runat="server" CssClass="AdditionalInformationPanel">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="AdditionalInfoSectionHeader FilesystemSectionHeader">Filesystem Locations</div>
                                </td>
                            </tr>
                            <tr>
                                <td class="DialogLabelBackground" style="margin-left:5px;"><asp:Label ID="Label10" runat="server" CssClass="DialogTextBoxLabel" Text="Study Location"></asp:Label></td>
                                <td ><asp:Label runat="server" ID="StudyLocation"></asp:Label></td>
                            </tr>
                            
                            <tr >
                                <td class="DialogLabelBackground" style="margin-left:5px;"><asp:Label ID="Label12" runat="server" CssClass="DialogTextBoxLabel" Text="Conflicting Study Location"></asp:Label></td>
                                <td><asp:Label runat="server" ID="ConflictingStudyLocation"></asp:Label></td>
                            </tr>
                        </table>                        
                        
                        </asp:Panel>
                        
                        
                    </asp:Panel>
                </ContentTemplate>
            </aspAjax:TabPanel>
        </aspAjax:TabContainer>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="OKButton" OnClick="OKButton_Click" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</ccAsp:ModalDialog>

<ccAsp:MessageBox runat="server" ID="MessageBox" />
