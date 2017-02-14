using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class PersistentDataManager : MonoBehaviour
{
    public const string MUSIC_ON = "music_on";
    public const string SOUND_ON = "sound_on";
    public const string MAX_LEVEL_REACHED = "max_level_reached";

    public void Start()
    {
        //Create the folder hierarchy if it does not exists
        string path = Application.persistentDataPath + "/LevelData";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //RecreateAllLevelData();
    }

    public void SetMusicStatus(bool bMusicOn)
    {
        PlayerPrefs.SetInt(MUSIC_ON, bMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsMusicOn()
    {
        int musicStatus = PlayerPrefs.GetInt(MUSIC_ON, 1);
        return (musicStatus == 1);
    }

    public void SetSoundActive(bool bSoundOn)
    {
        PlayerPrefs.SetInt(SOUND_ON, bSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsSoundOn()
    {
        int soundStatus = PlayerPrefs.GetInt(SOUND_ON, 1);
        return (soundStatus == 1);
    }

    public void SetMaxLevelReached(int levelNumber)
    {
        int previousMaxLevelReached = GetMaxLevelReached();
        if (levelNumber > previousMaxLevelReached)
        {
            PlayerPrefs.SetInt(MAX_LEVEL_REACHED, levelNumber);
            PlayerPrefs.Save();
        }
    }

    public int GetMaxLevelReached()
    {
        return PlayerPrefs.GetInt(MAX_LEVEL_REACHED, 0);
    }

    /**
    * Use this function to recreate all LevelData files from Level files contained inside the PublishedLevels folder
    **/
    public void RecreateAllLevelData()
    {
        LevelManager levelManager = GetComponent<LevelManager>();
        List<Level> publishedLevels = levelManager.GetAllPublishedLevelsFromDisk();

        //Destroy old data
        string levelDataPath = Application.persistentDataPath + "/LevelData";
        string[] files = Directory.GetFiles(levelDataPath);

        foreach (string file in files)
        {
            File.Delete(file);
        }

        //Create files
        for (int i = 0; i != publishedLevels.Count; i++)
        {
            LevelData levelData = new LevelData(publishedLevels[i].m_number);
            levelData.SaveToFile();
        }
    }
}

