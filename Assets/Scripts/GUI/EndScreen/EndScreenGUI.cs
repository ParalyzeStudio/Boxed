using UnityEngine;

public class EndScreenGUI : BaseGUI
{
    public void OnClickHome()
    {
        Dismiss(true);
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(GameController.GetInstance().StartMainMenu, 0.5f);
    }
}
