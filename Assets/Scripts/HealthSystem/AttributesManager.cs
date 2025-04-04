using UnityEngine;

public class AttributesManager : MonoBehaviour
{

    public int health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TakeDamage(int amount)
    {
        health -= amount;
    }
}
