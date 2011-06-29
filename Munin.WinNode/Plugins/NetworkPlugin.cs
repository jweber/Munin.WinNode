namespace Munin.WinNode.Plugins
{
    class NetworkPlugin : IPlugin
    {
        public string Name
        {
            get { return "network"; }
        }

        public string GetConfiguration()
        {
            return "network configuration";
        }

        public string GetValues()
        {
            return "network values";
        }
    }
}
