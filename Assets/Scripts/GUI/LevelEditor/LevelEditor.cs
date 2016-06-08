using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public LevelEditorMenuSwitcher m_levelEditorMenuSwitcherPfb;
    public SaveLoadLevelWindow m_saveLoadLevelWindowPfb;
    public ValidationWindow m_validationWindowPfb;
    public TestMenu m_testMenuPfb;
    public SolutionPanel m_solutionPanelPfb;

    public LevelEditorMenuSwitcher m_menuSwitcher { get; set; }
    private TestMenu m_testMenu;
    private SolutionPanel m_solutionPanel;  

    public Level m_editedLevel { get; set; }

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
                m_menuSwitcher.GetMainMenu().ToggleValidatePublishButtons(false);
            else
                m_menuSwitcher.GetMainMenu().ToggleValidatePublishButtons(true);
        }        
    }

    public void BuildMainMenu()
    {
        m_menuSwitcher = Instantiate(m_levelEditorMenuSwitcherPfb);
        m_menuSwitcher.name = "MainMenu";
        m_menuSwitcher.transform.SetParent(this.transform, false);
        m_menuSwitcher.Init(this);
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
        validationWindow.transform.SetParent(GameController.GetInstance().GetGUIManager().m_canvas.transform, false);
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

    public void OnClickTestLevel(bool isEditingLevel)
    {
        GameController.GetInstance().ClearLevel();

        if (isEditingLevel)
        {
            Floor clampedFloor = m_editedLevel.m_floor.Clamp();
            Level testLevel = new Level(clampedFloor);
            GameController.GetInstance().StartLevel(testLevel);
        }
        else
        {
            //GameController.GetInstance().StartLevel(m_editedLevel);
            GameController.GetInstance().ClearLevel();
            BuildLevel(m_editedLevel);
            DismissSolutions();
        }
    }

    public void DisplaySolutions(bool displayShortestSolutionOnly = true)
    {
        Brick.RollDirection[][] solutions = m_editedLevel.m_solutions;

        //Create an instance of a panel to display the arrow images
        m_solutionPanel = Instantiate(m_solutionPanelPfb);
        m_solutionPanel.gameObject.name = "SolutionPanel";
        m_solutionPanel.transform.SetParent(this.transform, false);

        if (!displayShortestSolutionOnly)
        {
            for (int i = 0; i != solutions.GetLength(0); i++)
            {
                m_solutionPanel.AddSolution(solutions[i]);
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

            m_solutionPanel.AddSolution(solutions[shortestSolutionIndex]);
        }
    }

    public void DismissSolutions()
    {
        if (m_solutionPanel != null)
        {
            Destroy(m_solutionPanel.gameObject);
            m_solutionPanel = null;
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
