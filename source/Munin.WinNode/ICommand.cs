namespace Munin.WinNode
{
    interface ICommand
    {
        string Command { get; }
        bool EndWithPeriod { get; }
        void Execute(string[] arguments, out string response);
    }
}
