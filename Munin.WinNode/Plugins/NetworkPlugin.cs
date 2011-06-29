using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace Munin.WinNode.Plugins
{
    class NetworkPlugin : IPlugin
    {
        IList<NetworkAdapter> _networkAdapters;

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
            return "network configuration";
        }

        public string GetValues()
        {
            return string.Format("down.value {1:0.00}{0}up.value {2:0.00}",
                                 Environment.NewLine,
                                 GetTotalBytesReceivedPerSecond(),
                                 GetTotalBytesSentPerSecond());
        }

        private float GetTotalBytesReceivedPerSecond()
        {
            return _networkAdapters.Sum(m => m.BytesReceivedPerSecond);
        }

        private float GetTotalBytesSentPerSecond()
        {
            return _networkAdapters.Sum(m => m.BytesSentPerSecond);
        }

        /// <summary>
        /// The management object searcher query attempts to only retrieve physical adapters. 
        /// See: http://weblogs.sqlteam.com/mladenp/archive/2010/11/04/find-only-physical-network-adapters-with-wmi-win32_networkadapter-class.aspx
        /// </summary>
        void EnumerateNetworkAdapters()
        {
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
            }

            public string Name { get; private set; }
            
            public float BytesReceivedPerSecond
            {
                get
                {
                    return _receivedPerformanceCounter.NextValue();
                }
            }
            
            public float BytesSentPerSecond
            {
                get { return _sentPerformanceCounter.NextValue(); }
            }
        }
    }
}
