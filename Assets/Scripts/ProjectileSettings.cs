using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSettings : MonoBehaviour
{
    private PlaneController spManager;
    [Tooltip("How many seconds between shots.")]
    public float firingSpeed = 1f;
    public AudioClip fireSound;

    // Start is called before the first frame update
    void Start()
    {
        spManager = PlaneController.instance;  // Obtain the serial port from the manager                                              
    }
    void Update()
    {
        if (fireSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(fireSound);
        }
    }
    void OnCollisionEnter(Collision other)
    {            

        if (other.gameObject.CompareTag("Enemy"))
        {

            // Call enemy scream sound through PlaneController
            PlaneController plane = FindObjectOfType<PlaneController>();
            // if (plane != null)
            // {
            // plane.PlayEnemyHitSound();  // Send 'H' to Arduino

            // }
                        plane.PlayEnemyHitSound();  // Send 'H' to Arduino

            Destroy(other.gameObject); // destroy enemy
            Destroy(gameObject);       // destroy projectile
            Debug.Log("Hit enemy!");

        }
    }

}
