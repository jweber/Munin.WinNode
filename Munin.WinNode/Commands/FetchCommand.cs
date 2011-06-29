namespace Munin.WinNode.Commands
{
    /// <summary>
    /// Fetches the data made available by the specified plugin
    /// </summary>
    class FetchCommand : ICommand
    {
        public string Command
        {
            get { return "fetch"; }
        }

        public void Execute(out string response)
        {
            throw new System.NotImplementedException();
        }
    }
}
