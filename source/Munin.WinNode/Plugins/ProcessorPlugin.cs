using System.Collections.Generic;
using System.Diagnostics;

namespace Munin.WinNode.Plugins
{
    class ProcessorPlugin : IPlugin
    {
        readonly IList<Processor> _processors;

        public ProcessorPlugin()
        {
            _processors = new List<Processor>();
            EnumerateProcessors();
        }

        public string Name
        {
            get { return "processor"; }
        }

        public string GetConfiguration()
        {
            var output = new List<string>
                         {
                             "graph_title Processor Time",
                             "graph_category System",
                             "graph_args --base 1000 -l 0",
                             "graph_vlabel % Processor Time"
                         };

            for (int i = 0; i < _processors.Count; i++)
            {
                string prefix = "processor";
                if (i > 0)
                    prefix += "_" + i + "_";

                output.Add(string.Format("{0}.label {1}", prefix, i));
                output.Add(string.Format("{0}.draw LINE", prefix));
                output.Add(string.Format("{0}.type GAUGE", prefix));
            }

            return output.Combine();
        }

        public string GetValues()
        {
            var output = new List<string>();
            for (int i = 0; i < _processors.Count; i++)
            {
                string prefix = "processor";
                if (i > 0)
                    prefix += "_" + i + "_";

                output.Add(string.Format("{0}.value {1}", prefix, _processors[i].GetProcessorTime));
            }

            return output.Combine();
        }

        void EnumerateProcessors()
        {
            Trace.WriteLine("Enumerating Processors");
            for (int i = 0; i < System.Environment.ProcessorCount; i++)
            {
                Trace.WriteLine(" + Found processor: " + i);
                var processor = new Processor(i.ToString());
                _processors.Add(processor);
            }
        }

        class Processor
        {
            readonly PerformanceCounter _counter;

            public Processor(string name)
            {
                this.Name = PerformanceCounterHelper.CleanName(name);

                this._counter = new PerformanceCounter("Processor", "% Processor Time", this.Name).Initialize();
            }

            public string Name { get; private set; }

            public float GetProcessorTime
            {
                get
                {
                    var value = _counter.NextValue();
                    Trace.WriteLine(string.Format(" + Processor '{0}' time: {1}", this.Name, value));
                    return value;
                }
            }
        }
    }
}
