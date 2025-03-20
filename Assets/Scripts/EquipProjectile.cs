using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipProjectile : MonoBehaviour
{
    [Tooltip("The prefab of the projectile to equip.")]
    public GameObject projectileToEquip;

    [Tooltip("The sound to play when this power-up is picked up.")]
    public AudioClip pickupSound;

    // This is a special function that is called when this object touches another object.
    public void OnCollisionEnter(Collision collision)
    {
        // Check if the thing that we touched has a PlayerController component.
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        
        if (playerController != null) // If the thing we collided with has a PlayerController component...
        {
            // Equip the projectile to the player.
            playerController.projectilePrefab = projectileToEquip;

            // Play the pickup sound.
            if (pickupSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(pickupSound);
            }
            
            // You could play an animation here!

            // Get rid of this power-up. Begone!
            Destroy(gameObject);
        }
    }
}
