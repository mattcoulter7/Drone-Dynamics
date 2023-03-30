using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class SyncTransform : SyncComponent
{
    public Vector3 targetPosition = new Vector3();
    public Vector3 targetEulerAngles = new Vector3();
    public float interpolation = 0.2f;

    public override void SyncIn(JObject data, ISyncer syncer)
    {
        targetPosition = new Vector3(
            (float)data["transform"]["position"][0],
            (float)data["transform"]["position"][1],
            (float)data["transform"]["position"][2]
        ) + transform.forward * 3;
        targetEulerAngles = new Vector3(
            (float)data["transform"]["eulerAngles"][0],
            (float)data["transform"]["eulerAngles"][1],
            (float)data["transform"]["eulerAngles"][2]
        );
    }
    public override void SyncOut(ref JObject data, ISyncer syncer)
    {
        base.SyncOut(ref data, syncer);
        data["transform"] = new JObject();
        data["transform"]["position"] = new JArray(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
        data["transform"]["eulerAngles"] = new JArray(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );
    }
    private void Update()
    {
        if (!local)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, interpolation);
            transform.eulerAngles = targetEulerAngles;
        }
    }
}