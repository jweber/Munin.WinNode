using System;
using System.Collections.Generic;
using System.Linq;
using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    /// <summary>
    /// Class to manage the various commands that the Munin Node protocol defines.
    /// </summary>
    static class CommandManager
    {
        static readonly IEnumerable<ICommand> _commands;
        
        static CommandManager()
        {
            _commands = FindCommands();
        }

        /// <summary>
        /// Registers any implementor of <see cref="ICommand"/> in the current
        /// assembly.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<ICommand> FindCommands()
        {
            return (from t in typeof(ICommand).Assembly.GetTypes()
                    where typeof(ICommand).IsAssignableFrom(t)
                        && ! t.IsInterface
                        && t != typeof(UnknownCommand)
                    select Activator.CreateInstance(t)).Cast<ICommand>().ToList();
        }

        /// <summary>
        /// Returns the <see cref="ICommand"/> instance from its <see cref="ICommand.Command"/>
        /// string name value. Returns <see cref="UnknownCommand"/> if no command exists.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ICommand CommandFromName(string message)
        {
            var command = _commands
                .Where(c => c.Command.Equals(message, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return command ?? new UnknownCommand();
        }

        /// <summary>
        /// Returns all available <see cref="ICommand.Command"/> values from
        /// registered commands.
        /// </summary>
        /// <returns></returns>
        public static string[] AllCommands()
        {
            return _commands
                .Select(c => c.Command)
                .ToArray();
        }
    }
}
