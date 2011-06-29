﻿using System;

namespace Munin.WinNode.Commands
{
    /// <summary>
    /// Returns a list of all available plugins on this node
    /// </summary>
    class ListCommand : ICommand
    {
        public string Command
        {
            get { return "list"; }
        }

        public void Execute(out string response)
        {
            throw new NotImplementedException();
        }
    }
}
