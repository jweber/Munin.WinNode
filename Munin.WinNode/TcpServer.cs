using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    class TcpServer
    {
        public TcpServer()
        {           
            var listener = new TcpListener(IPAddress.Any, 4949);
            listener.Start(1);
            listener.BeginAcceptTcpClient(ReceiveTcpClient, listener);

            Console.WriteLine("Waiting for connection");
        }

        string NormalizeMessage(string message)
        {
            message = message.ToUpper();
            message = Regex.Replace(message, @"\r?\n", string.Empty);
            return message;
        }

        void ReceiveTcpClient(IAsyncResult asyncResult)
        {
            var listener = (TcpListener) asyncResult.AsyncState;
            if (listener.Server.IsBound)
            {
                var client = listener.EndAcceptTcpClient(asyncResult);
                Console.WriteLine("Received connection from {0}", client.Client.RemoteEndPoint);

                var data = new byte[client.ReceiveBufferSize];
                var dataString = new StringBuilder();
                using (var stream = client.GetStream())
                {
                    var welcomeMessage = Encoding.ASCII.GetBytes("# Connected to Munin.WinNode" + Environment.NewLine);
                    stream.Write(welcomeMessage, 0, welcomeMessage.Length);

                    int readCount;
                    while ((readCount = stream.Read(data, 0, client.ReceiveBufferSize)) != 0)
                    {
                        dataString.Append(Encoding.ASCII.GetString(data, 0, readCount));
                        if (Regex.IsMatch(dataString.ToString(), @"\r?\n"))
                        {
                            string message = NormalizeMessage(dataString.ToString());
                            
                            if (message == "QUIT")
                                break;

                            MessageHandler(stream, message);
                            dataString.Clear();
                        }
                    }

                    Console.WriteLine("Closing connection from {0}", client.Client.RemoteEndPoint);
                }

                client.Close();

                listener.BeginAcceptTcpClient(ReceiveTcpClient, listener);
            }
        }

        static void MessageHandler(NetworkStream stream, string message)
        {
            var command = CommandManager.CommandFromName(message) ?? new UnknownCommand();
            string response = null;
            command.Execute(out response);
            if (! string.IsNullOrEmpty(response))
            {
                var responseBytes = Encoding.ASCII.GetBytes(response + Environment.NewLine);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}
