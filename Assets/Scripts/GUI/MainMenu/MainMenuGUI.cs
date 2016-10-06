using UnityEngine;

public class MainMenuGUI : BaseGUI
{
    public void OnClickSettings()
    {
        Debug.Log("OnClickSettings");
    }

    public void OnClickTrophy()
    {
        Debug.Log("OnClickTrophy");
    }

    public void OnClickInfo()
    {
        Debug.Log("OnClickInfo");
    }

    public void OnClickPlay()
    {
        Dismiss(true);

        CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();
        //callFuncHandler.AddCallFuncInstance(GameController.GetInstance().GetComponent<GUIManager>().DisplayLevelsGUI, 0.5f);
        callFuncHandler.AddCallFuncInstance(GameController.GetInstance().StartLevels, 0.5f);


        ////Generate random theme
        //ColorTheme defaultTheme = GameController.GetInstance().GetComponent<GUIManager>().m_themes.Themes[0];
        //ColorTheme randomTheme = new ColorTheme(defaultTheme, 180);

        //Color backgroundTopColor = randomTheme.m_backgroundGradientTopColor;
        //Color backgroundBottomColor = randomTheme.m_backgroundGradientBottomColor;

        //Debug.Log("backgroundTopColor:" + backgroundTopColor + " backgroundBottomColor:" + backgroundBottomColor);
    }
}