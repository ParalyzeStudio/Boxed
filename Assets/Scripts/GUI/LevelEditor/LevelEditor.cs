using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public const int FLOOR_DEFAULT_SIZE_FOR_EDITING = 20;

    public LevelEditorMenuSwitcher m_levelEditorMenuSwitcherPfb;
    public SaveLoadLevelWindow m_saveLoadLevelWindowPfb;
    public ValidationWindow m_validationWindowPfb;
    public PublishWindow m_publishWindowPfb;
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
        BONUSES_EDITING
    }

    public EditingMode m_editingMode { get; set; }

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
            Floor floor = new Floor(FLOOR_DEFAULT_SIZE_FOR_EDITING, FLOOR_DEFAULT_SIZE_FOR_EDITING);
            m_editedLevel = new Level(floor);
            GameController.GetInstance().RenderFloor(floor);
        }
        else
        {
            Floor unclampedFloor = level.m_floor.Unclamp(FLOOR_DEFAULT_SIZE_FOR_EDITING);
            level.m_floor = unclampedFloor;
            GameController.GetInstance().RenderFloor(unclampedFloor);

            if (level.m_validated)
                m_menuSwitcher.GetMainMenu().ToggleValidatePublishButtons(false);
            else
                m_menuSwitcher.GetMainMenu().ToggleValidatePublishButtons(true);

            m_editedLevel = level;
        }        
    }

    public void BuildMainMenu()
    {
        m_menuSwitcher = Instantiate(m_levelEditorMenuSwitcherPfb);
        m_menuSwitcher.name = "MenuSwitcher";
        m_menuSwitcher.transform.SetParent(this.transform, false);
        m_menuSwitcher.Init(this);
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
        if (m_testMenu != null)
            return;

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

    public void DisplaySolutions()
    {
        Brick.RollDirection[] solution = m_editedLevel.m_solution;

        //Create an instance of a panel to display the arrow images
        m_solutionPanel = Instantiate(m_solutionPanelPfb);
        m_solutionPanel.gameObject.name = "SolutionPanel";
        m_solutionPanel.transform.SetParent(this.transform, false);

        m_solutionPanel.AddSolution(solution);
    }

    public void DismissSolutions()
    {
        if (m_solutionPanel != null)
        {
            Destroy(m_solutionPanel.gameObject);
            m_solutionPanel = null;
        }
    }
}
