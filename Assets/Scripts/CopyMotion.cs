using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    public Transform targetLimb;
    ConfigurableJoint cj;

    public bool mirror;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Quaternion startRot;

    public bool shouldStartRot;

    public bool localRotation;

    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        startRot = transform.localRotation;
    }

    void Update()
    {
        Quaternion targetRot = targetLimb.rotation;
        if (localRotation)
        {
            targetRot = targetLimb.localRotation;
        }

        if (shouldStartRot)
        {
            if (!mirror) cj.targetRotation = targetRot * startRot;
            else cj.targetRotation = Quaternion.Inverse(targetRot) * startRot;
        }
        else
        {
            if (!mirror) cj.targetRotation = targetRot;
            else cj.targetRotation = Quaternion.Inverse(targetRot);
        }
    }
}
