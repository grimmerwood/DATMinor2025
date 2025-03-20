using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleOnHit : MonoBehaviour
{
    [Tooltip("How many seconds the invincibility effect lasts.")]
    public float invincibilityLength = 3f;

    // The collider on the same game object as this script.
    private Collider myCollider;

    // The time the invincibility effect started. Used to calculate when it should end.
    private float hitStartTime;

    // Start is called before the first frame update
    void Start()
    {
        // Find the collider attached to this same game object.
        myCollider = GetComponent<Collider>();

        if(myCollider == null)
        {
            // A baked in warning message if you forget a collider.
            Debug.LogWarning("InvincibleOnHit component missing a Collider on:" + gameObject.name);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        // If the time since the invincibility effect started is greater than the length of the effect...
        if (Time.time - hitStartTime > invincibilityLength && myCollider.enabled == false)
        {
            // End the effect.
            InvincibleEnd();
        }
    }

    // This is a public method that is called by other scripts to start the invincibility effect.
    public void InvincibleStart()
    {
        hitStartTime = Time.time;

        if (myCollider != null)
        {
            // We make ourselves invincible simply by temporarily disabling our collider.
            myCollider.enabled = false;
        }
    }

    // We call this method when we want the invincibility to end.
    private void InvincibleEnd()
    {
        if (myCollider != null)
        {
            myCollider.enabled = true;
        }
    }
}
