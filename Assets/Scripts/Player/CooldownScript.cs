using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownScript : MonoBehaviour
{
    /// <summary>
    /// Slider für den Cooldown vom Heal und dem Charge Attack
    /// </summary>
    public Slider chargedAttackCoolDownSlider;
    public Slider healCoolDownSlider;

    public void SetMaxChargedAttackCooldown(float maxCoolDown)
    {
        chargedAttackCoolDownSlider.maxValue = maxCoolDown;
        chargedAttackCoolDownSlider.value = 0;
    }

    public void SetMaxHealCooldown(float maxCoolDown)
    {
        healCoolDownSlider.maxValue = maxCoolDown;
        healCoolDownSlider.value = 0;
    }

    public void SetCurrentChargedAttackCooldown(float currentCooldown)
    {
        chargedAttackCoolDownSlider.value = currentCooldown;
    }

    public void SetCurrentHealCooldown(float currentCooldown)
    {
        healCoolDownSlider.value = currentCooldown;
    }
}
