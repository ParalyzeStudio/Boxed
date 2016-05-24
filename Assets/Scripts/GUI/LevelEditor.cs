using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    private CallFuncHandler m_callFuncHandler;

    public GameObject m_mainMenu;
    public GameObject m_tileEditingMenu;
    public GameObject m_checkpointsMenu;
    public GameObject m_bonusesMenu;
    public GameObject m_resetMenu;

    private GameObject m_activeMenu;

    public enum State
    {
        NONE = 0,
        SELECTING_TILES_BY_CLICKING,
        PICKING_STARTING_TILES,
        PICKING_ENDING_TILES
    }

    public State m_state { get; set; }

    private bool m_guiProcessedClick; //variable used to know if the Level Editor GUI has processed the click event

    public void Start()
    {
        ShowMainMenu();
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

        GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowTilesSelectionMenu), 0.03f);        
    }

    private void ShowTilesSelectionMenu()
    {
        m_state = State.SELECTING_TILES_BY_CLICKING;

        m_tileEditingMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_tileEditingMenu;
    }

    public void ValidateTilesSelection()
    {
        ShowMainMenu();
    }

    /**
    * Action called when clicking on 'Checkpoints' button
    **/
    public void DoCheckpointsEditing()
    {
        Debug.Log("Checkpoints");
        m_guiProcessedClick = true;
    }

    private void ShowCheckpointsMenu()
    {
        m_checkpointsMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_checkpointsMenu;
    }

    /**
    * Action called when clicking on 'Bonuses' button
    **/
    public void DoBonusesEditing()
    {
        Debug.Log("Bonuses");
        m_guiProcessedClick = true;
    }

    private void ShowBonusesMenu()
    {
        m_bonusesMenu.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = m_bonusesMenu;
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
        GameController gameController = this.GetComponent<GameController>();

        //Delete current floor and build a new one
        Destroy(gameController.m_floor);
        gameController.EnterLevelEditor();
    }

    /**
    * Action called when clicking on 'Save level' button
    **/
    public void SaveLevel()
    {
        m_guiProcessedClick = true;
        Debug.Log("Save level");
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
