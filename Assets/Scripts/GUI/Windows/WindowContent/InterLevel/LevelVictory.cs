using System.Collections;
using UnityEngine;

public class LevelVictory : GameWindowContent
{
    public override IEnumerator Show(float timeSpacing = DEFAULT_TIME_SPACING)
    {
        LevelData currentLevelData = GameController.GetInstance().GetLevelManager().GetLevelDataForCurrentLevel();
        if (currentLevelData.m_movesCount == 0 || currentLevelData.m_movesCount > GameController.GetInstance().m_currentMovesCount)
        {
            currentLevelData.m_movesCount = GameController.GetInstance().m_currentMovesCount;
            currentLevelData.SaveToFile();
        }

        yield return base.Show(timeSpacing);
    }

    public void OnClickSkip()
    {
        GameController.GetInstance().ClearLevel();
        //GameController.GetInstance().StartGameForLevel(GameController.GetInstance().GetLevelManager().m_currentLevel);

        //GameController.GetInstance().GetGUIManager().DismissInterLevelWindow();

        //update the current level
        int nextLevelNumber = GameController.GetInstance().GetLevelManager().m_currentLevel.m_number + 1;      
        GameController.GetInstance().GetComponent<PersistentDataManager>().SetMaxLevelReached(nextLevelNumber); //we reached the next level  

        Level nextLevel = GameController.GetInstance().GetLevelManager().GetNextLevel();
        if (nextLevel != null)
        {
            GameController.GetInstance().GetLevelManager().m_currentLevel = nextLevel;
            InterLevelWindow parentWindow = this.transform.parent.GetComponent<InterLevelWindow>();
            parentWindow.TransitionFromVictoryToIntroContent();
        }
        else
        {

        }
    }
}