using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public GameObject m_levelEditorMenuPfb;
    public SaveLoadLevelWindow m_saveLoadLevelWindowPfb;
    public ValidationWindow m_validationWindowPfb;
    public Button m_testLevelBtnPfb;
    private Button m_testLevelBtn;

    public Level m_editedLevel { get; set; }
    private Level m_testLevel; //the level that is being test after clicking on the Test Level button

    private static LevelEditor s_instance;

    public static LevelEditor GetInstance()
    {
        if (s_instance == null)
            s_instance = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();

        return s_instance;
    }

    public enum EditingMode
    {
        NONE = 0,
        TILES_EDITING,
        CHECKPOINTS_EDITING,
        BONUSES_EDITING,
        SAVING_LOADING_LEVEL, //a window is displayed to save or load a level from file  
    }

    public EditingMode m_editingMode { get; set; }

    private bool m_guiProcessedClick; //variable used to know if the Level Editor GUI has processed the click event

    public void Init()
    {
        BuildMainMenu();
        BuildLevel(null);
    }

    /**
    * Create a level to be edited by the user.
    * Passing null will load a default empty level
    **/
    public void BuildLevel(Level level)
    {
        GameController.GetInstance().BuildBonusesHolder();

        //Create the level that will be edited
        if (level == null)
        {
            Floor floor = new Floor(50, 50);
            m_editedLevel = new Level(floor);
            GameController.GetInstance().RenderFloor(floor);
        }
        else
        {
            m_editedLevel = level;
            GameController.GetInstance().RenderFloor(level.m_floor);
        }        
    }

    private void DisplayLevel(Level level)
    {
        GameController.GetInstance().ClearLevel();
        //GameController.GetInstance().StartLevel(level);
    }

    public void BuildMainMenu()
    {
        GameObject menu = (GameObject)Instantiate(m_levelEditorMenuPfb);
        menu.name = "MainMenu";
        menu.transform.SetParent(this.transform, false);

        menu.GetComponent<LevelEditorMenu>().Init(this);
    }

    public void ShowSaveLoadLevelWindow()
    {
        m_guiProcessedClick = true;
        m_editingMode = EditingMode.SAVING_LOADING_LEVEL;

        SaveLoadLevelWindow window = Instantiate(m_saveLoadLevelWindowPfb);
        window.transform.SetParent(this.transform, false);
        window.Init(this);
    }

    public void OnDismissSaveLoadLevelWindow()
    {
        m_editingMode = EditingMode.NONE;
    }

    public bool IsSaveLevelWindowActive()
    {
        return m_editingMode == EditingMode.SAVING_LOADING_LEVEL;
    }
    
    /**
    * This function is called after the user decided to validate the current level and after the Validate() function has returned
    **/
    public void DisplayLevelValidationOutput(Level.ValidationData output)
    {
        ValidationWindow validationWindow = Instantiate(m_validationWindowPfb);
        validationWindow.name = "ValidationWindow";
        validationWindow.transform.SetParent(GameController.GetInstance().m_canvas.transform, false);
        validationWindow.Populate(output);
    }

    public void ShowTestLevelButton()
    {
        m_testLevelBtn = Instantiate(m_testLevelBtnPfb);
        m_testLevelBtn.transform.SetParent(this.transform, false);

        m_testLevelBtn.onClick.AddListener(this.OnClickTestLevel);
    }

    public void DismissTestLevelButton()
    {
        Destroy(m_testLevelBtn.gameObject);
    }

    public void OnClickTestLevel()
    {
        //Save the currently edited floor
        Floor clampedFloor = m_editedLevel.m_floor.Clamp();

        GameController.GetInstance().RemoveFloor();
        GameController.GetInstance().RenderFloor(clampedFloor);
    }

    public void ProcessClick()
    {
        m_guiProcessedClick = true;
    }

    public bool GUIProcessedClick()
    {
        if (m_guiProcessedClick)
        {
            m_guiProcessedClick = false;
            return true;
        }

        return false;
    }
}
