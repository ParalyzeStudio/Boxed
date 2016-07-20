using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
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
    * Typically when app is launched we cache all levels obtained from disk in a list
    **/
    public void CacheLevels()
    {
        m_editedLevels = GetAllEditedLevelsFromDisk();
        m_publishedLevels = GetAllPublishedLevelsFromDisk();
        m_levels = GetLevelsFromResources();
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
        List<Level> levels = new List<Level>();

        Object levelObject = null;
        int levelIdx = 0;

        //Load levels we found consecutively, immediately break if we encounter a null level
        do
        {           
            string levelLocalPath = "Levels/Level_" + Level.GetNumberAsString(levelIdx + 1);
            Debug.Log(levelLocalPath);
            levelObject = Resources.Load(levelLocalPath);

            if (levelObject != null)
            {
                TextAsset levelContent = (TextAsset)levelObject;
                levels.Add(Level.LoadFromByteArray(levelContent.bytes));
            }
            else
                Debug.Log("null");

            levelIdx++;
        }
        while (levelObject != null);

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
}
