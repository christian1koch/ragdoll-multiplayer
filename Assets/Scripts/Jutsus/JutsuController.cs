using Unity.Netcode;
using UnityEngine;

public class JutsuController : NetworkBehaviour
{
    // list of all scripts inside this game object that extend the IJutsu interface
    private IJutsu[] jutsus;
    private int selectedJutsuIndex = 0;

    public AttributesManager playerAttributes;

    private void Awake()
    {
        // Get all components on this GameObject that implement the IJutsu interface
        jutsus = GetComponents<IJutsu>();
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        // on press click cast the jutsu
        if (Input.GetMouseButtonDown(0))
        {
            CastJutsu();
        }

        // on press 1 select the first jutsu
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectJutsu(0);
        }

        // on press 2 select the second jutsu
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectJutsu(1);
        }
    }


    private void CastJutsu()
    {
        Debug.Log("mana " + playerAttributes.mana);
        if (selectedJutsuIndex >= jutsus.Length)
        {
            return;
        }

        IJutsu jutsu = jutsus[selectedJutsuIndex];
        if (jutsu == null)
        {
            return;
        }
        if (playerAttributes.mana < jutsu.ManaValue)
        {
            Debug.Log("Not Enough Mana!");
            return;
        }
        jutsu.CastJutsu();
        playerAttributes.mana -= jutsu.ManaValue;

    }

    private void SelectJutsu(int index)
    {
        if (index >= 0 && index < jutsus.Length)
        {
            selectedJutsuIndex = index;
            Debug.Log("Selected Jutsu: " + jutsus[selectedJutsuIndex].Name);
        }
    }
}