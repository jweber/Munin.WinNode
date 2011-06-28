using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Munin.WinNode.Commands
{
    class VersionCommand : ICommand
    {
        public string Command
        {
            get { return "version"; }
        }

        public void Execute(out string response)
        {
            var version = typeof(Program).Assembly.GetName().Version;
            response = string.Format("Munin.WinNode on {0} version: {1}.{2}.{3}",
                                     Dns.GetHostName(),
                                     version.Major,
                                     version.Minor,
                                     version.Build);
        }
    }
}
