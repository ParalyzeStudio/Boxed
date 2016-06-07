using UnityEngine;
using UnityEngine.UI;

public class TestMenu : MonoBehaviour
{
    public Button m_displaySolutionsButton;
    private LevelEditor m_parentEditor;

    public void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;

        //disable the solutions button
        m_displaySolutionsButton.gameObject.SetActive(false);
    }

    public void OnClickTestLevel()
    {
        m_displaySolutionsButton.gameObject.SetActive(true);
        m_parentEditor.OnClickTestLevel();
    }

    public void OnClickDisplaySolutions()
    {
        Debug.Log("OnClickDisplaySolutions");
        m_parentEditor.DisplaySolutions(false);
    }
}
