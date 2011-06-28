using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Munin.WinNode.Commands
{
    class ListCommand : ICommand
    {
        public string Command
        {
            get { return "list"; }
        }

        public void Execute(out string response)
        {
            response = "plugin list";
        }
    }
}
