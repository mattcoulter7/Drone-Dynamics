using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketRunner;
using System;
using Unity.VisualScripting;
using System.Linq;

public class Multiplayer : MonoBehaviour, ISyncer
{
    [SerializeField] private GameObject multiplayerPrefab;

    private Connection connection = null;

    private Dictionary<string, ISyncIn> inRegistry { get; set; } = new Dictionary<string, ISyncIn>();
    private Dictionary<string, ISyncOut> outRegistry { get; set; } = new Dictionary<string, ISyncOut>();
    private Dictionary<string, long> syncTimes { get; set; } = new Dictionary<string, long>();
    private Dictionary<string, GameObject> objectRegistry { get; set; } = new Dictionary<string, GameObject>();

    private void Start()
    {
        connection = FindObjectOfType<Connection>();
        if (connection != null)
        {
            connection.On("data", SyncIn);

            // Start a background task to listen for incoming messages
            StartCoroutine(SyncCoroutine());
        }
    }
    public IEnumerator SyncCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SyncOut();
            yield return new WaitForEndOfFrame();
        }
    }
    public void RegisterIn(ISyncIn obj)
    {
        inRegistry[obj._id] = obj;
    }
    public void DeregisterIn(string _id)
    {
        inRegistry.Remove(_id);
    }
    public void RegisterOut(ISyncOut obj)
    {
        outRegistry[obj._id] = obj;
    }
    public void DeregisterOut(string _id)
    {
        outRegistry.Remove(_id);
    }
    private void SyncOut()
    {
        if (connection == null) return;

        foreach (var pair in outRegistry)
        {
            JObject packet = new JObject();
            pair.Value.SyncOut(ref packet, this);
            connection.Send("data", packet, Protocol.UDP);
        }
    }
    private void SyncIn(JObject data, Client sender)
    {
        string _id = data.Value<string>("_id");
        if (outRegistry.ContainsKey(_id)) return; // Don't sync itself


        // Check to see if we have already synced more recent data
        long _time = data.Value<long>("_time");
        syncTimes.TryGetValue(_id, out long _last_synced_time);
        if (_time < _last_synced_time)
        {
            return;
        }
        syncTimes[_id] = _time;

        // Handle Creating the game object
        string object_id = data.Value<string>("object_id");
        GameObject instance = null;
        objectRegistry.TryGetValue(object_id, out instance);
        if (instance == null)
        {
            string object_type = data.Value<string>("object_type");
            instance = Instantiate(multiplayerPrefab);
            objectRegistry[object_id] = instance;
        }

        ISyncIn obj = null;
        inRegistry.TryGetValue(_id, out obj);

        if (obj == null)
        {
            string _type = data.Value<string>("_type");

            Type type = Type.GetType(_type);
            if (type == null)
            {
                Debug.LogWarning($"Multiplayer Synced Type {_type} is not a real type.");
                return;
            }

            int _index = data.Value<int>("_index");
            obj = (ISyncIn)instance.GetComponentsInChildren(type)[_index];
            if (obj == null)
            {
                Debug.LogWarning($"Multiplayer Prefab Variant {multiplayerPrefab.name} does not have a {_type}.");
                return;
            }

            obj.Spawn(data, this);
        }

        obj.SyncIn(data, this);
    }
}