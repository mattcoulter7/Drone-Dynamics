using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using UnityEngine;


namespace SocketRunner
{
    public class UDPClient : Client
    {
        private UdpClient _client;
        private IPEndPoint _remoteEndpoint;

        public UDPClient(string ipAddress, int port, Action<JObject, Client> handler) : base(ipAddress, port, handler, Protocol.UDP)
        {
        }

        public override void Connect()
        {
            _client = new UdpClient();
            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }

        public override void SendMessage(string messageType, JObject messageBody)
        {
            JObject message = new JObject();
            message["event"] = messageType;
            message.Merge(messageBody);
            string messageStr = message.ToString();
            byte[] data = Encoding.ASCII.GetBytes(messageStr);
            _client.Send(data, data.Length, _remoteEndpoint);
        }

        protected override void ListenForMessages()
        {
            while (true)
            {
                if (_client != null && _client.Available > 0)
                {
                    byte[] buffer = _client.Receive(ref _remoteEndpoint);
                    string messageStr = Encoding.ASCII.GetString(buffer);
                    JObject message = JObject.Parse(messageStr);
                    handler.Invoke(message, this);
                }
                else
                {
                    // Sleep for a short time to avoid using too much CPU
                    Thread.Sleep(10);
                }
            }
        }

        public override void Disconnect()
        {
            _client.Close();
        }
    }
}