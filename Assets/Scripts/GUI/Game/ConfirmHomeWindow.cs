using UnityEngine;

public class ConfirmHomeWindow : MonoBehaviour
{
    public void OnClickYes()
    {
        GameGUI gameGUI = (GameGUI) GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
        gameGUI.Dismiss();
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(GameController.GetInstance().StartMainMenu, 0.5f);

        Destroy(this.gameObject);
    }

    public void OnClickNo()
    {
        Destroy(this.gameObject);
    }
}
