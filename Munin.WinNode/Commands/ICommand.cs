using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Munin.WinNode.Commands
{
    interface ICommand
    {
        string Command { get; }
        void Execute(out string response);
    }
}
