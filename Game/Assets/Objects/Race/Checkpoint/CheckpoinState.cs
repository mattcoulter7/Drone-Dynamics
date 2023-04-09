using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CheckpointState
{
    [field: SerializeField] public Checkpoint nextCheckpoint { get; private set; }
    [field: SerializeField] public Checkpoint lastCheckpoint { get; private set; }
    [field: SerializeField] public int circuitIndex { get; private set; }
    public CheckpointState(
        Checkpoint nextCheckpoint,
        Checkpoint lastCheckpoint,
        int circuitIndex
    )
    {
        this.nextCheckpoint = nextCheckpoint;
        this.lastCheckpoint = lastCheckpoint;
        this.circuitIndex = circuitIndex;
    }

    public void OnChanged()
    {
        if (lastCheckpoint != null)
        {
            lastCheckpoint.OnLostTarget();
        }
        if (nextCheckpoint != null)
        {
            nextCheckpoint.OnBecameTarget();
        }
    }
}