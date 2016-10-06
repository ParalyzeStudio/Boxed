using UnityEngine;
using UnityEngine.UI;

public class EditTilesSubMenu : LevelEditorMenu
{
    public ActionPanel m_tilesEditing;
    public ActionPanel m_checkpointsEditing;
    public ActionPanel m_switchesEditing;

    public void OnClickTiles()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.TILES_EDITING;
        m_parentEditor.m_activeEditingPanel = m_tilesEditing;
        m_tilesEditing.m_parentMenu = this;
        this.gameObject.SetActive(false);
        m_tilesEditing.gameObject.SetActive(true);
    }

    public void OnClickCheckpoints()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.CHECKPOINTS_EDITING;
        m_parentEditor.m_activeEditingPanel = m_checkpointsEditing;
        m_checkpointsEditing.m_parentMenu = this;
        this.gameObject.SetActive(false);
        m_checkpointsEditing.gameObject.SetActive(true);
    }

    public void OnClickSwitches()
    {
        m_parentEditor.m_editingMode = LevelEditor.EditingMode.SWITCHES_EDITING;
        m_parentEditor.m_activeEditingPanel = m_switchesEditing;
        m_switchesEditing.m_parentMenu = this;
        this.gameObject.SetActive(false);
        m_switchesEditing.gameObject.SetActive(true);
    }

    public void OnClickBack()
    {
        this.gameObject.SetActive(false);
        m_parentEditor.ShowMainMenu();
    }
}
