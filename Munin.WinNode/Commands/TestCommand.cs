using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Munin.WinNode.Commands
{
    class TestCommand : ICommand
    {
        public string Command
        {
            get { return "test"; }
        }

        public void Execute(out string response)
        {
            response = "this is a test command";
        }
    }
}
