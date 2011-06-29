using System;

namespace Munin.WinNode.Commands
{
    /// <summary>
    /// Returns the configuration for the specified plugin
    /// </summary>
    class ConfigCommand : ICommand
    {
        public string Command
        {
            get { return "config"; }
        }

        public void Execute(out string response)
        {
            throw new NotImplementedException();
        }
    }
}
