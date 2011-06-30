using System.Diagnostics;

namespace Munin.WinNode.Plugins
{
    class CpuPlugin : IPlugin
    {
        readonly PerformanceCounter _cpuPerformanceCounter;

        public CpuPlugin()
        {
            _cpuPerformanceCounter = new PerformanceCounter("Processor", "% User Time", "_Total");
            var init = _cpuPerformanceCounter.NextValue();
        }

        public string Name
        {
            get { return "cpu"; }
        }

        public string GetConfiguration()
        {
            return "cpu configuration";
        }

        public string GetValues()
        {
            return string.Format("cpu_user.value {0}", _cpuPerformanceCounter.NextValue());
        }
    }
}
