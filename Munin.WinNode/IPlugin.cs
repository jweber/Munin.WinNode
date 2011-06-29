namespace Munin.WinNode
{
    interface IPlugin
    {
        string Name { get; }
        string GetConfiguration();
        string GetValues();
    }
}
