using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<Level> m_levels;
    public List<Level> Levels
    {
        get
        {
            return m_levels;
        }
    }

    public void Start()
    {
        CacheLevels();
    }

    public Level GetLevelForNumber(int number)
    {
        if (number > m_levels.Count)
            return null;

        return m_levels[number - 1];
    }

    /**
    * Typically when app is launched we cache all levels obtained from disk in a list
    **/
    public void CacheLevels()
    {
        m_levels = GetAllLevelsFromDisk();
    }

    /**
    * Return all levels save on disk through a list of Level objects
    **/
    public List<Level> GetAllLevelsFromDisk()
    {   
        string levelsFolderPath = Application.persistentDataPath + "\\Levels";
        if (!Directory.Exists(levelsFolderPath))
            Directory.CreateDirectory(levelsFolderPath);
        string[] levelsFilenames = Directory.GetFiles(levelsFolderPath);
        List<Level> allLevels = new List<Level>(levelsFilenames.Length);

        for (int i = 0; i != levelsFilenames.Length; i++)
        {
            allLevels.Add(Level.LoadFromFile(levelsFilenames[i]));
        }

        return allLevels;
    }
}
