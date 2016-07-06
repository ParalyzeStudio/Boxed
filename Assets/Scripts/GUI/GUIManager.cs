using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public Canvas m_canvas;

    //level editor
    public LevelEditor m_levelEditorPfb;
    public LevelEditor m_levelEditor { get; set; }

    //main menu
    public MainMenuGUI m_mainMenuGUIPfb;

    //levels
    public LevelsGUI m_levelsGUIPfb;

    //themes
    public ColorThemes m_themes;

    //gradient background
    public GradientBackground m_gradientBackgroundPfb;
    public GradientBackground m_background { get; set; }

    public void Init()
    {
        m_themes = new ColorThemes();
        m_themes.Init();

        ShowBackgroundForTheme(m_themes.Themes[0]);    
    }

    public void ShowBackgroundForTheme(ColorTheme theme)
    {
        //show gradient background
        m_background = Instantiate(m_gradientBackgroundPfb);
        Color startColor = theme.m_backgroundGradientTopColor;
        Color endColor = theme.m_backgroundGradientBottomColor;
        m_background.Init(startColor, endColor);
    }

    /**
    * Tells if one of the GUI elements currently displayed on the screen intercepts the pointer event at location 'pointerLocation'
    **/
    public bool ProcessPointerEvent(Vector3 pointerLocation)
    {
        //Test with level editor if available
        if (m_levelEditor != null)
        {
            RectTransform[] childTransforms = m_levelEditor.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i != childTransforms.Length; i++)
            {
                if (m_levelEditor.transform != childTransforms[i] && RectTransformUtility.RectangleContainsScreenPoint(childTransforms[i], pointerLocation))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /**
    * Render the level editor GUI element that contains all menus/windows to easily create a level
    **/
    public void DisplayLevelEditor()
    {
        //build the level editor object
        m_levelEditor = Instantiate(m_levelEditorPfb);
        m_levelEditor.name = "LevelEditor";
        m_levelEditor.transform.SetParent(m_canvas.transform, false);
        m_levelEditor.Init();
    }

    public void DisplayMainMenu()
    {
        MainMenuGUI mainMenu = Instantiate(m_mainMenuGUIPfb);
        mainMenu.transform.SetParent(m_canvas.transform, false);
    }

    public void DisplayLevels()
    {
        LevelsGUI levels = Instantiate(m_levelsGUIPfb);
        levels.transform.SetParent(m_canvas.transform, false);
    }
}

