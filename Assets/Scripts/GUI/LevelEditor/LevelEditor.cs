using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : BaseGUI
{
    public const int FLOOR_DEFAULT_SIZE_FOR_EDITING = 35;

    //public LevelEditorMenuSwitcher m_levelEditorMenuSwitcherPfb;
    public SaveLoadLevelWindow m_saveLoadLevelWindowPfb;
    public ValidationWindow m_validationWindowPfb;
    public PublishWindow m_publishWindowPfb;
    public PublishedLevelsWindow m_publishedLevelsWindowPfb;
    //public TestMenu m_testMenuPfb;
    //public SolutionPanel m_solutionPanelPfb;

    //public LevelEditorMenuSwitcher m_menuSwitcher { get; set; }
    public LevelEditorMainMenu m_mainMenu;
    public TestMenu m_testMenu;
    public SolutionPanel m_solutionPanel;

    public ActionPanel m_activeEditingPanel { get; set; }
    public Level m_editedLevel { get; set; }

    public bool m_isTestMenuShown { get; set; }

    public enum EditingMode
    {
        NONE = 0,
        TILES_EDITING,
        CHECKPOINTS_EDITING,
        SWITCHES_EDITING,
        BONUSES_EDITING
    }

    public EditingMode m_editingMode { get; set; }

    public void Init()
    {
        BuildLevel(null);
        m_mainMenu.m_parentEditor = this;
        ShowMainMenu();
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
            Floor floor = new Floor(FLOOR_DEFAULT_SIZE_FOR_EDITING, FLOOR_DEFAULT_SIZE_FOR_EDITING);
            m_editedLevel = new Level(floor);
            GameController.GetInstance().RenderFloor(floor);
        }
        else
        {
            m_editedLevel = level;

            Floor unclampedFloor = level.m_floor.Unclamp(FLOOR_DEFAULT_SIZE_FOR_EDITING);
            level.m_floor = unclampedFloor;
            GameController.GetInstance().RenderFloor(unclampedFloor);

            //if (level.m_validated)
            //    m_mainMenu.ToggleValidatePublishButtons(false);
            //else
            //    m_mainMenu.ToggleValidatePublishButtons(true);            
        }        
    }

    public void ShowMainMenu()
    {
        m_mainMenu.gameObject.SetActive(true);
        m_mainMenu.InvalidateValidatePublishButtons();
    }

    public void HideMainMenu()
    {
        m_mainMenu.gameObject.SetActive(false);
    }

    public void ShowSaveLoadLevelWindow()
    {
        SaveLoadLevelWindow window = Instantiate(m_saveLoadLevelWindowPfb);
        window.transform.SetParent(this.transform, false);
        window.Init(this);
    }

    public void ShowPublishWindow()
    {
        PublishWindow window = Instantiate(m_publishWindowPfb);
        window.transform.SetParent(this.transform, false);
        window.Init(this);
    }

    public void ShowPublishedLevelsWindow()
    {
        PublishedLevelsWindow window = Instantiate(m_publishedLevelsWindowPfb);
        window.transform.SetParent(this.transform, false);
        window.Init(this);
    }

    public void OnDismissSaveLoadLevelWindow()
    {
        m_editingMode = EditingMode.NONE;
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
        m_isTestMenuShown = true;
        m_testMenu.Init(this);
        m_testMenu.gameObject.SetActive(true);
    }

    public void DismissTestMenu()
    {
        m_isTestMenuShown = false;
        m_testMenu.gameObject.SetActive(false);
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

    public void DisplaySolutions()
    {
        m_solutionPanel.gameObject.SetActive(true);

        Brick.RollDirection[] solution = m_editedLevel.m_solution;

        //Create an instance of a panel to display the arrow images
        //m_solutionPanel = Instantiate(m_solutionPanelPfb);
        //m_solutionPanel.gameObject.name = "SolutionPanel";
        //m_solutionPanel.transform.SetParent(this.transform, false);

        m_solutionPanel.SetSolution(solution);
    }

    public void DismissSolutions()
    {
        m_solutionPanel.gameObject.SetActive(false);
    }
}
