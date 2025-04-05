using UnityEngine;

public class CameraTargetSmooth : MonoBehaviour
{
    public Transform hips;            // Assign your ragdoll's hips
    public Transform cameraTarget;    // Your empty GameObject
    public float smoothSpeed = 5f;    // Adjust to taste

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - hips.position;
    }

    void FixedUpdate()
    {
        // 1. Smoothly move cameraTarget towards hips' position
        transform.position = hips.position + offset;
    }

}
