using System.Diagnostics;

namespace Munin.WinNode
{
    public static class PerformanceCounterHelper
    {
        public static string CleanName(string name)
        {
            name = name.Replace("\\", "_");
            name = name.Replace("/", "_");
            name = name.Replace("(", "[");
            name = name.Replace(")", "]");
            name = name.Replace("#", "_");

            return name;
        }

        /// <summary>
        /// Initializes the specified counter by calling its <see cref="PerformanceCounter.NextValue"/>
        /// method.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        public static PerformanceCounter Initialize(this PerformanceCounter counter)
        {
            var init = counter.NextValue();
            return counter;
        }

        /// <summary>
        /// Initializes the counters.
        /// </summary>
        /// <param name="counters">The counters.</param>
        public static void InitializeCounters(params PerformanceCounter[] counters)
        {
            foreach (var counter in counters)
            {
                counter.Initialize();
            }
        }
    }
}
