using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class RaceProgress : MonoBehaviour
{
    [SerializeField] private CheckpointState checkpointState = new CheckpointState(null, null, 0);
    private Race race;
    private void Awake()
    {
        race = FindObjectOfType<Race>();
    }
    private void Start()
    {
        OnCheckpoint(null);
    }

    private void OnRaceFinish()
    {

    }

    private void OnRaceBegin()
    {

    }

    private void OnCheckpoint(Checkpoint checkpoint)
    {
        if (race == null) return;
        if (!race.HasCheckpoints()) return;

        if (checkpoint != checkpointState.nextCheckpoint) return;

        checkpointState = race.GetNextCheckpoint(checkpointState);
        checkpointState.OnChanged();

        if (checkpointState.nextCheckpoint == null)
        {
            OnRaceFinish();
        }
        else
        {
            OnRaceBegin();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Checkpoint")) return;

        Checkpoint thisCheckpoint = other.gameObject.GetComponent<Checkpoint>();
        if (thisCheckpoint == null) return;

        OnCheckpoint(thisCheckpoint);
    }
}
