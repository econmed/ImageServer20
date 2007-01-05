using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace SampleShred2
{
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class SampleShredExtension : ShredBase
    {
        private readonly string _className;
        private readonly string _serviceEndPointName;
        private EventWaitHandle _stopSignal;

        public SampleShredExtension()
        {
            _className = this.GetType().ToString();
            _serviceEndPointName = "SampleShred2";
            _stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
            System.Diagnostics.Trace.WriteLine(_className + ": constructed");
        }

        public override void Start(int port)
        {
            Platform.Log(_className + "[" + AppDomain.CurrentDomain.FriendlyName + "]: Start invoked");
            _stopSignal.Reset();
            GlobalStore.Score = 0;
            GlobalStore.Darts = 0;

            StartHost<SampleShredServiceType, ISampleShred2Interface>(port, _serviceEndPointName, "pi digits calculator");

            // start up processing thread
            Thread t = new Thread(new ThreadStart(ComputePi));
            t.Start();

            // wait for host's stop signal
            _stopSignal.WaitOne();
            
            // wait for processing thread to finish
            t.Join();

            // stop hosting the service
            StopHost(_serviceEndPointName);
        }

        public override string GetFriendlyName()
        {
            return _serviceEndPointName;
        }

        public override void Stop()
        {
            _stopSignal.Set();
        }

        private void ComputePi()
        {
            while (true)
            {
                FindNextPi();
                if (_stopSignal.WaitOne(100, false))
                    break;
            }
        }

        private void FindNextPi()
        {
            double dartX;
            double dartY;

            dartX = GlobalStore.RandomGenerator.NextDouble();
            dartY = GlobalStore.RandomGenerator.NextDouble();

            if (((dartX * dartX) + (dartY * dartY)) <= 1.0)
                GlobalStore.Score++;
            GlobalStore.Darts++;

            GlobalStore.CurrentPi = 4.0 * (double)GlobalStore.Score / (double)GlobalStore.Darts;
            return;
        }


    }
}
