using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMainMenu : LevelEditorMenu
{
    public EditTilesSubMenu m_tilesEditingSubMenu;
    public ActionPanel m_bonusesEditing;
    public ResetConfirmationPanel m_resetConfirmation;

    public Button m_validateBtn;
    public Button m_publishBtn;

    public void OnClickEditTiles()
    {
        //m_parentEditor.m_editingMode = LevelEditor.EditingMode.TILES_EDITING;
        //ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_EDIT_TILES);

        this.gameObject.SetActive(false);
        m_tilesEditingSubMenu.gameObject.SetActive(true);
        m_tilesEditingSubMenu.m_parentEditor = this.m_parentEditor;
    }

    public void OnClickEditBonuses()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.BONUSES_EDITING;
        m_parentEditor.m_activeEditingPanel = m_bonusesEditing;
        this.gameObject.SetActive(false);
        m_bonusesEditing.gameObject.SetActive(true);
        m_bonusesEditing.m_parentMenu = this;
    }

    public void OnClickReset()
    {
        this.gameObject.SetActive(false);
        m_resetConfirmation.gameObject.SetActive(true);
        m_resetConfirmation.m_parentMenu = this;
    }

    public void OnClickValidateLevel()
    {
        m_parentEditor.m_computingSolution = true;
        m_parentEditor.m_editedLevel.Validate();
    }

    public void OnClickPublishLevel()
    {
        m_parentEditor.ShowPublishWindow();
    }

    public void OnClickSaveLoad()
    {
        m_parentEditor.ShowSaveLoadLevelWindow();
    }

    public void OnClickPublishedLevels()
    {
        m_parentEditor.ShowPublishedLevelsWindow();
    }

    public void InvalidateValidatePublishButtons()
    {
        if (!m_parentEditor.m_editedLevel.m_validated)
        {
            m_validateBtn.transform.parent.gameObject.SetActive(true);
            m_publishBtn.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_validateBtn.transform.parent.gameObject.SetActive(false);
            m_publishBtn.transform.parent.gameObject.SetActive(!m_parentEditor.m_editedLevel.m_published);
        }
    }
}
