using System.ComponentModel;
using System.ServiceProcess;

namespace Munin.WinNode
{
    [RunInstaller(true)]
    public sealed class ProjectServiceInstaller : ServiceInstaller
    {
        public const string ProjectServiceName = "Munin.WinNode";

        public ProjectServiceInstaller()
        {
            this.Description = ProjectServiceName;
            this.DisplayName = ProjectServiceName;
            this.ServiceName = ProjectServiceName;
            this.StartType = ServiceStartMode.Automatic;
        }
    }

    [RunInstaller(true)]
    public sealed class ProjectServiceProcessInstaller : ServiceProcessInstaller
    {
        public ProjectServiceProcessInstaller()
        {
            this.Account = ServiceAccount.LocalSystem;
        }
    }
}
