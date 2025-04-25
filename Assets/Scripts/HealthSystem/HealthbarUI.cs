using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthbarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private AttributesManager attributes;

    [SerializeField] private GameObject HPDisplayer;
    private void Start()
    {
        if (attributes != null)
        {
            UpdateHealthUI(attributes.health.Value);
            attributes.health.OnValueChanged += (oldVal, newVal) =>
            {
                UpdateHealthUI(newVal);
            };
        }
        else
        {
            HPDisplayer.SetActive(false);
        }
    }

    private void UpdateHealthUI(float value)
    {
        if (attributes == null) return;

        float percent = value / attributes.maxHealth;
        Debug.Log("percent: " + percent);
        healthSlider.value = percent;
        healthText.text = $"{Mathf.CeilToInt(value)} / {Mathf.CeilToInt(attributes.maxHealth)}";
    }

    public void SetAttributes(AttributesManager attr)
    {
        attributes = attr;
    }
}
