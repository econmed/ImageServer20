#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Defines an interface for providing custom documentation pages to be displayed in the documentation workspace.
    /// </summary>
    public interface ITechnologistDocumentationPageProvider
    {
        ITechnologistDocumentationPage[] GetDocumentationPages(ITechnologistDocumentationContext context);
    }

    /// <summary>
    /// Defines an extension point for adding custom documentation pages to the technologist documentation workspace.
    /// </summary>
    [ExtensionPoint]
    public class TechnologistDocumentationPageProviderExtensionPoint : ExtensionPoint<ITechnologistDocumentationPageProvider>
    {
    }

    /// <summary>
    /// Defines an interface for providing a custom documentation page with access to the documentation
    /// context.
    /// </summary>
    public interface ITechnologistDocumentationContext
    {
        /// <summary>
        /// Exposes the extended properties associated with the Order.  Modifications made to these
        /// properties by the documentation page will be persisted whenever the documentation workspace is saved.
        /// </summary>
        IDictionary<string, string> OrderExtendedProperties { get; }

        /// <summary>
        /// Gets the <see cref="ProcedurePlanDetail"/> representing this order.
        /// </summary>
        ProcedurePlanDetail ProcedurePlan { get; }

        /// <summary>
        /// Occurs when the value of the <see cref="ProcedurePlan"/> property changes.
        /// </summary>
        event EventHandler ProcedurePlanChanged;
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="TechnologistDocumentationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TechnologistDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TechnologistDocumentationComponent class
    /// </summary>
    [AssociateView(typeof(TechnologistDocumentationComponentViewExtensionPoint))]
    public class TechnologistDocumentationComponent : ApplicationComponent
    {
        #region TechnologistDocumentationContext class

        class TechnologistDocumentationContext : ITechnologistDocumentationContext
        {
            private readonly TechnologistDocumentationComponent _owner;

            public TechnologistDocumentationContext(TechnologistDocumentationComponent owner)
            {
                _owner = owner;
            }

            #region ITechnologistDocumentationContext Members

            public IDictionary<string, string> OrderExtendedProperties
            {
                get { return _owner._orderExtendedProperties; }
            }

            public ProcedurePlanDetail ProcedurePlan
            {
                get { return _owner._procedurePlan; }
            }

            public event EventHandler ProcedurePlanChanged
            {
                add { _owner._procedurePlanChanged += value; }
                remove { _owner._procedurePlanChanged -= value; }
            }


            #endregion
        }

        #endregion

        #region Private Members

        private readonly ModalityWorklistItem _worklistItem;
        private Dictionary<string, string> _orderExtendedProperties;

        private ProcedurePlanDetail _procedurePlan;
        private ProcedurePlanSummaryTable _procedurePlanSummaryTable;
        private event EventHandler _procedurePlanChanged;

        private SimpleActionModel _procedurePlanActionHandler;
        private ClickAction _startAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _bannerComponentHost;
        private ChildComponentHost _documentationHost;
        private TabComponentContainer _documentationTabContainer;

        private readonly List<ITechnologistDocumentationPage> _extensionPages = new List<ITechnologistDocumentationPage>();

        private PerformedProcedureComponent _ppsComponent;
        private TechnologistDocumentationOrderDetailsComponent _orderDetailsComponent;

        private bool _completeEnabled;
        private bool _saveEnabled = true;

        private event EventHandler _documentCompleted;
        private event EventHandler _documentSaved;

        #endregion

        public TechnologistDocumentationComponent(ModalityWorklistItem item)
        {
            _worklistItem = item;
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            InitializeProcedurePlanSummary();
            InitializeDocumentationTabPages();

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #endregion

        #region Presentation Model Methods

        public ApplicationComponentHost BannerHost
        {
            get { return _bannerComponentHost; }
        }

        public ApplicationComponentHost DocumentationHost
        {
            get { return _documentationHost; }
        }

        public ITable ProcedurePlanSummaryTable
        {
            get { return _procedurePlanSummaryTable; }
        }

        public event EventHandler ProcedurePlanChanged
        {
            add { _procedurePlanChanged += value; }
            remove { _procedurePlanChanged -= value; }
        }

        public ActionModelNode ProcedurePlanTreeActionModel
        {
            get { return _procedurePlanActionHandler; }
        }

        public void SaveDocumentation()
        {
            try
            {
                if (Save(false))
                    EventsHelper.Fire(_documentSaved, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool SaveEnabled
        {
            get { return _saveEnabled; }
        }

        public event EventHandler DocumentSaved
        {
            add { _documentSaved += value; }
            remove { _documentSaved -= value; }
        }

        public void CompleteDocumentation()
        {
            try
            {
                // validate first
                if (Save(true))
                    EventsHelper.Fire(_documentCompleted, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public bool CompleteEnabled
        {
            get { return _completeEnabled; }
        }

        public event EventHandler DocumentCompleted
        {
            add { _documentCompleted += value; }
            remove { _documentCompleted -= value; }
        }

        #endregion

        #region Action Handler Methods

        private void StartModalityProcedureSteps()
        {
            try
            {
                List<EntityRef> checkedMpsRefs = CollectionUtils.Map<ProcedurePlanSummaryTableItem, EntityRef, List<EntityRef>>(
                    ListCheckedSummmaryTableItems(),
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.ProcedureStepRef; });

                if (checkedMpsRefs.Count > 0)
                {
                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            StartModalityProcedureStepsRequest request = new StartModalityProcedureStepsRequest(checkedMpsRefs);
                            StartModalityProcedureStepsResponse response = service.StartModalityProcedureSteps(request);

                            RefreshProcedurePlanSummary(response.ProcedurePlan);
                            UpdateActionEnablement();

                            _ppsComponent.AddPerformedProcedureStep(response.StartedMpps);
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DiscontinueModalityProcedureSteps()
        {
            try
            {
                List<EntityRef> checkedMpsRefs = CollectionUtils.Map<ProcedurePlanSummaryTableItem, EntityRef, List<EntityRef>>(
                    ListCheckedSummmaryTableItems(),
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.ProcedureStepRef; });

                if (checkedMpsRefs.Count > 0)
                {
                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            DiscontinueModalityProcedureStepsRequest request = new DiscontinueModalityProcedureStepsRequest(checkedMpsRefs);
                            DiscontinueModalityProcedureStepsResponse response = service.DiscontinueModalityProcedureSteps(request);

                            RefreshProcedurePlanSummary(response.ProcedurePlan);
                            UpdateActionEnablement();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Private methods

        private bool Save(bool completeDocumentation)
        {
            // only do validation if they are completing the documentation, not if they are just saving a draft
            if(completeDocumentation)
            {
                if (this.HasValidationErrors)
                {
                    ShowValidation(true);
                    return false;
                }

                if (_documentationTabContainer.HasValidationErrors)
                {
                    _documentationTabContainer.ShowValidation(true);
                    return false;
                }
            }

            try
            {
                // allow extension pages to save data
                foreach(ITechnologistDocumentationPage page in _extensionPages)
                {
                    page.Save(completeDocumentation);
                }

                _ppsComponent.SaveData();
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                        {
                            // TODO clean this up - this is a bit ugly, not sure if there's a cleaner way
                            Dictionary<EntityRef, Dictionary<string, string>> ppsExtendedProperties
                                                 = new Dictionary<EntityRef, Dictionary<string, string>>();
                            foreach (ModalityPerformedProcedureStepSummary step in _ppsComponent.PerformedProcedureSteps)
                            {
                                ppsExtendedProperties[step.ModalityPerformendProcedureStepRef] = step.ExtendedProperties;
                            }


                            SaveDataRequest saveRequest =
                                new SaveDataRequest(_procedurePlan.OrderRef, _orderExtendedProperties, ppsExtendedProperties);
                            SaveDataResponse saveResponse = service.SaveData(saveRequest);

                            if (completeDocumentation)
                            {
                                CompleteOrderDocumentationRequest completeRequest =
                                    new CompleteOrderDocumentationRequest(saveResponse.ProcedurePlan.OrderRef);
                                CompleteOrderDocumentationResponse completeResponse =
                                    service.CompleteOrderDocumentation(completeRequest);

                                RefreshProcedurePlanSummary(completeResponse.ProcedurePlan);
                            }
                            else
                            {
                                RefreshProcedurePlanSummary(saveResponse.ProcedurePlan);
                            }
                        });

                return true;
            }
            catch(Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return false;
        }

        private void InitializeProcedurePlanSummary()
        {
            _procedurePlanSummaryTable = new ProcedurePlanSummaryTable();
            _procedurePlanSummaryTable.CheckedRowsChanged += delegate(object sender, EventArgs args) { UpdateActionEnablement(); };

            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    GetProcedurePlanForWorklistItemRequest procedurePlanRequest = new GetProcedurePlanForWorklistItemRequest(_worklistItem.ProcedureStepRef);
                    GetProcedurePlanForWorklistItemResponse procedurePlanResponse = service.GetProcedurePlanForWorklistItem(procedurePlanRequest);
                    _procedurePlan = procedurePlanResponse.ProcedurePlan;
                    _orderExtendedProperties = procedurePlanResponse.OrderExtendedProperties;
                });

            RefreshProcedurePlanSummary(_procedurePlan);

            InitializeProcedurePlanSummaryActionHandlers();
        }

        private void InitializeProcedurePlanSummaryActionHandlers()
        {
            _procedurePlanActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
            _startAction = _procedurePlanActionHandler.AddAction("start", SR.TitleStartMps, "Icons.StartToolSmall.png", SR.TitleStartMps, StartModalityProcedureSteps);
            _discontinueAction = _procedurePlanActionHandler.AddAction("discontinue", SR.TitleDiscontinueMps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMps, DiscontinueModalityProcedureSteps);
            UpdateActionEnablement();
        }

        private void InitializeDocumentationTabPages()
        {
            _bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_worklistItem));
            _bannerComponentHost.StartComponent();

            _documentationTabContainer = new TabComponentContainer();
            _documentationTabContainer.ValidationStrategy = new AllComponentsValidationStrategy();

            _orderDetailsComponent = new TechnologistDocumentationOrderDetailsComponent(_worklistItem, _orderExtendedProperties);
            _documentationTabContainer.Pages.Add(new TabPage("Order", _orderDetailsComponent));

            _ppsComponent = new PerformedProcedureComponent(_procedurePlan.OrderRef, this);
            _ppsComponent.ProcedurePlanChanged += delegate(object sender, ProcedurePlanChangedEventArgs e) { RefreshProcedurePlanSummary(e.ProcedurePlanDetail); };
            _documentationTabContainer.Pages.Add(new TabPage("Exam", _ppsComponent));

            // create extension pages
            TechnologistDocumentationContext context = new TechnologistDocumentationContext(this);
            foreach (ITechnologistDocumentationPageProvider pageProvider in (new TechnologistDocumentationPageProviderExtensionPoint()).CreateExtensions())
            {
                _extensionPages.AddRange(pageProvider.GetDocumentationPages(context));
            }

            foreach (ITechnologistDocumentationPage page in _extensionPages)
            {
                _documentationTabContainer.Pages.Add(new TabPage(page.Path.LocalizedPath, page.GetComponent()));
            }

            _documentationHost = new ChildComponentHost(this.Host, _documentationTabContainer);
            _documentationHost.StartComponent();

            SetInitialDocumentationTabPage();
        }

        private void SetInitialDocumentationTabPage()
        {
            string selectedTabName = TechnologistDocumentationComponentSettings.Default.InitiallySelectedTabPageName;
            if(string.IsNullOrEmpty(selectedTabName))
                return;

            TabPage requestedTabPage = CollectionUtils.SelectFirst(
                _documentationTabContainer.Pages,
                delegate(TabPage tabPage) { return tabPage.Name.Equals(selectedTabName, StringComparison.InvariantCultureIgnoreCase); });

            if (requestedTabPage != null)
                _documentationTabContainer.CurrentPage = requestedTabPage;
        }

        private List<ProcedurePlanSummaryTableItem> ListCheckedSummmaryTableItems()
        {
            return CollectionUtils.Map<Checkable<ProcedurePlanSummaryTableItem>, ProcedurePlanSummaryTableItem>(
                CollectionUtils.Select(
                    _procedurePlanSummaryTable.Items,
                    delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.IsChecked; }),
                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item; });
        }

        private void UpdateActionEnablement()
        {
            IList<ProcedurePlanSummaryTableItem> checkedSummaryTableItems = ListCheckedSummmaryTableItems();
            if (checkedSummaryTableItems.Count == 0)
            {
                _startAction.Enabled = _discontinueAction.Enabled = false;
            }
            else
            {
                // TODO: defer enablement to server
                _startAction.Enabled = CollectionUtils.TrueForAll(checkedSummaryTableItems,
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.State.Code == "SC"; });

                _discontinueAction.Enabled = CollectionUtils.TrueForAll(checkedSummaryTableItems,
                    delegate(ProcedurePlanSummaryTableItem item) { return item.mpsDetail.State.Code == "SC"; });
            }
        }

        private void RefreshProcedurePlanSummary(ProcedurePlanDetail procedurePlanDetail)
        {
            _procedurePlan = procedurePlanDetail;

            try
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                        {
                            CanCompleteOrderDocumentationResponse response = 
                                service.CanCompleteOrderDocumentation(new CanCompleteOrderDocumentationRequest(_procedurePlan.OrderRef));

                            _completeEnabled = response.CanComplete;
                            this.NotifyPropertyChanged("CompleteEnabled");
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            _procedurePlanSummaryTable.Items.Clear();
            foreach(ProcedureDetail rp in procedurePlanDetail.Procedures)
            {
                foreach(ModalityProcedureStepDetail mps in rp.ModalityProcedureSteps)
                {
                    _procedurePlanSummaryTable.Items.Add(
                        new Checkable<ProcedurePlanSummaryTableItem>(
                            new ProcedurePlanSummaryTableItem(rp, mps)));
                }
            }
            _procedurePlanSummaryTable.Sort();

            EventsHelper.Fire(_procedurePlanChanged, this, EventArgs.Empty);
        }

        #endregion
    }
}
