using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
    [SerializeField] private bool reverseLap = false;
    [SerializeField] private int numCircuits = 0;
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
    private void Start()
    {
        
    }
    
    public CheckpointState GetNextCheckpointState(CheckpointState checkpointState)
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
        int nextCheckpointIndex = checkpointState.reverse ? lastNextCheckpointIndex - 1 : lastNextCheckpointIndex + 1;

        Checkpoint nextCheckpoint = null;
        int circuitIndex = checkpointState.circuitIndex;
        bool reverse = checkpointState.reverse;

        // lap completed
        if (nextCheckpointIndex > checkpoints.Count - 1 || nextCheckpointIndex < 0)
        {
            if (reverseLap)
                reverse = !reverse;

            circuitIndex++;

            if (circuitIndex < numCircuits)
            {
                if (reverse)
                {
                    nextCheckpointIndex = checkpoints.Count - 2;
                }
                else
                {
                    nextCheckpointIndex = 0;
                }
            }
            else
            {
                nextCheckpointIndex = -1;
            }
        }

        nextCheckpoint = nextCheckpointIndex == -1 ? null : checkpoints[nextCheckpointIndex];

        // checkpoint within lap
        return new CheckpointState(
            nextCheckpoint, 
            checkpointState.nextCheckpoint,
            circuitIndex,
            reverse
        );
    }

    public bool HasCheckpoints()
    {
        return checkpoints.Count > 0;
    }
}
