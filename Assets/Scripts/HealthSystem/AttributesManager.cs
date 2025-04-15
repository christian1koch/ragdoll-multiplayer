using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AttributesManager : NetworkBehaviour
{

    public float maxHealth = 100;
    public float maxMana = 100;
    public float maxStamina = 100;

    public float initialManaRegen = 0.5f;
    public NetworkVariable<float> health = new NetworkVariable<float>();

    public float mana;

    public float stamina;

    public float healthRegen = 0.2f;
    public float manaRegen = 0.5f;
    public float staminaRegen = 10f;

    public bool shouldDestroyOnColission = false;

    private HashSet<string> staminaRegenLocks = new HashSet<string>();

    public bool shouldRegenStamina
    {
        get
        {
            if (staminaRegenLocks.Count == 0)
            {
                return true;
            }
            return false;
        }
    }

    void Update()
    {
        if (IsServer) // Make sure only the server performs these updates
        {
            if (health.Value < maxHealth)
            {
                health.Value += healthRegen * Time.deltaTime;
            }
        }
        if (IsOwner) // Only the owner can interact with their local UI
        {
            // The server should handle regeneration, not the client
            if (mana < maxMana)
            {
                mana += manaRegen * Time.deltaTime;
            }
            if (stamina < maxStamina && shouldRegenStamina)
            {
                stamina += staminaRegen * Time.deltaTime;
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void ApplyDamageServerRpc(int amount)
    {
        health.Value -= amount;
        Debug.Log("taking damage in take damage " + amount);
        if (health.Value < 0 && shouldDestroyOnColission)
        {
            Destroy(gameObject);
        }
    }

    public void LockStaminaRegen(string lockIdentifier)
    {
        staminaRegenLocks.Add(lockIdentifier);
    }

    public void RequestStaminaRegenUnlock(string lockIdentifier)
    {
        staminaRegenLocks.Remove(lockIdentifier);
    }

    public override void OnNetworkSpawn()
    {
        // Only do this on the server
        if (IsServer)
        {
            health.Value = maxHealth;
        }
        if (IsOwner)
        {
            mana = maxMana;
            stamina = maxStamina;
        }

        // if is owner and tag is player
        if (IsOwner && gameObject.CompareTag("Player"))
        {
            // Tell the HUD this is the local player
            HUD hud = FindFirstObjectByType<HUD>();
            if (hud != null)
            {
                hud.playerAttributes = this;
            }
        }
        if (!IsOwner)
        {
            // Connect health bar to this attributes manager
            HealthbarUI ui = GetComponentInChildren<HealthbarUI>();
            if (ui != null)
            {
                ui.SetAttributes(this);
            }

        }
    }




}
