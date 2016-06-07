using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMenu : MonoBehaviour
{
    private LevelEditor m_parentEditor;

    public GameObject m_mainMenu;
    public GameObject m_editTilesSubMenu;
    public GameObject m_checkpointsSubMenu;
    public GameObject m_bonusesSubMenu;
    public GameObject m_resetSubMenu;
    public Button m_validateBtn;
    public Button m_publishBtn;

    private GameObject m_activeMenu;

    private enum MenuID
    {
        ID_MAIN,
        ID_EDIT_TILES,
        ID_CHECKPOINTS,
        ID_BONUSES,
        ID_RESET
    }

    public void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;
        m_activeMenu = GetMenuForID(MenuID.ID_MAIN);
    }

    private void ShowMenu(MenuID iID)
    {
        GameObject menuObject = GetMenuForID(iID);

        menuObject.SetActive(true);
        m_activeMenu.SetActive(false);
        m_activeMenu = menuObject;
    }

    private GameObject GetMenuForID(MenuID iID)
    {
        if (iID == MenuID.ID_MAIN)
            return m_mainMenu;
        else if (iID == MenuID.ID_EDIT_TILES)
            return m_editTilesSubMenu;
        else if (iID == MenuID.ID_CHECKPOINTS)
            return m_checkpointsSubMenu;
        else if (iID == MenuID.ID_BONUSES)
            return m_bonusesSubMenu;
        else if (iID == MenuID.ID_RESET)
            return m_resetSubMenu;
        return m_editTilesSubMenu;
    }

    public void OnClickEditTiles()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.TILES_EDITING;
        ShowMenu(MenuID.ID_EDIT_TILES);
    }

    public void OnClickCheckpoints()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.CHECKPOINTS_EDITING;
        ShowMenu(MenuID.ID_CHECKPOINTS);
    }

    public void OnClickBonuses()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.BONUSES_EDITING;
        ShowMenu(MenuID.ID_BONUSES);
    }

    public void OnClickReset()
    {
        ShowMenu(MenuID.ID_RESET);
    }

    public void OnClickValidateLevel()
    {
        Level.ValidationData output = m_parentEditor.m_editedLevel.Validate(10);
        m_parentEditor.DisplayLevelValidationOutput(output);

        //remove the validate button and make the Test level button active
        if (output.m_success)
        {
            m_validateBtn.transform.parent.gameObject.SetActive(false);
            m_publishBtn.transform.parent.gameObject.SetActive(true);
            m_parentEditor.ShowTestLevelButton();            
        }
    }

    public void OnClickSaveLoad()
    {
        m_parentEditor.ShowSaveLoadLevelWindow();
    }

    public void OnClickValidateSubMenu()
    {
        ShowMenu(MenuID.ID_MAIN);

        if (m_parentEditor.m_editedLevel.m_validated)
        {
            //unvalidate the level because changes were made
            m_parentEditor.m_editedLevel.m_validated = false;
            //dismiss test level button in case it was active
            m_parentEditor.DismissTestLevelButton();
            //show the validate button
            m_validateBtn.transform.parent.gameObject.SetActive(true);
            m_publishBtn.transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnClickValidateResetSubMenu()
    {
        GameController gameController = GameController.GetInstance();

        //Delete current floor and build a new one
        gameController.ClearLevel();
        m_parentEditor.BuildLevel(null);

        ShowMenu(MenuID.ID_MAIN);
    }

    public void OnClickCancelResetSubMenu()
    {
        ShowMenu(MenuID.ID_MAIN);
    }

    public void Update()
    {        
        //detect if this panel contains the mouse click
        if (Input.GetMouseButton(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition))
            {
                Debug.Log("Main Menu processed click");
                m_parentEditor.ProcessClick();
            }
        }
    }
}

