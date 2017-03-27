using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    //end screen
    public EndScreenGUI m_endScreenGUIPfb;
    
    //inter-level screeen
    public InterLevelScreen m_interLevelScreenPfb;
    private InterLevelScreen m_interLevelScreen;

    //currently displayed GUI
    public BaseGUI m_currentGUI { get; set; }

    //themes
    //public ThemeManager m_themeManager;

    //gradient background
    public FSGradientBillboardQuad m_gradientQuadPfb;
    public FSGradientBillboardQuad m_background { get; set; }
    public FSGradientBillboardQuad m_overlay { get; set; }

    public void Init()
    {
        //m_themeManager = new ThemeManager();
        //m_themeManager.Init();
        
        ShowBackgroundForTheme(GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme());

        //if (m_overlay == null)
        //    BuildGradientOverlay();
    }

    public void ShowBackgroundForTheme(ThemeManager.Theme theme)
    {
        //show gradient background
        m_background = Instantiate(m_gradientQuadPfb);
        Color startColor = theme.m_backgroundGradientTopColor;
        Color endColor = theme.m_backgroundGradientBottomColor;

        m_background.Init(startColor, endColor);

        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.gameObject.transform.position;
        float distanceFromCamera = camera.farClipPlane - 1;
        m_background.GetComponent<GameObjectAnimator>().SetPosition(cameraPosition + distanceFromCamera * camera.transform.forward);
    }

    /**
    * Build a gradient billboard sprite that we put on the near clip plane of the camera to achieve fading effects
    **/
    public void BuildGradientOverlay()
    {
        m_overlay = Instantiate(m_gradientQuadPfb);
        FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        m_overlay.Init(background.m_topColor, background.m_bottomColor);
        m_overlay.name = "Overlay";

        m_overlay.m_topColor = new Color(m_overlay.m_topColor.r, m_overlay.m_topColor.g, m_overlay.m_topColor.b, 1);
        m_overlay.m_bottomColor = new Color(m_overlay.m_bottomColor.r, m_overlay.m_bottomColor.g, m_overlay.m_bottomColor.b, 1);
        m_overlay.InvalidateColors();

        //set the overlay at a short distance from camera so it is in front of all scene elements
        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.gameObject.transform.position;
        float distanceFromCamera = camera.nearClipPlane + 1;
        m_overlay.GetComponent<GameObjectAnimator>().SetPosition(cameraPosition + distanceFromCamera * camera.transform.forward);
    }

    /**
    * Tells if one of the GUI elements currently displayed on the screen intercepts the pointer event at location 'pointerLocation'
    **/
    public bool EventProcessedByGUI(Vector3 pointerLocation, TouchManager.PointerEventType eventType)
    {
        GameController.GameMode gameMode = GameController.GetInstance().m_gameMode;

        if (gameMode == GameController.GameMode.GAME)
        {
            GameGUI gameGUI = (GameGUI)m_currentGUI;
            if (gameGUI.m_confirmHomeWindow != null)
                return true;
        }

        Button[] childButtons = m_canvas.GetComponentsInChildren<Button>();
        for (int i = 0; i != childButtons.Length; i++)
        {
            RectTransform tf = childButtons[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(tf, pointerLocation))
            {
                return true;
            }
        }

        //Transform[] childTransforms = m_canvas.GetComponentsInChildren<Transform>();
        //for (int i = 0; i != childTransforms.Length; i++)
        //{
        //    RectTransform tf = childTransforms[i].GetComponent<RectTransform>();
        //    if (tf == m_currentGUI.transform || tf == m_canvas.transform)
        //        continue;
        //    if (RectTransformUtility.RectangleContainsScreenPoint(tf, pointerLocation))
        //    {
        //        return true;
        //    }
        //}

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

        m_currentGUI = levelEditor;
        levelEditor.Init();
        levelEditor.Show();        
    }

    public void DisplayMainMenuGUI()
    {
        //DestroyCurrentGUI();

        MainMenuGUI mainMenu = Instantiate(m_mainMenuGUIPfb);
        mainMenu.transform.SetParent(m_canvas.transform, false);
        m_currentGUI = mainMenu;

        mainMenu.Show();
    }

    public void DisplayLevelsGUI()
    {
        //DestroyCurrentGUI();

        LevelsGUI levels = Instantiate(m_levelsGUIPfb);
        levels.transform.SetParent(m_canvas.transform, false);
        m_currentGUI = levels;

        levels.Show();
    }

    public void DisplayGameGUIForLevel(Level level)
    {
        GameGUI gameGUI;
        if (m_currentGUI is GameGUI)
        {
            gameGUI = (GameGUI)m_currentGUI;
            gameGUI.BuildForLevel(level);
        }
        else
        {
            DestroyCurrentGUI();

            gameGUI = Instantiate(m_gameGUIPfb);
            gameGUI.transform.SetParent(m_canvas.transform, false);
            gameGUI.BuildForLevel(level);
            m_currentGUI = gameGUI;

            //Reorder interlevel screen in front
            if (m_interLevelScreen != null)
                m_interLevelScreen.transform.SetAsLastSibling();
        }

        gameGUI.Show();

        //GameGUI game = Instantiate(m_gameGUIPfb);
        //game.transform.SetParent(m_canvas.transform, false);
        //game.BuildForLevel(level);
        //m_currentGUI = game;

        //game.Show();
    }

    public void DisplayEndScreenGUI()
    {
        EndScreenGUI endScreen = Instantiate(m_endScreenGUIPfb);
        endScreen.transform.SetParent(m_canvas.transform, false);
        m_currentGUI = endScreen;

        endScreen.Show();
    }

    public void DismissCurrentGUI()
    {
        m_currentGUI.Dismiss();
    }
    
    public void ShowInterLevelScreen(GameController.GameStatus gameStatus)
    {
        if (m_interLevelScreen == null)
            m_interLevelScreen = Instantiate(m_interLevelScreenPfb);

        m_interLevelScreen.transform.SetAsLastSibling();

        m_interLevelScreen.transform.SetParent(m_canvas.transform, false);

        IEnumerator showCoroutine = m_interLevelScreen.ShowForGameStatus(gameStatus);
        StartCoroutine(showCoroutine);
    }

    public void DismissInterLevelScreen()
    {
        IEnumerator dismissCoroutine = m_interLevelScreen.Dismiss();
        StartCoroutine(dismissCoroutine);
    }

    public void DestroyInterLevelScreen()
    {
        if (m_interLevelScreen != null)
            Destroy(m_interLevelScreen.gameObject);
    }

    public void DestroyCurrentGUI()
    {
        if (m_currentGUI != null)
        {
            Destroy(m_currentGUI.gameObject);
            m_currentGUI = null;
        }
    }
}