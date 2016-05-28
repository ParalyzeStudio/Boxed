﻿using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    private CallFuncHandler m_callFuncHandler;

    public GameObject m_mainMenu;
    public GameObject m_tileEditingMenu;
    public GameObject m_checkpointsMenu;
    public GameObject m_bonusesMenu;
    public GameObject m_resetMenu;
    public GameObject m_saveLevelWindow;

    private GameObject m_activeMenu;

    public enum EditingMode
    {
        NONE = 0,
        TILES_EDITING,
        CHECKPOINTS_EDITING,
        BONUSES_EDITING        
    }

    public EditingMode m_editingMode { get; set; }

    public Tile m_startTile { get; set; }
    public Tile m_finishTile { get; set; }

    private bool m_guiProcessedClick; //variable used to know if the Level Editor GUI has processed the click event

    public void Start()
    {
        m_activeMenu = m_mainMenu;
    }

    public void ShowMainMenu()
    {
        m_activeMenu.SetActive(false);
        m_mainMenu.SetActive(true);
        m_activeMenu = m_mainMenu;
    }

    /**
    * Action called when clicking on 'edit tiles' button
    **/
    public void DoTilesSelection()
    {
        Debug.Log("EditTiles");
        m_guiProcessedClick = true;
        m_editingMode = EditingMode.TILES_EDITING;

        ShowTilesSelectionMenu();      
    }

    private void ShowTilesSelectionMenu()
    {

        m_tileEditingMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_tileEditingMenu;
    }

    public void ValidateTilesSelection()
    {
        m_guiProcessedClick = true;
        Debug.Log("ValidateTilesSelection");
        ShowMainMenu();
    }

    /**
    * Action called when clicking on 'Checkpoints' button
    **/
    public void DoCheckpointsEditing()
    {
        Debug.Log("Checkpoints");
        m_guiProcessedClick = true;
        m_editingMode = EditingMode.CHECKPOINTS_EDITING;

        ShowCheckpointsMenu();
    }

    private void ShowCheckpointsMenu()
    {
        m_checkpointsMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_checkpointsMenu;
    }

    public void ValidateCheckpoints()
    {
        m_guiProcessedClick = true;
        Debug.Log("ValidateCheckpoints");
        ShowMainMenu();
    }

    /**
    * Action called when clicking on 'Bonuses' button
    **/
    public void DoBonusesEditing()
    {
        Debug.Log("Bonuses");
        m_guiProcessedClick = true;
        m_editingMode = EditingMode.BONUSES_EDITING;

        ShowBonusesMenu();
    }

    private void ShowBonusesMenu()
    {
        m_bonusesMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_bonusesMenu;
    }

    public void ValidateBonuses()
    {
        m_guiProcessedClick = true;
        Debug.Log("ValidateBonuses");
        ShowMainMenu();
    }

    /**
    * Action called when clicking on 'Reset level' button
    **/
    public void ResetLevel()
    {
        Debug.Log("Reset");
        m_guiProcessedClick = true;
        ShowResetConfirmationMenu();
       
    }

    private void ShowResetConfirmationMenu()
    {
        m_resetMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_resetMenu;
    }

    public void ConfirmResetLevel()
    {
        m_guiProcessedClick = true;
        GameController gameController = GameController.GetInstance();

        //Delete current floor and build a new one
        gameController.BuildFloor(null);

        //Destroy bonuses
        Destroy(gameController.m_bonuses);
        gameController.BuildBonusesHolder();

        //gameController.EnterLevelEditor();
        ShowMainMenu();
    }

    public void CancelResetLevel()
    {
        ShowMainMenu();
    }

    /**
    * Action called when clicking on 'Save level' button
    **/
    public void DoSaveLevel()
    {
        m_guiProcessedClick = true;
        Debug.Log("Save level");

        GameObject saveLevelWindowObject = (GameObject)Instantiate(m_saveLevelWindow);
        saveLevelWindowObject.transform.SetParent(GameController.GetInstance().m_canvas.transform, false);

        SaveLevelWindow window = saveLevelWindowObject.GetComponent<SaveLevelWindow>();
        window.Init(null);

        //Floor floor = this.GetComponent<GameController>().m_floor;
        //floor.PrepareForSaving();
    }

    public CallFuncHandler GetCallFuncHandler()
    {
        if (m_callFuncHandler == null)
            m_callFuncHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<CallFuncHandler>();

        return m_callFuncHandler;
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
