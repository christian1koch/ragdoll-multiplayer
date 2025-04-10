using UnityEngine;

public class AttributesManager : MonoBehaviour
{

    public int health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool shouldDestroyOnColission = false;
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0 && shouldDestroyOnColission)
        {
            Destroy(gameObject);
        }
    }
}
