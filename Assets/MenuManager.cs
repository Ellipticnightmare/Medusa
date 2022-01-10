using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    float fillPerc;
    public Image fillImg;
    public Text speedrunTimer, rankingIndicator;
    public PlayFabController playfabInstancer;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "VictoryScene")
            speedrunTimer.text = FormatTime(PlayerPrefs.GetFloat("recentTime"));
        if ((SceneManager.GetActiveScene().name == "MenuScene" || SceneManager.GetActiveScene().name == "VictoryScene") && playfabInstancer.ranking != 0)
        {
            if (playfabInstancer.ranking != -1)
                rankingIndicator.text = "Your Current ranked position is: #" + playfabInstancer.ranking;
            else
                rankingIndicator.text = "No Successful run found";
        }
        if (fillImg != null)
            fillImg.fillAmount = fillPerc / 1;

        if (fillPerc < 1)
            fillPerc += 1.5f * Time.deltaTime;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void BeginHighlight(Image fillImgIn)
    {
        fillImg = fillImgIn;
        fillPerc = 0;
    }
    public void EndHighlight(Image fillImgIn)
    {
        fillImgIn.fillAmount = 0f;
        fillImg = null;
    }
    public void HitPlay()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void JumpMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}