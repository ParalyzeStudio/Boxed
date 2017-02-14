using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGUI : BaseGUI
{
    public Button m_resetBtn;
    public Button m_musicBtn;
    public Button m_soundBtn;
    public Button m_infoBtn;

    private bool m_settingsSubButtonsVisible;

    public Image[] m_subSettingsLines;

    private bool m_animatingSubSettings;
    
    public void Start()
    {
        DismissSingleSubButton(m_resetBtn, false);
        DismissSingleSubButton(m_musicBtn, false);
        DismissSingleSubButton(m_soundBtn, false);
        DismissSingleSubButton(m_infoBtn, false);

        for (int i = 0; i != m_subSettingsLines.Length; i++)
        {
            DismissSubSettingsLineSegment(m_subSettingsLines[i], false);
        }

        m_settingsSubButtonsVisible = false;
        m_animatingSubSettings = false;
    }

    /**
    * Animate settings sub buttons sequentially to show them
    **/
    private IEnumerator ShowSettingsSubButtons()
    {
        m_animatingSubSettings = true;

        yield return new WaitForSeconds(0.1f);

        ShowSingleSubButton(m_resetBtn);
        yield return new WaitForSeconds(0.1f);

        ShowSingleSubButton(m_musicBtn);
        yield return new WaitForSeconds(0.1f);

        ShowSingleSubButton(m_soundBtn);
        yield return new WaitForSeconds(0.1f);

        ShowSingleSubButton(m_infoBtn);
        yield return new WaitForSeconds(0.1f);

        m_animatingSubSettings = false;

        yield return null;
    }

    /**
    * Animate settings sub buttons sequentially to dismiss them
    **/
    private IEnumerator DismissSettingsSubButtons()
    {
        m_animatingSubSettings = true;

        DismissSingleSubButton(m_infoBtn);
        yield return new WaitForSeconds(0.1f);

        DismissSingleSubButton(m_soundBtn);
        yield return new WaitForSeconds(0.1f);

        DismissSingleSubButton(m_musicBtn);
        yield return new WaitForSeconds(0.1f);

        DismissSingleSubButton(m_resetBtn);
        yield return new WaitForSeconds(0.1f);

        m_animatingSubSettings = false;

        yield return null;
    }

    /**
    * Animate segments of a line sequentially to show them
    **/
    private IEnumerator ShowSubSettingsLine()
    {
        for (int i = 0; i != m_subSettingsLines.Length; i++)
        {
            ShowSubSettingsLineSegment(m_subSettingsLines[i]);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    /**
    * Animate segments of a line sequentially to dismiss them
    **/
    private IEnumerator DismissSubSettingsLine()
    {
        for (int i = m_subSettingsLines.Length - 1; i >= 0; i--)
        {
            DismissSubSettingsLineSegment(m_subSettingsLines[i]);
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    /**
    * Show one single settings sub button
    **/
    private void ShowSingleSubButton(Button button, bool animated = true)
    {
        GUIImageAnimator buttonAnimator = button.GetComponent<GUIImageAnimator>();
        if (!animated)
            buttonAnimator.SetScale(Vector3.one);
        else        
            buttonAnimator.ScaleTo(Vector3.one, 0.3f);
    }

    /**
    * Dismiss one single settings sub button
    **/
    private void DismissSingleSubButton(Button button, bool animated = true)
    {
        GUIImageAnimator buttonAnimator = button.GetComponent<GUIImageAnimator>();
        if (!animated)
            buttonAnimator.SetScale(Vector3.zero);
        else
            buttonAnimator.ScaleTo(Vector3.zero, 0.3f);
    }

    private void ShowSubSettingsLineSegment(Image lineSegment, bool animated = true)
    {
        GUIImageAnimator segmentAnimator = lineSegment.GetComponent<GUIImageAnimator>();
        if (!animated)
            segmentAnimator.SetScale(Vector3.one);
        else
            segmentAnimator.ScaleTo(Vector3.one, 0.2f);
    }

    private void DismissSubSettingsLineSegment(Image lineSegment, bool animated = true)
    {
        GUIImageAnimator segmentAnimator = lineSegment.GetComponent<GUIImageAnimator>();
        if (!animated)
            segmentAnimator.SetScale(new Vector3(0, 1, 1));
        else
            segmentAnimator.ScaleTo(new Vector3(0, 1, 1), 0.2f);
    }

    public void OnClickSettings()
    {
        if (m_animatingSubSettings)
            return;

        m_settingsSubButtonsVisible = !m_settingsSubButtonsVisible;
        if (m_settingsSubButtonsVisible)
        {
            StartCoroutine("ShowSettingsSubButtons");
            StartCoroutine("ShowSubSettingsLine");
        }
        else
        {
            StartCoroutine("DismissSettingsSubButtons");
            StartCoroutine("DismissSubSettingsLine");
        }
    }

    public void OnClickTrophy()
    {
        Debug.Log("OnClickTrophy");
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

    public void OnClickReset()
    {

    }

    public void OnClickSound()
    {

    }

    public void OnClickMusic()
    {

    }

    public void OnClickInfo()
    {
        Debug.Log("OnClickInfo");
    }
}