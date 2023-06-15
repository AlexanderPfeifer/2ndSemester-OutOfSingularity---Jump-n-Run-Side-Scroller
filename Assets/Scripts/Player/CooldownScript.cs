using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownScript : MonoBehaviour
{
    public Slider chargedAttackCoolDownSlider;
    public Slider healCoolDownSlider;


    public void SetMinCooldown(float minCoolDown)
    {
        chargedAttackCoolDownSlider.minValue = minCoolDown;
        chargedAttackCoolDownSlider.value = minCoolDown;

        healCoolDownSlider.minValue = minCoolDown;
        healCoolDownSlider.value = minCoolDown;
    }

    public void SetMaxChargedAttackCooldown(float maxCoolDown)
    {
        chargedAttackCoolDownSlider.maxValue = maxCoolDown;
        chargedAttackCoolDownSlider.value = maxCoolDown;
    }

    public void SetMaxHealCooldown(float maxCoolDown)
    {
        healCoolDownSlider.maxValue = maxCoolDown;
        healCoolDownSlider.value = maxCoolDown;
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
