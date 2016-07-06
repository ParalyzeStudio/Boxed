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
        LEVELS,
        GAME,
        LEVEL_EDITOR
    }
    public GameMode m_gameMode;

    private static GameController s_instance;

    public void Start()
    {
        //cache levels
        LevelManager levelManager = this.GetComponent<LevelManager>();
        levelManager.CacheLevels();

        //init the camera
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<IsometricCameraController>().Init();

        //init the GUI manager
        GUIManager guiManager = GetComponent<GUIManager>();
        guiManager.Init();       

        if (m_gameMode == GameMode.LEVEL_EDITOR)
        {
            EnterLevelEditor();
        }
        else if (m_gameMode == GameMode.MAIN_MENU)
        {
            //Show whole gui (title + buttons)
            guiManager.DisplayMainMenu();
        }
        else if (m_gameMode == GameMode.LEVELS)
        {
            guiManager.DisplayLevels();
        }
        else
        {           
            StartLevel(levelManager.GetPublishedLevelForNumber(1));

            m_gameStarted = true;
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

            Tile[] coveredTiles = new Tile[2];
            coveredTiles[0] = (level == null) ? m_floor.m_floorData.GetCenterTile() : level.m_floor.GetStartTile();
            //coveredTiles[1] = m_floor.m_floorData.GetNextTileForDirection(coveredTiles[0], Brick.RollDirection.BOTTOM);
            coveredTiles[1] = null;
            m_brick.BuildOnTiles(coveredTiles);
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

    public enum GameStatus
    {
        IDLE, //the game has not started yet, controls are disabled
        RUNNING, //game is running normally
        VICTORY, //game has ended on a victory
        DEFEAT //game has ended on a defeat
    }

    private bool m_gameStarted;

    //cache the values of defeat or victory so we do not have to check the game status again if one of this case already happened
    private bool m_defeat;
    private bool m_victory;

    private GameStatus GetGameStatus()
    {
        if (!m_gameStarted)
            return GameStatus.IDLE;

        if (m_victory || m_brick.IsOnFinishTile())
        {
            m_victory = true;
            return GameStatus.VICTORY;
        }

        if (m_defeat || m_brick.IsFalling())
        {
            m_defeat = true;
            return GameStatus.DEFEAT;
        }

        return GameStatus.RUNNING;
    }

    public void Update()
    {
        //GameStatus gameStatus = GetGameStatus();
        //if (gameStatus == GameStatus.VICTORY)
        //    Debug.Log("victory");
        //else if (gameStatus == GameStatus.DEFEAT)
        //    Debug.Log("defeat");
    }
}
