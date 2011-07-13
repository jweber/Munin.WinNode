using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Munin.WinNode.Plugins.AspNet
{
    public class RequestsPlugin : IPlugin
    {
        const string Category = "ASP.NET Applications";
        const string CounterName = "Requests/Sec";

        readonly bool _isApplicable = false;
        readonly IList<PerformanceCounter> _counters;

        public RequestsPlugin()
        {
            _counters = new List<PerformanceCounter>();

            if (PerformanceCounterHelper.CategoryExists(Category))
            {
                _isApplicable = true;
                Initialize();
            }
        }

        private void Initialize()
        {
            var category = new PerformanceCounterCategory(Category);           
            var instances = category.GetInstanceNames();

            foreach (var instanceName in instances)
            {
                _counters.Add(new PerformanceCounter(Category, CounterName, instanceName));
            }

            PerformanceCounterHelper.InitializeCounters(_counters.ToArray());
        }

        public string Name
        {
            get { return "aspnet_requests"; }
        }

        public bool IsApplicable
        {
            get { return _isApplicable; }
        }

        public string GetConfiguration()
        {
            var output = new List<string>
                         {
                             "graph_title ASP.NET Requests/Second",
                             "graph_category ASP.NET",
                             "graph_args --base 1000 -l 0",
                             "graph_vlabel Requests/Second"
                         };

            foreach (var counter in _counters)
            {
                string prefix = GetInstanceName(counter);
                output.Add(string.Format("{0}.label {1}", prefix, counter.InstanceName));
                output.Add(string.Format("{0}.draw LINE", prefix));
                output.Add(string.Format("{0}.type GAUGE", prefix));
            }

            return output.Combine();
        }

        public string GetValues()
        {
            var output = new List<string>();
            foreach (var counter in _counters)
            {
                string prefix = GetInstanceName(counter);
                output.Add(string.Format("{0}.value {1:0}", prefix, counter.NextValue()));
            }

            return output.Combine();           
        }

        private string GetInstanceName(PerformanceCounter counter)
        {
            return string.Format("instance_{0}", counter.InstanceName);
        }
    }
}
