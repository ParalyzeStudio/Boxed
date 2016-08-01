using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelData
{
    public bool m_done; //has the level already been won in the past
    public bool m_reachedBestScore; //has the level been won with reaching the lowest actions count

    public int m_currentActionsCount; //when playing a level, store here the current count of actions

    private int m_levelNumber;

    public LevelData(int levelNumber)
    {
        m_levelNumber = levelNumber;
        m_currentActionsCount = 0;
        m_done = false;
        m_reachedBestScore = false;
    }

    public bool SaveToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        try
        {
            string filePath = Application.persistentDataPath + "/LevelData/level_" + m_levelNumber + ".dat";
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            
            fs.Close();
            return false; //failed to open or create the file
        }

        bf.Serialize(fs, this);
        fs.Close();

        return true;
    }

    public static LevelData LoadFromFile(int levelNumber)
    {
        LevelData levelData = null;

        string filePath = Application.persistentDataPath + "/LevelData/level_" + levelNumber + ".dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = null;
            try
            {
                fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (Exception)
            {
                fs.Close();
                levelData = new LevelData(levelNumber);
                return levelData;
            }
            levelData = (LevelData)bf.Deserialize(fs);
            fs.Close();
        }
        else
        {
            levelData = new LevelData(levelNumber);
            levelData.SaveToFile();
        }
        return levelData;
    }
}