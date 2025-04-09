using UnityEngine;

// TODO: Change name to correct name
public class ColissionTest : MonoBehaviour
{
    public int damage = 10;
    public bool shouldDestroyOnColission = true;

    public bool isPunching = false;

    private void OnCollisionEnter(Collision collision)
    {


        AttributesManager targetHealth = collision.gameObject.GetComponent<AttributesManager>();
        if (targetHealth != null && isPunching)
        {
            targetHealth.TakeDamage(damage);
        }
        else
        {
            Debug.Log("No AttributesManager found on the collided object.");
        }
    }
}
