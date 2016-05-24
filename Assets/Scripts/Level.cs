using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Level
{
    public int m_number;

    public int m_gridSize;
    
    public Tile[] m_tiles; //rectangular grid of tiles in this level
    public Tile m_startTile; //tile initially covered by the brick
    public Tile m_endTile; //tile the brick need to reach.

    public Level(int number, int gridSize, Tile[] tiles, Tile startTile, Tile endTile)
    {
        m_number = number;
        m_gridSize = gridSize;
        m_tiles = tiles;
        m_startTile = startTile;
        m_endTile = endTile;
    }

    /**
     * Save level data to the associated file
     * **/
    private bool SaveToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        try
        {
            fs = File.Open(Application.persistentDataPath + "/level_" + this.m_number + ".dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }
        catch (Exception)
        {
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
        Level levelData = null;

        string filePath = Application.persistentDataPath + "/level_" + iLevelNumber + ".dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = null;
            try
            {
                fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (System.Exception)
            {
                fs.Close();
                throw new IOException("Cannot load level " + iLevelNumber);
            }

            levelData = (Level)bf.Deserialize(fs);

            fs.Close();
        }

        return levelData;
    }
}
