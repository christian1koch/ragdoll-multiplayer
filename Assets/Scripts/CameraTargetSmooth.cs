using UnityEngine;

public class CameraTargetSmooth : MonoBehaviour
{
    public Transform hips;            // Assign your ragdoll's hips
    public float smoothSpeed = 5f;    // Adjust to taste

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - hips.position;
    }

    void FixedUpdate()
    {
        // Get the hips rotation in euler angles
        Vector3 hipsEuler = hips.rotation.eulerAngles;

        // Keep only the y component
        hipsEuler.x = 0f;
        hipsEuler.z = 0f;

        // Create a new rotation that has only the yaw
        Quaternion onlyYawRotation = Quaternion.Euler(hipsEuler);

        // Apply that rotation to the offset
        transform.position = hips.position + onlyYawRotation * offset;

    }

}
