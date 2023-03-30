using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketRunner
{
    public abstract class Client
    {
        protected Action<JObject, Client> _handler;

        public Client(Action<JObject, Client> handler)
        {
            _handler = handler;
        }
        public void Start()
        {
            // Start a background task to listen for incoming messages
            Task.Factory.StartNew(() => ListenForMessages());
        }
        public abstract void SendMessage(string messageType, JObject messageBody);
        public abstract void Disconnect();
        protected abstract void ListenForMessages();
    }
}