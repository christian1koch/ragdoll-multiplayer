using UnityEngine;

public class AttributesManager : MonoBehaviour
{

    public float maxHealth = 100;
    public float maxMana = 100;
    public float maxStamina = 100;
    public float health;

    public float mana;

    public float stamina;

    public float healthRegen = 0.2f;
    public float manaRegen = 0.5f;
    public float staminaRegen = 10f;

    public bool shouldRegenStamina = true;

    public bool shouldDestroyOnColission = false;

    void Start()
    {
        health = maxHealth;
        mana = maxMana;
        stamina = maxStamina;
    }

    void Update()
    {
        if (health < maxHealth)
        {
            health += healthRegen * Time.deltaTime;
        }
        if (mana < maxMana)
        {
            mana += manaRegen * Time.deltaTime;
        }
        if (stamina < maxStamina && shouldRegenStamina)
        {
            stamina += staminaRegen * Time.deltaTime;
        }
    }


    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0 && shouldDestroyOnColission)
        {
            Destroy(gameObject);
        }
    }


}
