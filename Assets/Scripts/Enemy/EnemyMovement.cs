using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCombined : MonoBehaviour
{
    [Header("Oscillating Movement Settings")]
    [Tooltip("Speed of oscillation movement")]
    public float oscillationSpeed = 2f;

    [Tooltip("Distance to move left and right")]
    public float oscillationDistance = 5f;

    [Header("Forward Movement Settings")]
    [Tooltip("The direction the object moves forward over time")]
    public Vector3 moveDirection = new Vector3(0f, 0f, -5f);

    private Vector3 startPoint, endPoint;

    void Start()
    {
        // Store the initial position
        startPoint = transform.position;

        // Calculate the endpoint by moving to the right
        endPoint = startPoint + Vector3.right * oscillationDistance;

        // Start the oscillation coroutine
        StartCoroutine(MoveBackAndForth());
    }

    void Update()
    {
        // Move the object forward continuously
        transform.position += moveDirection * Time.deltaTime;
    }

    IEnumerator MoveBackAndForth()
    {
        while (true) // Infinite loop to keep moving back and forth
        {
            yield return StartCoroutine(MoveTo(endPoint));
            yield return StartCoroutine(MoveTo(startPoint));
        }
    }

    IEnumerator MoveTo(Vector3 target)
    {
        float time = 0f;
        Vector3 start = transform.position; // Start from the current position

        while (time < 1f) // Loop until movement completes
        {
            time += Time.deltaTime * oscillationSpeed;
            time = Mathf.Clamp01(time);

            // Move only along the X-axis while preserving the forward movement
            transform.position = new Vector3(
                Mathf.Lerp(start.x, target.x, time), // Lerp only X movement
                transform.position.y,               // Keep Y position the same
                transform.position.z                // Keep forward movement unchanged
            );

            yield return null; // Wait for the next frame
        }
    }
}
