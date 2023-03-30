using Newtonsoft.Json.Linq;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

public abstract class SyncComponent : MonoBehaviour, ISyncIn, ISyncOut
{
    public Protocol protocol { get; set; } = Protocol.UDP;
    public bool local { get; private set; }
    public string _id { get; set; } = "";
    public string _type { get; private set; } = "";
    public int _index { get; private set; } = 0;
    private ISyncer syncer;
    private UniqueID object_id;
    public virtual void Start()
    {
        Type type = GetType();
        _type = type.Name;
        syncer = FindObjectOfType<Multiplayer>();
        object_id = GetComponentInParent<UniqueID>();

        local = object_id != null;
        if (local)
        {
            _id = Guid.NewGuid().ToString();
            _index = object_id.GetComponentsInChildren(type).ToList().FindIndex(obj => obj == this);
            syncer.RegisterOut(this);
        }
    }
    public virtual void Spawn(JObject data, ISyncer syncer) {
        _id = data.Value<string>("_id");
        syncer.RegisterIn(this);
    }
    public abstract void SyncIn(JObject data, ISyncer syncer);
    public virtual void SyncOut(ref JObject data, ISyncer syncer) {
        if (!local)
            throw new Exception("Trying to sync out and incoming object");

        data["_id"] = _id;
        data["_type"] = _type;
        data["_index"] = _index;
        data["_time"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        data["object_id"] = object_id._id;
        data["object_type"] = object_id.gameObject.tag;
    }
    private void OnDestroy()
    {
        if (syncer == null) return;
        if (local)
            syncer.DeregisterOut(_id);
        else
            syncer.DeregisterIn(_id);
    }
}