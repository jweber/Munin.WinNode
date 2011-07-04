using System.ComponentModel;
using System.ServiceProcess;

namespace Munin.WinNode
{
    [RunInstaller(true)]
    public sealed class ProjectServiceInstaller : ServiceInstaller
    {
        public const string ServiceName = "Munin.WinNode";

        public ProjectServiceInstaller()
        {
            this.Description = ServiceName;
            this.DisplayName = ServiceName;
            base.ServiceName = ServiceName;
            this.StartType = ServiceStartMode.Automatic;
        }
    }

    [RunInstaller(true)]
    public sealed class ProjectServiceProcessInstaller : ServiceProcessInstaller
    {
        public ProjectServiceProcessInstaller()
        {
            this.Account = ServiceAccount.NetworkService;
        }
    }
}
