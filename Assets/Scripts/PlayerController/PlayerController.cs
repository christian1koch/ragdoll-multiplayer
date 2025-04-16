using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float baseSpeed = 200;
    public float speed = 200;
    public float jumpForce = 2000;

    private readonly float jumpStaminaCost = 2;
    public float punchStaminaCost = 25f;

    public Camera cam;

    public GameObject hips;

    public ConfigurableJoint hipJoint;
    public Rigidbody hipsRigidBody;

    public Rigidbody rightHandRb;

    public float punchTorqueForce = 100f;

    public float fallMultiplier = 2.0f;

    public Animator targetAnimator;

    private AttributesManager playerAttributes;

    public float groundCheckDistance = 2.0f;

    public bool isGrounded;


    [Header("Dash Settings")]
    public float dashForce = 800f;
    public float dashCooldown = 1.0f;

    private float lastDashTime;
    private bool canDash = true;

    private List<Rigidbody> allRigidbodies;

    public CinemachineCamera aimCam;
    public CinemachineCamera followCam;

    public Renderer ragdollRenderer;

    private bool isChargingChakra = false;

    public Punch punchController;

    public WallWalker wallWalker;

    public override void OnNetworkSpawn()
    {
        hipsRigidBody = hips.GetComponent<Rigidbody>();
        allRigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        // Hide the cursor
        Cursor.visible = false;

        // Optionally lock the cursor to the center of the screen
        // to keep it from moving around at all:
        Cursor.lockState = CursorLockMode.Locked;
        ragdollRenderer.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        if (!IsOwner)
        {
            cam.enabled = false;
            cam.GetComponentInChildren<CinemachineBrain>().enabled = false;
            cam.tag = "Untagged";
            aimCam.enabled = false;
            followCam.enabled = false;

        }
        else
        {
            cam.enabled = true;
            cam.GetComponentInChildren<CinemachineBrain>().enabled = true;
            cam.tag = "MainCamera";
            aimCam.enabled = true;
            followCam.enabled = true;
            IgnoreOwnCollision();
            playerAttributes = gameObject.GetComponent<AttributesManager>();
        }

    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            punchController.TryPunch();
        }
        HandleDashInput();
    }
    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        OnCameraAimChange();
        OnManaRecharge();
        onVerticalMovementDisableStaminaRegen();
        if (!isChargingChakra)
        {
            onJump();
            onRun();
            MoveCharacter();
        }

    }

    private void MoveCharacter()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (!wallWalker.isWallWalking)
        {
            // ============ NORMAL GROUND MOVEMENT ============
            float targetAngle = cam.transform.eulerAngles.y;
            hipJoint.targetRotation = Quaternion.Euler(0f, -targetAngle, 0f);

            Vector3 moveDirForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 moveDirRight = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.right;

            hipsRigidBody.AddForce(forwardInput * speed * moveDirForward);
            hipsRigidBody.AddForce(horizontalInput * speed * moveDirRight);
        }
        else
        {
            // ============ WALL WALKING MOVEMENT ============
            Vector3 inputDir = (cam.transform.forward * forwardInput + cam.transform.right * horizontalInput).normalized;
            Vector3 moveDirection = Vector3.ProjectOnPlane(inputDir, wallWalker.GravityUp).normalized;

            if (inputDir.sqrMagnitude > 0.1f)
            {
                // Face move direction
                hipJoint.targetRotation = Quaternion.LookRotation(moveDirection, wallWalker.GravityUp);
            }

            hipsRigidBody.AddForce(moveDirection * speed);
        }

        // Animation stays the same
        if (forwardInput == 0 && horizontalInput == 0)
        {
            targetAnimator.SetBool("isWalking", false);
        }
        else
        {
            targetAnimator.SetBool("isWalking", true);
        }
    }



    private void onVerticalMovementDisableStaminaRegen()
    {
        Vector3 origin = hipsRigidBody.transform.position;
        Vector3 direction = hipsRigidBody.transform.TransformDirection(Vector3.down);

        RaycastHit hit;
        LayerMask groundLayer = LayerMask.GetMask("noselfcolission");
        groundLayer = ~groundLayer;
        // Send a ray straight down
        bool isGrounded = Physics.Raycast(origin, direction, out hit, groundCheckDistance, groundLayer);

        if (!isGrounded)
        {
            playerAttributes.LockStaminaRegen("jump");
        }
        if (isGrounded && !playerAttributes.shouldRegenStamina)
        {
            playerAttributes.RequestStaminaRegenUnlock("jump");
        }
    }

    private void onJump()
    {
        float jumpInput = Input.GetAxis("Jump");
        if (jumpInput > 0 && playerAttributes.stamina >= jumpStaminaCost)
        {
            hipsRigidBody.AddForce(wallWalker.GravityUp * jumpForce);
            playerAttributes.stamina -= jumpStaminaCost;
        }
    }


    private void onRun()
    {
        float forwardInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift) && forwardInput != 0 && playerAttributes.stamina > 0)
        {
            speed = baseSpeed * 1.5f;
            playerAttributes.LockStaminaRegen("run");
            targetAnimator.SetBool("isRunning", true);
            playerAttributes.stamina -= 10 * Time.deltaTime;
        }
        else
        {
            speed = baseSpeed;
            targetAnimator.SetBool("isRunning", false);
            playerAttributes.RequestStaminaRegenUnlock("run");
        }
    }

    private IEnumerator Dash(Vector3 direction)
    {
        canDash = false;
        lastDashTime = Time.time;

        foreach (Rigidbody rb in allRigidbodies)
        {
            rb.AddForce(direction * dashForce, ForceMode.Impulse);
        }
        playerAttributes.stamina -= 25;

        yield return new WaitForSeconds(0.3f); // Duration of the dash
        canDash = true;
    }

    private void HandleDashInput()
    {

        if (!Input.GetKeyDown(KeyCode.LeftCommand) && !Input.GetKeyDown(KeyCode.LeftControl))
            return;
        if (playerAttributes.stamina < 25)
            return;
        if (!canDash || Time.time - lastDashTime < dashCooldown)
            return;

        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDir == Vector3.zero)
            return;

        float targetAngle = cam.transform.eulerAngles.y;
        Vector3 dashDirection = Quaternion.Euler(0f, targetAngle, 0f) * inputDir.normalized;

        StartCoroutine(Dash(dashDirection));
    }

    void IgnoreOwnCollision()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>();

        for (int i = 0; i < allColliders.Length; i++)
        {
            for (int j = i + 1; j < allColliders.Length; j++)
            {
                Physics.IgnoreCollision(allColliders[i], allColliders[j]);
            }
        }
    }

    void OnManaRecharge()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Debug.Log("pressing X");
            playerAttributes.manaRegen = playerAttributes.initialManaRegen * 10;
            isChargingChakra = true;
            targetAnimator.SetBool("isCharging", true);
            targetAnimator.SetBool("isRunning", false);
            targetAnimator.SetBool("isWalking", false);
            return;
        }
        isChargingChakra = false;
        targetAnimator.SetBool("isCharging", false);

        playerAttributes.manaRegen = playerAttributes.initialManaRegen;
    }

    private void OnCameraAimChange()
    {
        float targetAngle = cam.transform.eulerAngles.y;

        Vector3 forwardProjected = Quaternion.LookRotation(Vector3.ProjectOnPlane(cam.transform.forward, wallWalker.GravityUp), wallWalker.GravityUp) * Vector3.forward;

        hipJoint.targetRotation = Quaternion.LookRotation(forwardProjected, wallWalker.GravityUp);
    }




}
