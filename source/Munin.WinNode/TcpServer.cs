using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Munin.WinNode
{
    class TcpServer : IDisposable
    {
        readonly TcpListener _listener;
        readonly Thread _thread;

        public TcpServer()
        {
            this.Host = Configuration.GetValue("MuninNode", "Host", "*");
            this.Port = Configuration.GetValue("MuninNode", "Port", 4949);
            
            _listener = new TcpListener(HostToIpAddress(this.Host), this.Port);
            _thread = new Thread(ListenForClient);
        }

        public string Host { get; private set; }
        public int Port { get; private set; }

        private IPAddress HostToIpAddress(string host)
        {
            if (string.IsNullOrWhiteSpace(host) || host == "*")
                return IPAddress.Any;

            return IPAddress.Parse(host);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            _thread.Abort();
            _listener.Stop();
        }

        private void ListenForClient()
        {
            Logging.Info("Waiting for connection...");
            this._listener.Start();
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(object client)
        {
            var tcpClient = (TcpClient) client;
            Logging.Info("Received connection from {0}", tcpClient.Client.RemoteEndPoint);
            
            var data = new byte[tcpClient.ReceiveBufferSize];
            var dataString = new StringBuilder();
          
            using (var stream = tcpClient.GetStream())
            {
                WelcomeMessage(stream);

                int readCount;
                
                while ((readCount = stream.Read(data, 0, tcpClient.ReceiveBufferSize)) != 0)
                {
                    dataString.Append(Encoding.ASCII.GetString(data, 0, readCount));
                    Logging.Debug("Input: {0}", Regex.Replace(dataString.ToString(), @"\r?\n", string.Empty));
                    if (Regex.IsMatch(dataString.ToString(), @"\r?\n"))
                    {
                        string message = NormalizeMessage(dataString.ToString());
                            
                        if (message == "QUIT")
                            break;

                        MessageHandler(stream, message);
                        dataString.Clear();
                    }
                }

                Logging.Info("Closing connection from {0}", tcpClient.Client.RemoteEndPoint);
            }

            tcpClient.Close();
        }

        string NormalizeMessage(string message)
        {
            message = message.ToUpper();
            message = Regex.Replace(message, @"\r?\n", string.Empty);
            return message;
        }

        void WelcomeMessage(Stream stream)
        {
            var welcomeMessage = Encoding.ASCII.GetBytes(string.Format("# munin node at {0}{1}", Dns.GetHostName(), Configuration.NewLine));
            stream.Write(welcomeMessage, 0, welcomeMessage.Length);
        }

        static void MessageHandler(NetworkStream stream, string message)
        {
            var messageParts = MessageParts.FromString(message);
            var command = CommandManager.CommandFromName(messageParts.Command);
            
            string response = null;
            command.Execute(messageParts.Arguments, out response);
            
            if (! string.IsNullOrEmpty(response))
            {
                string responseMessage = string.Format("{1}{0}", Configuration.NewLine, response);
                if (command.EndResponseWithPeriod)
                    responseMessage += "." + Configuration.NewLine;

                var responseBytes = Encoding.ASCII.GetBytes(responseMessage);
                stream.Write(responseBytes, 0, responseBytes.Length);

                Logging.Debug("Response: {0}{1}{0}", Configuration.NewLine, responseMessage);
            }
        }
    }
}
