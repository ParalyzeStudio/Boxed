using UnityEngine;
using UnityEngine.UI;

public class TestMenu : MonoBehaviour
{
    public Button m_testLevelButton;
    public Button m_displaySolutionsButton;
    private LevelEditor m_parentEditor;

    private bool m_testingLevel;
    private bool m_solutionsDisplayed;

    public void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;

        //disable the solutions button
        m_displaySolutionsButton.gameObject.SetActive(false);

        m_testingLevel = false;
        m_solutionsDisplayed = false;
    }

    public void OnClickTestLevel()
    {
        if (m_testingLevel)
        {
            m_testingLevel = false;

            m_displaySolutionsButton.gameObject.SetActive(false);
            m_testLevelButton.GetComponentInChildren<Text>().text = "TestLevel";
        }
        else
        {
            m_testingLevel = true;

            m_displaySolutionsButton.gameObject.SetActive(true);

            //change the text on the test level button
            m_testLevelButton.GetComponentInChildren<Text>().text = "BackToEditor";
        }
        
        m_parentEditor.OnClickTestLevel(m_testingLevel);
    }

    public void OnClickDisplaySolutions()
    {
        if (m_solutionsDisplayed)
        {
            m_solutionsDisplayed = false;
            m_parentEditor.DismissSolutions();
        }
        else
        {
            m_solutionsDisplayed = true;
            m_parentEditor.DisplaySolutions();
        }
    }
}
