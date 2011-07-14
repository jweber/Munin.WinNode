using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Munin.WinNode
{
    static class PluginManager
    {
        static readonly List<IPlugin> _plugins = new List<IPlugin>();
       
        /// <summary>
        /// Registers all plugins
        /// </summary>
        public static void RegisterPlugins()
        {
            RegisterPlugins(Assembly.GetCallingAssembly());
            RegisterExternalPlugins();
        }

        static void RegisterExternalPlugins()
        {
            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            var assemblyPaths = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly);

            foreach ( var assemblyPath in assemblyPaths )
            {

                byte[] assemblyBytes = null;
                try
                {
                    assemblyBytes = File.ReadAllBytes(assemblyPath);
                }
                catch (Exception)
                {
                    Logging.Error("Failed to read in assembly '{0}'", assemblyPath);
                }

                if (assemblyBytes != null)
                {
                    try
                    {
                        RegisterPlugins(AppDomain.CurrentDomain.Load(assemblyBytes));
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(string.Format("Failed initializing plugins from assembly '{0}'", assemblyPath), ex);
                    }
                }
            }
        }

        static void RegisterPlugins(Assembly assembly)
        {
            var assemblyPlugins = (from p in assembly.GetTypes()
                                   where typeof(IPlugin).IsAssignableFrom(p)
                                         && ! p.IsInterface
                                   select p).ToList();

            foreach (var assemblyPlugin in assemblyPlugins)
            {
                try
                {
                    var plugin = (IPlugin) Activator.CreateInstance(assemblyPlugin);
                    if (!plugin.IsApplicable)
                    {
                        Logging.Debug("Skipped plugin '{0} due to being not applicable'", plugin.Name);
                        continue;
                    }

                    Logging.Info("Registered the '{0}' plugin from '{1}'", plugin.Name, plugin.GetType().FullName);
                    _plugins.Add(plugin);
                }
                catch (Exception)
                {
                    Logging.Error("Failed to initialize the plugin from type '{0}'", assemblyPlugin.FullName);
                }
            }
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
