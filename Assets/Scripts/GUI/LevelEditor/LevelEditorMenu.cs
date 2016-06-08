using UnityEngine;

/**
* Class that can serve both as a main menu and a sub menu with a validate and also cancel button
**/
public class LevelEditorMenu : MonoBehaviour
{
    protected LevelEditorMenuSwitcher m_parentSwitcher;

    public void Init(LevelEditorMenuSwitcher parentSwitcher)
    {
        m_parentSwitcher = parentSwitcher;
    }

    public virtual void OnClickValidateSubMenu()
    {
        m_parentSwitcher.m_parentEditor.m_editingMode = LevelEditor.EditingMode.NONE;
        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_MAIN);

        if (m_parentSwitcher.m_parentEditor.m_editedLevel.m_validated)
        {
            //unvalidate the level because changes were made
            m_parentSwitcher.m_parentEditor.m_editedLevel.m_validated = false;
            //dismiss test level button in case it was active
            m_parentSwitcher.m_parentEditor.DismissTestMenu();
            //show the validate button
            m_parentSwitcher.GetMainMenu().ToggleValidatePublishButtons(true);
        }
    }

    public virtual void OnClickCancelSubMenu()
    {

    }
}
