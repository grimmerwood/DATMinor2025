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

    [Tooltip("Z position threshold after which the enemy is destroyed")]
    public float despawnZ = -10f;  // Adjust this based on your scene

    void Start()
    {
        // Store the initial position
        startPoint = transform.position;

        // Randomize oscillation direction (left or right)
        float direction = Random.value < 0.5f ? -1f : 1f;

        // Calculate endpoint based on direction
        endPoint = startPoint + Vector3.right * direction * oscillationDistance;

        // Start oscillation after a small random delay to desynchronize enemies
        float delay = Random.Range(0f, 2f);
        StartCoroutine(StartOscillationWithDelay(delay));
    }

    void Update()
    {
        // Move the object forward continuously
        transform.position += moveDirection * Time.deltaTime;

        // Move the object forward continuously
        transform.position += moveDirection * Time.deltaTime;

        // Destroy the enemy if it has passed behind the player
        if (transform.position.z < despawnZ)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator StartOscillationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(MoveBackAndForth());
    }

    IEnumerator MoveBackAndForth()
    {
        while (true)
        {
            yield return StartCoroutine(MoveTo(endPoint));
            yield return StartCoroutine(MoveTo(startPoint));
        }
    }

    IEnumerator MoveTo(Vector3 target)
    {
        float time = 0f;
        Vector3 start = transform.position;

        while (time < 1f)
        {
            time += Time.deltaTime * oscillationSpeed;
            time = Mathf.Clamp01(time);

            transform.position = new Vector3(
                Mathf.Lerp(start.x, target.x, time),
                transform.position.y,
                transform.position.z
            );

            yield return null;
        }
    }
}

