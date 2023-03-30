using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace SocketRunner
{
    public abstract class Client
    {
        protected string ipAddress { get; private set; }
        protected int port { get; private set; }
        public Protocol protocol { get; private set; }

        protected Action<JObject, Client> handler { get; private set; }

        public Client(string ipAddress, int port, Action<JObject, Client> handler, Protocol protocol)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.handler = handler;
            this.protocol = protocol;

            // Start a background task to listen for incoming messages
            Task.Factory.StartNew(() => ListenForMessages());
        }
        public abstract void SendMessage(string messageType, JObject messageBody);
        public abstract void Connect();
        public abstract void Disconnect();
        protected abstract void ListenForMessages();
    }
}