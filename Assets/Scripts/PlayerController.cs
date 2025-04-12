using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 400;
    public float jumpForce = 2000;

    private readonly float jumpStaminaCost = 2;

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
    void Start()
    {
        hipsRigidBody = hips.GetComponent<Rigidbody>();
        // Hide the cursor
        Cursor.visible = false;

        // Optionally lock the cursor to the center of the screen
        // to keep it from moving around at all:
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PunchCoroutine());
            float targetAngle = cam.eulerAngles.y;
            Vector3 moveDirForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rightHandRb.AddForce(punchSpeed * moveDirForward);
        }
    }
    void FixedUpdate()
    {

        MoveCharacter();
    }

    private void MoveCharacter()
    {
        onVerticalMovementDisableStaminaRegen();
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(0f, 0f, forwardInput).normalized;
        float targetAngle = cam.eulerAngles.y;

        Vector3 moveDirForward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        Vector3 moveDirHorizontal = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.right;
        hipJoint.targetRotation = Quaternion.Euler(0f, -targetAngle, 0f);
        float jumpInput = Input.GetAxis("Jump");
        if (jumpInput > 0 && playerAttributes.stamina >= jumpStaminaCost)
        {
            hipsRigidBody.AddForce(new Vector3(0, jumpForce, 0));
            playerAttributes.stamina -= jumpStaminaCost;
        }
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
        // set the CollissionTest from the right hand to true
        rightHandRb.GetComponent<ColissionTest>().isPunching = true;
        // then wait 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        rightHandRb.GetComponent<ColissionTest>().isPunching = false;
        // then set the CollissionTest from the right hand to false
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

        if (!isGrounded && playerAttributes.shouldRegenStamina)
        {
            playerAttributes.shouldRegenStamina = false;
        }
        if (isGrounded && !playerAttributes.shouldRegenStamina)
        {
            playerAttributes.shouldRegenStamina = true;
        }
    }
}
