using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSettings : MonoBehaviour
{
    [Tooltip("How many seconds between shots.")]
    public float firingSpeed = 1f;

    [Tooltip("Speed of the projectile.")]
    public float moveSpeed = 10f;

    [Tooltip("How long before it self-destructs.")]
    public float lifetime = 3f;

    [Tooltip("The damage this projectile deals.")]
    public float damage = 1f;

    [Tooltip("The sound effect that plays when fired.")]
    public AudioClip fireSound;

    void Start()
    {
        // Play fire sound once at creation
        if (fireSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(fireSound);
        }

        // Destroy the bullet after some time
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward each frame
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit");
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                Debug.Log("Damage");
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy bullet
        }
    }
}