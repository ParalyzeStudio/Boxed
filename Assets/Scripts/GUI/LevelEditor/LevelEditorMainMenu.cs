using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMainMenu : LevelEditorMenu
{
    public Button m_validateBtn;
    public Button m_publishBtn;

    public void OnClickEditTiles()
    {
        m_parentSwitcher.m_parentEditor.m_editingMode = LevelEditor.EditingMode.TILES_EDITING;
        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_EDIT_TILES);
    }

    public void OnClickCheckpoints()
    {
        m_parentSwitcher.m_parentEditor.m_editingMode = LevelEditor.EditingMode.CHECKPOINTS_EDITING;
        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_CHECKPOINTS);
    }

    public void OnClickBonuses()
    {
        m_parentSwitcher.m_parentEditor.m_editingMode = LevelEditor.EditingMode.BONUSES_EDITING;
        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_BONUSES);
    }

    public void OnClickReset()
    {
        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_RESET);
    }

    public void OnClickValidateLevel()
    {
        Level.ValidationData output = m_parentSwitcher.m_parentEditor.m_editedLevel.Validate(10);
        m_parentSwitcher.m_parentEditor.DisplayLevelValidationOutput(output);

        //remove the validate button and make the Test level button active
        if (output.m_success)
        {
            ToggleValidatePublishButtons(false);
            m_parentSwitcher.m_parentEditor.ShowTestMenu();
        }
    }

    public void OnClickPublishLevel()
    {
        m_parentSwitcher.m_parentEditor.ShowPublishWindow();
    }

    public void OnClickSaveLoad()
    {
        m_parentSwitcher.m_parentEditor.ShowSaveLoadLevelWindow();
    }

    public void OnClickPublishedLevels()
    {
        m_parentSwitcher.m_parentEditor.ShowPublishedLevelsWindow();
    }

    public void ToggleValidatePublishButtons(bool bShowValidateBtn)
    {
        if (bShowValidateBtn)
        {
            m_validateBtn.transform.parent.gameObject.SetActive(true);
            m_publishBtn.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_validateBtn.transform.parent.gameObject.SetActive(false);
            m_publishBtn.transform.parent.gameObject.SetActive(true);
        }
    }
}
