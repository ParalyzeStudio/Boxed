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

    //actions count
    public Text m_currentActionsCount;
    public Text m_targetActionsCount;

    //confirm home window
    public ConfirmHomeWindow m_confirmHomeWindowPfb;
    public ConfirmHomeWindow m_confirmHomeWindow { get; set; }

    public void BuildForLevel(Level level)
    {
        m_level = level;
        //if (m_overlay == null)
        //    BuildGradientOverlay();

        //disable any displayed solution panel
        m_solutionPanelActive = false;
        m_solutionPanel.gameObject.SetActive(false);

        //clear any solution on previous level and build the new one
        ClearSolution(); 
        BuildSolution();

        //actions count
        InitTargetActionsCount();
        UpdateActionsCount();
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
        m_targetActionsCount.text = currentLevel.m_solution.Length.ToString();
        m_targetActionsCount.color = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme.m_highScoreColor;
    }

    public void UpdateActionsCount()
    {
        m_currentActionsCount.text = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevelData.m_currentActionsCount.ToString();
    }

    private Quaternion GetArrowRotationForDirection(Brick.RollDirection direction)
    {
        if (direction == Brick.RollDirection.LEFT)
            return Quaternion.AngleAxis(180, Vector3.forward);
        else if (direction == Brick.RollDirection.TOP)
            return Quaternion.AngleAxis(90, Vector3.forward);
        else if (direction == Brick.RollDirection.RIGHT)
            return Quaternion.identity;
        else
            return Quaternion.AngleAxis(270, Vector3.forward);
    }

    public void OnClickRestart()
    {
        Dismiss();
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(GameController.GetInstance().RestartLevel, 0.5f);
    }

    public void OnClickHome()
    {
        m_confirmHomeWindow = Instantiate(m_confirmHomeWindowPfb);
        m_confirmHomeWindow.transform.SetParent(GameController.GetInstance().GetComponent<GUIManager>().m_canvas.transform, false);
    }

    public void OnClickSolution()
    {
        ToggleSolution();
    }

    public void OnClickRollLeft()
    {
        GameController.GetInstance().m_brick.GetComponent<BrickController>().RollLeft();
    }

    public void OnClickRollRight()
    {
        GameController.GetInstance().m_brick.GetComponent<BrickController>().RollRight();
    }

    public void OnClickRollTop()
    {
        GameController.GetInstance().m_brick.GetComponent<BrickController>().RollTop();
    }

    public void OnClickRollBottom()
    {
        GameController.GetInstance().m_brick.GetComponent<BrickController>().RollBottom();
    }
}
