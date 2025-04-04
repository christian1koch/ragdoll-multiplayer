using TMPro;
using UnityEngine;

public class Display : MonoBehaviour
{

    public TMP_Text healthDisplay;
    public AttributesManager atm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (atm == null || healthDisplay == null)
        {
            return;
        }
        healthDisplay.SetText("Health: " + atm.health);
    }
}
