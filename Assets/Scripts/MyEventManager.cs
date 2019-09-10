using System;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using Random = UnityEngine.Random;

public class MyEventManager : MonoBehaviour
{
    public PlayerController PlayerController;
    public GameObject MainView;
    public TextMeshProUGUI ScoreLabel;
    public GameObject RateusPanel;
    public TextMeshProUGUI CompletedMessage;
    public LevelManager LevelManager;
    private static Eatable[] _eatables;
    public GameObject[] Levels;
    public static Color ChoosenMainColor; //for trail prefabs: they r generated in other class
    public Theme[] Themes;
    public int Level = 1;

    public void Restart()
    {
        Start();
    }

    // Use this for initialization
    void Start()
    {
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize("9868f21e-838d-4c9f-81f7-39a0d00f0746");
        }

        HandleLevelLoading();
        _eatables = FindObjectsOfType<Eatable>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void OnPlayClick()
    {
        MainView.SetActive(false);
    }

    public void HandleLevelLoading()
    {
        Level = PlayerPrefs.GetInt(FinalConstants.LevelTag, 1);
        PlayerController.ManualStart();
        string levelLabel = "Level#" + Level + "_EatableCollection";
        //update score
        ScoreLabel.text = PlayerPrefs.GetInt(FinalConstants.LevelTag, 1).ToString();

        foreach (var level in Levels)
        {
            level.SetActive(level.name.Equals(levelLabel));
        }

        LoadRandomTheme();
    }

    private void LoadRandomTheme()
    {
        int random = Random.Range(0, Themes.Length);
        Theme choosenTheme = Themes[random];
        ChoosenMainColor = choosenTheme.MainColor;
        Camera.main.backgroundColor = choosenTheme.BgColor;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>().material.color =
            choosenTheme.MainColor;

        GameObject[] allGrids = GameObject.FindGameObjectsWithTag("Grid");
        foreach (GameObject grid in allGrids)
        {
            grid.GetComponent<Renderer>().material.color = choosenTheme.GridColor;
        }

        Eatable[] allEatables = FindObjectsOfType<Eatable>();
        foreach (var eatable in allEatables)
        {
            eatable.DefaultColor = choosenTheme.MainColor;
            eatable.EatenColor = choosenTheme.GridColor;
            eatable.GetComponent<Renderer>().material.color = choosenTheme.MainColor;
        }
    }

    public void OnEatingUpdate()
    {
        for (var index = 0; index < _eatables.Length; index++)
        {
            Eatable eatable = _eatables[index];
            if (!eatable.IsEaten)
                return;
            if (index == _eatables.Length - 1)
            {
                if (Level == 39)
                    ActivateRateUsPanel("Congratulations! You Completed Your Quest");
                else
                    ActivateRateUsPanel(
                        FinalConstants.CongraMessages[
                            Random.Range(0, FinalConstants.CongraMessages.Length - 1)]);
                Time.timeScale = 0;
            }
        }
    }

    public void ActivateRateUsPanel(String message)
    {
        CompletedMessage.text = message;
        RateusPanel.SetActive(true);
    }

    public void RemoveAllTrails()
    {
        GameObject[] customTrails = GameObject.FindGameObjectsWithTag("CustomTrails");
        foreach (GameObject customTrail in customTrails)
        {
            Destroy(customTrail);
        }
    }

    private void NextLevel()
    {
        LevelSingleton.CompletedLevel(Level);
        ++Level;
        LevelSingleton.NextLevelIncrement(Level);
        LevelManager.UpdateUnlockedLevels();

        PlayerPrefs.SetInt(FinalConstants.LevelTag, Level);
        ResetEatablesState();
        Start();
        RemoveAllTrails();
    }

    public void ResetEatablesState()
    {
        var eatables = FindObjectsOfType<Eatable>();
        foreach (Eatable eatable in eatables)
        {
            eatable.IsEaten = false;
        }
    }

    public void OnRateClick()
    {
        Application.OpenURL(FinalConstants.PlayStoreLink);
    }

    public void OnNextPlayClick()
    {
        if (Level % 3 == 0 && Advertisement.IsReady())
        {
            Advertisement.Show("", new ShowOptions {resultCallback = HandleAdResult});
        }
        else
        {
            if (Level == 39) return;
            RateusPanel.SetActive(false);
            Time.timeScale = 1;
            NextLevel();
        }
    }

    private void HandleAdResult(ShowResult showResult)
    {
        RateusPanel.SetActive(false);
        Time.timeScale = 1;
        NextLevel();
    }

    public void OnFacebookClick()
    {
        Application.OpenURL(FinalConstants.FacebookLink);
    }
}