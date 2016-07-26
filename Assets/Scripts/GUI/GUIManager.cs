using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public Canvas m_canvas;

    //level editor
    public LevelEditor m_levelEditorPfb;

    //main menu
    public MainMenuGUI m_mainMenuGUIPfb;

    //levels
    public LevelsGUI m_levelsGUIPfb;

    //game
    public GameGUI m_gameGUIPfb;

    //currently displayed GUI
    public BaseGUI m_currentGUI { get; set; }

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
        GameController.GameMode gameMode = GameController.GetInstance().m_gameMode;

        ////Test with level editor if available
        //if (gameMode == GameController.GameMode.LEVEL_EDITOR)
        //{
        //    RectTransform[] childTransforms = m_levelEditor.GetComponentsInChildren<RectTransform>();
        //    for (int i = 0; i != childTransforms.Length; i++)
        //    {
        //        if (m_levelEditor.transform != childTransforms[i] && RectTransformUtility.RectangleContainsScreenPoint(childTransforms[i], pointerLocation))
        //        {
        //            return true;
        //        }
        //    }
        //}
        //else if (gameMode == GameController.GameMode.LEVEL_EDITOR)
        //{

        //}

        RectTransform[] childTransforms = m_currentGUI.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i != childTransforms.Length; i++)
        {
            if (m_currentGUI.transform != childTransforms[i] && RectTransformUtility.RectangleContainsScreenPoint(childTransforms[i], pointerLocation))
            {
                return true;
            }
        }

        return false;
    }

    /**
    * Render the level editor GUI element that contains all menus/windows to easily create a level
    **/
    public void DisplayLevelEditorGUI()
    {
        //build the level editor object
        LevelEditor levelEditor = Instantiate(m_levelEditorPfb);
        levelEditor.name = "LevelEditor";
        levelEditor.transform.SetParent(m_canvas.transform, false);
        levelEditor.Init();

        m_currentGUI = levelEditor;
    }

    public void DisplayMainMenuGUI()
    {
        DestroyCurrentGUI();

        MainMenuGUI mainMenu = Instantiate(m_mainMenuGUIPfb);
        mainMenu.transform.SetParent(m_canvas.transform, false);
        m_currentGUI = mainMenu;

        mainMenu.Show();
    }

    public void DisplayLevelsGUI()
    {
        DestroyCurrentGUI();

        LevelsGUI levels = Instantiate(m_levelsGUIPfb);
        levels.transform.SetParent(m_canvas.transform, false);
        m_currentGUI = levels;

        levels.Show();
    }

    public void DisplayGameGUIForLevel(Level level)
    {
        DestroyCurrentGUI();

        GameGUI game;
        if (m_currentGUI is GameGUI)
        {
            game = (GameGUI)m_currentGUI;
            game.BuildForLevel(level);
        }
        else
        {
            game = Instantiate(m_gameGUIPfb);
            game.transform.SetParent(m_canvas.transform, false);
            game.BuildForLevel(level);
            m_currentGUI = game;
        }

        game.Show();

        //GameGUI game = Instantiate(m_gameGUIPfb);
        //game.transform.SetParent(m_canvas.transform, false);
        //game.BuildForLevel(level);
        //m_currentGUI = game;

        //game.Show();
    }

    public void DismissCurrentGUI()
    {
        m_currentGUI.Dismiss();
    }
    
    private void DestroyCurrentGUI()
    {
        if (m_currentGUI != null)
            Destroy(m_currentGUI.gameObject);
    }
}