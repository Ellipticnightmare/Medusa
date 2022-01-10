using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public GameObject settingsUI;
    public Dropdown qualDropdown, resolutionDropdown;
    public Toggle fullScreenToggle;
    Resolution[] availableResolutions;
    public void ToggleSettingsMenu()
    {
        settingsUI.SetActive(!settingsUI.activeInHierarchy);
        if (settingsUI.activeInHierarchy)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    // Start is called before the first frame update
    public void Start()
    {
        int resolutionIndex = 0;
        qualDropdown.value = QualitySettings.GetQualityLevel();
        fullScreenToggle.isOn = Screen.fullScreen;
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> availableResolutionsList = new List<string>();
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string optionVal = availableResolutions[i].width + " x " + availableResolutions[i].height;
            if (!availableResolutionsList.Contains(optionVal))
                availableResolutionsList.Add(optionVal);
            if (availableResolutions[i].width == Screen.currentResolution.width && availableResolutions[i].height == Screen.currentResolution.height)
                resolutionIndex = i;
        }
        resolutionDropdown.AddOptions(availableResolutionsList);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
        qualDropdown.RefreshShownValue();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsMenu();
        }
    }
    public void SetQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }
    public void FullscreenToggle(bool value)
    {
        Screen.fullScreen = value;
    }
    public void SetResolution(int value)
    {
        Resolution resolutionVar = availableResolutions[value];
        Screen.SetResolution(resolutionVar.width, resolutionVar.height, Screen.fullScreen);
    }
}