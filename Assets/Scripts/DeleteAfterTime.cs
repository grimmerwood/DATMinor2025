using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{
    [Tooltip("How many seconds before this object is deleted.")]
    public float timeToDelete = 1f;
    
    // How much time has passed since this object was created. Starts at 0.
    private float timeAlive = 0f;
    
    // Update is called once per frame
    void Update()
    {
        // Add the time since last frame to the `timeAlive` variable.
        timeAlive += Time.deltaTime;

        // If this object has lived longer than we want it to...
        if (timeAlive > timeToDelete)
        {
            // ... then delete it! See you Space Cowboy.
            Destroy(gameObject);
        }
    }
}
