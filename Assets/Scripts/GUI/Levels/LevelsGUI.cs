using UnityEngine;
using UnityEngine.UI;

public class LevelsGUI : MonoBehaviour
{
    public GameObject m_levelsList;
    public GameObject m_levelItemPfb;

    public void Init()
    {
        PopulateList();
    }

    private void PopulateList()
    {
        LevelManager levelManager = GameController.GetInstance().GetComponent<LevelManager>();

        for (int i = 0; i != levelManager.PublishedLevels.Count; i++)
        {
            Level level = levelManager.PublishedLevels[i];

            GameObject levelItemObject = (GameObject)Instantiate(m_levelItemPfb);
            levelItemObject.GetComponentInChildren<Text>().text = level.m_number + " - " + level.m_title;
        }
    }
}
