using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_brickPfb;
    public GameObject m_floorPfb;

    public BrickRenderer m_brick { get; set; }
    public FloorRenderer m_floor { get; set; }
    public GameObject m_bonuses { get; set; } //use this object to hold bonus objects

    public enum GameMode
    {
        MAIN_MENU,
        GAME,
        LEVEL_EDITOR
    }
    public GameMode m_gameMode;

    private static GameController s_instance;

    public void Start()
    {
        //init the camera
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<IsometricCameraController>().Init();

        //init the GUI manager
        GetComponent<GUIManager>().Init();       

        if (m_gameMode == GameMode.LEVEL_EDITOR)
        {
            EnterLevelEditor();
        }
        else if (m_gameMode == GameMode.MAIN_MENU)
        {   
            //Show whole gui (title + buttons)
            this.GetComponent<GUIManager>().DisplayMainMenu();
        }
        else
        {
            LevelManager levelManager = this.GetComponent<LevelManager>();
            levelManager.CacheLevels();
            StartLevel(levelManager.GetPublishedLevelForNumber(1));
        }
    }

    public static GameController GetInstance()
    {
        if (s_instance == null)
            s_instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        return s_instance;
    }

    public void EnterLevelEditor()
    {
        this.GetComponent<GUIManager>().DisplayLevelEditor();
    }

    public void ClearLevel()
    {
        RemoveFloor();
        RemoveBrick();
        RemoveBonuses();
    }

    public void RemoveFloor()
    {
        if (m_floor != null)
        {
            Destroy(m_floor.gameObject);
            m_floor = null;
        }
    }

    public void RemoveBrick()
    {
        if (m_brick != null)
        {
            Destroy(m_brick.gameObject);
            m_brick = null;
        }
    }

    public void RemoveBonuses()
    {
        if (m_bonuses != null)
        {
            Destroy(m_bonuses.gameObject);
            m_bonuses = null;
        }
    }

    public void StartLevel(Level level)
    {
        BuildBonusesHolder();
        RenderFloor(level.m_floor);
        BuildBrick(level);
    }

    public void RenderFloor(Floor floor)
    {
        if (m_floor != null)
        {
            Destroy(m_floor.gameObject);
            m_floor = null;
        }

        //Render the floor
        GameObject floorObject = (GameObject)Instantiate(m_floorPfb);
        m_floor = floorObject.GetComponent<FloorRenderer>();
        m_floor.Render(floor);
    }

    private void BuildBrick(Level level)
    {
        if (level != null)
        {
            GameObject brickObject = (GameObject)Instantiate(m_brickPfb);
            m_brick = brickObject.GetComponent<BrickRenderer>();

            m_brick.BuildOnTile((level == null) ? m_floor.m_floorData.GetCenterTile() : level.m_floor.GetStartTile());
            //m_brick.BuildOnTile(m_floor.m_floorData.Tiles[1]);
        }
    }

    public void BuildBonusesHolder()
    {
        m_bonuses = new GameObject("Bonuses");
    }

    public GUIManager GetGUIManager()
    {
        return this.GetComponent<GUIManager>();
    }
}
