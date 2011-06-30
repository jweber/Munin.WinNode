using System;
using System.Collections.Generic;
using System.Linq;

namespace Munin.WinNode
{
    static class PluginManager
    {
        static readonly IEnumerable<IPlugin> _plugins;
        
        static PluginManager()
        {
            _plugins = FindPlugins();
        }

        static IEnumerable<IPlugin> FindPlugins()
        {
            return (from t in typeof(IPlugin).Assembly.GetTypes()
                    where typeof(IPlugin).IsAssignableFrom(t)
                        && ! t.IsInterface
                    select Activator.CreateInstance(t)).Cast<IPlugin>().ToList();
        }

        /// <summary>
        /// Returns the <see cref="IPlugin"/> instance from its <see cref="IPlugin.Name"/>
        /// string name value. Returns <c>null</c> if no plugin exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IPlugin PluginFromName(string name)
        {
            var plugin = _plugins
                .Where(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return plugin;
        }

        /// <summary>
        /// Returns all available <see cref="IPlugin.Name"/> values from
        /// registered plugins.
        /// </summary>
        /// <returns></returns>
        public static string[] AllPlugins()
        {
            return _plugins
                .Select(c => c.Name)
                .ToArray();
        }
    }
}
