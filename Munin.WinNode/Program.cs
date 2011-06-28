using System;
using System.Threading;
using System.Threading.Tasks;

namespace Munin.WinNode
{
    class Program
    {
        static void Main(string[] args)
        {
            new Task(() => new TcpServer()).Start();

            Console.WriteLine("Press enter to stop");
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
