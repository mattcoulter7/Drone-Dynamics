using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using SocketRunner;
using System;

public class Connection : MonoBehaviour
{
    public string host;
    public int port;
    [field: SerializeField] public long ping { get; private set; } = -1;
    private TCPClient tcpClient = null;
    private UDPClient udpClient = null;

    private Dictionary<string, Action<JObject, SocketRunner.Client>> _handlers = new Dictionary<string, Action<JObject, SocketRunner.Client>>();

    // Start is called before the first frame update
    private void Start()
    {
        tcpClient = new TCPClient(host, port, OnMessage);
        udpClient = new UDPClient(host, port + 1, OnMessage);

        tcpClient.Start();
        udpClient.Start();

        On("pong", (data, sender) =>
        {
            Debug.Log("Receieving Ping");
            long send_time = data.Value<long>("time");
            long receive_time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ping = (receive_time - send_time);
        });

        StartCoroutine(PingCoroutine());
    }

    public void Send(string type, JObject data)
    {
        udpClient.SendMessage(type, data);
    }

    private void OnMessage(JObject data, SocketRunner.Client sender)
    {
        string messageType = data.Value<string>("event");
        if (_handlers.ContainsKey(messageType))
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _handlers[messageType](data, sender);
            });
        }
    }

    private IEnumerator PingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            Debug.Log("Sending Ping");
            JObject data = new JObject();
            data["time"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Send("ping", data);
        }
    }

    public void On(string messageType, Action<JObject, SocketRunner.Client> handler)
    {
        if (!_handlers.ContainsKey(messageType))
        {
            _handlers[messageType] = handler;
        }
        else
        {
            _handlers[messageType] += handler;
        }
    }
}
