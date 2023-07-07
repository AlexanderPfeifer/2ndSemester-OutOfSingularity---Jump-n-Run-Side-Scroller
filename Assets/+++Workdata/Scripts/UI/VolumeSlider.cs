using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    /// <summary>
    /// erstellt enums für alle sound kategorien
    /// </summary>
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        AMBIENCE,
        SFX
    }

    /// <summary>
    /// volumetypes von den sound kategorien
    /// </summary>
    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    /// <summary>
    /// slider von den sounds
    /// </summary>
    private Slider volumeSlider;

    /// <summary>
    /// erstellt bezug zum slider 
    /// </summary>
    private void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    /// <summary>
    /// geht immer wieder alle volume typed durch um nach änderungen zu suchen bei onslidervaluechanged
    /// </summary>
    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                break;
            case VolumeType.MUSIC:
                break;
            case VolumeType.AMBIENCE:
                break;
            case VolumeType.SFX:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// setzt die lautstärke der sinds gleich mit dem value des sliders
    /// </summary>
    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                AudioManager.instance.musicVolume = volumeSlider.value;
                break;
            case VolumeType.AMBIENCE:
                AudioManager.instance.ambienceVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                AudioManager.instance.SFXVolume = volumeSlider.value;
                break;
        }
    }
}
