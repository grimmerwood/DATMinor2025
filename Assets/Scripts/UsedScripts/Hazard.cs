using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Hazard : MonoBehaviour
{
    [Tooltip("How much damage we will do every time we touch a Health.")]
    public float damage = 1;

    [Tooltip("Hazards don't hurt Healths of the same faction.")]
    public bool isPlayer = false;

    [Tooltip("Whether or not this object should delete itself after it deals damage. This is especially useful for bullets.")]
    public bool deleteAfterCollision = false;

    // Start is called before the first frame update
    public void Start()
    {
        // If we don't have a collider, warn the user.
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning(name + " is missing a collider!");
        }
    }

    // This is a special function that is called when this object touches another object.
    public void OnCollisionEnter(Collision collision)
    {
        // Check if the thing that we touched has a Health component.
        Health health = collision.gameObject.GetComponent<Health>();

        // If it does, and it's a different faction...

            // Faction check
            if (health != null && health.isPlayer != isPlayer)
            {

                // Deal damage
                health.TakeDamage(damage);

                // Destroy bullet if needed
                if (deleteAfterCollision)
                {
                    Destroy(gameObject);
                }
            }
        }

        // After that, check if the thing that touched us has an InvincibleOnHit
        //InvincibleOnHit invincibleOnHit = collision.gameObject.GetComponent<InvincibleOnHit>();

        //if(invincibleOnHit != null)
        //{
        //    invincibleOnHit.InvincibleStart();
        //}
}
