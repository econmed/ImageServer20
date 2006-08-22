using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;

namespace ClearCanvas.Dicom.DataStore
{
    public sealed partial class DataAbstractionLayer 
    {
        #region Handcoded Members
        #region Private Members

        private static readonly ISessionFactory _sessionFactory;
        //private static Dictionary<int, IDicomDictionary> _dicomDictionaries;
        private static Dictionary<Thread, ISession> _sessions;
        private static Dictionary<ISession, IDataStore> _dataStores;
        private static Dictionary<ISession, IDataStoreWriteAccessor> _dataStoreWriteAccessors;
        private static IDicomDictionary _dicomDictionary;

        private static void CloseSessionFactory()
        {          
            if (_sessionFactory != null)
                _sessionFactory.Close();
        }

        private static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }

        private static Dictionary<ISession, IDataStore> DataStores
        {
            get { return _dataStores; }
        }

        private static Dictionary<Thread, ISession> Sessions
        {
            get { return _sessions; }
        }

        //private static Dictionary<int, IDicomDictionary> DicomDictionaries
        //{
        //    get { return _dicomDictionaries; }
        //}

        private static Dictionary<ISession, IDataStoreWriteAccessor> DataStoreWriteAccessors
        {
            get { return _dataStoreWriteAccessors; }
        }

        private static ISession CurrentSession
        {
            get 
            {
                // look for stale sessions and get rid of them
                List<KeyValuePair<Thread,ISession>> toRemoveArray = new List<KeyValuePair<Thread,ISession>>();
                foreach (KeyValuePair<Thread, ISession> iterator in DataAbstractionLayer.Sessions)
                {
                    if (!iterator.Key.IsAlive)
                        toRemoveArray.Add(iterator);
                }
                foreach (KeyValuePair<Thread, ISession> iterator in toRemoveArray)
                {
                    // get rid of session-dependent objects that we remember about
                    DataAbstractionLayer.DataStores.Remove(iterator.Value);
                    DataAbstractionLayer.DataStoreWriteAccessors.Remove(iterator.Value);

                    // close the session and get rid of the session that we remember about
                    iterator.Value.Close();
                    DataAbstractionLayer.Sessions.Remove(iterator.Key);
                }
                toRemoveArray.Clear();

                Thread currentThread = Thread.CurrentThread;
                if (DataAbstractionLayer.Sessions.ContainsKey(currentThread))
                {
                    return DataAbstractionLayer.Sessions[currentThread];
                }
                else
                {
                    ISession newSession = DataAbstractionLayer.SessionFactory.OpenSession();
                    DataAbstractionLayer.Sessions.Add(currentThread, newSession);
                    return newSession;
                }
            }
        }

        #endregion

        static DataAbstractionLayer()
        {
            Configuration cfg = new Configuration();
            string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            cfg.Configure(assemblyName + ".cfg.xml");
            cfg.AddAssembly(assemblyName);
            _sessionFactory = cfg.BuildSessionFactory();

            _sessions = new Dictionary<Thread, ISession>();
            _dataStores = new Dictionary<ISession, IDataStore>();
            _dataStoreWriteAccessors = new Dictionary<ISession, IDataStoreWriteAccessor>();
        }

        //public static void StartSession()
        //{
        //    // this just creates a new session if necessary
        //    ISession currentSession = DataAbstractionLayer.CurrentSession;
        //}

        //public static void EndSession()
        //{
        //    ISession currentSession = DataAbstractionLayer.CurrentSession;
        //    DataAbstractionLayer.DataStores.Remove(currentSession);
        //    DataAbstractionLayer.DataStoreWriteAccessors.Remove(currentSession);
        //    currentSession.Close();
        //    DataAbstractionLayer.Sessions.Remove(Thread.CurrentThread);
        //}

        public static IDataStore GetIDataStore()
        {
            ISession session = DataAbstractionLayer.CurrentSession;
            if (DataAbstractionLayer.DataStores.ContainsKey(session))
            {
                return DataAbstractionLayer.DataStores[session];
            }
            else
            {
                IDataStore dataStore = new DataStore(session);
                DataAbstractionLayer.DataStores.Add(session, dataStore);
                return dataStore;
            }
        }

        public static IDataStoreWriteAccessor GetIDataStoreWriteAccessor()
        {
            ISession session = DataAbstractionLayer.CurrentSession;
            if (DataAbstractionLayer.DataStoreWriteAccessors.ContainsKey(session))
            {
                return DataAbstractionLayer.DataStoreWriteAccessors[session];
            }
            else
            {
                IDataStoreWriteAccessor dataStoreWriteAccessor = new DataStoreWriteAccessor(session);
                DataAbstractionLayer.DataStoreWriteAccessors.Add(session, dataStoreWriteAccessor);
                return dataStoreWriteAccessor;
            }
        }

        public static IDicomDictionary GetIDicomDictionary()
        {
            if (null == _dicomDictionary)
                _dicomDictionary = new DicomDictionary(DataAbstractionLayer.SessionFactory.OpenSession());

            return _dicomDictionary;
        }
        #endregion
    }
}
