using UnityEngine;
using UnityEngine.UI;

public class SolutionReveal : GameWindowElement
{
    public GameObject m_solutionLineArrows;
    public GameObject m_solutionLineSquares;
    private bool m_solutionDisplayUseSquares = true;

    private const int SOLUTION_MAX_ICONS_PER_LINE = 15;

    public override void Show()
    {
        BuildSolution();

        base.Show();
    }

    private void BuildSolution()
    {
        Brick.RollDirection[] solution = GameController.GetInstance().GetLevelManager().m_currentLevel.m_solution;

        GameObject arrowsHolder = m_solutionLineArrows.transform.parent.gameObject;
        GameObject squaresHolder = m_solutionLineSquares.transform.parent.gameObject;
        arrowsHolder.gameObject.SetActive(!m_solutionDisplayUseSquares);
        squaresHolder.gameObject.SetActive(m_solutionDisplayUseSquares);

        Image arrowIcon = m_solutionLineArrows.GetComponentInChildren<Image>();
        Image squaresIcon = m_solutionLineSquares.GetComponentInChildren<Image>();
        arrowIcon.gameObject.SetActive(false);
        squaresIcon.gameObject.SetActive(false);

        int numLines = solution.Length / SOLUTION_MAX_ICONS_PER_LINE + 1;
        int iconCountPerLine = solution.Length / numLines;
        int lastLineIconCount = solution.Length - (numLines - 1) * iconCountPerLine;

        GameObject emptyLine = Instantiate(m_solutionLineSquares);
        for (int i = 0; i != numLines; i++)
        {
            GameObject line;
            if (i == 0)
                line = m_solutionLineArrows;
            else
            {
                line = Instantiate(emptyLine);
                line.transform.SetParent(m_solutionLineArrows.transform.parent, false);
            }

            int iconCount = (i == numLines - 1 ? lastLineIconCount : iconCountPerLine);
            for (int j = 0; j != iconCount; j++)
            {
                int iconIndex = i * iconCountPerLine + j;
                Image icon = GetSolutionIconForDirection(arrowIcon, solution[iconIndex]);

                icon.transform.SetParent(line.transform, false);
            }
        }

        emptyLine = Instantiate(m_solutionLineSquares);
        for (int i = 0; i != numLines; i++)
        {
            GameObject line;
            if (i == 0)
                line = m_solutionLineSquares;
            else
            {
                line = Instantiate(emptyLine);
                line.transform.SetParent(m_solutionLineSquares.transform.parent, false);
            }
            for (int j = 0; j != (i == numLines - 1 ? lastLineIconCount : iconCountPerLine); j++)
            {
                int iconIndex = i * iconCountPerLine + j;
                Image icon = GetSolutionIconForDirection(squaresIcon, solution[iconIndex]);

                icon.transform.SetParent(line.transform, false);
            }
        }
    }

    private Image GetSolutionIconForDirection(Image iconPfb, Brick.RollDirection direction)
    {
        Image icon = Instantiate(iconPfb);
        icon.gameObject.SetActive(true);
        RectTransform iconTransform = icon.GetComponent<RectTransform>();

        switch (direction)
        {
            case Brick.RollDirection.TOP_RIGHT:
                iconTransform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Brick.RollDirection.BOTTOM_LEFT:
                iconTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Brick.RollDirection.BOTTOM_RIGHT:
                iconTransform.rotation = Quaternion.Euler(0, 0, 180);
                break;
        }

        return icon;
    }

    public void OnClickSwitchDisplay()
    {
        m_solutionDisplayUseSquares = !m_solutionDisplayUseSquares;

        //duplicate the lines holder
        GameWindowElement squaresHolder = m_solutionLineSquares.transform.parent.GetComponent<GameWindowElement>();
        GameWindowElement arrowsHolder = m_solutionLineArrows.transform.parent.GetComponent<GameWindowElement>();

        //deactivate it
        if (m_solutionDisplayUseSquares)
        {
            squaresHolder.gameObject.SetActive(true);
            arrowsHolder.gameObject.SetActive(false);
        }
        else
        {
            squaresHolder.gameObject.SetActive(false);
            arrowsHolder.gameObject.SetActive(true);
        }
    }

    public void OnClickCloseSolution()
    {
        PauseMenu parentMenu = this.transform.parent.GetComponent<PauseMenu>();
        parentMenu.DismissDarkBackground();

        Dismiss(0.4f);
        StartCoroutine(ResetPositionAfterDelay(0.4f));
        Destroy(this.gameObject, 0.4f);
    }
}
