using UnityEngine;

public class WallCameraAlign : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private WallWalker wallWalker;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0.5f, 1.5f, -3f);
    [SerializeField] private float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null || wallWalker == null) return;

        // Create offset relative to wall surface
        Quaternion surfaceRot = Quaternion.LookRotation(player.forward, wallWalker.GravityUp);
        Vector3 targetPosition = player.position + surfaceRot * cameraOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, surfaceRot, Time.deltaTime * smoothSpeed);
    }
}