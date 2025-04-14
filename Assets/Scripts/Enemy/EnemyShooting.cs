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
        // Tell Unity to never let enemy bullets collide with enemies
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyBullet"), LayerMask.NameToLayer("Enemy"));

        // Start firing bullets
        InvokeRepeating("FireBullets", shootInterval, shootInterval);
    }

    void FireBullets()
    {
        if (bulletPrefab == null || firePoint == null || bulletsPerShot <= 0) return;

        if (bulletsPerShot == 1)
        {
            ShootBullet(firePoint.forward);
            return;
        }

        float angleStep = spreadAngle / (bulletsPerShot - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * firePoint.forward;
            ShootBullet(direction);
        }
    }

    void ShootBullet(Vector3 direction)
    {
        Vector3 spawnPosition = firePoint.position + direction.normalized * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.layer = LayerMask.NameToLayer("EnemyBullet");



        // Set up Hazard component on the bullet
        Hazard hazard = bullet.GetComponent<Hazard>();
        if (hazard != null)
        {
            hazard.isPlayer = false;                // Enemy bullet
            hazard.damage = 1f;                     // Damage to player
            hazard.deleteAfterCollision = true;     // Bullet disappears after hit
        }


        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * bulletSpeed;
        }
       
        // Ensure the bullet is on the right layer
        bullet.layer = LayerMask.NameToLayer("EnemyBullet");

       
    }
}



