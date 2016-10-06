using UnityEngine;

/**
* Class that can serve both as a main menu and a sub menu with a validate and also cancel button
**/
public class ActionPanel : MonoBehaviour
{
    public LevelEditorMenu m_parentMenu { get; set; }

    public virtual void OnClickValidate()
    {
        m_parentMenu.m_parentEditor.m_editedLevel.m_validated = false;
        m_parentMenu.m_parentEditor.m_editedLevel.m_published = false;

        m_parentMenu.m_parentEditor.m_editingMode = LevelEditor.EditingMode.NONE;
        this.gameObject.SetActive(false);
        m_parentMenu.gameObject.SetActive(true);

        //if (m_parentMenu.m_parentEditor.m_editedLevel.m_validated)
        //{
        //    //unvalidate the level because changes were made
        //    m_parentMenu.m_parentEditor.m_editedLevel.m_validated = false;
        //    //dismiss test level button in case it was active
        //    m_parentMenu.m_parentEditor.DismissTestMenu();
        //    ////show the validate button
        //    //if (m_parentMenu is LevelEditorMainMenu)
        //    //    ((LevelEditorMainMenu) m_parentMenu).ToggleValidatePublishButtons(true);
        //}
    }

    public virtual void OnClickCancel()
    {
        m_parentMenu.m_parentEditor.m_editingMode = LevelEditor.EditingMode.NONE;
        m_parentMenu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
