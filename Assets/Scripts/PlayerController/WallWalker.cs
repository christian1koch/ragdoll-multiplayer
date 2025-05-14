using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class WallWalker : NetworkBehaviour
{
    [Header("Wall Walk Settings")]
    public float gravityStrength = 9.81f;
    public float wallCheckDistance = 1.5f;
    public LayerMask wallMask;

    [Header("References")]
    public Rigidbody rb;
    public AttributesManager attributes;

    private Vector3 currentGravity = Vector3.down;
    public bool isWallWalking = false;
    private bool isGrounded = true;

    public Vector3 GravityDirection => currentGravity.normalized;

    public Vector3 GravityUp => -currentGravity.normalized;

    public List<Rigidbody> allRigidbodies = new List<Rigidbody>();

    private void Start()
    {
        if (allRigidbodies.Count == 0)
            allRigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());

        // foreach (var rb in allRigidbodies)
        //     rb.useGravity = false;  // Disable built-in gravity
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E)) // Try attach to wall
        {
            TryAttachToWall();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isWallWalking)
        {
            RequestDetachServerRpc();
        }
    }

    private void FixedUpdate()
    {
        // if (!IsServer) return;

        // foreach (var rb in allRigidbodies)
        // {
        //     rb.AddForce(currentGravity * gravityStrength, ForceMode.Acceleration);
        // }

        // if (isWallWalking)
        // {
        //     Quaternion targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, GravityUp), GravityUp);
        //     rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * 5f));
        // }
    }

    private void TryAttachToWall()
    {
        Debug.Log("Throwing ray");
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, wallCheckDistance, wallMask))
        {
            if (attributes.stamina > 0)
            {
                RequestAttachToWallServerRpc(hit.normal);
            }
        }
    }

    [ServerRpc]
    private void RequestAttachToWallServerRpc(Vector3 normal)
    {
        Quaternion targetRot = Quaternion.LookRotation(Vector3.Cross(transform.right, normal), -normal);
        transform.rotation = targetRot;

        currentGravity = -normal;
        isWallWalking = true;

        Debug.Log("Attached to wall with normal: " + normal);
    }

    [ServerRpc]
    private void RequestDetachServerRpc()
    {
        currentGravity = Vector3.down;
        isWallWalking = false;
        Debug.Log("Detached from wall.");
    }

    // Optional: auto detach when stamina is low
    // private void LateUpdate()
    // {
    //     if (!IsServer) return;

    //     if (isWallWalking && attributes.stamina < 1f)
    //     {
    //         currentGravity = Vector3.down;
    //         isWallWalking = false;
    //         Debug.Log("Auto-detached due to low stamina.");
    //     }
    // }
}
