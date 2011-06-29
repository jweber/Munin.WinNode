using System;
using System.Threading.Tasks;
using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    class Program
    {
        static void Main(string[] args)
        {
            new Task(() => new TcpServer()).Start();

            Console.WriteLine(string.Format("Munin.WinNode version: {0}", VersionCommand.GetVersionString()));
            Console.WriteLine("Press <enter> to stop");
            while(true)
            {
                var readLine = Console.ReadLine() ?? "";
                switch (readLine.ToLowerInvariant())
                {
                    case "cls":
                        Console.Clear();
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
