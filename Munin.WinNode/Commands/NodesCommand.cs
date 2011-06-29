using System;
using System.Net;

namespace Munin.WinNode.Commands
{
    /// <summary>
    /// Currently only supports a single node
    /// </summary>
    class NodesCommand : ICommand
    {
        public string Command
        {
            get { return "nodes"; }
        }

        public void Execute(out string response)
        {
            response = string.Format("{0}{1}.{1}", Dns.GetHostName(), Environment.NewLine);
        }
    }
}
