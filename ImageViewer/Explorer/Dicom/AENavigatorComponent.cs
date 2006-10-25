using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class AENavigatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(AENavigatorComponentViewExtensionPoint))]
	public class AENavigatorComponent : ApplicationComponent
	{
        #region Fields

        private DicomServerTree _dicomServerTree;
        private event EventHandler _selectedServerChanged;
        private AEServerGroup _selectedServers;

        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My Studies";
        private static String _myServersRoot = "MyServersRoot";
        private static String _myServersXmlFile = "DicomAEServers.xml";

        public DicomServerTree DicomServerTree
        {
            get { return _dicomServerTree; }
            set { _dicomServerTree = value; }
        }

        public AEServerGroup SelectedServers
        {
            get { return _selectedServers; }
            set { _selectedServers = value; }
        }

        public static String MyServersTitle
        {
            get { return AENavigatorComponent._myServersTitle; }
        }

        public static String MyDatastoreTitle
        {
            get { return AENavigatorComponent._myDatastoreTitle; }
        }

        public static String MyServersRoot
        {
            get { return AENavigatorComponent._myServersRoot; }
        }

        public static String MyServersXmlFile
        {
            get { return AENavigatorComponent._myServersXmlFile; }
        }

        #endregion

        public AENavigatorComponent()
        {
            _selectedServers = new AEServerGroup();
            _dicomServerTree = new DicomServerTree();
            if (_dicomServerTree.CurrentServer != null && _dicomServerTree.CurrentServer.IsServer)
            {
                _selectedServers.Servers.Add((DicomServer)_dicomServerTree.CurrentServer);
                _selectedServers.Name = _dicomServerTree.CurrentServer.ServerName;
                _selectedServers.GroupID = _dicomServerTree.CurrentServer.ServerPath + "/" + _selectedServers.Name;
            }

        }

        public IDicomServer AddEditServer(IDicomServer dataNode)
        {
            if (dataNode == null)
                return null;
            _dicomServerTree.CurrentServer = dataNode;
            DicomServerEditComponent editor = new DicomServerEditComponent(_dicomServerTree);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Add New Server");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                SetSelectedServer((DicomServer)_dicomServerTree.CurrentServer);
                return _dicomServerTree.CurrentServer;
            }
            return null;
        }

        public IDicomServer AddEditServerGroup(IDicomServer dataNode, bool isNewGroup)
        {
            if (dataNode == null || dataNode.IsServer)
                return null;
            _dicomServerTree.CurrentServer = (DicomServerGroup)dataNode;
            string title = isNewGroup ? "Add New Server Group" : "Edit Server Group";
            DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(_dicomServerTree, isNewGroup);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, title);

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                SelectChanged(_dicomServerTree.CurrentServer);
                return _dicomServerTree.CurrentServer;
            }
            return null;
        }

        public void CEchoServer()
        {
            if (_selectedServers.Servers.Count == 0)
            {
                throw new DicomServerException("There are no servers selected. Please select servers and try again.");
                return;
            }
            LocalAESettings myAESettings = new LocalAESettings();
            ApplicationEntity myAE = new ApplicationEntity(new HostName("localhost"), new AETitle(myAESettings.AETitle), new ListeningPort(myAESettings.Port));
            StringBuilder msgText = new StringBuilder();
            msgText.AppendFormat("C-ECHO Verification:\r\n\r\n");
            using (DicomClient client = new DicomClient(myAE))
            {
                foreach (DicomServer ae in _selectedServers.Servers)
                {
                    if (client.Verify(ae.DicomAE))
                        msgText.AppendFormat("    {0}: successful    \r\n", ae.ServerPath + "/" + ae.ServerName);
                    else
                        msgText.AppendFormat("    {0}: fail    \r\n", ae.ServerPath + "/" + ae.ServerName);
                }
            }
            msgText.AppendFormat("\r\n");
            throw new DicomServerException(msgText.ToString());
            return;
        }

        public bool DeleteServer(IDicomServer dataNode)
        {
            _dicomServerTree.CurrentServer = _dicomServerTree.RemoveDicomServer(dataNode);
            if (_dicomServerTree.CurrentServer == null)
                return false;
            _selectedServers.Servers = _dicomServerTree.FindChildServers(_dicomServerTree.CurrentServer);
            _selectedServers.Name = _dicomServerTree.CurrentServer.ServerName;
            _selectedServers.GroupID = _dicomServerTree.CurrentServer.ServerPath + "/" + _selectedServers.Name;
            _dicomServerTree.SaveDicomServers();
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            return true;
        }

        private void SetSelectedServer(DicomServer server)
        {
            _selectedServers = new AEServerGroup();
            _selectedServers.Servers.Add(server);
            _selectedServers.Name = server.ServerName;
            _selectedServers.GroupID = server.ServerPath + "/" + server.ServerName;
            _dicomServerTree.CurrentServer = server;
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
        }

        public void SelectChanged(IDicomServer dataNode)
        {
            if (dataNode.IsServer)
            {
                SetSelectedServer((DicomServer)dataNode);
            }
            else
            {
                _selectedServers = new AEServerGroup();
                _selectedServers.Servers = _dicomServerTree.FindChildServers((DicomServerGroup)dataNode);
                _selectedServers.GroupID = dataNode.ServerPath + "/" + dataNode.ServerName;
                _selectedServers.Name = dataNode.ServerName;
                _dicomServerTree.CurrentServer = dataNode;
                EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            }

        }

        public event EventHandler SelectedServerChanged
        {
            add { _selectedServerChanged += value; }
            remove { _selectedServerChanged -= value; }
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

    }

}
