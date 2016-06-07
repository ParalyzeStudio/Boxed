using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[Serializable]
public class Level
{
    public int m_number;
    public Floor m_floor;
    public string m_title;

    //cache the values of start tiles and finish tiles here to avoid traversing the floor to find them
    public Tile m_startTile;
    public Tile m_finishTile;

    //also cache the values for bonuses
    public List<Bonus> m_bonuses;

    public bool m_validated; //has the level been validated
    public Brick.RollDirection[][] m_solutions;

    public Level(Floor floor)
    {
        m_floor = floor;

        m_validated = false;
        m_bonuses = new List<Bonus>();
    }

    /**
    * Use this class to store the output (success or failure) of the level validation process
    **/
    public class ValidationData
    {
        public bool m_success;

        //in case of success
        public SolutionNode[][] m_solutions;

        //in case of failure
        public bool m_startTileSet;
        public bool m_finishTileSet;
        public List<Bonus> m_unreachableBonuses;

        public void AddUnreachableBonus(Bonus bonus)
        {
            if (m_unreachableBonuses == null)
                m_unreachableBonuses = new List<Bonus>();

            m_unreachableBonuses.Add(bonus);
        }
    }
    
    /**
    * Call this method to validate a level that has been created inside the level editor
    **/
    public ValidationData Validate(int maxMovements)
    {
        ValidationData validationData = new ValidationData();

        //First check if start tile and finish tiles have been set
        if (m_startTile == null || m_finishTile == null)
        {
            validationData.m_success = false;
            validationData.m_startTileSet = (m_startTile != null);
            validationData.m_finishTileSet = (m_finishTile != null);

            return validationData;
        }
        else
        {
            validationData.m_startTileSet = true;
            validationData.m_finishTileSet = true;
        }

        //Now check if there is at least one valid path from start tile to finish tile
        SolutionNode[][] solutions = Solve(maxMovements);
        validationData.m_solutions = solutions;
        if (solutions == null)
        {
            validationData.m_success = false;
            return validationData;
        }       

        //Check if all bonuses are reachable
        //for (int i = 0; i != m_bonuses.Count; i++)
        //{
        //    if (!IsBonusReachable(m_bonuses[i]))
        //    {
        //        validationData.m_success = false;
        //        validationData.AddUnreachableBonus(m_bonuses[i]);
        //        return validationData;
        //    }
        //}

        validationData.m_success = true;
        m_validated = true;

        CopySolutionsToLevel(validationData.m_solutions);

        return validationData;
    }

    /**
    * Programmatically solve this level by testing every possible movement of the brick
    * Define a number of maximum movements a brick is allowed to do for a given path
    **/
    public SolutionNode[][] Solve(int maxMovements)
    {
        SolutionTree solutionTree = new SolutionTree(maxMovements, m_startTile, m_finishTile);
        return solutionTree.SearchForSolutions();
    }

    /**
    * Transform the nodes solutions into sequence of rolling movements and copy them to this level
    **/
    public void CopySolutionsToLevel(SolutionNode[][] solutions)
    {
        m_solutions = new Brick.RollDirection[solutions.GetLength(0)][];

        for (int i = 0; i != solutions.GetLength(0); i++)
        {
            Brick.RollDirection[] solution = new Brick.RollDirection[solutions[i].Length];
            for (int j = 0; j != solution.Length; j++)
            {
                solution[j] = solutions[i][j].m_direction;
            }

            m_solutions[i] = solution;
        }
    }

    /**
    * Tells if a bonus can be reached by the brick
    **/
    public bool IsBonusReachable(Bonus bonus)
    {
        Tile targetTile = m_floor.FindTileForBonus(bonus);
        SolutionTree solutionTree = new SolutionTree(100, m_startTile, targetTile, true); //big enough number of movements
        return solutionTree.SearchForSolutions() != null; //if we found at least one path we're good to go
    }

    /**
     * Save level data to the associated file
     * **/
    public bool SaveToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        string levelsFolderPath = Application.persistentDataPath + "/Levels/EditedLevels";
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
        string filePath = Application.persistentDataPath + "Levels/EditedLevels/level_" + iLevelNumber + ".dat";
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
