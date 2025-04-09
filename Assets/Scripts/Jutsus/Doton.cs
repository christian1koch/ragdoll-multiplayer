using UnityEngine;

public class Doton : MonoBehaviour, IJutsu
{
    public bool isSelected;

    public GameObject dotonShieldObject;

    public Transform cameraTransform;

    public string Name => "Doton";
    public int JutsuId => 1;

    void Update()
    {
        // on press u cast the jutsu
        if (Input.GetKeyDown(KeyCode.U))
        {
            CastJutsu();
        }
    }

    private void CastJutsu()
    {
        // spawns the doton sheild in front of the caster
        GameObject dotonShield = Instantiate(dotonShieldObject, transform.position, Quaternion.Euler(0f, 0f, 0f));
    }

    void IJutsu.CastJutsu()
    {
        CastJutsu();
    }
}