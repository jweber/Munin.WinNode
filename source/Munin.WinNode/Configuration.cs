using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Munin.WinNode
{
    static class Configuration
    {
        const string ConfigurationFileName = "munin-node.ini";

        private static string ConfigurationFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationFileName); }
        }

        public static string NewLine
        {
            get { return "\n"; }
        }

        public static string GetValue(string section, string key, string defaultValue)
        {
            var retVal = new StringBuilder(255);
            var status = GetPrivateProfileString(section, key, defaultValue, retVal, 255, ConfigurationFilePath);

            Trace.WriteLine(string.Format(@"Read configuration value {0}\{1} as '{2}'", section, key, retVal));
            
            return retVal.ToString();
        }

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
