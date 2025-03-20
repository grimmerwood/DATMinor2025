using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // This is a static variable. It can be accessed from any other script, and there is only one. This is called a "singleton".
    public static AudioManager Instance { get; private set; }

    // This AudioSource will be assigned at the start of the game automatically, so we don't need to assign it in the inspector.
    private AudioSource audioSource;

    // Awake is called before Start. We want to do this in case other scripts want to use AudioManager in their Start method.
    public void Awake()
    {
        if (Instance != null)
        {
            print("There are two AudioManager instances!");
        }
        else
        {
            Instance = this;
        }
        
        // Cache the AudioSource for later use.
        audioSource = GetComponent<AudioSource>();
    }

    // This is a public method that any script can call.
    public void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }
}
