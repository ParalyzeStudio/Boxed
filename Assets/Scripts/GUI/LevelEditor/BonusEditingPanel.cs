using UnityEngine;
using UnityEngine.UI;

public class BonusEditingPanel : ActionPanel
{
    public override void OnClickValidate()
    {
        GameController.GetInstance().m_floor.m_floorData.ClearBonusTilesCachedValue();
        base.OnClickValidate();
    }
}
