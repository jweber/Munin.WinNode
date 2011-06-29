using System.Net;

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

            response = string.Format("Munin.WinNode on {0} version: {1}",
                                     Dns.GetHostName(),
                                     GetVersionString());
        }

        public static string GetVersionString()
        {
            var version = typeof(Program).Assembly.GetName().Version;
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
