using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InterLevelScreen : MonoBehaviour
{
    public GameObject m_circleEmitter; //the empty game object used to emit circles from the finger icon
    public Text m_nextLevelTitle; //text containing the number of the next level to play
    public Text m_defeatText; //text displayed when the player lost the game
    public Text m_touchToBeginText; //text below the finger icon
    public Text m_touchToContinueText; //another text below the finger icon
    public Text m_touchToRetryText; //another text below the finger icon
    public Text m_chapterStatsTitle; //text to sum up chapter statistics

    //public Text m_chapterEndText; //text displayed at the end of the chapter
    private State m_state;
    private enum State
    {
        DISMISSED, //screen is outside the window and invisible
        ENTERING, //screen is currently entering the scene with animation
        SET, //screen is set up
        LEAVING //screen is currently leaving the scene with animation
    }

    private GameController.GameStatus m_gameStatus; //the game status associated to the display of this screen
    private bool m_showingResults; //in case of victory, are we displaying the first part with game results

    private GUIImageAnimator m_animator;

    private const float ANIMATION_DURATION = 1.5f; //fixed duration of the entering/leaving animations

    private void UpdateContent()
    {
        switch (m_gameStatus)
        {
            case GameController.GameStatus.IDLE:
                m_defeatText.gameObject.SetActive(false);
                m_nextLevelTitle.gameObject.SetActive(true);
                UpdateLevelTitle();
                break;
            case GameController.GameStatus.DEFEAT:
                m_defeatText.gameObject.SetActive(true);
                m_nextLevelTitle.gameObject.SetActive(false);
                break;
            case GameController.GameStatus.RETRY:                
                m_defeatText.gameObject.SetActive(false);
                m_nextLevelTitle.gameObject.SetActive(true);
                UpdateLevelTitle();
                break;
            case GameController.GameStatus.VICTORY:
                ShowResults();
                m_defeatText.gameObject.SetActive(false);
                m_nextLevelTitle.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        ActivateCorrectTextForGameStatus();
    }

    public IEnumerator ShowForGameStatus(GameController.GameStatus gameStatus)
    {
        if (m_state == State.DISMISSED)
        {
            m_state = State.ENTERING;

            //update the content before animating it
            m_gameStatus = gameStatus;
            UpdateContent();

            float screenHeight = GetComponent<Image>().rectTransform.rect.height;
            GetAnimator().SyncPositionFromTransform();
            GetAnimator().TranslateBy(new Vector3(0, -screenHeight, 0), ANIMATION_DURATION, 0, ValueAnimator.InterpolationType.HERMITE2);

            yield return new WaitForSeconds(ANIMATION_DURATION);

            OnSetUp();
        }

        yield return null;
    }

    public IEnumerator Dismiss()
    {
        if (m_state == State.SET)
        {
            m_state = State.LEAVING;
            float screenHeight = GetComponent<Image>().rectTransform.rect.height;
            GetAnimator().TranslateBy(new Vector3(0, screenHeight, 0), ANIMATION_DURATION, 0, ValueAnimator.InterpolationType.HERMITE2);

            yield return new WaitForSeconds(ANIMATION_DURATION);

            OnDismissed();
        }

        yield return null;
    }

    /**
    * Called when the screen is set up after the entering animation
    **/
    public void OnSetUp()
    {
        m_state = State.SET;

        GameController.GetInstance().ClearLevel();

        //AdBuddiz
        //AdBuddizBinding.ShowAd();

        //Unity ADS
        //GameController.GetInstance().GetComponent<AdManager>().TryToPlayAd();
    }

    /**
    * Called when the screen is dismissed after the leaving animation
    **/
    public void OnDismissed()
    {
        m_state = State.DISMISSED;
    }

    /**
    * Display the chapter and level to play
    **/
    private void UpdateLevelTitle()
    {
        Level level = GameController.GetInstance().GetLevelManager().m_currentLevel;
        m_nextLevelTitle.text = (level.GetChapterIndex() + 1) + " - " + level.GetChapterRelativeNumber();
    }

    /**
    * When the level is done, show some statistics (collected bonuses, par score)
    **/
    public void ShowResults()
    {
        m_showingResults = true;
    }

    /**
    * Dismiss the previously shown results
    **/
    public void DismissResults()
    {
        m_showingResults = false;
    }

    /**
    * Display a simple message
    **/
    //public void ShowChapterEnd()
    //{
    //    m_chapterEndText.gameObject.SetActive(true);
    //}

    public void OnClickTouchToPlay()
    {
        if (m_state != State.SET)
            return;

        LevelManager levelManager = GameController.GetInstance().GetLevelManager();
        if (m_gameStatus == GameController.GameStatus.VICTORY)
        {
            if (m_showingResults)
            {
                int levelRelativeNumber = levelManager.m_currentLevel.GetChapterRelativeNumber();
                if (levelRelativeNumber < LevelManager.NUM_LEVELS_PER_CHAPTER) //there is a next level in this chapter
                {
                    DismissResults(); //dismiss the current displayed results
                    ActivateCorrectTextForGameStatus(); //change the text below the finger icon slightly
                    levelManager.m_currentLevel = levelManager.GetNextLevel(); //update the current level inside LevelManager
                }
                else
                {
                    //TODO else fade out the 'touch to continue' text and replace it with 'go on next chapter'
                    //TODO this button leads to the Levels screen
                    DisplayChapterSummary();
                }
            }
        }

        GameController.GetInstance().ClearLevel();
        GameController.GetInstance().StartGameForLevel(levelManager.m_currentLevel);

        StartCoroutine("Dismiss");
    }

    /**
    * Select the correct text
    **/
    private void ActivateCorrectTextForGameStatus()
    {
        switch (m_gameStatus)
        {
            case GameController.GameStatus.IDLE:
                m_touchToRetryText.gameObject.SetActive(false);
                m_touchToBeginText.gameObject.SetActive(true);
                m_touchToContinueText.gameObject.SetActive(false);
                break;
            case GameController.GameStatus.DEFEAT:
                m_touchToRetryText.gameObject.SetActive(true);
                m_touchToBeginText.gameObject.SetActive(false);
                m_touchToContinueText.gameObject.SetActive(false);
                break;
            case GameController.GameStatus.RETRY:
                m_touchToRetryText.gameObject.SetActive(false);
                m_touchToBeginText.gameObject.SetActive(true);
                m_touchToContinueText.gameObject.SetActive(false);
                break;
            case GameController.GameStatus.VICTORY:
                if (m_showingResults)
                {
                    m_touchToRetryText.gameObject.SetActive(false);
                    m_touchToBeginText.gameObject.SetActive(false);
                    m_touchToContinueText.gameObject.SetActive(true);
                }
                else
                {
                    m_touchToRetryText.gameObject.SetActive(false);
                    m_touchToBeginText.gameObject.SetActive(true);
                    m_touchToContinueText.gameObject.SetActive(false);
                }
                break;
        }
    }

    public void DisplayChapterSummary()
    {
        m_chapterStatsTitle.gameObject.SetActive(true);

        m_touchToRetryText.gameObject.SetActive(false);
        m_touchToBeginText.gameObject.SetActive(false);
        m_touchToContinueText.gameObject.SetActive(true);
    }

    private GUIImageAnimator GetAnimator()
    {
        if (m_animator == null)
            m_animator = this.GetComponent<GUIImageAnimator>();

        return m_animator;
    }
}
