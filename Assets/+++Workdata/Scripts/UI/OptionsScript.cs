using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    /// <summary>
    /// array von allen screen resolutions
    /// </summary>
    Resolution[] resolutions;

    /// <summary>
    /// text mesh pro resolution drop down menu
    /// </summary>
    public TMPro.TMP_Dropdown resolutionDropdown;

    /// <summary>
    /// cleared alle resolutions und sucht nach der current resolution, nimmt sie dann an und fügt andere resolution im dropdown hinzu
    /// </summary>
    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && 
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// setzt ausgewählte resolution mit screen gleich
    /// </summary>
    /// <param name="resolutionIndex"></param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// aktiviert bzw. deaktiviert fullscreen modus
    /// </summary>
    /// <param name="isFullScreen"></param>
    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
