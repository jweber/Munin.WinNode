using System.ServiceProcess;

namespace Munin.WinNode
{
    partial class MuninWinNodeService : ServiceBase
    {
        TcpServer _server;

        public MuninWinNodeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            PluginManager.RegisterPlugins();
            _server = new TcpServer();
            _server.Start();
        }

        protected override void OnStop()
        {
            if (_server != null)
                _server.Dispose();
        }
    }
}
