//using UnityEngine;
//using System;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Collections.Generic;

//using Permutation = System.Collections.Generic.List<int>;

//[Serializable]
//public class Level
//{
//    public int m_number;
//    public Floor m_floor;
//    public string m_title;

//    public bool m_validated; //has the level been validated
//    public Brick.RollDirection[] m_solution;

//    public bool m_published; //has the level been published

//    public class ValidationData
//    {
//        public bool m_success;

//        //in case of success store the shortest solution
//        public SolutionNode[] m_solution;

//        //in case of failure
//        public bool m_startTileSet;
//        public bool m_finishTileSet;
//        public bool m_bonusesAreReachable;
//    }



//public Level(Level oldLevel, Switch[] switches)
//{
//    m_number = oldLevel.m_number;
//    m_floor = oldLevel.m_floor;
//    m_title = oldLevel.m_title;
//    m_validated = oldLevel.m_validated;
//    m_solution = oldLevel.m_solution;
//    m_published = oldLevel.m_published;

//    m_switches = switches;
//}

//    /**
//     * Load level saved at location 'path'
//     * **/
//    public static Level LoadFromFile(string path)
//    {
//        Level levelData = null;

//        if (File.Exists(path))
//        {
//            BinaryFormatter bf = new BinaryFormatter();
//            FileStream fs = null;
//            try
//            {
//                fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
//            }
//            catch (System.Exception)
//            {
//                fs.Close();
//                throw new IOException("Cannot load level at location:" + path);
//            }

//            levelData = (Level)bf.Deserialize(fs);

//            fs.Close();
//        }

//        return levelData;
//    }

//    public static Level LoadFromByteArray(byte[] bytes)
//    {
//        MemoryStream memoryStream = new MemoryStream(bytes);

//        BinaryFormatter bf = new BinaryFormatter();
//        Level level = (Level)bf.Deserialize(memoryStream);
//        memoryStream.Close();

//        return level;
//    }
//}
