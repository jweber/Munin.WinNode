using System.Collections.Generic;
using System.Diagnostics;

namespace Munin.WinNode.Plugins.AspNet
{
    public class RequestTransferPlugin : IPlugin
    {
        const string Category = "ASP.NET Applications";

        InstanceRequestTransfer _instance;
        readonly bool _isApplicable = false;

        public RequestTransferPlugin()
        {
            if (PerformanceCounterHelper.CategoryExists(Category))
            {
                _isApplicable = true;
                Initialize();
            }
        }

        private void Initialize()
        {
            _instance = new InstanceRequestTransfer("__TOTAL__");
        }

        public string Name
        {
            get { return "aspnet_requesttransfer"; }
        }

        public bool IsApplicable
        {
            get { return _isApplicable; }
        }

        public string GetConfiguration()
        {
            var output = new List<string>
                         {
                             "graph_title ASP.NET Request Transfer",
                             "graph_category ASP.NET",
                             "graph_args --base 1000 -l 0",
                             "graph_vlabel Bytes Transferred",
                             "sent.draw AREA",
                             "sent.label Sent Bytes",
                             "received.draw LINE2",
                             "received.label Received Bytes"
                         };

            return output.Combine();
        }

        public string GetValues()
        {
            return new[]
                   {
                       string.Format("received.value {0:0}", _instance.BytesReceivedTotal),
                       string.Format("sent.value {0:0}", _instance.BytesSentTotal)
                   }.Combine();
        }

        class InstanceRequestTransfer
        {
            readonly PerformanceCounter _bytesReceivedCounter;
            readonly PerformanceCounter _bytesSentCounter;

            public InstanceRequestTransfer(string instanceName)
            {
                InstanceName = instanceName;

                _bytesReceivedCounter = new PerformanceCounter(Category, "Request Bytes In Total", instanceName)
                    .Initialize();

                _bytesSentCounter = new PerformanceCounter(Category, "Request Bytes Out Total", instanceName)
                    .Initialize();
            }

            public string InstanceName { get; private set; }

            public float BytesReceivedTotal
            {
                get { return _bytesReceivedCounter.NextValue(); }
            }

            public float BytesSentTotal
            {
                get { return _bytesSentCounter.NextValue(); }
            }
        }
    }
}
