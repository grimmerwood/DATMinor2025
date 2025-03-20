using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patroller : MonoBehaviour
{
    [Tooltip("The offset that this object moves in each patrol.")]
    public Vector3 patrolOffset = new Vector3(-1f, 0f, 0);
    [Tooltip("How long it takes to complete one patrol.")]
    public float timePerPatrol = 3f;
    [Tooltip("Should the object move in a sine wave pattern?")]
    public bool sineWave = true;
    [Tooltip("Should the object start at a random time offset?")]
    public bool randomTimeOffset = false;
    
    // How long we have been patrolling.
    private float timePatrolling = 0f;

    // Start is called before the first frame update
    public void Start()
    {
        // Start in the middle of the patrol.
        timePatrolling = timePerPatrol / 2f;

        // If we want to randomize our start time...
        if (randomTimeOffset)
        {
            // ... then add a random amount of time to it.
            timePatrolling += Random.Range(-timePerPatrol, timePerPatrol);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Add the time since last frame to the `timePatrolling` variable.
        timePatrolling += Time.deltaTime;

        float patrolProgress = timePatrolling / timePerPatrol;

        // Calculate the direction to move.
        float directionMultiplier;
        if (sineWave)
        {
            directionMultiplier = Mathf.Sin(patrolProgress * Mathf.PI);
        }
        else
        {
            directionMultiplier = (int)patrolProgress % 2 == 0 ? 1 : -1;
        }
        
        // Move in that direction.
        transform.localPosition += patrolOffset * directionMultiplier * Time.deltaTime / timePerPatrol;
    }
}