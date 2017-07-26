using System.Collections;
using UnityEngine;

public class ResetConfirm : GameWindowContent
{
    public void OnClickConfirm()
    {
        MainPageMenu parentWindow = this.transform.parent.GetComponent<MainPageMenu>();
        parentWindow.StartCoroutine(PerformReset());
    }

    public void OnClickCancel()
    {
        GoToSettings();
    }

    public void OnClickGoBack()
    {
        GoToSettings();
    }

    private IEnumerator PerformReset()
    {
        ////Display a message announcing reset has been correctly performed
        //m_resetChoice.Dismiss();

        //yield return new WaitForSeconds(m_resetChoice.m_animationDuration);
        //m_resetChoice.ResetPositionAfterDelay(0);
        //m_resetChoice.gameObject.SetActive(false);

        MainPageMenu parentWindow = this.transform.parent.GetComponent<MainPageMenu>();
        yield return parentWindow.StartCoroutine(parentWindow.DismissCurrentContent());

        //Perform actual reset
        GameController.GetInstance().GetComponent<PersistentDataManager>().OnGameReset();
        GameController.GetInstance().GetLevelManager().OnGameReset();

        //update the background color of the main menu
        ThemeManager.Theme theme = GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme();
        Color bgTopColor = theme.m_backgroundGradientTopColor;
        Color bgBottomColor = theme.m_backgroundGradientBottomColor;
        GUIManager guiManager = GameController.GetInstance().GetGUIManager();
        guiManager.m_background.ChangeColorsTo(bgTopColor, bgBottomColor, 0.5f);

        //m_resetConfirmation.gameObject.SetActive(true);
        //m_resetConfirmation.SetOpacity(0);
        //m_resetConfirmation.Show();

        yield return parentWindow.StartCoroutine(parentWindow.ShowContentForID(MainPageMenu.ContentID.RESET_DONE));
    }

    private void GoToSettings()
    {
        MainPageMenu parentWindow = this.transform.parent.GetComponent<MainPageMenu>();
        parentWindow.ShowBackButton();
        parentWindow.StartCoroutine(parentWindow.ShowContentForID(MainPageMenu.ContentID.SETTINGS));
    }
}
