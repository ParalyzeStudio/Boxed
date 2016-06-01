using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Level
{
    public int m_number;
    public Floor m_floor;
    public string m_title;

    public Level(int number, Floor floor, string title)
    {
        m_number = number;
        m_floor = floor;
        m_title = title;
    }

    /**
     * Save level data to the associated file
     * **/
    public bool SaveToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        string levelsFolderPath = Application.persistentDataPath + "/Levels";
        try
        {
            fs = File.Open(levelsFolderPath + "/level_" + this.m_number + ".dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            Debug.Log("level saved to path:" + levelsFolderPath + "/level_" + this.m_number + ".dat");
        }
        catch (Exception)
        {
            Debug.Log("FAILED saving level to path:" + levelsFolderPath + "/level_" + this.m_number + ".dat");
            fs.Close();
            return false; //failed to open or create the file
        }

        bf.Serialize(fs, this);
        fs.Close();

        return true;
    }

    /**
     * Load level with number 'iLevelNumber' from file 
     * **/
    public static Level LoadFromFile(int iLevelNumber)
    {
        string filePath = Application.persistentDataPath + "Levels\\level_" + iLevelNumber + ".dat";
        return LoadFromFile(filePath);
    }

    /**
     * Load level saved at location 'path'
     * **/
    public static Level LoadFromFile(string path)
    {
        Level levelData = null;
        
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = null;
            try
            {
                fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (System.Exception)
            {
                fs.Close();
                throw new IOException("Cannot load level at location:" + path);
            }

            levelData = (Level)bf.Deserialize(fs);

            fs.Close();
        }

        return levelData;
    }

    /**
    * Return the level number as a string, adding zeros before it to match a 3 digit number
    **/
    public static string GetNumberAsString(int number)
    {
        string strNumber;
        if (number < 10)
            strNumber = "00" + number.ToString();
        else if (number < 100)
            strNumber = "0" + number.ToString();
        else
            strNumber = number.ToString();

        return strNumber;
    }
}
