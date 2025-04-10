using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{


    [Header("Bullet Settings")]
    public GameObject bulletPrefab;      // The bullet to fire
    public Transform firePoint;          // Where bullets spawn from
    public float bulletSpeed = 10f;      // How fast the bullets travel

    [Header("Firing Pattern Settings")]
    public int bulletsPerShot = 3;       // Number of bullets per shot (use odd numbers for a centered shot)
    public float spreadAngle = 30f;      // Total angle between outermost bullets
    public float shootInterval = 1.5f;   // Time between shots

    void Start()
    {
        // Start firing repeatedly every shootInterval seconds
        InvokeRepeating("FireBullets", shootInterval, shootInterval);
    }

    void FireBullets()
    {
        if (bulletPrefab == null || firePoint == null || bulletsPerShot <= 0) return;

        // If just one bullet, shoot it forward
        if (bulletsPerShot == 1)
        {
            ShootBullet(firePoint.forward);
            return;
        }

        // Spread multiple bullets in a fan shape
        float angleStep = spreadAngle / (bulletsPerShot - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = startAngle + angleStep * i;

            // Create a new direction by rotating the forward vector
            Vector3 direction = Quaternion.Euler(0, angle, 0) * firePoint.forward;

            ShootBullet(direction);
        }
    }

    void ShootBullet(Vector3 direction)
    {
        // Create the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Move it forward
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * bulletSpeed;
        }

    }
}


