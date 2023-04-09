using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    [SerializeField] private int numCircuits = 0;
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
    private void Start()
    {
        
    }
    
    public CheckpointState GetNextCheckpoint(CheckpointState checkpointState)
    {
        if (checkpoints.Count == 0) return null;
        // first checkpoint assignment
        if (checkpointState.nextCheckpoint == null)
            return new CheckpointState(
                checkpoints[0], 
                checkpointState.nextCheckpoint, 
                checkpointState.circuitIndex
            );

        int lastNextCheckpointIndex = checkpoints.IndexOf(checkpointState.nextCheckpoint);
        int nextCheckpointIndex = lastNextCheckpointIndex + 1;

        // lap completed
        if (nextCheckpointIndex > checkpoints.Count - 1)
        {
            Checkpoint nextCheckpoint = null;
            if (checkpointState.circuitIndex < numCircuits - 1)
            {
                nextCheckpoint = checkpoints[0];
            }
            return new CheckpointState(
                nextCheckpoint,
                checkpointState.nextCheckpoint,
                checkpointState.circuitIndex + 1
            );
        }

        // checkpoint within lap
        return new CheckpointState(
            checkpoints[nextCheckpointIndex], 
            checkpointState.nextCheckpoint, 
            checkpointState.circuitIndex
        );
    }

    public bool HasCheckpoints()
    {
        return checkpoints.Count > 0;
    }
}
