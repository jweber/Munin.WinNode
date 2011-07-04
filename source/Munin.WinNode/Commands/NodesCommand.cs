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

        public bool EndResponseWithPeriod
        {
            get { return true; }
        }

        public void Execute(string[] arguments, out string response)
        {
            response = string.Format("{0}", Dns.GetHostName());
        }
    }
}
