using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelIntro : GameWindowContent
{
    public Text m_levelNumber;

    public override IEnumerator Show(float timeSpacing = DEFAULT_TIME_SPACING)
    {
        Level level = GameController.GetInstance().GetLevelManager().m_currentLevel;
        m_levelNumber.text = (level.GetChapterIndex() + 1) + " - " + level.GetChapterRelativeNumber();

        yield return base.Show(timeSpacing);
    }

    public void OnClickSkip()
    {
        GameController.GetInstance().ClearLevel();
        GameController.GetInstance().StartGameForLevel(GameController.GetInstance().GetLevelManager().m_currentLevel);

        GameController.GetInstance().GetGUIManager().DismissInterLevelWindow();
    }
}