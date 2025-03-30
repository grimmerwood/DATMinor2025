using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLerp : MonoBehaviour
{
    // Speed of movement
    public float speed = 2f;

    // Distance to move from the starting point
    public float distance = 5f;

    // Stores the start and end positions for movement
    private Vector3 startPoint, endPoint;

    void Start()
    {
        // Set the starting position
        startPoint = transform.position;

        // Calculate the endpoint by moving to the right by 'distance' units
        endPoint = startPoint + Vector3.right * distance;

        // Start the movement coroutine
        StartCoroutine(MoveBackAndForth());
    }

    IEnumerator MoveBackAndForth()
    {
        while (true) // Infinite loop to keep moving back and forth
        {
            // Move to the endpoint
            yield return StartCoroutine(MoveTo(endPoint));

            // Move back to the starting position
            yield return StartCoroutine(MoveTo(startPoint));
        }
    }

    IEnumerator MoveTo(Vector3 target)
    {
        float time = 0f;  // Tracks progress from 0 (start) to 1 (end)
        Vector3 start = transform.position; // Store the starting position

        while (time < 1f) // Loop until movement is complete
        {
            time += Time.deltaTime * speed; // Increment time based on speed

            // Ensure 'time' doesn't exceed 1 (prevents overshooting)
            time = Mathf.Clamp01(time);

            // Smoothly interpolate position between 'start' and 'target'
            transform.position = Vector3.Lerp(start, target, time);

            yield return null; // Wait for the next frame before continuing
        }
    }
}

