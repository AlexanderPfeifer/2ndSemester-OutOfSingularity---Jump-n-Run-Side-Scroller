using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    /// <summary>
    /// slider für den ingame health fill
    /// </summary>
    public Slider uiSlider;
    /// <summary>
    /// fill von ingame healthbar
    /// </summary>
    public Image fill;

    /// <summary>
    /// setzt den maximalen fill von der healthbar
    /// </summary>
    /// <param name="health"></param>
    public void SetMaxHealth(int health)
    {
        uiSlider.maxValue = health;
        uiSlider.value = health;
    }

    /// <summary>
    /// refreshed slider wert vom health
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        uiSlider.value = health;
    }
}
