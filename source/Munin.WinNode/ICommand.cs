namespace Munin.WinNode
{
    interface ICommand
    {
        string Command { get; }
        void Execute(string[] arguments, out string response);
    }
}
