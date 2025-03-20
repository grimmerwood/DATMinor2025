using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
    [Tooltip("The direction, x, y, and z, that the object moves")]
    public Vector3 direction = new Vector3(0f, 0f, -5f);

    // Update is called once per frame
    void Update()
    {
        // Move the object in the direction specified by the `direction` variable.
        transform.transform.position += direction * Time.deltaTime;
    }
}
