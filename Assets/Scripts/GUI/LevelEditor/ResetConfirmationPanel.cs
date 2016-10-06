using UnityEngine;

public class ResetConfirmationPanel : ActionPanel
{
    public override void OnClickValidate()
    {
        //Delete current floor and build a new one
        GameController.GetInstance().ClearLevel();
        m_parentMenu.m_parentEditor.BuildLevel(null);

        base.OnClickValidate();

        m_parentMenu.m_parentEditor.DismissTestMenu();
    }

    public override void OnClickCancel()
    {
        base.OnClickCancel();
    }
}
