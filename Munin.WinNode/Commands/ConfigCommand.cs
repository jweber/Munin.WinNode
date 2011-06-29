using System;

namespace Munin.WinNode.Commands
{
    /// <summary>
    /// Returns the configuration for the specified plugin
    /// </summary>
    class ConfigCommand : FetchCommand
    {
        public override string Command
        {
            get { return "config"; }
        }

        protected override string PluginValue(IPlugin plugin)
        {
            return plugin.GetConfiguration();
        }
    }
}
