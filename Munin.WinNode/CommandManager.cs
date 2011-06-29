using System;
using System.Collections.Generic;
using System.Linq;
using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    static class CommandManager
    {
        static readonly IEnumerable<ICommand> _commands;
        
        static CommandManager()
        {
            _commands = FindCommands();
        }

        static IEnumerable<ICommand> FindCommands()
        {
            return (from t in typeof(ICommand).Assembly.GetTypes()
                    where typeof(ICommand).IsAssignableFrom(t)
                        && ! t.IsInterface
                        && t != typeof(UnknownCommand)
                    select Activator.CreateInstance(t)).Cast<ICommand>();
        }

        public static ICommand CommandFromName(string message)
        {
            return _commands.Where(c => c.Command.Equals(message, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public static string[] AllCommands()
        {
            return _commands.Select(c => c.Command).ToArray();
        }
    }
}
