using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSettings : MonoBehaviour
{
    [Tooltip("How many seconds between shots.")]
    public float firingSpeed = 1f;
    [Tooltip("The sound effect that plays when fired.")]
    public AudioClip fireSound;

    // Start is called before the first frame update
    void Start()
    {
        if (fireSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(fireSound);
        }
    }
}
