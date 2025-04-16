// WallCameraRoll.cs
using Unity.Cinemachine;
using UnityEngine;

public class WallCameraRoll : MonoBehaviour
{
    public WallWalker wallWalker;
    public float rollSmoothSpeed = 5f;
    public Transform target;

    void LateUpdate()
    {
        if (wallWalker == null) return;

        // Use wall gravity to define the up direction
        Vector3 upDirection = wallWalker.GravityUp;

        // Look from camera to target with the wall-based up direction
        Vector3 toTarget = (target.position - transform.position).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(toTarget, upDirection);

        // Smoothly rotate toward the target
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rollSmoothSpeed);
    }
}
