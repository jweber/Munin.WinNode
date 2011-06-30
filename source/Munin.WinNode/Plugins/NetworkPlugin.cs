using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Munin.WinNode.Plugins
{
    class NetworkPlugin : IPlugin
    {
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
                       "graph_title Network Traffic",
                       "graph_order down up",
                       "graph_args --base 1000",
                       "graph_vlabel bits in (-) / out (+) per second",
                       "graph_category network",
                       "graph_info This graph shows the traffic of the network interfaces.",
                       "down.label bps",
                       "down.type DERIVE",
                       "down.graph no",
                       "up.label bps",
                       "up.type DERIVE",
                       "up.negative down",
                   }.Combine();
        }

        public string GetValues()
        {
            Trace.WriteLine("Fetching network statistics");

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
            Trace.WriteLine("Enumerating network adapters");
            var search = new ManagementObjectSearcher(
                @"SELECT * FROM Win32_NetworkAdapter 
                WHERE NetConnectionStatus=2
                AND PhysicalAdapter = 1
                AND Manufacturer != 'Microsoft'
                AND NOT PNPDeviceID LIKE 'ROOT\\%'");

            var adapterObjects = search.Get();
            foreach (ManagementObject adapterObject in adapterObjects)
            {
                string name = adapterObject["Name"].ToString();
                Trace.WriteLine(" + Found network adapter named: " + name);

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
                name = name.Replace("\\", "_");
                name = name.Replace("/", "_");
                name = name.Replace("(", "[");
                name = name.Replace(")", "]");
                name = name.Replace("#", "_");

                this.Name = name;
                this._receivedPerformanceCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
                this._sentPerformanceCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);

                var initReceive = _receivedPerformanceCounter.NextValue();
                var initSent = _sentPerformanceCounter.NextValue();
            }

            public string Name { get; private set; }
            
            public float BitsPerSecondReceived
            {
                get
                {
                    var bps = _receivedPerformanceCounter.NextValue() * 8;
                    Trace.WriteLine(string.Format(" + Adapter '{0}' received bps: {1}", this.Name, bps));
                    return bps;
                }
            }
            
            public float BitsPerSecondSent
            {
                get
                {
                    var bps = _sentPerformanceCounter.NextValue() * 8;
                    Trace.WriteLine(string.Format(" + Adapter '{0}' sent bps: {1}", this.Name, bps));
                    return bps;
                }
            }
        }
    }
}
