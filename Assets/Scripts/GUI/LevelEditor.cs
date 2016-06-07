using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public GameObject m_levelEditorMenuPfb;
    public SaveLoadLevelWindow m_saveLoadLevelWindowPfb;
    public ValidationWindow m_validationWindowPfb;
    public TestMenu m_testMenuPfb;
    public SolutionPanel m_solutionPanelPfb;

    private LevelEditorMenu m_mainMenu;
    private TestMenu m_testMenu;    

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

            if (level.m_validated)
                m_mainMenu.ToggleValidatePublishButtons(false);
            else
                m_mainMenu.ToggleValidatePublishButtons(true);
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

        m_mainMenu = menu.GetComponent<LevelEditorMenu>();
        m_mainMenu.Init(this);
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

    /**
    * Menu that is displayed at the bottom of the screen when testing a level inside the level editor
    **/
    public void ShowTestMenu()
    {
        m_testMenu = Instantiate(m_testMenuPfb);
        m_testMenu.gameObject.name = "TestMenu";
        m_testMenu.gameObject.transform.SetParent(this.transform, false);
        m_testMenu.Init(this);
    }

    public void DismissTestMenu()
    {
        if (m_testMenu != null)
        {
            Destroy(m_testMenu.gameObject);
            m_testMenu = null;
        }
    }

    public void OnClickTestLevel()
    {
        //Save the currently edited floor
        Floor clampedFloor = m_editedLevel.m_floor.Clamp();

        GameController.GetInstance().RemoveFloor();
        GameController.GetInstance().RemoveBonuses();
        GameController.GetInstance().BuildBonusesHolder();
        GameController.GetInstance().RenderFloor(clampedFloor);
    }

    public void DisplaySolutions(bool displayShortestSolutionOnly = true)
    {
        Brick.RollDirection[][] solutions = m_editedLevel.m_solutions;

        //Create an instance of a panel to display the arrow images
        SolutionPanel solutionPanel = Instantiate(m_solutionPanelPfb);
        solutionPanel.gameObject.name = "SolutionPanel";
        solutionPanel.transform.SetParent(this.transform, false);

        if (!displayShortestSolutionOnly)
        {
            for (int i = 0; i != solutions.GetLength(0); i++)
            {
                solutionPanel.AddSolution(solutions[i]);
            }
        }
        else
        {
            //for the moment just display the shortest one
            int shortestSolutionIndex = 0;
            int shortestSolutionLength = solutions[0].Length;
            for (int i = 1; i != solutions.GetLength(0); i++)
            {
                if (solutions[i].Length < shortestSolutionLength)
                {
                    shortestSolutionIndex = i;
                    shortestSolutionLength = solutions[i].Length;
                }
            }

            solutionPanel.AddSolution(solutions[shortestSolutionIndex]);
        }
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
