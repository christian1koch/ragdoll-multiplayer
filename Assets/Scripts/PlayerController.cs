using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float baseSpeed = 200;
    public float speed = 200;
    public float jumpForce = 2000;

    private readonly float jumpStaminaCost = 2;
    public float punchStaminaCost = 25f;

    public Transform cam;

    public GameObject hips;

    public ConfigurableJoint hipJoint;
    public Rigidbody hipsRigidBody;

    public Rigidbody rightHandRb;

    public float punchSpeed = 20.0f;

    public float fallMultiplier = 2.0f;

    public Animator targetAnimator;

    public AttributesManager playerAttributes;

    public float groundCheckDistance = 2.0f;

    public bool isGrounded;


    [Header("Dash Settings")]
    public float dashForce = 800f;
    public float dashCooldown = 1.0f;

    private float lastDashTime;
    private bool canDash = true;

    private List<Rigidbody> allRigidbodies;
    void Start()
    {
        hipsRigidBody = hips.GetComponent<Rigidbody>();
        allRigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        // Hide the cursor
        Cursor.visible = false;

        // Optionally lock the cursor to the center of the screen
        // to keep it from moving around at all:
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        bool isPunching = rightHandRb.GetComponent<ColissionTest>().isPunching;
        bool enoughStaminaToPunch = playerAttributes.stamina >= punchStaminaCost;
        if (Input.GetKeyDown(KeyCode.F) && !isPunching && enoughStaminaToPunch)
        {
            StartCoroutine(PunchCoroutine());
        }
        HandleDashInput();
    }
    void FixedUpdate()
    {
        onVerticalMovementDisableStaminaRegen();
        onJump();
        onRun();
        MoveCharacter();

    }

    private void MoveCharacter()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        float targetAngle = cam.eulerAngles.y;

        Vector3 moveDirForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        Vector3 moveDirHorizontal = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.right;
        hipJoint.targetRotation = Quaternion.Euler(0f, -targetAngle, 0f);
        if (forwardInput == 0)
        {
            targetAnimator.SetBool("isWalking", false);
        }
        else
        {
            targetAnimator.SetBool("isWalking", true);
        }
        hipsRigidBody.AddForce(forwardInput * speed * moveDirForward);
        hipsRigidBody.AddForce(horizontalInput * speed * moveDirHorizontal);

    }
    private IEnumerator PunchCoroutine()
    {
        rightHandRb.GetComponent<ColissionTest>().isPunching = true;
        targetAnimator.SetTrigger("punchRight");
        playerAttributes.stamina -= punchStaminaCost;
        float targetAngle = cam.eulerAngles.y;
        Vector3 moveDirForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        rightHandRb.AddForce(punchSpeed * moveDirForward);
        yield return new WaitForSeconds(0.5f);
        rightHandRb.GetComponent<ColissionTest>().isPunching = false;
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
            hipsRigidBody.AddForce(new Vector3(0, jumpForce, 0));
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

        if (!Input.GetKeyDown(KeyCode.LeftCommand))
            return;
        if (playerAttributes.stamina < 25)
            return;
        if (!canDash || Time.time - lastDashTime < dashCooldown)
            return;

        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDir == Vector3.zero)
            return;

        float targetAngle = cam.eulerAngles.y;
        Vector3 dashDirection = Quaternion.Euler(0f, targetAngle, 0f) * inputDir.normalized;

        StartCoroutine(Dash(dashDirection));
    }






}
