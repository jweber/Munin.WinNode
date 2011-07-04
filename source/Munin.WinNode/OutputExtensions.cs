using System;
using System.Collections.Generic;

namespace Munin.WinNode
{
    public static class OutputExtensions
    {
        /// <summary>
        /// Returns the <paramref name="input"/> array joined by
        /// the system newline definition.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Combine(this IEnumerable<string> input)
        {
            return string.Join(Configuration.NewLine, input);
        }
    }
}
