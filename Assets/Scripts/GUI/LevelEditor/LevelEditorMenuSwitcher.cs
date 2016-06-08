using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMenuSwitcher : MonoBehaviour
{
    public LevelEditor m_parentEditor { get; set; }

    public LevelEditorMenu m_mainMenu;
    public EditTilesSubMenu m_editTilesSubMenu;
    public LevelEditorMenu m_checkpointsSubMenu;
    public LevelEditorMenu m_bonusesSubMenu;
    public LevelEditorMenu m_resetSubMenu;

    private LevelEditorMenu m_activeMenu;

    public enum MenuID
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
        ShowMenu(MenuID.ID_MAIN);
    }

    public void ShowMenu(MenuID iID)
    {
        LevelEditorMenu menuObject = GetMenuForID(iID);
        menuObject.Init(this);

        menuObject.gameObject.SetActive(true);
        if (m_activeMenu != null)
            m_activeMenu.gameObject.SetActive(false);
        m_activeMenu = menuObject;
    }

    public LevelEditorMenu GetMenuForID(MenuID iID)
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

    public LevelEditorMainMenu GetMainMenu()
    {
        return (LevelEditorMainMenu)GetMenuForID(MenuID.ID_MAIN);
    }
}

