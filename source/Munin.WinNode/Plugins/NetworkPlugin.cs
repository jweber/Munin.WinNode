using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using log4net;

namespace Munin.WinNode.Plugins
{
    class NetworkPlugin : IPlugin
    {
        const string ConfigurationSection = "NetworkPlugin";

        readonly IList<NetworkAdapter> _networkAdapters;

        public NetworkPlugin()
        {
            _networkAdapters = new List<NetworkAdapter>();           
            EnumerateNetworkAdapters();
        }

        public string Name
        {
            get { return "network"; }
        }

        public string GetConfiguration()
        {
            return new[]
                   {
                       "graph_args --base 1000 --lower-limit 0",
                       "graph_title Network Traffic",
                       "graph_order down up",
                       "graph_vlabel Bits per second",
                       //"down.cdef down,8,*",
                       "down.draw AREA",
                       "down.label Received bps",
                       //"up.cdef up,8,*",
                       "up.draw LINE1",
                       "up.label Sent bps"
                   }.Combine();
        }

        public string GetValues()
        {
            Logging.Logger.Info("Fetching network statistics");

            return new[]
                   {
                       string.Format("down.value {0:0}", GetTotalBitsReceivedPerSecond()),
                       string.Format("up.value {0:0}", GetTotalBitsSentPerSecond())
                   }.Combine();
            
        }

        private float GetTotalBitsReceivedPerSecond()
        {
            return _networkAdapters.Sum(m => m.BitsPerSecondReceived);
        }

        private float GetTotalBitsSentPerSecond()
        {
            return _networkAdapters.Sum(m => m.BitsPerSecondSent);
        }

        /// <summary>
        /// The management object searcher query attempts to only retrieve physical adapters. 
        /// See: http://weblogs.sqlteam.com/mladenp/archive/2010/11/04/find-only-physical-network-adapters-with-wmi-win32_networkadapter-class.aspx
        /// </summary>
        void EnumerateNetworkAdapters()
        {
            string adapterName = Configuration.GetValue(ConfigurationSection, "AdapterName", "*");
            if (adapterName != "*")
            {
                adapterName = PerformanceCounterHelper.CleanName(adapterName);

                Logging.Logger.InfoFormat("Using configured network adapter '{0}'", adapterName);
                var networkAdapter = new NetworkAdapter(adapterName);
                _networkAdapters.Add(networkAdapter);
                return;
            }

            Logging.Logger.Info("Enumerating network adapters");
            var search = new ManagementObjectSearcher(
                @"SELECT * FROM Win32_NetworkAdapter 
                WHERE PhysicalAdapter = 1
                AND NOT PNPDeviceID LIKE 'ROOT\\%'");

            var adapterObjects = search.Get();
            foreach (ManagementObject adapterObject in adapterObjects)
            {
                string name = adapterObject["Name"].ToString();
                Logging.Logger.InfoFormat(" + Found network adapter named: {0}", name);

                var networkAdapter = new NetworkAdapter(name);
                _networkAdapters.Add(networkAdapter);
            }
        }

        class NetworkAdapter
        {
            readonly PerformanceCounter _receivedPerformanceCounter;
            readonly PerformanceCounter _sentPerformanceCounter;

            public NetworkAdapter(string name)
            {
                this.Name = PerformanceCounterHelper.CleanName(name);
                this._receivedPerformanceCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", this.Name).Initialize();
                this._sentPerformanceCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", this.Name).Initialize();
            }

            public string Name { get; private set; }
            
            public float BitsPerSecondReceived
            {
                get
                {
                    var bps = _receivedPerformanceCounter.NextValue() * 8;
                    Logging.Logger.InfoFormat(" + Adapter '{0}' received bps: {1}", this.Name, bps);
                    return bps;
                }
            }
            
            public float BitsPerSecondSent
            {
                get
                {
                    var bps = _sentPerformanceCounter.NextValue() * 8;
                    Logging.Logger.InfoFormat(" + Adapter '{0}' sent bps: {1}", this.Name, bps);
                    return bps;
                }
            }
        }
    }
}
