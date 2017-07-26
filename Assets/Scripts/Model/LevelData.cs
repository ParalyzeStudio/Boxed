using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelData
{
    private int m_levelNumber;
    public int m_movesCount; //the lowest moves count the player has reached so far, if level has not been played yet set it to 0 
    public bool m_solutionPurchased; //has the player purchased the solution of this level and made it available

    public LevelData(int levelNumber)
    {
        m_levelNumber = levelNumber;
        m_movesCount = 0;
    }

    public bool SaveToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        string folderPath = Application.persistentDataPath + "/LevelData";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        FileStream fs = null;
        try
        {
            string filePath = folderPath  + "/level_" + m_levelNumber + ".dat";
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

        string folderPath = Application.persistentDataPath + "/LevelData";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = folderPath  + "/level_" + levelNumber + ".dat";
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

    public void Reset()
    {
        m_movesCount = 0;
        m_solutionPurchased = false;
        SaveToFile();
    }
}