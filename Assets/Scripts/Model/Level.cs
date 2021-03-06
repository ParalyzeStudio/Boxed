﻿using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Level
{
    public int m_number;
    public Floor m_floor;
    public string m_title;

    public bool m_validated; //has the level been validated
    public Brick.RollDirection[] m_solution;

    public bool m_published; //has the level been published

    private const int MAX_SOLUTION_TREE_HEIGHT = 40;

    private static System.Diagnostics.Stopwatch s_stopwatch;

    public Level(Floor floor)
    {
        m_floor = floor;

        m_validated = false;
    }

    //public Level(Level oldLevel)
    //{
    //    m_number = oldLevel.m_number;
    //    m_floor = oldLevel.m_floor;
    //    m_title = oldLevel.m_title;
    //    m_validated = oldLevel.m_validated;
    //    m_solution = oldLevel.m_solution;
    //    m_published = oldLevel.m_published;
    //}

    public Level(Level other)
    {
        m_number = other.m_number;
        m_title = other.m_title;
        m_validated = other.m_validated;
        m_published = other.m_published;
        m_solution = other.m_solution;

        //deep copy floor tiles
        m_floor = new Floor(other.m_floor);
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
    public void Validate()
    {
        ValidationData validationData = new ValidationData();

        Tile startTile = m_floor.GetStartTile();
        Tile finishTile = m_floor.GetFinishTile();

        //First check if start and finish tiles have been set
        if (startTile == null || finishTile == null)
        {
            validationData.m_success = false;
            validationData.m_startTileSet = (startTile != null);
            validationData.m_finishTileSet = (finishTile != null);

            LevelEditor levelEditor = ((LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI);
            levelEditor.OnFinishValidatingLevel(validationData);
        }
        else
        {
            validationData.m_startTileSet = true;
            validationData.m_finishTileSet = true;

            //Now check if there is at least one valid path from start tile to finish tile
            s_stopwatch = new System.Diagnostics.Stopwatch();
            s_stopwatch.Start();

            Solve(validationData);


            s_stopwatch.Stop();
            Debug.Log("elapsed:" + s_stopwatch.ElapsedMilliseconds);
        }
    }

    public void Solve(ValidationData validationData)
    {
        int treeHeight = 2;

        SolveTree(treeHeight, validationData);





        //SolutionNode[][] solutions;
        //do
        //{
        //    Debug.Log("solving tree with height:" + treeHeight);
        //    SolveTree(treeHeight);
        //    //SolutionTree solutionTree = new SolutionTree(treeHeight, this);


        //    //QueuedThreadedJobsManager threadManager = GameController.GetInstance().GetComponent<QueuedThreadedJobsManager>();


        //    //solutions = solutionTree.SearchForSolutions();

        //    //if (solutions == null)
        //    //{
        //    //    //check if we failed to reach at least one leaf node of this tree
        //    //    //if this is the case, no need to search for solutions on a bigger tree as we won't go any further than we have already reached
        //    //    if (!solutionTree.m_maximumHeightReached)
        //    //        break;
        //    //}

        //    treeHeight++;
        //}
        //while (solutions == null && treeHeight < SOLUTION_MAX_MOVEMENTS);

        //if (solutions != null)
        //    return solutions[0];
        //else
        //    return null;
    }

    private void SolveTree(int treeHeight, ValidationData validationData)
    {
        Debug.Log(">>>>>>>Solving tree with height " + treeHeight);
        
        //for (int i = 0; i != m_floor.Tiles.Length; i++)
        //{
        //    Tile tile = m_floor.Tiles[i];
        //    if (tile.CurrentState == Tile.State.TRIGGERED_BY_SWITCH)
        //    {
        //        Debug.Log("isliftup:" + ((TriggeredTile)tile).m_isLiftUp);
        //    }
        //}
        //bool duplicateIsOn = ((SwitchTile)GameController.GetInstance().m_floor.m_floorData.Tiles[66]).m_isOn;
        //Floor editedLevelFloor = ((LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI).m_editedLevel.m_floor;
        //bool editedIsOn = ((SwitchTile)editedLevelFloor.Tiles[66]).m_isOn;
        //bool thisIsOn = ((SwitchTile)m_floor.Tiles[66]).m_isOn;
        //Debug.Log(duplicateIsOn + "-" + editedIsOn + "-" + thisIsOn);


        Level duplicateLevelForSolving = new Level(this);
        GameController.GetInstance().m_floorRenderer.m_floorData = duplicateLevelForSolving.m_floor; //replace temporarily the global floor instance
        SolutionTree solutionTree = new SolutionTree(treeHeight, duplicateLevelForSolving); //perform the algorithm on a copied tree to not modify the original state of the floor
        solutionTree.SearchForSolutions(validationData);
    }

    public void OnFinishComputingSolutionsForTree(SolutionTree tree, ValidationData validationData)
    {
        //bool duplicateIsOn = ((SwitchTile)GameController.GetInstance().m_floor.m_floorData.Tiles[66]).m_isOn;
        //Floor editedLevelFloor = ((LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI).m_editedLevel.m_floor;
        //bool editedIsOn = ((SwitchTile)editedLevelFloor.Tiles[66]).m_isOn;
        //bool thisIsOn = ((SwitchTile)m_floor.Tiles[66]).m_isOn;
        //Debug.Log(duplicateIsOn + "-" + editedIsOn + "-" + thisIsOn);

        if (tree.m_solutions == null)
        {
            //check if we failed to reach at least one leaf node of this tree
            //if this is the case, no need to search for solutions on a bigger tree as we won't go any further than we have already reached
            if (tree.m_maximumHeightReached)
            {
                int nextTreeHeight = tree.m_maximumHeight + 1;
                if (nextTreeHeight <= MAX_SOLUTION_TREE_HEIGHT)
                    SolveTree(nextTreeHeight, validationData);
                else
                    OnFinishSolvingLevel(null, validationData);
            }
            else
                OnFinishSolvingLevel(null, validationData);
        }
        else
            OnFinishSolvingLevel(tree.m_solutions[0], validationData);
    }

    private void OnFinishSolvingLevel(SolutionNode[] shortestSolution, ValidationData validationData)
    {
        s_stopwatch.Stop();

        //revert back to the correct global floor instance
        LevelEditor levelEditor = (LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
        Level editedLevel = levelEditor.m_editedLevel;
        GameController.GetInstance().m_floorRenderer.m_floorData = editedLevel.m_floor;

        if (shortestSolution == null) //no solution
        {
            validationData.m_success = false;
            validationData.m_solution = null;
            m_validated = false;
        }
        else
        {
            validationData.m_success = true;
            validationData.m_bonusesAreReachable = true;
            validationData.m_solution = shortestSolution;
            m_validated = true;
            editedLevel.CopySolution(validationData.m_solution);
        }
        
        levelEditor.OnFinishValidatingLevel(validationData);
    }

    //private SolutionNode[] ExtractSolutionForPermutation(Permutation permutation, int maxSolutionLength)
    //{
    //    List<Tile> bonusTiles = this.m_floor.GetBonusTiles();
    //    List<SolutionNode> solution = new List<SolutionNode>();

    //    int solutionLength = 0; //the length of the solution for this permutation

    //    Tile[] subpathStartTiles = new Tile[2];
    //    Tile subPathFinishTile;
    //    int subpathsCount = permutation.Count + 1;
    //    int p = 0;
    //    while (p < subpathsCount)
    //    {
    //        if (p == 0)
    //        {
    //            subpathStartTiles[0] = this.m_floor.GetStartTile();
    //            subpathStartTiles[1] = null;
    //            subPathFinishTile = bonusTiles[permutation[0]];
    //        }
    //        else if (p == subpathsCount - 1)
    //        {
    //            subPathFinishTile = this.m_floor.GetFinishTile();
    //        }
    //        else
    //        {
    //            subPathFinishTile = bonusTiles[permutation[p]];
    //        }

    //        SolutionTree solutionTree = new SolutionTree(100, subpathStartTiles, subPathFinishTile, (p == subpathsCount - 1));
    //        SolutionNode[][] subpathSolutions = solutionTree.SearchForSolutions();

    //        if (subpathSolutions != null)
    //        {
    //            SolutionNode[] subpathSolution = subpathSolutions[0];

    //            solutionLength += subpathSolution.Length;
    //            if (solutionLength > maxSolutionLength)
    //            {
    //                return null;
    //            }
    //            else
    //            {
    //                for (int m = 0; m != subpathSolution.Length; m++)
    //                {
    //                    SolutionNode node = subpathSolution[m];
    //                }

    //                solution.AddRange(subpathSolution);

    //                if (p < subpathsCount - 1)
    //                    subpathStartTiles = subpathSolution[subpathSolution.Length - 1].m_coveredTiles; //set the start tiles for next subpath
    //            }
    //        }
    //        else
    //            return null;

    //        p++;
    //    }

    //    return solution.ToArray();
    //}

    /**
    * Find all permutations between bonuses
    * The number of permutations is !bonusesCount
    **/
    //private List<Permutation> FindBonusPermutations()
    //{
    //    return FindPermutationsInInterval(m_floor.GetBonusTiles().Count - 1);
    //}

    /**
    * Find all permutations for the interval [0-N]
    **/
    //private List<Permutation> FindPermutationsInInterval(int N)
    //{
    //    if (N < 0)
    //        return null;

    //    //to compute the factorial function, as the number of bonuses wont exceed 3, just use a simple loop, no need for a complex and efficient algorithm
    //    int permutationsCount = 1;
    //    int i = 2;
    //    while (i <= N)
    //    {
    //        permutationsCount *= i;
    //        i++;
    //    }

    //    List<Permutation> permutations = new List<Permutation>(permutationsCount);

    //    if (N == 0)
    //        permutations.Add(new Permutation { 0 });
    //    else
    //    {
    //        List<Permutation> lPermutations = FindPermutationsInInterval(N - 1);

    //        int j = 0;
    //        while (j <= N)
    //        {
    //            //First copy the permutations
    //            List<Permutation> copiedPermutations = new List<Permutation>();
    //            for (int p = 0; p != lPermutations.Count; p++)
    //            {
    //                copiedPermutations.Add(new Permutation(lPermutations[p]));
    //            }

    //            InsertInPermutationsAtIndex(j, N, copiedPermutations);
    //            permutations.AddRange(copiedPermutations);
    //            j++;
    //        }
    //    }

    //    return permutations;
    //}

    //private void InsertInPermutationsAtIndex(int index, int item, List<Permutation> permutations)
    //{
    //    for (int i = 0; i != permutations.Count; i++)
    //    {
    //        permutations[i].Insert(index, item);
    //    }
    //}

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
    public void CopySolution(SolutionNode[] solution)
    {
        m_solution = new Brick.RollDirection[solution.Length];

        for (int i = 0; i != solution.Length; i++)
        {
            m_solution[i] = solution[i].m_direction;
        }
    }

    public bool Save()
    {
        //m_floor = m_floor.Clamp(); //clamp floor before saving
        //m_floor.ClearCachedValues(); //remove cached values

        Floor clampedFloor = m_floor.Clamp(); //clamp floor before saving

        Floor originalFloor = m_floor;
        m_floor = clampedFloor;

        string editedLevelsFolderPath = Application.persistentDataPath + "/Levels/EditedLevels";
        bool levelSaved = SaveToFile(editedLevelsFolderPath);

        m_floor = originalFloor;
        return levelSaved;
    }

    public bool Publish()
    {
        m_published = true;

        m_floor = m_floor.Clamp(); //clamp floor before saving

        //Create a new LevelData file
        LevelData levelData = new LevelData(this.m_number);
        levelData.SaveToFile();

        string publishedLevelsFolderPath = Application.persistentDataPath + "/Levels/PublishedLevels";
        return SaveToFile(publishedLevelsFolderPath);
    }

    /**
     * Save level data to file
     * **/
    public bool SaveToFile(string path)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = null;
        try
        {
            string filePath = path + "/" + GetFilename() + ".dat";
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            Debug.Log("level saved to path:" + filePath);
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

    public static Level LoadFromByteArray(byte[] bytes)
    {
        MemoryStream memoryStream = new MemoryStream(bytes);

        BinaryFormatter bf = new BinaryFormatter();
        Level level = (Level)bf.Deserialize(memoryStream);
        memoryStream.Close();

        return level;
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
        return "Level_" + GetNumberAsString(this.m_number);
    }

    /**
    * Return the relative number of this level [1 - NUM_LEVELS_PER_CHAPTER]
    **/
    public int GetChapterRelativeNumber()
    {
        return ((m_number - 1) % LevelManager.NUM_LEVELS_PER_CHAPTER) + 1;
    }

    /**
    * Return the index of the chapter that contains this level
    **/
    public int GetChapterIndex()
    {
        return (m_number - 1) / LevelManager.NUM_LEVELS_PER_CHAPTER;
    }

    ///
    /// DYNAMIC DATA//
    ///
    //public void InitDynamicData()
    //{
    //    m_dynamicData.m_currentActionsCount = 0;
    //}

    //public void IncrementActionsCount()
    //{
    //    m_dynamicData.m_currentActionsCount++;
    //}

    //public int GetActionsCount()
    //{
    //    return m_dynamicData.m_currentActionsCount;
    //}
}
