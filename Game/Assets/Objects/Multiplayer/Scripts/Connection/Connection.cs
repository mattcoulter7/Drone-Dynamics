using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using SocketRunner;
using System;
using System.Threading.Tasks;

public class Connection : MonoBehaviour
{
    public string tcpHost;
    public int tcpPort;
    public long tcpPing { get; private set; } = -1;
    public string udpHost;
    public int udpPort;
    public long udpPing { get; private set; } = -1;

    private Dictionary<Protocol, SocketRunner.Client> clients = new Dictionary<Protocol, SocketRunner.Client>();
    private Dictionary<string, Action<JObject, SocketRunner.Client>> handlers = new Dictionary<string, Action<JObject, SocketRunner.Client>>();

    // Start is called before the first frame update
    private void Start()
    {
        RegisterClient(new TCPClient(tcpHost, tcpPort, OnMessage));
        RegisterClient(new UDPClient(udpHost, udpPort, OnMessage));

        On("pong", (data, sender) =>
        {
            long send_time = data.Value<long>("time");
            long receive_time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (sender.protocol == Protocol.UDP)
                udpPing = (receive_time - send_time);
            else if (sender.protocol == Protocol.TCP)
                tcpPing = (receive_time - send_time);
        });

        StartCoroutine(PingCoroutine());
    }

    private void RegisterClient(SocketRunner.Client client)
    {
        Task.Factory.StartNew(() =>
        {
            try
            {
                client.Connect();
            }
            catch (Exception)
            {
                Debug.Log("Unable to connect to server");
            }
        });
        clients[client.protocol] = client;
    }

    private SocketRunner.Client GetClient(Protocol protocol)
    {
        clients.TryGetValue(protocol, out var client);
        return client;
    }

    public void Send(string type, JObject data, Protocol protocol)
    {
        var client = GetClient(protocol);
        if (client == null) return;

        client.SendMessage(type, data);
    }

    private void OnMessage(JObject data, SocketRunner.Client sender)
    {
        string messageType = data.Value<string>("event");
        if (handlers.ContainsKey(messageType))
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                handlers[messageType](data, sender);
            });
        }
    }

    private IEnumerator PingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            JObject data = new JObject();
            data["time"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            try
            {
                Send("ping", data, Protocol.TCP);
            } 
            catch (Exception)
            {
                Debug.LogWarning("Unable to communicate with TCP server");
            }

            try
            {
                Send("ping", data, Protocol.UDP);
            }
            catch (Exception)
            {
                Debug.LogWarning("Unable to communicate with UDP server");
            }
        }
    }

    public void On(string messageType, Action<JObject, SocketRunner.Client> handler)
    {
        if (!handlers.ContainsKey(messageType))
        {
            handlers[messageType] = handler;
        }
        else
        {
            handlers[messageType] += handler;
        }
    }
}
