using UnityEngine;

public class ConfirmHomeWindow : MonoBehaviour
{
    public void OnClickYes()
    {
        GameGUI gameGUI = (GameGUI) GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
        gameGUI.Dismiss(true);
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(GameController.GetInstance().ClearLevel, 0.5f);
        StartCoroutine(GameController.GetInstance().StartMainMenu(0.5f));

        Destroy(this.gameObject);
        gameGUI.m_confirmHomeWindow = null;
    }

    public void OnClickNo()
    {
        Destroy(this.gameObject);

        GameGUI gameGUI = (GameGUI)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
        gameGUI.m_confirmHomeWindow = null;
    }
}
