using System.Diagnostics;
using System.Linq;

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
        /// Returns <c>true</c> if the <paramref name="category"/> exists
        /// as a PerformanceCounter category on the local system.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool CategoryExists(string category)
        {
            return PerformanceCounterCategory.GetCategories().Any(m => m.CategoryName == category);
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
