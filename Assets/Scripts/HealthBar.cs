using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthbar;
    public Gradient gradient;
    public Image image;

    private void Start()
    {
        healthbar = GetComponent<Slider>();
        
    }

    public void SetMaxHealth(int health)
    {
        if (image == null)
        {
            image = gameObject.GetComponentInChildren<Image>();
        }
        healthbar.maxValue = health;
        healthbar.value = health;
        image.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        if(image == null)
        {
            image = gameObject.GetComponentInChildren<Image>();
        }
        healthbar.value = health;

        image.color = gradient.Evaluate(healthbar.normalizedValue);
    }
}
