using UnityEngine;
using UnityEngine.UI;

public class EditBonusesSubMenu : LevelEditorMenu
{
    public override void OnClickValidateSubMenu()
    {
        GameController.GetInstance().m_floor.m_floorData.ClearBonusTilesCachedValue();
        base.OnClickValidateSubMenu();
    }
}
