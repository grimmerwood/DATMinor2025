using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Which prefab to spawn on fire.")]
    public GameObject projectilePrefab;

    [Tooltip("Which position to spawn the projectiles from.")]
    public Transform spawnPoint;

    [Tooltip("How fast the player moves left and right.")]
    public float moveSpeed = 3f;

    [Tooltip("The amount of rotation applied while the player is moving.")]
    public Vector3 rotationAmount = new Vector3(0, 45f, 0);

    // A timestamp of the moment we last fired
    private float lastTimeFired;

    // Update is called once per frame
    void Update()
    {
        // Move and rotate the player depending on what buttons we are pressing.
        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = Quaternion.identity;
        
        if( Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) )
        {
            targetPosition.x -= moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(rotationAmount);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) )
        {
            targetPosition.x += moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(-rotationAmount);
        }
        
        transform.position = targetPosition;
        // This lerp makes the rotation smooth.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);

        // If we are holding space, pew pew!
        if( Input.GetKey(KeyCode.Space) )
        {
            Fire();
        }
    }

    // We call this method to pew pew.
    private void Fire()
    {
        // If we don't have a projectile prefab, log a warning.
        if (projectilePrefab == null)
        {
            Debug.LogWarning(name + " does not have a projectile prefab assigned.");
            return; // Return is a keyword that exits the method early.
        }
        
        // Get the projectile settings from our attached projectile prefab.
        ProjectileSettings projectileSettings = projectilePrefab.GetComponent<ProjectileSettings>();

        // If the projectile doesn't have settings, we can't fire it. Log a warning.
        if (projectileSettings == null)
        {
            Debug.LogWarning("The equipped projectile is missing settings.");
        }
        // If we have fired too recently, don't fire again.
        else if (Time.time - lastTimeFired > projectileSettings.firingSpeed)
        {
            lastTimeFired = Time.time;
            // Spawn a new instance of the prefab.
            Instantiate(projectilePrefab, spawnPoint.transform.position, Quaternion.identity);
        }
    }
}