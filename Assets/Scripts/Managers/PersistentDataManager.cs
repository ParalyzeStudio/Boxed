using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class PersistentDataManager : MonoBehaviour
{
    private const string MUSIC_ON = "music_on";
    private const string SOUND_ON = "sound_on";
    //private const string MAX_LEVEL_REACHED = "max_level_reached";
    private const string FIRST_LAUNCH = "first_launch"; //tells if this is the application first launch that follows a fresh install
    private const string CURRENT_CHAPTER_INDEX = "current_chapter_index";
    private const string LAST_UNLOCKED_CHAPTER_INDEX = "last_unlocked_chapter_index";
    private const string ADS_ENABLED = "ads_enabled";
    private const string CREDITS_AMOUNT = "credits_amount";

    private bool m_prefsDirty; //has one of the above prefs been modified and a save onto the disk is necessary at some point

    public void Awake()
    {
        m_prefsDirty = false;
    }

    public void DebugLogValues()
    {
        Debug.Log("MUSIC_ON " + IsMusicOn());
        Debug.Log("SOUND_ON " + IsSoundOn());
        //Debug.Log("MAX_LEVEL_REACHED " + GetMaxLevelReached());
        Debug.Log("FIRST_LAUNCH " + IsFirstLaunch());
        Debug.Log("CURRENT_CHAPTER_INDEX " + GetCurrentChapterIndex());
        Debug.Log("ADS_ENABLED " + AreAdsEnabled());
        Debug.Log("CREDITS_AMOUNT " + GetCreditsAmount());
}

    public void SetMusicOn(bool bMusicOn)
    {
        PlayerPrefs.SetInt(MUSIC_ON, bMusicOn ? 1 : 0);
        m_prefsDirty = true;
    }

    public bool IsMusicOn()
    {
        int musicStatus = PlayerPrefs.GetInt(MUSIC_ON, 1);
        return (musicStatus == 1);
    }

    public void SetSoundOn(bool bSoundOn)
    {
        PlayerPrefs.SetInt(SOUND_ON, bSoundOn ? 1 : 0);
        m_prefsDirty = true;
    }

    public bool IsSoundOn()
    {
        int soundStatus = PlayerPrefs.GetInt(SOUND_ON, 1);
        return (soundStatus == 1);
    }

    //public void SetMaxLevelReached(int levelNumber, bool forced = false)
    //{
    //    int previousMaxLevelReached = GetMaxLevelReached();
    //    if (!forced && levelNumber <= previousMaxLevelReached)
    //        return;

    //    PlayerPrefs.SetInt(MAX_LEVEL_REACHED, levelNumber);
    //    m_prefsDirty = true;
    //}

    //public int GetMaxLevelReached()
    //{
    //    return PlayerPrefs.GetInt(MAX_LEVEL_REACHED, 0);
    //}

    public bool IsFirstLaunch()
    {
        return PlayerPrefs.HasKey(FIRST_LAUNCH);
    }

    public void SetFirstLaunchDone()
    {
        PlayerPrefs.SetInt(FIRST_LAUNCH, 1);
        m_prefsDirty = true;
    }

    public int GetCurrentChapterIndex()
    {
        return PlayerPrefs.GetInt(CURRENT_CHAPTER_INDEX, 0);
    }

    public void SetCurrentChapterIndex(int index)
    {
        PlayerPrefs.SetInt(CURRENT_CHAPTER_INDEX, index);
        m_prefsDirty = true;
    }

    public int GetLastUnlockedChapterIndex()
    {
        return PlayerPrefs.GetInt(LAST_UNLOCKED_CHAPTER_INDEX, 0);
    }

    public void SetLastUnlockedChapterIndex(int index)
    {
        PlayerPrefs.SetInt(LAST_UNLOCKED_CHAPTER_INDEX, index);
        m_prefsDirty = true;
    }

    public void DisableAds()
    {
        PlayerPrefs.SetInt(ADS_ENABLED, 0);
        m_prefsDirty = true;
    }

    public bool AreAdsEnabled()
    {
        return PlayerPrefs.GetInt(ADS_ENABLED, 1) == 1;
    }

    public void SetCreditsAmount(int creditsAmount)
    {
        PlayerPrefs.SetInt(CREDITS_AMOUNT, creditsAmount);
        m_prefsDirty = true;
    }

    public int GetCreditsAmount()
    {
        return PlayerPrefs.GetInt(CREDITS_AMOUNT, 1);
    }

    /**
    * Save prefs stored in memory onto disk if necessary
    **/
    public void SavePrefs()
    {
        if (m_prefsDirty)
            PlayerPrefs.Save();
    }

    /**
    * Reset prefs to default values when player resets the entire game
    **/
    public void OnGameReset()
    {
        SetCurrentChapterIndex(0);
        SetLastUnlockedChapterIndex(0);
        SetCreditsAmount(0);
        SavePrefs();
    }

    /**
    * Use this function to recreate all LevelData files from Level files contained inside the PublishedLevels folder
    * Use this only for DEBUG purpose 
    **/
    public void RecreateAllLevelDataForPublishedLevels()
    {
        LevelManager levelManager = GameController.GetInstance().GetLevelManager();
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

