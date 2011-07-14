using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Munin.WinNode
{
    public static class Configuration
    {
        const string ConfigurationFileName = "munin-node.ini";

        private static string ConfigurationFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationFileName); }
        }

        /// <summary>
        /// Gets the new line character.
        /// </summary>
        public static string NewLine
        {
            get { return "\n"; }
        }

        /// <summary>
        /// Reads the configuration value from the <c>munin-node.ini</c> file or returns
        /// <paramref name="defaultValue"/> if the <paramref name="section"/> or <paramref name="key"/>
        /// are not defined.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string GetValue(string section, string key, string defaultValue)
        {
            var retVal = new StringBuilder(255);
            var status = GetPrivateProfileString(section, key, defaultValue, retVal, 255, ConfigurationFilePath);

            Logging.Debug(@"Read configuration value {0}\{1} as '{2}'", section, key, retVal);
            
            return retVal.ToString();
        }

        /// <summary>
        /// Reads the configuration value from the <c>munin-node.ini</c> file or returns
        /// <paramref name="defaultValue"/> if the <paramref name="section"/> or <paramref name="key"/>
        /// are not defined. The returned values from this method are cast to the type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetValue<T>(string section, string key, T defaultValue)
        {
            string value = GetValue(section, key, defaultValue.ToString());
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            try
            {
                return (T) Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
