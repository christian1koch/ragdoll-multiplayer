using UnityEngine;

public class AttributesManager : MonoBehaviour
{

    public int health;

    public int mana;

    public int stamina;


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
