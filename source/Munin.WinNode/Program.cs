using System;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += EmbeddedAssemblyResolver;

            ConfigureLog4Net();

            if (Environment.UserInteractive)
            {
                ProcessArguments(args);
            }
            else
            {
                ServiceBase.Run(new MuninWinNodeService());
            }
        }

        /// <summary>
        /// All references to embedded DLL assembly resources need to be out of the <c>Main</c> method
        /// as the JIT compilation will attempt to load the assembly before our overloaded AssemblyResolve
        /// delegate has been added.
        /// </summary>
        static void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
        }

        static Assembly EmbeddedAssemblyResolver(object sender, ResolveEventArgs args)
        {
            var resourceName = string.Format("Munin.WinNode.Resources.{0}.dll", new AssemblyName(args.Name).Name);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                var assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);

                return Assembly.Load(assemblyData);
            }
        }

        private static void ProcessArguments(string[] args)
        {
            switch(GetArgument(args))
            {
                case "interactive":
                    InteractiveRun();
                    break;
                case "install":
                    InstallAndStart();
                    break;
                case "uninstall":
                    StopAndUninstall();
                    break;
                case "start":
                    StartService();
                    break;
                case "stop":
                    StopService();
                    break;
                case "restart":
                    RestartService();
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        private static void InteractiveRun()
        {
            PluginManager.RegisterPlugins();

            using (var tcpServer = new TcpServer())
            {
                tcpServer.Start();

                Console.WriteLine(string.Format("Munin.WinNode version: {0}", VersionCommand.GetVersionString()));
                Console.WriteLine(string.Format("Listening on {0}:{1}", tcpServer.Host, tcpServer.Port));
                Console.WriteLine("Press <enter> to stop");
                while(true)
                {
                    var readLine = Console.ReadLine() ?? "";
                    switch (readLine.ToLowerInvariant())
                    {
                        case "cls":
                            Console.Clear();
                            break;
                        default:
                            return;
                    }
                }               
            }            
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"
Munin.WinNode
--------------------------------------
Command line options:
    Munin.WinNode                - start command line server mode
    Munin.WinNode /install       - installs and starts the Windows service
    Munin.WinNode /uninstall     - stops and uninstalls the Windows service
    Munin.WinNode /start         - starts the installed Windows service
    Munin.WinNode /stop          - stops the installed Windows service
    Munin.WinNode /restart       - restarts the installed Windows service
");
        }

        private static string GetArgument(string[] args)
        {
            if (args == null || args.Length == 0)
                return "interactive";

            if (args[0].StartsWith("/") == false)
                return "help";

            return args[0].Substring(1).ToLowerInvariant();
        }

        private static void StopService()
        {
            var serviceController = new ServiceController(ProjectServiceInstaller.ProjectServiceName);
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        private static void StartService()
        {
            var serviceController = new ServiceController(ProjectServiceInstaller.ProjectServiceName);
            if (serviceController.Status != ServiceControllerStatus.Running)
            {
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
        }

        private static void RestartService()
        {
            var serviceController = new ServiceController(ProjectServiceInstaller.ProjectServiceName);
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            }

            if (serviceController.Status != ServiceControllerStatus.Running)
            {
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
        }

        private static void InstallAndStart()
        {
            if (ServiceIsInstalled())
            {
                Console.WriteLine("Service is already installed");
            }
            else
            {
                ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                var startController = new ServiceController(ProjectServiceInstaller.ProjectServiceName);
                startController.Start();
            }
        }

        private static void StopAndUninstall()
        {
            if (!ServiceIsInstalled())
            {
                Console.WriteLine("Service is not installed");
            }
            else
            {
                var serviceController = new ServiceController(ProjectServiceInstaller.ProjectServiceName);
                if (serviceController.Status == ServiceControllerStatus.Running)
                    serviceController.Stop();

                ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
        }

        private static bool ServiceIsInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == ProjectServiceInstaller.ProjectServiceName);
        }
    }
}
