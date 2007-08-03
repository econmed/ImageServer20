using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using ClearCanvas.Common;


namespace ClearCanvas.Server.ShredHost
{
    public static class ShredHost
    {
        /// <summary>
        /// Starts the ShredHost routine.
        /// </summary>
        /// <returns>true - if the ShredHost is currently running, false - if ShredHost is stopped.</returns>
        public static bool Start()
        {
            lock (_lockObject)
            {
                if (RunningState.Running == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            Platform.Log(LogLevel.Info, "Starting up in AppDomain [" + AppDomain.CurrentDomain.FriendlyName + "]");

            // the ShredList and shreds objects are proxy objects that actually exist
            // in the secondary AppDomain
			//AppDomain stagingDomain = AppDomain.CreateDomain("StagingDomain");
            ExtensionScanner scanner = (ExtensionScanner)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, "ClearCanvas.Server.ShredHost.ExtensionScanner");
            ShredStartupInfoList shredStartupInfoList = null;

            try
            {
                shredStartupInfoList = scanner.ScanExtensions();
            }
            catch (PluginException pluginException)
            {
                // There was a problem loading the plugins, including if there were no plugins found
                // This is an innocuous problem, and just means that there are no shreds to run
                Platform.Log(LogLevel.Warn, pluginException);
            }

            StartShreds(shredStartupInfoList);

            // all the shreds have been created, so we can dismantle the secondary domain that was used 
            // for scanning for all Extensions that are shreds
			//AppDomain.Unload(stagingDomain);

			_sed = WcfHelper.StartHttpHost<ShredHostServiceType, IShredHost>("ShredHost", "Host program of multiple indepdent service-like subprograms", ShredHostServiceSettings.Instance.ShredHostHttpPort);
            Platform.Log(LogLevel.Info, "ShredHost WCF Service started on port " + ShredHostServiceSettings.Instance.ShredHostHttpPort.ToString());
            lock (_lockObject)
            {
                _runningState = RunningState.Running;
            }

            return (RunningState.Running == _runningState);
        }

        /// <summary>
        /// Stops the running ShredHost.
        /// </summary>
        /// <returns>true - if the ShredHost is running, false - if the ShredHost is stopped.</returns>
        public static bool Stop()
        {
            lock (_lockObject)
            {
                if (RunningState.Stopped == _runningState || RunningState.Transition == _runningState)
                    return (RunningState.Running == _runningState);

                _runningState = RunningState.Transition;
            }

            // correct sequence should be to stop the WCF host so that we don't
            // receive any more incoming requests
            Platform.Log(LogLevel.Info, "Shred Host stop request received");
            WcfHelper.StopHost(_sed);
            Platform.Log(LogLevel.Info, "ShredHost WCF Service stopped");
            StopShreds();
            Platform.Log(LogLevel.Info, "Completing Shred Host stop");


            _shredInfoList.Clear();
            lock (_lockObject)
            {
                _runningState = RunningState.Stopped;
            }

            return (RunningState.Running == _runningState);
        }

        static public bool IsShredHostRunning
        {
            get
            {
                bool isRunning;
                lock (_lockObject)
                {
                    isRunning = (RunningState.Running == _runningState);
                }

                return isRunning;
            }
        }

        static public bool StartShred(WcfDataShred shred)
        {
            Platform.Log(LogLevel.Info, "Attempting to start shred: " + shred.Name);
            return ShredHost.ShredControllerList[shred.Id].Start();
        }

        static public bool StopShred(WcfDataShred shred)
        {
            Platform.Log(LogLevel.Info, "Attempting to stop shred: " + shred.Name);
            return ShredHost.ShredControllerList[shred.Id].Stop();
        }

        static ShredHost()
        {
            _shredInfoList = new ShredControllerList();
            _sed = null;
            _runningState = RunningState.Stopped;
        }

        private static void StartShreds(ShredStartupInfoList shredStartupInfoList)
        {
            if (null != shredStartupInfoList)
            {
                // create the data structure that will hold the shreds and their thread, etc. related objects
                foreach (ShredStartupInfo shredStartupInfo in shredStartupInfoList)
                {
                    if (null != shredStartupInfo)
                    {
                        // clone the shredStartupInfo structure into the current AppDomain, otherwise, once the StagingDomain 
                        // has been unloaded, the shredStartupInfo structure will be destroyed
                        ShredStartupInfo newShredStartupInfo = new ShredStartupInfo(shredStartupInfo.AssemblyPath, shredStartupInfo.ShredName, shredStartupInfo.ShredTypeName);
                        
                        // create the controller that will allow us to start and stop the shred
                        ShredController shredController = new ShredController(newShredStartupInfo);
                        _shredInfoList.Add(shredController);
                    }

                }
            }

            foreach (ShredController shredController in _shredInfoList)
            {
                shredController.Start();
            }
        }

        private static void StopShreds()
        {
            foreach (ShredController shredController in _shredInfoList)
            {
                string displayName = shredController.Shred.GetDisplayName();
                Platform.Log(LogLevel.Info, displayName + ": Signalling stop");
                shredController.Stop();
                Platform.Log(LogLevel.Info, displayName + ": Stopped");
            }

        }

        #region Print asms in AD helper f(x)
        public static void PrintAllAssembliesInAppDomain(AppDomain ad)
        {
            Assembly[] loadedAssemblies = ad.GetAssemblies();
            Console.WriteLine("***** Here are the assemblies loaded in {0} *****\n",
                ad.FriendlyName);
            foreach (Assembly a in loadedAssemblies)
            {
                Console.WriteLine("-> Name: {0}", a.GetName().Name);
                Console.WriteLine("-> Version: {0}", a.GetName().Version);
            }
        }
        #endregion

        #region Private Members
        private static ShredControllerList _shredInfoList;
        private static ServiceEndpointDescription _sed;
        private static RunningState _runningState;
        private static object _lockObject = new object();
        #endregion

        internal static ShredControllerList ShredControllerList
        {
            get { return _shredInfoList; }
        }
    }    
}
