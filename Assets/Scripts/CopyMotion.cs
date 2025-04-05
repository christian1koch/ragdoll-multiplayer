using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    public Transform targetLimb;
    ConfigurableJoint cj;

    public bool mirror;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Quaternion startRot;

    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        startRot = transform.localRotation;
    }

    void Update()
    {
        if (!mirror) cj.targetRotation = targetLimb.localRotation * startRot;
        else cj.targetRotation = Quaternion.Inverse(targetLimb.localRotation) * startRot;
    }
}
