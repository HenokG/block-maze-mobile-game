using System;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Image levelContainer;
    public MyEventManager MyEventManager;
    public GameObject UnlockLevelsContent;
    private static int AfterAdCompletionUnlock;

    // Use this for initialization
    void Start()
    {
        int levelLabeler = 0;
        for (int i = 1; i <= MyEventManager.Levels.Length / 3; i++)
        {
            GameObject newUnlocksContainer =
                UnlockLevelsContent.transform.Find("unlock_container (" + i + ")").gameObject;
            for (int j = 1; j <= 3; j++)
            {
                Transform akafi = newUnlocksContainer.transform.Find(j.ToString());
                akafi.Find("level").GetComponent<TextMeshProUGUI>().text = (++levelLabeler).ToString();
                akafi.name = levelLabeler.ToString();
                Button unlockButton = akafi.Find("unlock").GetComponent<Button>();
                unlockButton.name = (levelLabeler).ToString();
                unlockButton.onClick.AddListener(() => OnUnlockClick(unlockButton.gameObject));
                akafi.GetComponent<Button>().onClick.AddListener(() => OnUnlockClick(unlockButton.gameObject));
            }
        }

        //hide levels view on start
        levelContainer.gameObject.SetActive(false);
        UpdateUnlockedLevels();
    }

    //toggle Leves Ui on levels button click
    public void OnLevelButtonClick()
    {
        levelContainer.gameObject.SetActive(!levelContainer.gameObject.activeSelf);
    }

    public void OnRemoveAdClick()
    {
        //remove ad permanently
        //TODO remove ad when recieving money
        PlayerPrefs.SetInt(FinalConstants.AdRemoved, 1);
    }

    public void OnUnlockClick(GameObject g)
    {
        bool adRemoved = PlayerPrefs.GetInt(FinalConstants.AdRemoved, 0) != 0;
//        adRemoved = false;
        if (adRemoved)
        {
            LevelSingleton.UnlockLevel(Convert.ToInt32(g.name));
        }
        else
        {
            //launch video ad for unlocking next level
           ShowAd();
            AfterAdCompletionUnlock = Convert.ToInt32(g.name);
        }

        Transform temp = g.transform.Find("btnLabel");
        if (temp.gameObject.GetComponent<TextMeshProUGUI>().text == "Play" ||
            temp.gameObject.GetComponent<TextMeshProUGUI>().text == "Finished") //if play instead of unlock then play
        {
            PlayerPrefs.SetInt(FinalConstants.LevelTag, int.Parse(g.name));
            levelContainer.gameObject.SetActive(false);
            OnResetCurrentLevel();
        }

        UpdateUnlockedLevels();
    }

    //update unlocked levels ui
    public void UpdateUnlockedLevels()
    {
        for (int i = 0; i < LevelSingleton.getInstance().Levels.Count; i++)
        {
            Level level = LevelSingleton.getInstance().Levels[i];
            for (int j = 1; j <= MyEventManager.Levels.Length / 3; j++)
            {
                GameObject randomUnlocksContainer =
                    UnlockLevelsContent.transform.Find("unlock_container (" + j + ")").gameObject;
                Transform tr = randomUnlocksContainer.transform.Find(level.LevelNumber.ToString());
                if (tr)
                {
                    if (level.Completed)
                    {
                        tr.GetComponent<Image>().color = Color.blue;
                        tr.Find(level.LevelNumber.ToString()).Find("btnLabel").GetComponent<TextMeshProUGUI>().text =
                            "Finished";
                    }
                    else if (level.Unlocked)
                    {
                        tr.GetComponent<Image>().color = Color.gray;
                        tr.Find(level.LevelNumber.ToString()).Find("btnLabel").GetComponent<TextMeshProUGUI>().text =
                            "Play";
                    }
                }
            }
        }
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("rewardedVideo", new ShowOptions(){resultCallback = HandleAdResult});
        }
    }

    private void HandleAdResult(ShowResult showResult)
    {
        if (showResult != ShowResult.Finished) return;
        LevelSingleton.UnlockLevel(AfterAdCompletionUnlock);
    }
    
    public void OnResetCurrentLevel()
    {
        MyEventManager.HandleLevelLoading();
        MyEventManager.Restart();
        MyEventManager.ResetEatablesState();
        MyEventManager.RemoveAllTrails();
    }
}