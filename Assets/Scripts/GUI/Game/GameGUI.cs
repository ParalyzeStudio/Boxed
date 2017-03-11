using UnityEngine;
using UnityEngine.UI;

public class GameGUI : BaseGUI
{
    public Text m_levelNumber;
    public Level m_level { get; set; }
    
    //solution panel
    public Transform m_solutionPanel;
    private bool m_solutionPanelActive;
    public Image m_solutionArrowPfb;

    //PAR score
    public Image m_parScoreFill;
    public Text m_currentActionsCount;
    public Text m_targetActionsCount;
    private int m_solutionMinLength;

    //confirm home window
    public ConfirmHomeWindow m_confirmHomeWindowPfb;
    public ConfirmHomeWindow m_confirmHomeWindow { get; set; }

    //Tutorials
    public Tutorial[] m_tutorials;
    private int m_currentTutorialNumber; //the tutorial currently shown

    //inter-level screeen
    public GUIImageAnimator m_interLevelScreen;
    private bool m_interLevelScreenShown;

    public void BuildForLevel(Level level)
    {
        m_level = level;

        //disable any displayed solution panel
        m_solutionPanelActive = false;
        m_solutionPanel.gameObject.SetActive(false);

        //clear any solution on previous level and build the new one
        ClearSolution(); 
        BuildSolution();

        //actions count
        InitTargetActionsCount();
        UpdateParScore();

        m_interLevelScreenShown = false;
    }
    

    public override void Show()
    {
        m_levelNumber.text = m_level.m_number.ToString();
        base.Show();
    }

    public override void Dismiss(bool bDestroyOnFinish = false)
    {
        base.Dismiss(bDestroyOnFinish);
    }

    public void BuildSolution()
    {
        Level currentLevel = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevel;
        Brick.RollDirection[] solution = currentLevel.m_solution;
        int numArrows = solution.Length;
        int maxArrowsPerLine = 20;
        int numLines = (numArrows / maxArrowsPerLine) + 1;
        int numArrowsPerLine = numArrows / numLines;
        int numArrowsOnFirstLine = (numLines > 1) ? numArrows - ((numLines - 1) * numArrowsPerLine) : numArrowsPerLine;
        
        for (int i = 0; i != numLines; i++)
        {
            int arrowsCount = (i == 0) ? numArrowsOnFirstLine : numArrowsPerLine;

            //Create a line
            GameObject line = new GameObject();
            line.name = "Line";
            HorizontalLayoutGroup hlg = line.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            line.transform.SetParent(m_solutionPanel, false);

            for (int j = 0; j != arrowsCount; j++)
            {
                Image solutionArrow = GameObject.Instantiate(m_solutionArrowPfb);
                int arrowIdx;
                if (i == 0)
                    arrowIdx = j;
                else
                    arrowIdx = numArrowsOnFirstLine + (i - 1) * numArrowsPerLine + j;
                solutionArrow.transform.rotation = GetArrowRotationForDirection(solution[arrowIdx]);
                solutionArrow.transform.SetParent(line.transform, false);
            }
        }
    }

    public void ClearSolution()
    {
        HorizontalLayoutGroup[] lines = m_solutionPanel.GetComponentsInChildren<HorizontalLayoutGroup>();
        for (int i = 0; i != lines.Length; i++)
        {
            Destroy(lines[i].gameObject);
        }
    }

    private void ToggleSolution()
    {        
        m_solutionPanel.gameObject.SetActive(!m_solutionPanelActive);
        m_solutionPanelActive = !m_solutionPanelActive;
    }

    private void InitTargetActionsCount()
    {
        Level currentLevel = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevel;
        m_solutionMinLength = currentLevel.m_solution.Length;
        m_targetActionsCount.text = m_solutionMinLength.ToString();
    }

    public void UpdateParScore()
    {
        LevelData levelData = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevelData;
        float scoreRatio = Mathf.Clamp01(levelData.m_currentActionsCount / (float)m_solutionMinLength);
        m_parScoreFill.fillAmount = scoreRatio;

        m_currentActionsCount.text = levelData.m_currentActionsCount.ToString();
    }

    private Quaternion GetArrowRotationForDirection(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.BOTTOM_LEFT)
            return Quaternion.AngleAxis(180, Vector3.forward);
        else if (direction == Brick.RollDirection.TOP_LEFT)
            return Quaternion.AngleAxis(90, Vector3.forward);
        else if (direction == Brick.RollDirection.TOP_RIGHT)
            return Quaternion.identity;
        else
            return Quaternion.AngleAxis(270, Vector3.forward);
    }

    /**
    * Show the first tutorial available for this level
    **/
    public bool ShowFirstTutorial()
    {
        int tutorialIndex = 0;
        while (!ShowTutorial(m_tutorials[tutorialIndex]) && tutorialIndex < m_tutorials.Length)
        {
            tutorialIndex++;
            if (tutorialIndex == m_tutorials.Length)
                return false;
        }

        m_currentTutorialNumber = tutorialIndex + 1;
        return true;
    }

    /**
    * Show the next tutorial if available
    **/
    public bool ShowNextTutorial()
    {
        if (m_currentTutorialNumber < m_tutorials.Length)
            return ShowTutorial(m_tutorials[m_currentTutorialNumber]);

        return false;
    }

    public bool ShowTutorial(Tutorial tutorial)
    {
        //check if this tutorial matches the current level
        int currentLevelNumber = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevel.m_number;
        if (tutorial.m_levelNumber == currentLevelNumber)
        {
            //PersistentDataManager persistentDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
            //if (currentLevelNumber > persistentDataManager.GetMaxLevelReached())
            //{
                Tutorial tutorialCopy = Instantiate(tutorial); //cannot use the prefab directly because of Unity denial
                tutorialCopy.transform.SetParent(this.transform, false);
            //}

            return true;
        }

        return false;
    }

    public void ShowInterLevelScreen()
    {
        if (!m_interLevelScreenShown)
        {
            m_interLevelScreenShown = true;

            float screenHeight = m_interLevelScreen.GetComponent<Image>().rectTransform.rect.height;
            Debug.Log(m_interLevelScreen.GetComponent<Image>().rectTransform.localPosition);
            m_interLevelScreen.SyncPositionFromTransform();
            m_interLevelScreen.TranslateBy(new Vector3(0, -screenHeight, 0), 1.5f, 0, ValueAnimator.InterpolationType.HERMITE2);
        }
    }

    public void DismissInterLevelScreen()
    {
        if (m_interLevelScreenShown)
        {
            m_interLevelScreenShown = false;

            float screenHeight = m_interLevelScreen.GetComponent<Image>().rectTransform.rect.height;
            m_interLevelScreen.TranslateBy(new Vector3(0, screenHeight, 0), 1.5f, 0, ValueAnimator.InterpolationType.HERMITE2);
        }
    }

    public void OnClickRestart()
    {
        Dismiss();
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(GameController.GetInstance().RestartLevel, 0.5f);
    }

    public void OnClickHome()
    {
        //m_confirmHomeWindow = Instantiate(m_confirmHomeWindowPfb);
        //m_confirmHomeWindow.transform.SetParent(GameController.GetInstance().GetComponent<GUIManager>().m_canvas.transform, false);

        if (!m_interLevelScreenShown)
            ShowInterLevelScreen();
        else
            DismissInterLevelScreen();
    }

    public void OnClickSolution()
    {
        ToggleSolution();
    }
}
