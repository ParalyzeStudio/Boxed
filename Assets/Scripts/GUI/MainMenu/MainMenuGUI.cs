using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGUI : BaseGUI
{
    public Image m_title;
    public PlayButton m_playBtn;
    public Text m_playText;
    public Button m_settingsBtn;
    public Button m_trophyBtn;
    public Button m_resetBtn;
    public Button m_musicBtn;
    public Button m_soundBtn;
    public Button m_infoBtn;
    public Image m_musicBtnCross;
    public Image m_soundBtnCross;

    private bool m_settingsSubButtonsVisible;

    public Image[] m_subSettingsLines;

    private bool m_animatingSubSettings;

    public Image m_darkVeil;
    public GameObject m_confirmResetWindow;
    private bool m_confirmResetWindowVisible;

    private const float TITLE_SHOW_DELAY = 1.0f;
    private const float SIDE_BUTTONS_SHOW_DELAY = 2.0f;
    private const float PLAY_BUTTON_SHOW_DELAY = 2.0f;

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
        m_confirmResetWindowVisible = false;

        //Translate down and fade in title
        GUIImageAnimator titleAnimator = m_title.GetComponent<GUIImageAnimator>();
        titleAnimator.SyncPositionFromTransform();
        titleAnimator.SetPosition(titleAnimator.GetPosition() + new Vector3(0, 50, 0));
        titleAnimator.TranslateBy(new Vector3(0, -50, 0), 0.75f, TITLE_SHOW_DELAY);
        titleAnimator.SetOpacity(0);
        titleAnimator.FadeTo(1.0f, 1.0f, TITLE_SHOW_DELAY);

        //animate side buttons
        GUIImageAnimator trophyButtonAnimator = m_trophyBtn.GetComponent<GUIImageAnimator>();
        trophyButtonAnimator.SetScale(Vector3.zero);
        trophyButtonAnimator.ScaleTo(Vector3.one, 0.5f, SIDE_BUTTONS_SHOW_DELAY + 0.1f);

        GUIImageAnimator settingsButtonAnimator = m_settingsBtn.GetComponent<GUIImageAnimator>();
        settingsButtonAnimator.SetScale(Vector3.zero);
        settingsButtonAnimator.ScaleTo(Vector3.one, 0.5f, SIDE_BUTTONS_SHOW_DELAY);

        //show play button
        m_playBtn.Show(PLAY_BUTTON_SHOW_DELAY);

        //show play text
        GUITextAnimator playTextAnimator = m_playText.GetComponent<GUITextAnimator>();
        playTextAnimator.SetOpacity(0);
        playTextAnimator.FadeTo(1.0f, 0.75f, PLAY_BUTTON_SHOW_DELAY);
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

        if (button == m_musicBtn)
        {
            PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
            m_musicBtnCross.gameObject.SetActive(!pDataManager.IsMusicOn());
        }
        else if (button == m_soundBtn)
        {
            PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
            m_soundBtnCross.gameObject.SetActive(!pDataManager.IsSoundOn());
        }
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
            GameController.GetInstance().GetComponent<PersistentDataManager>().SavePrefs();
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
        //Translate up and fade out title
        GUIImageAnimator titleAnimator = m_title.GetComponent<GUIImageAnimator>();
        titleAnimator.TranslateBy(new Vector3(0, 50, 0), 0.75f, 0);
        titleAnimator.FadeTo(0.0f, 1.0f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        //fade out play button
        m_playBtn.Dismiss();

        //dismiss play text
        GUITextAnimator playTextAnimator = m_playText.GetComponent<GUITextAnimator>();
        playTextAnimator.FadeTo(0.0f, 0.75f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        //dismiss side buttons
        m_trophyBtn.interactable = false;
        GUIImageAnimator trophyButtonAnimator = m_trophyBtn.GetComponent<GUIImageAnimator>();
        trophyButtonAnimator.ScaleTo(Vector3.zero, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        m_settingsBtn.interactable = false;
        GUIImageAnimator settingsButtonAnimator = m_settingsBtn.GetComponent<GUIImageAnimator>();
        settingsButtonAnimator.ScaleTo(Vector3.zero, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        //Dismiss(true);
        Destroy(this.gameObject, 1.0f);

        CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();
        //callFuncHandler.AddCallFuncInstance(GameController.GetInstance().GetComponent<GUIManager>().DisplayLevelsGUI, 0.5f);
        callFuncHandler.AddCallFuncInstance(GameController.GetInstance().StartLevels, 1.25f);

        //dismiss sub settings buttons if visible
        if (m_settingsSubButtonsVisible)
        {
            GameController.GetInstance().GetComponent<PersistentDataManager>().SavePrefs();
            StartCoroutine("DismissSettingsSubButtons");
            StartCoroutine("DismissSubSettingsLine");
        }


        ////Generate random theme
        //ColorTheme defaultTheme = GameController.GetInstance().GetComponent<GUIManager>().m_themes.Themes[0];
        //ColorTheme randomTheme = new ColorTheme(defaultTheme, 180);

        //Color backgroundTopColor = randomTheme.m_backgroundGradientTopColor;
        //Color backgroundBottomColor = randomTheme.m_backgroundGradientBottomColor;

        //Debug.Log("backgroundTopColor:" + backgroundTopColor + " backgroundBottomColor:" + backgroundBottomColor);
    }

    public void OnClickReset()
    {
        if (!m_confirmResetWindowVisible)
        {
            m_darkVeil.gameObject.SetActive(true);
            m_confirmResetWindowVisible = true;
            m_confirmResetWindow.SetActive(true);
        }
        else
        {
            m_darkVeil.gameObject.SetActive(false);
            m_confirmResetWindow.SetActive(false);
            m_confirmResetWindowVisible = false;
        }       
    }

    public void OnClickConfirmReset()
    {
        GameController.GetInstance().ResetGame();
        m_darkVeil.gameObject.SetActive(false);
        m_confirmResetWindow.SetActive(false);
        m_confirmResetWindowVisible = false;
    }

    public void OnClickDismissResetWindow()
    {
        m_darkVeil.gameObject.SetActive(false);
        m_confirmResetWindow.SetActive(false);
        m_confirmResetWindowVisible = false;
    }

    public void OnClickSound()
    {
        //Toggle the value stored in prefs
        PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        bool isSoundActive = pDataManager.IsSoundOn();
        pDataManager.SetSoundOn(!isSoundActive);

        //Toggle the cross on the button icon
        m_soundBtnCross.gameObject.SetActive(isSoundActive);

        //TODO Toggle the actual sound
    }

    public void OnClickMusic()
    {
        //Toggle the value stored in prefs
        PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        bool isMusicActive = pDataManager.IsMusicOn();
        pDataManager.SetMusicOn(!isMusicActive);

        //Toggle the cross on the button icon
        m_musicBtnCross.gameObject.SetActive(isMusicActive);

        //TODO Toggle the actual music
    }

    public void OnClickInfo()
    {
        Debug.Log("OnClickInfo");
    }
}