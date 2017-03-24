using System.Collections;

public class EndScreenGUI : BaseGUI
{
    public void OnClickHome()
    {
        Dismiss(true);
        
        StartCoroutine(GameController.GetInstance().StartMainMenu(0.5f));
    }
}
