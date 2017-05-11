using UnityEngine;

public class LevelDefeat : GameWindowContent
{
    public void OnClickSkip()
    {
        GameController.GetInstance().ClearLevel();
        GameController.GetInstance().StartGameForLevel(GameController.GetInstance().GetLevelManager().m_currentLevel);

        GameController.GetInstance().GetGUIManager().DismissInterLevelWindow();
    }
}