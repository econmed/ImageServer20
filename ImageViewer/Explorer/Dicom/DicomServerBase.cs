using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class DicomServerBase : IDicomServer
    {
        private List<IDicomServer> _childServers;
        private string _serverName;
        private string _serverPath;

        public void AddChild(IDicomServer child)
        {
            ChildServers.Add(child);
        }

        #region DicomServerBase Members

        public List<IDicomServer> ChildServers
        {
            get
            {
                if (_childServers == null)
                {
                    _childServers = new List<IDicomServer>();
                }
                return _childServers;
            }
        }

        public abstract bool IsServer
        {
            get;
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }

        public abstract string ServerDetails
        {
            get;
        }

        #endregion
    }
}
