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

    public bool m_validated; //has the level been validated
    public Brick.RollDirection[] m_solution;

    public bool m_published; //has the level been published

    public Level(Floor floor)
    {
        m_floor = floor;

        m_validated = false;
    }

    /**
    * Use this class to store the output (success or failure) of the level validation process
    **/
    public class ValidationData
    {
        public bool m_success;

        //in case of success store the shortest solution
        public SolutionNode[] m_solution;

        //in case of failure
        public bool m_startTileSet;
        public bool m_finishTileSet;
        public bool m_bonusesAreReachable;
    }

    /**
    * Call this method to validate a level that has been created inside the level editor
    **/
    public ValidationData Validate(int maxMovements)
    {
        ValidationData validationData = new ValidationData();

        Tile startTile = m_floor.GetStartTile();
        Tile finishTile = m_floor.GetFinishTile();

        //First check if start tile and finish tiles have been set
        if (startTile == null || finishTile == null)
        {
            validationData.m_success = false;
            validationData.m_startTileSet = (startTile != null);
            validationData.m_finishTileSet = (finishTile != null);

            return validationData;
        }
        else
        {
            validationData.m_startTileSet = true;
            validationData.m_finishTileSet = true;
        }

        //Now check if there is at least one valid path from start tile to finish tile
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        SolutionNode[][] solutions = Solve(maxMovements);

        sw.Stop();
        Debug.Log("Level solved in " + sw.ElapsedMilliseconds + " ms");

        if (solutions == null || solutions.GetLength(0) == 0) //no solution
        {
            validationData.m_success = false;
            validationData.m_solution = null;
            return validationData;
        }
        else if (solutions.GetLength(0) == 1) //some bonuses were not on the solution path
        {
            Debug.Log("bonuses not reachable");
            validationData.m_success = false;
            validationData.m_bonusesAreReachable = false;
            validationData.m_solution = solutions[0];
        }
        else
        {
            validationData.m_success = true;
            validationData.m_bonusesAreReachable = true;
            validationData.m_solution = solutions[1];
        }

        validationData.m_success = true;
        m_validated = true;

        CopySolutionToLevel(validationData.m_solution);

        return validationData;
    }

    /**
    * Programmatically solve this level by testing every possible movement of the brick
    * Define a number of maximum movements a brick is allowed to do for a given path
    **/
    public SolutionNode[][] Solve(int maxMovements)
    {
        SolutionTree solutionTree = new SolutionTree(this);
        return solutionTree.SearchForSolutions(SolutionTree.SHORTEST_SOLUTION | SolutionTree.SHORTEST_SOLUTION_WITH_BONUSES);
    }

    /**
    * Tells if one or more of the bonuses on the floor are not reachable and return them
    **/
    //public List<Bonus> FindUnreachableBonuses()
    //{
    //    List<Bonus> unreachableBonuses = new List<Bonus>();

    //    for (int i = 0; i != m_bonuses.Count; i++)
    //    {
    //        Bonus bonus = m_bonuses[i];

    //        SolutionTree bonusTree = new SolutionTree(50, m_startTile, m_floor.FindTileForBonus(bonus), false, true);
    //        SolutionNode[][] solution = bonusTree.SearchForSolutions();

    //        if (solution == null) //one bonus is not reachable
    //            unreachableBonuses.Add(bonus);
    //    }

    //    return unreachableBonuses;
    //}

    /**
    * Transform the nodes solutions into sequence of rolling movements and copy them to this level
    **/
    public void CopySolutionToLevel(SolutionNode[] solution)
    {
        m_solution = new Brick.RollDirection[solution.Length];

        for (int i = 0; i != solution.Length; i++)
        {
            m_solution[i] = solution[i].m_direction;
        }
    }

    public bool Save()
    {        
        m_floor = m_floor.Clamp(); //clamp floor before saving
        m_floor.ClearCachedValues(); //remove cached values

        string editedLevelsFolderPath = Application.persistentDataPath + "/Levels/EditedLevels";
        return SaveToFile(editedLevelsFolderPath);
    }

    public bool Publish()
    {
        m_floor = m_floor.Clamp(); //clamp floor before saving
        m_floor.ClearCachedValues(); //remove cached values

        string publishedLevelsFolderPath = Application.persistentDataPath + "/Levels/PublishedLevels";
        return SaveToFile(publishedLevelsFolderPath);
    }

    /**
     * Save level data to the associated file
     * **/
    public bool SaveToFile(string path)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        try
        {
            fs = File.Open(path + "/" + GetFilename() + ".dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            Debug.Log("level saved to path:" + path + "/" + GetFilename() + ".dat");
        }
        catch (Exception)
        {
            Debug.Log("FAILED saving level to path:" + path + "/" + GetFilename() + ".dat");
            fs.Close();
            return false; //failed to open or create the file
        }

        bf.Serialize(fs, this);
        fs.Close();

        return true;
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

    public string GetFilename()
    {
        if (m_title == null)
            return "Level_" + GetNumberAsString(this.m_number);
        else
            return m_title.Replace(" ", "_");
    }
}
