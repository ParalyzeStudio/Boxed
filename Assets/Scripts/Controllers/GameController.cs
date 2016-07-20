﻿using UnityEngine;

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

    public int m_levelToStartInGameMode = 1;

    private Level m_currentLevel;

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
            //Show whole gui (title + buttons)
            guiManager.DisplayMainMenuGUI();
        }
        else if (m_gameMode == GameMode.LEVELS)
        {
            guiManager.DisplayLevelsGUI();
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
        this.GetComponent<GUIManager>().DisplayLevelEditorGUI();
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

    /**
    * Called when we want to start a level from game scene (not level editor)
    **/
    public bool StartGameForLevelNumber(int levelNumber)
    {
        Level level = GetComponent<LevelManager>().GetLevelForNumber(levelNumber);
        if (level != null)
        {
            StartGameForLevel(level);
            return true;
        }
        else
            return false;
    }

    public void StartGameForLevel(Level level)
    {
        m_victory = false;
        m_defeat = false;

        m_gameMode = GameMode.GAME;
        m_currentLevel = level;
        StartLevel(level);

        GetComponent<GUIManager>().DisplayGameGUIForLevel(m_currentLevel);

        m_gameStatus = GameStatus.RUNNING;
    }

    private void StartNextLevel()
    {
        int nextLevelNumber = m_currentLevel.m_number + 1;
        StartGameForLevelNumber(nextLevelNumber);
    }

    public void RestartLevel()
    {
        ClearLevel();
        StartGameForLevel(m_currentLevel);
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

    public GameStatus m_gameStatus { get; set; }

    //cache the values of defeat or victory so we do not have to check the game status again if one of this case already happened
    private bool m_defeat;
    private bool m_victory;

    private void CheckForVictoryDefeat()
    {
        if (m_victory || m_brick.IsOnFinishTile())
        {
            Debug.Log("m_victory");
            m_victory = true;
            m_gameStatus = GameStatus.VICTORY;
        }

        if (m_defeat || m_brick.IsFalling())
        {
            m_defeat = true;
            m_gameStatus = GameStatus.DEFEAT;
        }
    }

    public void Update()
    {
        if (m_gameStatus == GameStatus.VICTORY)
        {
            int nextLevelNumber = m_currentLevel.m_number + 1;
            GetComponent<CallFuncHandler>().AddCallFuncInstance(GetComponent<GUIManager>().DismissCurrentGUI, 1.0f);

            m_gameStatus = GameStatus.IDLE;
            GetComponent<CallFuncHandler>().AddCallFuncInstance(new CallFuncHandler.CallFunc(ClearLevel), 1.5f);
            GetComponent<CallFuncHandler>().AddCallFuncInstance(new CallFuncHandler.CallFunc(StartNextLevel), 1.55f);
        }
        else if (m_gameStatus == GameStatus.DEFEAT)
        {
            Debug.Log("defeat");
            m_gameStatus = GameStatus.IDLE;
        }
        else if (m_gameStatus == GameStatus.RUNNING)
        {
            CheckForVictoryDefeat();
        }
    }
}