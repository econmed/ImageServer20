using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolGroupEditorComponentViewExtensionPoint))]
    public class ProtocolGroupEditorComponent : ApplicationComponent
    {
        #region Private fields

        private EntityRef _protocolGroupRef;
        private ProtocolGroupSummary _protocolGroupSummary;
        private ProtocolGroupDetail _protocolGroupDetail;

        private readonly bool _isNew;

        private ProtocolCodeTable _availableProtocolCodes;
        private ProtocolCodeTable _selectedProtocolCodes;
        private ProtocolCodeDetail _selectedProtocolCodesSelection;

        private SimpleActionModel _selectedProtocolCodesActionHandler;
        private readonly string _moveCodeUpKey = "MoveCodeUp";
        private readonly string _moveCodeDownKey = "MoveCodeDown";
        private readonly string _newCodeKey = "NewCode";

        private RequestedProcedureTypeGroupSummaryTable _availableReadingGroups;
        private RequestedProcedureTypeGroupSummaryTable _selectedReadingGroups;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupEditorComponent()
        {
            _isNew = true;
        }

        public ProtocolGroupEditorComponent(EntityRef protocolGroupRef)
        {
            _isNew = false;
            _protocolGroupRef = protocolGroupRef;
        }

        #endregion

        #region ApplicationComponent overrides

        public override void Start()
        {
            _availableProtocolCodes = new ProtocolCodeTable();
            _selectedProtocolCodes = new ProtocolCodeTable();

            _selectedProtocolCodesActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _selectedProtocolCodesActionHandler.AddAction(_moveCodeUpKey, SR.TitleMoveProtocolCodeUp, "Icons.UpToolSmall.png", SR.TitleMoveProtocolCodeUp, MoveProtocolCodeUp);
            _selectedProtocolCodesActionHandler.AddAction(_moveCodeDownKey, SR.TitleMoveProtocolCodeDown, "Icons.DownToolSmall.png", SR.TitleMoveProtocolCodeDown, MoveProtocolCodeDown);
            _selectedProtocolCodesActionHandler.AddAction(_newCodeKey, SR.TitleNewProtocolCode, "Icons.AddToolSmall.png", SR.TitleNewProtocolCode, AddNewProtocolCode);
            _selectedProtocolCodesActionHandler[_moveCodeUpKey].Enabled = false;
            _selectedProtocolCodesActionHandler[_moveCodeDownKey].Enabled = false;
            _selectedProtocolCodesActionHandler[_newCodeKey].Enabled = true;

            _availableReadingGroups = new RequestedProcedureTypeGroupSummaryTable();
            _selectedReadingGroups = new RequestedProcedureTypeGroupSummaryTable();

            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                    {
                        GetProtocolGroupEditFormDataRequest request = new GetProtocolGroupEditFormDataRequest();
                        GetProtocolGroupEditFormDataResponse editFormDataResponse = service.GetProtocolGroupEditFormData(request);
                        
                        _availableProtocolCodes.Items.AddRange(editFormDataResponse.ProtocolCodes);
                        _availableReadingGroups.Items.AddRange(editFormDataResponse.ReadingGroups);

                        if(_isNew)
                        {
                            _protocolGroupDetail = new ProtocolGroupDetail();
                        }
                        else
                        {
                            LoadProtocolGroupForEditResponse response =
                                service.LoadProtocolGroupForEdit(new LoadProtocolGroupForEditRequest(_protocolGroupRef));

                            _protocolGroupDetail = response.Detail;

                            _selectedProtocolCodes.Items.AddRange(_protocolGroupDetail.Codes);
                            _selectedReadingGroups.Items.AddRange(_protocolGroupDetail.ReadingGroups);
                        }

                        foreach (ProtocolCodeDetail item in _selectedProtocolCodes.Items)
                        {
                            _availableProtocolCodes.Items.Remove(item);
                        }

                        foreach (RequestedProcedureTypeGroupSummary item in _selectedReadingGroups.Items)
                        {
                            _availableReadingGroups.Items.Remove(item);
                        }
                    });

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Public Properties

        public ProtocolGroupSummary ProtocolGroupSummary
        {
            get { return _protocolGroupSummary; }
        }
        #endregion

        #region Presentation Model

        public string Name
        {
            get { return _protocolGroupDetail.Name; }
            set
            {
                _protocolGroupDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _protocolGroupDetail.Description; }
            set
            {
                _protocolGroupDetail.Description = value;
                this.Modified = true;
            }
        }

        public ITable AvailableProtocolCodes
        {
            get { return _availableProtocolCodes; }
        }

        public ITable SelectedProtocolCodes
        {
            get { return _selectedProtocolCodes; }
        }

        public ActionModelNode SelectedProtocolCodesActionModel
        {
            get { return _selectedProtocolCodesActionHandler; }
        }

        public ISelection SelectedProtocolCodesSelection
        {
            get { return new Selection(_selectedProtocolCodesSelection); }
            set
            {
                _selectedProtocolCodesSelection = (ProtocolCodeDetail)value.Item;
                SelectedProtocolCodesSelectionChanged();
            }
        }

        private void SelectedProtocolCodesSelectionChanged()
        {
            bool somethingSelected = _selectedProtocolCodesSelection != null;

            _selectedProtocolCodesActionHandler[_moveCodeUpKey].Enabled = somethingSelected;
            _selectedProtocolCodesActionHandler[_moveCodeDownKey].Enabled = somethingSelected;
        }

        public ITable AvailableReadingGroups
        {
            get { return _availableReadingGroups; }
        }

        public ITable SelectedReadingGroups
        {
            get { return _selectedReadingGroups; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveProtocolGroup, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        #region Private methods

        private void SaveChanges()
        {
            _protocolGroupDetail.Codes.Clear();
            _protocolGroupDetail.Codes.AddRange(_selectedProtocolCodes.Items);

            _protocolGroupDetail.ReadingGroups.Clear();
            _protocolGroupDetail.ReadingGroups.AddRange(_selectedReadingGroups.Items);

            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                {
                    if (_isNew)
                    {
                        AddProtocolGroupResponse response = service.AddProtocolGroup(new AddProtocolGroupRequest(_protocolGroupDetail));
                        _protocolGroupRef = response.Summary.EntityRef;
                        _protocolGroupSummary = response.Summary;
                    }
                    else
                    {
                        UpdateProtocolGroupResponse response = service.UpdateProtocolGroup(new UpdateProtocolGroupRequest(_protocolGroupRef, _protocolGroupDetail));
                        _protocolGroupRef = response.Summary.EntityRef;
                        _protocolGroupSummary = response.Summary;
                    }
                });
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion

        public void AddNewProtocolCode()
        {
            try
            {
                ProtocolCodeEditorComponent editor = new ProtocolCodeEditorComponent();
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, SR.TitleAddProtocolCode);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _selectedProtocolCodes.Items.Add(editor.ProtocolCode);
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void MoveProtocolCodeUp()
        {
            if(_selectedProtocolCodesSelection != null)
            {
                int index = _selectedProtocolCodes.Items.IndexOf(_selectedProtocolCodesSelection);
                if(index > 0)
                {
                    // Swap selected item with preceding item
                    _selectedProtocolCodes.Items[index] = _selectedProtocolCodes.Items[index - 1];
                    _selectedProtocolCodes.Items[index - 1] = _selectedProtocolCodesSelection;

                    // Ensures that UI updates and correct row is highlighted
                    NotifyPropertyChanged("SelectedProtocolCodesSelection");

                    this.Modified = true;
                }
            }
        }

        public void MoveProtocolCodeDown()
        {
            if (_selectedProtocolCodesSelection != null)
            {
                int index = _selectedProtocolCodes.Items.IndexOf(_selectedProtocolCodesSelection);
                if (index < _selectedProtocolCodes.Items.Count - 1)
                {
                    // Swap selected item with following item
                    _selectedProtocolCodes.Items[index] = _selectedProtocolCodes.Items[index + 1];
                    _selectedProtocolCodes.Items[index + 1] = _selectedProtocolCodesSelection;

                    // Ensures that UI updates and correct row is highlighted
                    NotifyPropertyChanged("SelectedProtocolCodesSelection");

                    this.Modified = true;
                }
            }
        }

    }
}
