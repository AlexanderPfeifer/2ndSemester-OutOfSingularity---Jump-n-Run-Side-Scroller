using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    public Slider inGameSlider;
    public Slider uiSlider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth(int health)
    {
        inGameSlider.maxValue = health;
        inGameSlider.value = health;

        uiSlider.maxValue = health;
        uiSlider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        uiSlider.value = health;

        inGameSlider.value = health;

        fill.color = gradient.Evaluate(inGameSlider.normalizedValue);
    }
}
