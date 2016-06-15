using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<Level> m_editedLevels;
    public List<Level> EditedLevels
    {
        get
        {
            return m_editedLevels;
        }
    }

    private List<Level> m_publishedLevels;
    public List<Level> PublishedLevels
    {
        get
        {
            return m_publishedLevels;
        }
    }

    public void Start()
    {
        CacheLevels();
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

    /**
    * Typically when app is launched we cache all levels obtained from disk in a list
    **/
    public void CacheLevels()
    {
        m_editedLevels = GetAllEditedLevelsFromDisk();
        m_publishedLevels = GetAllPublishedLevelsFromDisk();
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
}
