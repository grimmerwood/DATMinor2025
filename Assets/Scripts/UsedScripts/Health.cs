using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Health : MonoBehaviour
{
    private PlaneControllerUnified playerController;
    [Tooltip("How much damage this entity takes before it dies.")]
    public float maximumHitPoints = 10;
    public int startingHitPoints = 5;

    private float currentHitPoints;

    [Tooltip("The number of points that will be awarded upon death. Only enemies should have points.")]
    public int pointValue;

    [Tooltip("Hazards don't hurt Healths of the same faction.")]
    public bool isPlayer;

    [Tooltip("The sound to play on a hit that doesn't result in a death. Defaults to nothing.")]
    public AudioClip hitSound;

    [Tooltip("The sound to play on death. Defaults to nothing.")]
    public AudioClip deathSound;

    [Tooltip("How long this object is invincible after taking damage.")]
    public float invincibleDuration = 1f;

    
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    // This function allows other scripts (like UIController) to check how much health we have.
    public float GetCurrentHitPoints()
    {
        return currentHitPoints;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        currentHitPoints = startingHitPoints;

    if (GetComponent<Collider>() == null)
    {
        Debug.LogWarning(name + " is missing a collider!");
    }

    // Link to the player's controller if this is the player
    if (isPlayer)
    {
        playerController = FindObjectOfType<PlaneControllerUnified>();
    }
    }

    // Update is called every frame. Here we update the invincibility timer.
    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                //Debug.Log($"{gameObject.name} is no longer invincible.");
            }
        }
    }

    // This is the public function to apply damage.
    public void TakeDamage(float damageAmount)
    {
        // If we're currently invincible, ignore the damage.
        if (isInvincible)
        {
            //Debug.Log($"{gameObject.name} is invincible and ignored the damage.");
            return;
        }

        ModifyHitPoints(-damageAmount);

        // Activate invincibility after getting hit.
        isInvincible = true;
        invincibilityTimer = invincibleDuration;
        //Debug.Log($"{gameObject.name} is now invincible for {invincibleDuration} seconds.");
    }

    public void Heal(float healAmount)
    {
        ModifyHitPoints(healAmount);
    }
    public void ResetHealth()
    {
    currentHitPoints = startingHitPoints;

    if (UIController.Instance != null && isPlayer)
    {
        UIController.Instance.UpdatePlayerHealth(currentHitPoints, maximumHitPoints);
    }
}
    //This function adds or subtracts health
    // This function changes our current health by a certain amount.
    private void ModifyHitPoints(float modAmount)
    {
        currentHitPoints = Mathf.Clamp(currentHitPoints + modAmount, 0, maximumHitPoints);
        Debug.Log($"{gameObject.name} now has {currentHitPoints} HP");

        if (UIController.Instance != null && isPlayer)
        {
            UIController.Instance.UpdatePlayerHealth(currentHitPoints, maximumHitPoints);
        }

        if (currentHitPoints <= 0)
        {
            Die();
        }
        else if (modAmount < 0)
        {
            if (hitSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(hitSound);
            }
        }
    }

    
    // This function handles death.
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");

    if (deathSound != null && AudioManager.Instance != null)
    {
        AudioManager.Instance.PlaySound(deathSound);
    }

    if (UIController.Instance != null)
    {
        UIController.Instance.ChangeScore(pointValue);

        if (isPlayer)
        {
            UIController.Instance.PlayerDied(); // 🔴 New line: Notify UIController
        }
    }

    if (isPlayer && playerController != null)
    {
        playerController.isPlayerAlive = false;
    }
    else
    {
        Destroy(transform.root.gameObject); // 🔴 🔄 Only this line changed
    }
    }
    internal void AddHealth(int healAmount)
    {
        Heal(healAmount);
    }

    public bool IsDead()
   { 
    return currentHitPoints <= 0;
   }
   
}
