using UnityEngine;

public class CollisionDamager : MonoBehaviour
{

    public int damage = 10;
    public bool shouldDestroyOnColission = true;

    private void OnCollisionEnter(Collision collision)
    {
        AttributesManager targetHealth = collision.gameObject.GetComponent<AttributesManager>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            if (shouldDestroyOnColission)
            {
                Destroy(gameObject);
            }
        }
    }

}
