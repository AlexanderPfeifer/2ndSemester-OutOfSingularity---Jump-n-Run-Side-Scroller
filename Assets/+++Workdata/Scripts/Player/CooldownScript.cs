using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownScript : MonoBehaviour
{
    /// <summary>
    /// Slider für den Cooldown von dem Charge Attack
    /// </summary>
    public Slider chargedAttackCoolDownSlider;
    /// <summary>
    /// Slider für den Cooldown von dem Heal
    /// </summary>
    public Slider healCoolDownSlider;

    /// <summary>
    /// setzt maximalen cooldown wert für den charge attack
    /// </summary>
    /// <param name="maxCoolDown"></param>
    public void SetMaxChargedAttackCooldown(float maxCoolDown)
    {
        chargedAttackCoolDownSlider.maxValue = maxCoolDown;
        chargedAttackCoolDownSlider.value = maxCoolDown;
    }

    /// <summary>
    /// setzt maximalen wert für den cooldown vom heal
    /// </summary>
    /// <param name="maxCoolDown"></param>
    public void SetMaxHealCooldown(float maxCoolDown)
    {
        healCoolDownSlider.maxValue = maxCoolDown;
        healCoolDownSlider.value = maxCoolDown;
    }

    /// <summary>
    /// refreshed den slider wert des charge attack cooldowns
    /// </summary>
    /// <param name="currentCooldown"></param>
    public void SetCurrentChargedAttackCooldown(float currentCooldown)
    {
        chargedAttackCoolDownSlider.value = currentCooldown;
    }
    /// <summary>
    /// refreshed den slider wert des heal cooldowns
    /// </summary>
    /// <param name="currentCooldown"></param>
    public void SetCurrentHealCooldown(float currentCooldown)
    {
        healCoolDownSlider.value = currentCooldown;
    }
}
