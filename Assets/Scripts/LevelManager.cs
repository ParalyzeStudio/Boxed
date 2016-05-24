using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<Level> m_levels;

    public void ObtainLevels()
    {
        m_levels = new List<Level>();
        Level level = null;
        int levelNumber = 1;
        while (true)
        {
            level = Level.LoadFromFile(levelNumber);
            levelNumber++;
            if (level != null)
                m_levels.Add(level);
            else
                break;
        }
    }

    public Level GetLevelForNumber(int number)
    {
        return m_levels[number - 1];
    }
}
