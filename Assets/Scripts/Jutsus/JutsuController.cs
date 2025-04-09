using UnityEngine;

public class JutsuController : MonoBehaviour
{
    // list of all scripts inside this game object that extend the IJutsu interface
    private IJutsu[] jutsus;
    private int selectedJutsuIndex = 0;

    private void Awake()
    {
        // Get all components on this GameObject that implement the IJutsu interface
        jutsus = GetComponents<IJutsu>();
    }

    private void Update()
    {
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
        if (selectedJutsuIndex < jutsus.Length)
        {
            IJutsu jutsu = jutsus[selectedJutsuIndex];
            if (jutsu != null)
            {
                jutsu.CastJutsu();
            }
        }
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