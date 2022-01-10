using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;
using System.Collections.Generic;
using System.Linq;

public class PlayFabController : MonoBehaviour
{
    public GameObject infoPanel;
    public static PlayFabController instance;
    #region LoginStuff
    string userEmail, userNickname;
    public InputField email, nickname;
    public Text errorText;
    public void Start()
    {
        if (PlayerPrefs.HasKey("userEmail"))
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "InitialLaunch")
                UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }
        instance = this;
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "144";
        Login();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MenuScene")
            GetLeaderboard();
    }
    void OnLoginSuccess(LoginResult result)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "InitialLaunchScene")
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        else
        {
            //Run logic to determine Ranking Score
            int rankedPosition = 0;

            ranking = rankedPosition;
        }
    }
    void OnLoginFailure(PlayFabError error)
    {
        ranking = 00000;
        GetUserEmail();
        GetUserNickname();
        if (PlayerPrefs.HasKey("userEmail"))
            Login();
    }
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Login();
        Debug.Log("Successfully Registered Player");
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = userNickname }, OnDisplayName, OnLoginFailure);
    }
    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {

    }
    void OnRegisterFailure(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.InvalidEmailAddress)
            errorText.text = "Invalid Email Address";
        else if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
            errorText.text = "Username not available, please try again";
        else
            errorText.text = "Email or Nickname invalid, please check and try again";
        Debug.LogError(error);
        email.text = "";
        nickname.text = "";
    }
    public void GetUserEmail()
    {
        userEmail = PlayerPrefs.GetString("userEmail");
        Debug.Log("email: " + PlayerPrefs.GetString("userEmail"));
    }
    public void GetUserNickname()
    {
        userNickname = PlayerPrefs.GetString("userNickname");
        Debug.Log("nickname: " + PlayerPrefs.GetString("userNickname"));
    }
    public void RegisterUser()
    {
        errorText.text = "";
        if (email.text != null)
        {
            PlayerPrefs.SetString("userEmail", email.text);
            GetUserEmail();
        }
        if (nickname.text != null)
        {
            PlayerPrefs.SetString("userNickname", nickname.text);
            GetUserNickname();
        }

        if (PlayerPrefs.HasKey("userNickname") && PlayerPrefs.HasKey("userEmail"))
        {
            var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = "Password", Username = userNickname };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        }
    }
    void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = "Password" };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }
    public void ToggleInfoPanel()
    {
        infoPanel.SetActive(!infoPanel.activeInHierarchy);
    }
    #endregion
    #region PlayerStats
    public int ranking;
    public float time;
    public Color oddLeaderboardColor, evenLeaderboardColor;
    public GameObject leaderboardHolder, leaderboardObj, leaderboardName;

    public void PostScore()
    {
        int timeRounded = Mathf.FloorToInt(PlayerPrefs.GetFloat("timeElapsed"));
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate{StatisticName = "time", Value = timeRounded},
            }
        },
        result => { Debug.Log("User Updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }
    #region LeaderboardFunctions
    public void GetLeaderboard()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "time" };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
    }
    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        bool isOdd = true;
        List<string> top20Names = new List<string>();
        result.Leaderboard.Sort();
        foreach (var item in result.Leaderboard)
        {
            Debug.Log("Found a new entry");
            if (leaderboardHolder.GetComponentsInChildren<Text>().Length < 60)
            {
                GameObject newHolder = Instantiate(leaderboardName, leaderboardHolder.transform);
                if (isOdd)
                    newHolder.GetComponent<Image>().color = oddLeaderboardColor;
                else
                    newHolder.GetComponent<Image>().color = evenLeaderboardColor;
                isOdd = !isOdd;
                Text[] texts = newHolder.GetComponentsInChildren<Text>();
                texts[0].text = ": " + item.DisplayName;
                texts[1].text = FormatTime(item.StatValue);
                for (int i = 0; i < result.Leaderboard.Count; i++)
                {
                    if (result.Leaderboard[i].DisplayName == item.DisplayName)
                        texts[2].text = "#" + (i + 1);
                }
                top20Names.Add(item.DisplayName);
            }
            if (!top20Names.Contains(PlayerPrefs.GetString("userNickname")))
            {
                for (int i = 0; i < result.Leaderboard.Count; i++)
                {
                    if (result.Leaderboard[i].DisplayName == PlayerPrefs.GetString("userNickname"))
                    {
                        GameObject newHolder = Instantiate(leaderboardName, leaderboardHolder.transform);
                        if (isOdd)
                            newHolder.GetComponent<Image>().color = oddLeaderboardColor;
                        else
                            newHolder.GetComponent<Image>().color = evenLeaderboardColor;
                        isOdd = !isOdd;
                        Text[] texts = newHolder.GetComponentsInChildren<Text>();
                        texts[0].text = ": " + result.Leaderboard[i].DisplayName;
                        texts[1].text = FormatTime(result.Leaderboard[i].StatValue);
                        texts[2].text = "#" + (i + 1);
                    }
                }
            }
        }
        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            if (result.Leaderboard[i].DisplayName == PlayerPrefs.GetString("userNickname"))
                ranking = i + 1;
            else
                ranking = -1;
        }
    }
    public string FormatTime(int time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:000}:{1:00}", minutes, seconds);
    }
    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    public void ToggleLeaderBoard()
    {
        leaderboardObj.SetActive(!leaderboardObj.activeInHierarchy);
        foreach (var item in leaderboardHolder.GetComponentsInChildren<Text>())
        {
            Destroy(item.transform.parent.gameObject);
        }
        GetLeaderboard();
    }
    #endregion
    #endregion
}