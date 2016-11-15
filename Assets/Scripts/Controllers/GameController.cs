﻿using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_brickPfb;
    public GameObject m_floorPfb;

    public BrickRenderer m_brickRenderer { get; set; }
    public FloorRenderer m_floorRenderer { get; set; }
    public GameObject m_bonuses { get; set; } //use this object to hold bonus objects

    private GUIManager m_guiManager; //maybe to speed up a bit instead of calling GetComponent<>

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

        //m_gameMode = GameMode.MAIN_MENU;

        if (m_gameMode == GameMode.LEVEL_EDITOR)
        {
            EnterLevelEditor();
        }
        else if (m_gameMode == GameMode.MAIN_MENU)
        {
            StartMainMenu();
        }
        else if (m_gameMode == GameMode.LEVELS)
        {
            StartLevels();
        }
        else if (m_gameMode == GameMode.END_SCREEN)
        {
            ShowEndScreen();
        }
        else
        {
            StartGameForLevelNumber(m_levelToStartInGameMode);
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
        m_gameMode = GameMode.LEVEL_EDITOR;

        this.GetComponent<GUIManager>().DisplayLevelEditorGUI();
    }

    public void StartMainMenu()
    {
        m_gameMode = GameMode.MAIN_MENU;

        //Show whole gui (title + buttons)
        GetComponent<GUIManager>().DisplayMainMenuGUI();
    }

    public void StartLevels()
    {
        m_gameMode = GameMode.LEVELS;
        
        GetComponent<GUIManager>().DisplayLevelsGUI();
    }

    public void ShowEndScreen()
    {
        m_gameMode = GameMode.END_SCREEN;

        GetComponent<GUIManager>().DisplayEndScreenGUI();
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

    public void StartLevel(Level level)
    {
        BuildBonusesHolder();
        RenderFloor(level.m_floor);
        BuildBrick(level);
        DropBrick(); //drop animation when level begins

        
        Tile finishTile = level.m_floor.GetFinishTile();
        TileRenderer finishTileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(finishTile);
        finishTileRenderer.GenerateGlowSquaresOnFinishTile();
    }

    /**
    * Called when we want to start a level from game scene (not level editor)
    **/
    private bool StartGameForLevelNumber(int levelNumber)
    {
        Level level = GetComponent<LevelManager>().GetLevelForNumber(levelNumber);
        if (level != null)
        {
            StartGameForLevel(level);
            return true;
        }
        else
        {
            GetGUIManager().DestroyCurrentGUI();
            ShowEndScreen();
            return false;
        }
    }

    public void StartGameForLevel(Level level)
    {
        m_victory = false;
        m_defeat = false;

        m_gameMode = GameMode.GAME;

        LevelManager levelManager = GetComponent<LevelManager>();
        levelManager.m_currentLevel = level;
        levelManager.m_currentLevelData = LevelData.LoadFromFile(level.m_number);
        levelManager.m_currentLevelData.m_currentActionsCount = 0;
        StartLevel(level);
        
        GetComponent<GUIManager>().DisplayGameGUIForLevel(level);

        m_gameStatus = GameStatus.RUNNING;
    }

    private void StartNextLevel()
    {
        int nextLevelNumber = GetComponent<LevelManager>().m_currentLevel.m_number + 1;
        StartGameForLevelNumber(nextLevelNumber);
    }

    public void RestartLevel()
    {
        ClearLevel();
        StartGameForLevel(GetComponent<LevelManager>().m_currentLevel);
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

    private void DropBrick()
    {
        float dropHeight = 4.0f * Brick.BRICK_BASIS_DIMENSION;
        Vector3 brickFinalPosition = m_brickRenderer.gameObject.transform.localPosition;
        m_brickRenderer.gameObject.transform.localPosition += new Vector3(0, dropHeight, 0);

        float dropDuration = 0.5f;

        BrickAnimator brickAnimator = m_brickRenderer.GetComponent<BrickAnimator>();
        brickAnimator.UpdatePivotPoint(Vector3.zero);
        brickAnimator.TranslateTo(brickFinalPosition, dropDuration, 0, ValueAnimator.InterpolationType.HERMITE1, false);

        CallFuncHandler callFuncHandler = GetInstance().GetComponent<CallFuncHandler>();
        callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(OnBrickLanding), dropDuration);
    }

    private void OnBrickLanding()
    {
        Tile landedTile = m_brickRenderer.m_brick.m_coveredTiles.GetFirstTile();
        TileRenderer tileRenderer = m_floorRenderer.GetRendererForTile(landedTile);
        tileRenderer.DisplayGlowSquareOnBrickLanding();
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
        DEFEAT //game has ended on a defeat
    }

    public GameStatus m_gameStatus { get; set; }

    //cache the values of defeat or victory so we do not have to check the game status again if one of this case already happened
    private bool m_defeat;
    private bool m_victory;

    private void CheckForVictoryDefeat()
    {
        if (m_defeat || m_brickRenderer.IsFalling())
        {
            Debug.Log("DEFEAT");
            m_defeat = true;
            m_gameStatus = GameStatus.DEFEAT;
        }
        else
        {
            if (m_victory || m_brickRenderer.IsOnFinishTile())
            {
                m_victory = true;
                m_gameStatus = GameStatus.VICTORY;
            }
        }
    }

    public GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GetComponent<GUIManager>();

        return m_guiManager;
    }

    public void Update()
    {
        if (m_gameMode == GameMode.GAME)
        {
            if (m_gameStatus == GameStatus.VICTORY)
            {
                LevelData currentLevelData = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevelData;
                if (!currentLevelData.m_done)
                {
                    currentLevelData.m_done = true;
                    currentLevelData.SaveToFile();
                }

                Level currentLevel = GameController.GetInstance().GetComponent<LevelManager>().m_currentLevel;
                int nextLevelNumber = currentLevel.m_number + 1;
                GetComponent<CallFuncHandler>().AddCallFuncInstance(GetComponent<GUIManager>().DismissCurrentGUI, 1.0f);

                m_gameStatus = GameStatus.IDLE;
                GetComponent<CallFuncHandler>().AddCallFuncInstance(ClearLevel, 1.5f);
                GetComponent<CallFuncHandler>().AddCallFuncInstance(StartNextLevel, 1.55f);
            }
            else if (m_gameStatus == GameStatus.DEFEAT)
            {
                m_gameStatus = GameStatus.IDLE;

                GetComponent<CallFuncHandler>().AddCallFuncInstance(GetComponent<GUIManager>().DismissCurrentGUI, 1.0f);
                GetComponent<CallFuncHandler>().AddCallFuncInstance(ClearLevel, 1.5f);
                GetComponent<CallFuncHandler>().AddCallFuncInstance(RestartLevel, 2.0f);
            }
            else if (m_gameStatus == GameStatus.RUNNING)
            {
                CheckForVictoryDefeat();
            }
        }
    }
}
