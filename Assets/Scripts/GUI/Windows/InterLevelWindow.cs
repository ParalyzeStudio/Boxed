using UnityEngine;

public class InterLevelWindow : GameWindow
{
    public LevelIntro m_levelIntro;
    public LevelVictory m_levelVictory;
    public LevelDefeat m_levelDefeat;

    public bool ShowForStatus(GameController.GameStatus gameStatus)
    {
        GameWindowContent content;
        //select the right content for the parameter 'gameStatus'
        switch (gameStatus)
        {
            case GameController.GameStatus.IDLE:
                content = m_levelIntro;
                break;
            case GameController.GameStatus.VICTORY:
                content = m_levelVictory;
                break;
            case GameController.GameStatus.DEFEAT:
                content = m_levelDefeat;
                break;
            case GameController.GameStatus.RETRY:
                content = m_levelIntro;
                break;
            default:
                content = m_levelIntro;
                break;
        }

        ThemeManager.Theme currentTheme = GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme();
        m_backgroundTopColor = currentTheme.m_backgroundGradientTopColor;
        m_backgroundBottomColor = currentTheme.m_backgroundGradientBottomColor;

        return base.ShowContent(content);
    }

    public void TransitionFromVictoryToIntroContent()
    {
        Destroy(m_content.gameObject, BACKGROUND_FADE_DURATION);
        StartCoroutine(DismissCurrentContent());
        StartCoroutine(ShowContentAfterDelay(m_levelIntro, BACKGROUND_FADE_DURATION));
    }
}
