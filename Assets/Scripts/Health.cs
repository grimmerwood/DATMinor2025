using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Health : MonoBehaviour
{
    [Tooltip("How much damage this entity takes before it dies.")]
    public float maximumHitPoints = 3;

    [Tooltip("The number of points that will be awarded upon death. Only enemies should have points.")]
    public int pointValue;

    [Tooltip("Hazards don't hurt Healths of the same faction.")]
    public bool isPlayer;

    [Tooltip("The sound to play on a hit that doesn't result in a death. Defaults to nothing.")]
    public AudioClip hitSound;

    [Tooltip("The sound to play on death. Defaults to nothing.")]
    public AudioClip deathSound;

    // A private variable to keep track of our current health.
    private float currentHitPoints;

    // A public function to get our current health. The UIController uses this to display how much health we have.
    public float GetCurrentHitPoints()
    {
        return currentHitPoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = maximumHitPoints;
        
        // If we don't have a collider, warn the user.
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning(name + " is missing a collider!");
        }
    }

    // A public function that other scripts call to damage this entity.
    public void TakeDamage(float damageAmount)
    {
        ModifyHitPoints(-damageAmount);
    }

    //This function adds or subtracts health
    private void ModifyHitPoints(float modAmount )
    {
        // We clamp the health so it can't be less than 0 or greater than maximumHitPoints.
        currentHitPoints = Mathf.Clamp(currentHitPoints + modAmount, 0, maximumHitPoints);

        if( currentHitPoints <= 0 ) // If we have 0 health... we die!
        {
            Die();
        }
        else if( modAmount < 0 ) // If we took damage but did not die, play a sound!
        {
            if (hitSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(hitSound);
            }
            // You could also add an animation here!
        }
    }

    // We call this function when our health reaches 0.
    private void Die()
    {
        // Play death sound.
        if (deathSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(deathSound);
        }

        // Give some score.
        if (UIController.Instance != null)
        {
            UIController.Instance.ChangeScore(pointValue);
        }
        
        // You could also add an animation here!

        // Destroy this entity.
        Destroy(gameObject);
    }
}