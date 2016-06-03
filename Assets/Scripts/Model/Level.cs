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

    //cache the values of start tiles and finish tiles here to avoid traversing the floor to find them
    public Tile m_startTile;
    public Tile m_finishTile;

    public Level(int number, Floor floor, string title)
    {
        m_number = number;
        m_floor = floor;
        m_title = title;

        m_startTile = floor.GetStartTile();
        m_finishTile = floor.GetFinishTile();
    }

    /**
    * Programmatically solve this level by testing every possible movement of the brick
    * Define a number of maximum movements a brick is allowed to do for a given path
    **/
    public void Solve(int maxMovements)
    {
        SolutionTree solutionTree = new SolutionTree(this, maxMovements);
        SolutionNode[][] solutions = solutionTree.SearchForSolutions();
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
