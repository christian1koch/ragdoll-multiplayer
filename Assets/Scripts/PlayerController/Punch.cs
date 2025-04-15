using System.Collections;
using UnityEngine;

public class Punch : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rightHandRb;
    public Animator animator;
    public AttributesManager attributes;
    public Camera cam;

    [Header("Punch Settings")]
    public float punchTorqueForce = 100f;
    public float punchStaminaCost = 20f;
    public float punchCooldown = 0.5f;

    private bool isPunching;

    public bool CanPunch => !isPunching && attributes.stamina >= punchStaminaCost;

    [SerializeField] private PunchColissionManager punchCollision;

    public void TryPunch()
    {
        if (CanPunch)
        {
            StartCoroutine(PunchCoroutine());
        }
    }

    private IEnumerator PunchCoroutine()
    {
        isPunching = true;

        animator.SetTrigger("punchRight");
        attributes.stamina -= punchStaminaCost;

        yield return new WaitForSeconds(0.1f); // Optional windup

        Vector3 targetAngle = cam.transform.eulerAngles;
        Vector3 moveDirForward = targetAngle + Vector3.forward;
        Vector3 torqueDir = Vector3.Cross(rightHandRb.transform.up, moveDirForward).normalized;

        rightHandRb.AddTorque(torqueDir * punchTorqueForce, ForceMode.Impulse);
        punchCollision.EnableDamage();

        yield return new WaitForSeconds(punchCooldown);

        punchCollision.DisableDamage();
        isPunching = false;
    }
}