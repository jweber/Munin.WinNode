namespace Munin.WinNode.Commands
{
    interface ICommand
    {
        string Command { get; }
        void Execute(out string response);
    }
}
