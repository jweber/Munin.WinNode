using System.Net;

namespace Munin.WinNode.Commands
{
    class VersionCommand : ICommand
    {
        public string Command
        {
            get { return "version"; }
        }

        public bool EndResponseWithPeriod
        {
            get { return false; }
        }

        public void Execute(string[] arguments, out string response)
        {

            response = string.Format("Munin.WinNode on {0} version: {1}",
                                     Dns.GetHostName(),
                                     GetVersionString(true));
        }

        public static string GetVersionString(bool includeRevision = false)
        {
            var version = typeof(Program).Assembly.GetName().Version;
            if (! includeRevision)
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            return version.ToString();
        }
    }
}
