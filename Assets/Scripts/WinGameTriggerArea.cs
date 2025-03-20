using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameTriggerArea : MonoBehaviour
{
    // `OnTriggerEnter` is a special function that is called when a physics collision happens with a trigger area.
    public void OnTriggerEnter(Collider collider)
    {
        // If the object we collided with has the `PlayerController` component, AND we have a UIController loaded...
        if (collider.gameObject.GetComponent<PlayerController>() != null && UIController.Instance != null)
        {
            // ... then tell the UIController to show the win screen.
            UIController.Instance.ShowWinScreen();
        }
    }
}
