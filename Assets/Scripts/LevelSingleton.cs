using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This singleton is used to load the level states(unlocked|completed) from disk(playerprefs)
//1,1,0 is the format used for serializing levelnumber,unlocked,completed

public class LevelSingleton
{
    private static List<Level> _levels;
    private static LevelSingleton _levelSingleton = new LevelSingleton();

    public List<Level> Levels
    {
        get { return _levels; }
        set { _levels = value; }
    }

    private LevelSingleton()
    {
//        PlayerPrefs.SetString(FinalConstants.LevelStates, ",1,1,0");
        List<string> levelsString = PlayerPrefs.GetString(FinalConstants.LevelStates, ",1,1,0").Split(',').ToList();
        Levels = new List<Level>();
        for (int i = 1; i < levelsString.Count; i++) //starting from 1 cause the first element is empty
        {
            Level l = new Level();
            l.LevelNumber = Convert.ToInt32(levelsString[i]);
            l.Unlocked = levelsString[i + 1].Equals("1");
            l.Completed = levelsString[i + 2].Equals("1");
            _levels.Add(l);
            i += 2;
        }
    }

    public static LevelSingleton getInstance()
    {
        if (_levelSingleton == null)
        {
            _levelSingleton = new LevelSingleton();
        }

        return _levelSingleton;
    }

    public static void UnlockLevel(int levelNo)
    {
        foreach (Level level in _levels)
        {
            if (levelNo.Equals(level.LevelNumber))
            {
                level.Unlocked = true;
                SaveToDisk();
                return;
            }
        }

        _levels.Add(new Level(levelNo, true, false));
        SaveToDisk();
    }

    public static void CompletedLevel(int levelNo)
    {
        foreach (Level level in _levels)
        {
            if (levelNo.Equals(level.LevelNumber))
            {
                level.Completed = true;
                SaveToDisk();
                return;
            }
        }
    }

    public static void NextLevelIncrement(int levelNo)
    {
        foreach (Level level in _levels)
        {
            if (levelNo.Equals(level.LevelNumber)) return;
        }

        _levels.Add(new Level(levelNo, true, false));
        SaveToDisk();
    }

    private static void SaveToDisk()
    {
        String toBeSaved = "";
        foreach (Level level in _levels)
        {
            toBeSaved += "," + level.LevelNumber;
            toBeSaved += level.Unlocked ? ",1" : ",0";
            toBeSaved += level.Completed ? ",1" : ",0";
        }

        PlayerPrefs.SetString(FinalConstants.LevelStates, toBeSaved);
        Reload();
    }

    private static void Reload()
    {
        _levelSingleton = new LevelSingleton();
    }
}