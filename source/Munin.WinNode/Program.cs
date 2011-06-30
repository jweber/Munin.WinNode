using System;
using System.Net;
using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcpServer = new TcpServer();
            tcpServer.Start();

            Console.WriteLine(string.Format("Munin.WinNode version: {0}", VersionCommand.GetVersionString()));
            Console.WriteLine(string.Format("Listening on {0}:{1}", Dns.GetHostName(), 4949));
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
                        tcpServer.Stop();
                        return;
                }
            }
        }
    }
}
