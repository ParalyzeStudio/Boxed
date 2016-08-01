using UnityEngine;
using UnityEngine.UI;

public class GameGUI : BaseGUI
{
    public Text m_levelNumber;
    public Level m_level { get; set; }

    //fading overlay
    public GradientBackground m_overlayPfb;
    public GradientBackground m_overlay { get; set; }

    //solution panel
    public Transform m_solutionPanel;
    private bool m_solutionPanelActive;
    public Image m_solutionArrowPfb;

    //actions count
    public Text m_currentActionsCount;
    public Text m_targetActionsCount;

    public void BuildForLevel(Level level)
    {
        m_level = level;
        if (m_overlay == null)
            BuildGradientOverlay();

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

    /**
    * Build a gradient billboard sprite that we put on the near clip plane of the camera to achieve fading effects
    **/
    public void BuildGradientOverlay()
    {
        m_overlay = Instantiate(m_overlayPfb);
        GradientBackground background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        m_overlay.Init(background.m_startColor, background.m_endColor);
        m_overlay.name = "Overlay";

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);

        //set the background at a long distance from camera so it is behind all scene elements
        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.gameObject.transform.position;
        float distanceFromCamera = camera.nearClipPlane + 10;
        m_overlay.GetComponent<QuadAnimator>().SetPosition(cameraPosition + distanceFromCamera * camera.transform.forward);
    }

    public override void Show()
    {
        m_levelNumber.text = m_level.m_number.ToString();
        base.Show();

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(1);
        overlayAnimator.FadeTo(0.0f, 0.5f);
    }

    public override void Dismiss()
    {
        base.Dismiss();
        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);
        overlayAnimator.FadeTo(1.0f, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, false);
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
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(new CallFuncHandler.CallFunc(GameController.GetInstance().RestartLevel), 0.5f);
    }

    public void OnClickSolution()
    {
        ToggleSolution();
    }
}
