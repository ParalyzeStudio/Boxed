using UnityEngine;
using UnityEngine.UI;

public class ResetEditingSubMenu : LevelEditorMenu
{
    public override void OnClickValidateSubMenu()
    {
        GameController gameController = GameController.GetInstance();

        //Delete current floor and build a new one
        gameController.ClearLevel();
        m_parentSwitcher.m_parentEditor.BuildLevel(null);

        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_MAIN);
        m_parentSwitcher.GetMainMenu().ToggleValidatePublishButtons(true);
        m_parentSwitcher.m_parentEditor.DismissTestMenu();
    }

    public override void OnClickCancelSubMenu()
    {
        m_parentSwitcher.ShowMenu(LevelEditorMenuSwitcher.MenuID.ID_MAIN);
    }
}
