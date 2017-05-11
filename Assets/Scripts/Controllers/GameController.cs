//#define START_FROM_SCENE //use this to start from one particular scene defined inside the inspector

using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_brickPfb;
    public GameObject m_floorPfb;

    public BrickRenderer m_brickRenderer { get; set; }
    public FloorRenderer m_floorRenderer { get; set; }
    public GameObject m_bonuses { get; set; } //use this object to hold bonus objects

    private GUIManager m_guiManager; //to speed up a bit instead of calling GetComponent<>
    private LevelManager m_levelManager; //to speed up a bit instead of calling GetComponent<>

    public enum GameMode
    {
        MAIN_MENU,
        LEVELS,
        GAME,
        LEVEL_EDITOR,
        END_SCREEN
    }
    public GameMode m_gameMode;

    public int m_levelToStartInGameMode = 1;

    private static GameController s_instance;

    //variables linked to the current played level
    public int m_currentMovesCount { get; set; } //the current count of moves the player has done so far

    public void Start()
    {
        Debug.Log(">>>>GameController Start");

        //AdBuddiz
        //AdBuddizBinding.SetTestModeActive();
        //AdBuddizBinding.SetAndroidPublisherKey("c89af728-2208-49e6-a5de-8e9f7185a8a1");
        //AdBuddizBinding.CacheAds();

        //init the camera
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<IsometricCameraController>().Init();

        //init the level manager
        GetLevelManager().Init();

        //init the theme manager
        //GetComponent<ThemeManager>().Init();

        //init the GUI manager
        GetGUIManager().Init();

#if START_FROM_SCENE
        if (m_gameMode == GameMode.LEVEL_EDITOR)
        {
            StartCoroutine(EnterLevelEditor());
        }
        else if (m_gameMode == GameMode.MAIN_MENU)
        {
            StartCoroutine(StartMainMenu());
        }
        else if (m_gameMode == GameMode.LEVELS)
        {
            StartCoroutine(StartLevels());
        }
        else if (m_gameMode == GameMode.END_SCREEN)
        {
            StartCoroutine(ShowEndScreen());
        }
        else
        {
            StartGameForLevelNumber(m_levelToStartInGameMode);
        }
#else
        m_gameMode = GameMode.MAIN_MENU;
        StartCoroutine(StartMainMenu());
#endif
    }

    public static GameController GetInstance()
    {
        if (s_instance == null)
            s_instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        return s_instance;
    }

    public IEnumerator EnterLevelEditor(float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        m_gameMode = GameMode.LEVEL_EDITOR;
        GetGUIManager().RebuildGUI();

        yield return null;
    }

    public IEnumerator StartMainMenu(float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        Debug.Log(">>>>StartMainMenu");

        m_gameMode = GameMode.MAIN_MENU;
        GetGUIManager().RebuildGUI();

        yield return null;
    }

    public IEnumerator StartLevels(float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        m_gameMode = GameMode.LEVELS;
        GetGUIManager().RebuildGUI();

        yield return null;
    }

    public IEnumerator ShowEndScreen(float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        m_gameMode = GameMode.END_SCREEN;
        GetGUIManager().RebuildGUI();

        yield return null;
    }

    public void ClearLevel()
    {
        RemoveFloor();
        RemoveBrick();
        RemoveBonuses();
    }

    public void RemoveFloor()
    {
        if (m_floorRenderer != null)
        {
            Destroy(m_floorRenderer.gameObject);
            m_floorRenderer = null;
        }
    }

    public void RemoveBrick()
    {
        if (m_brickRenderer != null)
        {
            Destroy(m_brickRenderer.gameObject);
            m_brickRenderer = null;
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

    public void BuildLevel(Level level)
    {
        BuildBonusesHolder();
        RenderFloor(level.m_floor);
        BuildBrick(level);
    }

    public void StartLevel(Level level)
    {
        TeleportBrick(); //drop animation when level begins

        //Tile finishTile = level.m_floor.GetFinishTile();
        //TileRenderer finishTileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(finishTile);
        //finishTileRenderer.GenerateGlowSquaresOnFinishTile();
    }

    public void BuildGameForLevelNumber(int levelNumber)
    {
        BuildGameForLevel(GetLevelManager().GetLevelForNumber(levelNumber));        
    }

    /**
    * Called when we want to start a level from game scene (not level editor)
    **/
    private bool StartGameForLevelNumber(int levelNumber)
    {
        Level level = GetLevelManager().GetLevelForNumber(levelNumber);
        if (level != null)
        {
            StartGameForLevel(level);
            return true;
        }
        else
        {
            GetGUIManager().DestroyCurrentGUI();
            StartCoroutine(ShowEndScreen());
            return false;
        }
    }

    public void BuildGameForLevel(Level level)
    {

    }

    public void StartGameForLevel(Level level)
    {
        //m_victory = false;
        //m_defeat = false;

        m_gameMode = GameMode.GAME;

        LevelManager levelManager = GetLevelManager();
        levelManager.m_currentLevel = level;
        BuildLevel(level);
        StartLevel(level);
        
        GetGUIManager().RebuildGUI();

        m_gameStatus = GameStatus.IDLE;

        //show tutorial if relevant by searching for a tutorial matching this level
        GameGUI gameGUI = ((GameGUI)GetComponent<GUIManager>().m_currentGUI);
        if (!gameGUI.ShowFirstTutorial())
            m_gameStatus = GameStatus.RUNNING;
    }

    public bool StartNextLevel()
    {
        int nextLevelNumber = GetLevelManager().m_currentLevel.m_number + 1;
        StartGameForLevelNumber(nextLevelNumber);

        return true;
    }

    public void RenderFloor(Floor floor)
    {
        if (m_floorRenderer != null)
        {
            Destroy(m_floorRenderer.gameObject);
            m_floorRenderer = null;
        }

        //Render the floor
        GameObject floorObject = (GameObject)Instantiate(m_floorPfb);
        m_floorRenderer = floorObject.GetComponent<FloorRenderer>();
        m_floorRenderer.Render(floor);
    }

    private void BuildBrick(Level level)
    {
        if (level != null)
        {
            GameObject brickObject = (GameObject)Instantiate(m_brickPfb);
            m_brickRenderer = brickObject.GetComponent<BrickRenderer>();

            Tile[] coveredTiles = new Tile[2];
            coveredTiles[0] = (level == null) ? m_floorRenderer.m_floorData.GetCenterTile() : level.m_floor.GetStartTile();
            //coveredTiles[1] = m_floor.m_floorData.GetNextTileForDirection(coveredTiles[0], Brick.RollDirection.BOTTOM);
            coveredTiles[1] = null;
            m_brickRenderer.BuildOnTiles(coveredTiles);
        }
    }

    private void TeleportBrick()
    {
        m_brickRenderer.OnStartTeleportation();
    }

    public void BuildBonusesHolder()
    {
        m_bonuses = new GameObject("Bonuses");
    }

    public enum GameStatus
    {
        IDLE, //the game has not started yet, controls are disabled
        RUNNING, //game is running normally
        VICTORY, //game has ended on a victory
        DEFEAT, //game has ended on a defeat
        RETRY, //player has clicked on the retry button, interrupting the current game leaving it in ambiguous state
        PAUSED //game is paused
    }

    public GameStatus m_gameStatus { get; set; }

    //cache the values of defeat or victory so we do not have to check the game status again if one of this case already happened
    //private bool m_defeat;
    //private bool m_victory;

    private void CheckForVictoryDefeat()
    {
        if (m_gameStatus == GameStatus.RUNNING)
        {
            if (m_brickRenderer.IsFalling())
                m_gameStatus = GameStatus.DEFEAT;
            else if (m_brickRenderer.IsOnFinishTile())
            {
                m_gameStatus = GameStatus.VICTORY;
            }
        }

        //if (m_defeat || m_brickRenderer.IsFalling())
        //{
        //    m_defeat = true;
        //    m_gameStatus = GameStatus.DEFEAT;
        //}
        //else
        //{
        //    if (m_victory || m_brickRenderer.IsOnFinishTile())
        //    {
        //        Debug.Log("victory");
        //        m_victory = true;
        //        m_gameStatus = GameStatus.VICTORY;
        //    }
        //}
    }

    /**
    * Reset the game
    **/
    public void ResetGame()
    {
        //reset the last level reached
        PersistentDataManager persistentDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        persistentDataManager.SetMaxLevelReached(0, true);

        //reset data for every level
        GetLevelManager().CreateOrOverwriteAllLevelData();
    }

    //public IEnumerator ShowInterlevelScreenAfterDelay(float delay, GameStatus gameStatus)
    //{
    //    if (delay > 0)
    //        yield return new WaitForSeconds(delay);

    //    GetGUIManager().ShowInterLevelScreen(gameStatus);

    //    yield return null;
    //}

    public IEnumerator ShowInterlevelWindowAfterDelay(float delay, GameStatus gameStatus)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        GetGUIManager().ShowInterLevelWindow(gameStatus);

        yield return null;
    }

    public GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GetComponent<GUIManager>();

        return m_guiManager;
    }

    public LevelManager GetLevelManager()
    {
        if (m_levelManager == null)
            m_levelManager = GetComponent<LevelManager>();

        return m_levelManager;
    }

    public void Update()
    {
        if (m_gameMode == GameMode.GAME)
        {
            if (m_gameStatus == GameStatus.VICTORY)
            {
                m_gameStatus = GameStatus.IDLE;

                StartCoroutine(ShowInterlevelWindowAfterDelay(1.0f, GameStatus.VICTORY));
            }
            else if (m_gameStatus == GameStatus.DEFEAT)
            {
                m_gameStatus = GameStatus.IDLE;
                
                StartCoroutine(ShowInterlevelWindowAfterDelay(0.4f, GameStatus.DEFEAT));
            }
            else if (m_gameStatus == GameStatus.RUNNING)
            {
                CheckForVictoryDefeat();
            }
        }
    }
}
