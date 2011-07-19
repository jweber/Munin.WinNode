using System;

namespace Munin.WinNode.Commands
{
    /// <summary>
    /// Fetches the data made available by the specified plugin
    /// </summary>
    class FetchCommand : ICommand
    {
        public virtual string Command
        {
            get { return "fetch"; }
        }

        public bool EndResponseWithPeriod
        {
            get { return true; }
        }

        public void Execute(string[] arguments, out string response)
        {
            if (arguments == null || arguments.Length == 0)
            {
                response = "# Unknown service";
                return;
            }

            string pluginName = arguments[0];
            var plugin = PluginManager.PluginFromName(pluginName);
            if (plugin == null)
            {
                response = "# Unknown service";
                return;
            }

            response = PluginValue(plugin);
        }

        protected virtual string PluginValue(IPlugin plugin)
        {
            try
            {
                return plugin.GetValues();
            }
            catch (Exception ex)
            {
                string message = string.Format("Plugin '{0}' threw exception when attempting to obtain values", plugin.Name);
                Logging.Error(message, ex);
            }

            return string.Empty;
        }
    }
}
