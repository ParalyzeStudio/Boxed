using UnityEngine;
using UnityEngine.UI;

public class SolutionButton : GameWindowElement
{
    public Text m_buySolutionDescription;
    public Text m_revealSolutionDescription;

    public void InvalidateContent()
    {
        LevelManager levelManager = GameController.GetInstance().GetLevelManager();
        LevelData levelData = levelManager.GetLevelDataForCurrentLevel();
        if (!levelData.m_solutionPurchased)
            m_buySolutionDescription.text = "Purchase solution for " + "12" + " credits";
       
        m_buySolutionDescription.gameObject.SetActive(!levelData.m_solutionPurchased);
        m_revealSolutionDescription.gameObject.SetActive(levelData.m_solutionPurchased);
    }
}
