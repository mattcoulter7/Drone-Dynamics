using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace SocketRunner
{
    public class TCPClient : Client
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public TCPClient(string ipAddress, int port, Action<JObject, Client> handler) : base(ipAddress, port, handler, Protocol.TCP)
        {
        }

        public override void Connect()
        {
            _client = new TcpClient(ipAddress, port);
            _stream = _client.GetStream();
        }

        public override void SendMessage(string messageType, JObject messageBody)
        {
            JObject message = new JObject();
            message["event"] = messageType;
            message.Merge(messageBody);
            string messageStr = message.ToString();
            byte[] data = Encoding.ASCII.GetBytes(messageStr);
            _stream.Write(data, 0, data.Length);
        }

        protected override void ListenForMessages()
        {
            while (true)
            {
                if (_client != null && _client.Available > 0)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string messageStr = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        JObject message = JObject.Parse(messageStr);
                        handler.Invoke(message, this);
                    }
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
            _stream.Close();
            _client.Close();
        }
    }
}