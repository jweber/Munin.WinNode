using System;
using System.Diagnostics;

namespace Munin.WinNode.Plugins
{
    class CpuPlugin : IPlugin
    {
        readonly PerformanceCounter _userCounter, _systemCounter, _idleCounter;

        public CpuPlugin()
        {
            _userCounter = new PerformanceCounter("Processor", "% User Time", "_Total");
            _systemCounter = new PerformanceCounter("Processor", "% Privileged Time", "_Total");
            _idleCounter = new PerformanceCounter("Processor", "% Idle Time", "_Total");

            InitializeCounters(_userCounter, _systemCounter, _idleCounter);
        }

        private void InitializeCounters(params PerformanceCounter[] counters)
        {
            foreach (var counter in counters)
            {
                var init = counter.NextValue();
            }
        }

        public string Name
        {
            get { return "cpu"; }
        }

        public string GetConfiguration()
        {
            var output = new[]
                         {
                            "graph_title CPU Usage",
                            "graph_order system user idle",
                            "graph_args --base 1000 -r --lower-limit 0 --upper-limit 100",
                            "graph_vlabel %",
                            "graph_scale no",
                            "graph_info This graph shows how CPU time is spent.",
                            "graph_category system",
                            "graph_period second",
                            "system.label system",
                            "system.draw AREA",
                            "system.min 0",
                            "system.type DERIVE",
                            "system.info CPU time spend by the kernel in system activities",
                            "user.label user",
                            "user.draw STACK",
                            "user.min 0",
                            "user.type DERIVE",
                            "user.info CPU time spent by normal programs and daemons",
                            "idle.label idle",
                            "idle.draw STACK",
                            "idle.min 0",
                            "idle.type DERIVE",
                            "idle.info Idle CPU time"
                         };

            return output.Combine();
        }

        public string GetValues()
        {
            var output = new[]
                         {
                             string.Format("system.value {0:0}", _systemCounter.NextValue()),
                             string.Format("user.value {0:0}", _userCounter.NextValue()),
                             string.Format("idle.value {0:0}", _idleCounter.NextValue())
                         };

            return output.Combine();
        }
    }
}
