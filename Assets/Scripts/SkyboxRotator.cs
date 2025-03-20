using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("The skybox material to rotate.")]
    public Material skyboxMaterial;

    [Tooltip("How many seconds it takes for the skybox to rotate 360 degrees.")]
    public float timeForFullRotation = 180;

    public void Update()
    {
        float rotationProgress = Time.time / timeForFullRotation;

        skyboxMaterial.SetFloat("_Rotation", rotationProgress * 360);
    }
}