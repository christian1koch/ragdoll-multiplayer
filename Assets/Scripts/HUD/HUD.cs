using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI health;
    public TextMeshProUGUI mana;
    public TextMeshProUGUI stamina;

    public AttributesManager playerAttributes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        health.SetText("HP " + Mathf.FloorToInt(playerAttributes.health.Value));
        mana.SetText("MANA " + Mathf.FloorToInt(playerAttributes.mana));
        stamina.SetText("STAMINA " + Mathf.FloorToInt(playerAttributes.stamina));
    }
}
