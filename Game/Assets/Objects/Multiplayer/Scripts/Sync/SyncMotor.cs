using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class SyncMotor: SyncComponent
{
    private Motor motor;
    public override void Start()
    {
        base.Start();
        motor = GetComponent<Motor>();
    }
    public override void Spawn(JObject data, ISyncer syncer)
    {
        base.Spawn(data, syncer);
        motor = GetComponent<Motor>();
    }

    public override void SyncIn(JObject data, ISyncer syncer)
    {
        motor.SetRPM(data.Value<float>("targetRpm"));
    }
    public override void SyncOut(ref JObject data, ISyncer syncer)
    {
        base.SyncOut(ref data, syncer);
        data["targetRpm"] = motor.targetRpm;
    }
}