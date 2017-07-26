using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public const int NUM_LEVELS_PER_CHAPTER = 15;
    public const int NUM_CHAPTERS = 4;

    //levels that are currently edited and can be reopened inside the level editor
    private List<Level> m_editedLevels;
    public List<Level> EditedLevels
    {
        get
        {
            return m_editedLevels;
        }
    }

    //levels that have been published through the level editor
    private List<Level> m_publishedLevels;
    public List<Level> PublishedLevels
    {
        get
        {
            return m_publishedLevels;
        }
    }

    //levels that have been created by me and set into the final executable
    private List<Level> m_levels;
    public List<Level> Levels
    {
        get
        {
            return m_levels;
        }
    }


    private LevelData[] m_cachedLevelData;
    public LevelData[] CachedLevelData
    {
        get
        {
            return m_cachedLevelData;
        }
    }

    public Level m_currentLevel { get; set; }

    public void Init()
    {
        Debug.Log(">>>>CacheLevels");

        //cache levels written on disk into memory
        CacheLevels();
        CacheLevelData();

        Debug.Log(">>>>Levels CACHED");

        //Create the LevelData/ folder hierarchy if it does not exists
        string path = Application.persistentDataPath + "/LevelData";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //initialize one LevelData for every file
        PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        if (pDataManager.IsFirstLaunch())
        {
            pDataManager.SetFirstLaunchDone();
            CreateOrOverwriteAllLevelData();
        }

        //RepublishAllLevelsForUpgrade();
    }

    //private void RepublishAllLevelsForUpgrade()
    //{
    //    List<NewLevel> oldPublishedLevels = GetAllPublishedLevelsFromDisk();
    //    for (int i = 0; i != oldPublishedLevels.Count; i++)
    //    {
    //        NewLevel upgradedLevel = new Level(oldPublishedLevels[i]);
    //        upgradedLevel.Publish();
    //    }

    //    List<Level> oldEditedLevels = GetAllEditedLevelsFromDisk();
    //    for (int i = 0; i != oldEditedLevels.Count; i++)
    //    {
    //        NewLevel upgradedLevel = new NewLevel(oldEditedLevels[i]);
    //        upgradedLevel.Save();
    //    }
    //}

    public Level GetEditedLevelForNumber(int number)
    {
        for (int i = 0; i != m_editedLevels.Count; i++)
        {
            if (m_editedLevels[i].m_number == number)
                return m_editedLevels[i];
        }

        return null;
    }

    public Level GetPublishedLevelForNumber(int number)
    {
        for (int i = 0; i != m_publishedLevels.Count; i++)
        {
            if (m_publishedLevels[i].m_number == number)
                return m_publishedLevels[i];
        }

        return null;
    }

    public Level GetNextPublishedLevelForLevel(Level level)
    {
        for (int i = 0; i != m_publishedLevels.Count; i++)
        {
            if (level == m_publishedLevels[i] && i < m_publishedLevels.Count - 1)
                return m_publishedLevels[i];
        }

        return null;
    }

    public Level GetPreviousPublishedLevelForLevel(Level level)
    {
        for (int i = 1; i != m_publishedLevels.Count; i++)
        {
            if (level == m_publishedLevels[i])
                return m_publishedLevels[i - 1];
        }

        return null;
    }

    /**
    * Return a level that has been generated and set into the final executable
    **/
    public Level GetLevelForNumber(int number)
    {
        for (int i = 0; i != m_levels.Count; i++)
        {
            if (m_levels[i].m_number == number)
                return m_levels[i];
        }

        return null;
    }

    /**
    * Return the direct next level of the current one
    **/
    public Level GetNextLevel()
    {
        if (m_currentLevel.m_number < m_levels.Count)
            return m_levels[m_currentLevel.m_number];

        return null;
    }

    /**
    * Typically when app is launched we cache all levels obtained from disk in a list
    **/
    public void CacheLevels()
    {
        m_editedLevels = GetAllEditedLevelsFromDisk();
        m_publishedLevels = GetAllPublishedLevelsFromDisk();
        m_levels = GetLevelsFromResources();
    }

    public void CacheLevelData()
    {
        int levelsCount = NUM_CHAPTERS * NUM_LEVELS_PER_CHAPTER;
        m_cachedLevelData = new LevelData[levelsCount];
        for (int i = 0; i != levelsCount; i++)
        {
            m_cachedLevelData[i] = LevelData.LoadFromFile(i + 1);
        }
    }

    public LevelData GetLevelDataForCurrentLevel()
    {
        return GetLevelDataForLevel(m_currentLevel);
    }

    public LevelData GetLevelDataForLevel(Level level)
    {
        return m_cachedLevelData[level.m_number - 1];
    }

    /**
    * Called when the player performs a manual reset on the game
    **/
    public void OnGameReset()
    {
        for (int i = 0; i != m_cachedLevelData.Length; i++)
        {
            m_cachedLevelData[i].Reset();
        }
    }

    /**
    * Return all levels save on disk through a list of Level objects
    **/
    public List<Level> GetAllEditedLevelsFromDisk()
    {
        string editedLevelsPath = Application.persistentDataPath + "/Levels/EditedLevels";
        return GetAllLevelsFromFolder(editedLevelsPath);
    }

    public List<Level> GetAllPublishedLevelsFromDisk()
    {
        string publishedLevelsPath = Application.persistentDataPath + "/Levels/PublishedLevels";
        return GetAllLevelsFromFolder(publishedLevelsPath);
    }

    /**
    * Return the levels stored inside the Resources folder of this executable
    **/
    public List<Level> GetLevelsFromResources()
    {
        int levelsCount = NUM_CHAPTERS * NUM_LEVELS_PER_CHAPTER;
        List<Level> levels = new List<Level>(levelsCount);        
        for (int i = 0; i != levelsCount; i++)
        {
            string levelLocalPath = "Levels/Level_" + Level.GetNumberAsString(i + 1);
            Object levelObject = Resources.Load(levelLocalPath);

            if (levelObject != null)
            {
                TextAsset levelContent = (TextAsset)levelObject;
                levels.Add(Level.LoadFromByteArray(levelContent.bytes));
            }
            //else levels.Add(null);
        }

        return levels;
    }

    public List<Level> GetAllLevelsFromFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        string[] levelsFilenames = Directory.GetFiles(folderPath);
        List<Level> allLevels = new List<Level>(levelsFilenames.Length);

        for (int i = 0; i != levelsFilenames.Length; i++)
        {
            allLevels.Add(Level.LoadFromFile(levelsFilenames[i]));
        }

        return allLevels;
    }

    public void DeletePublishedLevelFileForLevel(Level level)
    {
        string publishedLevelsPath = Application.persistentDataPath + "/Levels/PublishedLevels";
        string[] levelFilenames = Directory.GetFiles(publishedLevelsPath);

        for (int i = 0; i != levelFilenames.Length; i++)
        {
            Level loadedLevel = Level.LoadFromFile(levelFilenames[i]);

            if (loadedLevel.m_number == level.m_number)
                File.Delete(levelFilenames[i]);
        }
    }

    /**
    * Create or recreate one LevelData file for each Level in this game
    * That will overwrite any existing file so be cautious
    **/
    public void CreateOrOverwriteAllLevelData()
    {
        int levelsCount = NUM_CHAPTERS * NUM_LEVELS_PER_CHAPTER;

        //Destroy old data if any
        string levelDataPath = Application.persistentDataPath + "/LevelData";
        Directory.Delete(levelDataPath);
        Directory.CreateDirectory(levelDataPath);
        //string[] files = Directory.GetFiles(levelDataPath);

        //foreach (string file in files)
        //{
        //    File.Delete(file);
        //}

        //create new one
        for (int i = 0; i != levelsCount; i++)
        {
            LevelData levelData = new LevelData(i + 1);
            levelData.SaveToFile();
        }
    }
}
