using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGUI : BaseGUI
{
    public Image m_title;
    private const float TITLE_SHOW_DELAY = 1.0f;
    private const float MENU_BUTTON_SHOW_DELAY = 1.5f;
    private const float TOUCH_TO_PLAY_SHOW_DELAY = 1.5f;

    public GUIImageAnimator m_menuBtnAnimator;
    public MainPageMenu m_menuPfb;
    public MainPageMenu m_menu { get; set; }

    public Button m_playBtn;
    public GUIImageAnimator m_fingerIconAnimator;
    public GUITextAnimator m_touchToPlayAnimator;

    public PolygonEmitter m_circleEmitter;

    //public PlayButton m_playBtn;
    //public Text m_playText;
    //public Button m_settingsBtn;
    //public Button m_trophyBtn;
    //public Button m_resetBtn;
    //public Button m_musicBtn;
    //public Button m_soundBtn;
    //public Button m_infoBtn;
    //public Image m_musicBtnCross;
    //public Image m_soundBtnCross;

    //private bool m_settingsSubButtonsVisible;

    //public Image[] m_subSettingsLines;

    //private bool m_animatingSubSettings;

    //public Image m_darkVeil;
    //public GameObject m_confirmResetWindow;
    //private bool m_confirmResetWindowVisible;
    //private const float SIDE_BUTTONS_SHOW_DELAY = 2.0f;
    //private const float PLAY_BUTTON_SHOW_DELAY = 2.0f;

    public void Start()
    {
        //DismissSingleSubButton(m_resetBtn, false);
        //DismissSingleSubButton(m_musicBtn, false);
        //DismissSingleSubButton(m_soundBtn, false);
        //DismissSingleSubButton(m_infoBtn, false);

        //for (int i = 0; i != m_subSettingsLines.Length; i++)
        //{
        //    DismissSubSettingsLineSegment(m_subSettingsLines[i], false);
        //}

        //m_settingsSubButtonsVisible = false;
        //m_animatingSubSettings = false;
        //m_confirmResetWindowVisible = false;

        //Translate down and fade in title
        GUIImageAnimator titleAnimator = m_title.GetComponent<GUIImageAnimator>();
        titleAnimator.SyncPositionFromTransform();
        titleAnimator.SetPosition(titleAnimator.GetPosition() + new Vector3(0, 50, 0));
        titleAnimator.TranslateBy(new Vector3(0, -50, 0), 0.75f, TITLE_SHOW_DELAY);
        titleAnimator.SetOpacity(0);
        titleAnimator.FadeTo(1.0f, 1.0f, TITLE_SHOW_DELAY);

        //Translate left and fade in menu button
        m_menuBtnAnimator.SyncPositionFromTransform();
        Vector3 menuBtnFinalPosition = m_menuBtnAnimator.GetPosition();
        m_menuBtnAnimator.SetPosition(menuBtnFinalPosition + new Vector3(150, 0, 0));
        m_menuBtnAnimator.TranslateTo(menuBtnFinalPosition, 0.4f, MENU_BUTTON_SHOW_DELAY);
        m_menuBtnAnimator.SetOpacity(0);
        m_menuBtnAnimator.FadeTo(1.0f, 0.2f, MENU_BUTTON_SHOW_DELAY);

        //fade in touch to play group
        m_circleEmitter.m_active = false;
        StartCoroutine(SetCircleEmitterActiveAfterDelay(true, TOUCH_TO_PLAY_SHOW_DELAY));
        m_fingerIconAnimator.SetOpacity(0);
        m_fingerIconAnimator.FadeTo(1.0f, 0.3f, TOUCH_TO_PLAY_SHOW_DELAY);
        m_touchToPlayAnimator.SetOpacity(0);
        m_touchToPlayAnimator.FadeTo(1.0f, 0.3f, TOUCH_TO_PLAY_SHOW_DELAY);

        //make the play button interactable only after all elements have completed their animation
        m_playBtn.interactable = false;
        StartCoroutine(MakePlayButtonInteractableAfterDelay(MENU_BUTTON_SHOW_DELAY));

        ////animate side buttons
        //GUIImageAnimator trophyButtonAnimator = m_trophyBtn.GetComponent<GUIImageAnimator>();
        //trophyButtonAnimator.SetScale(Vector3.zero);
        //trophyButtonAnimator.ScaleTo(Vector3.one, 0.5f, SIDE_BUTTONS_SHOW_DELAY + 0.1f);

        //GUIImageAnimator settingsButtonAnimator = m_settingsBtn.GetComponent<GUIImageAnimator>();
        //settingsButtonAnimator.SetScale(Vector3.zero);
        //settingsButtonAnimator.ScaleTo(Vector3.one, 0.5f, SIDE_BUTTONS_SHOW_DELAY);

        ////show play button
        //m_playBtn.Show(PLAY_BUTTON_SHOW_DELAY);

        ////show play text
        //GUITextAnimator playTextAnimator = m_playText.GetComponent<GUITextAnimator>();
        //playTextAnimator.SetOpacity(0);
        //playTextAnimator.FadeTo(1.0f, 0.75f, PLAY_BUTTON_SHOW_DELAY);
    }

    private IEnumerator MakePlayButtonInteractableAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        m_playBtn.interactable = true;
    }
    
    private IEnumerator SetCircleEmitterActiveAfterDelay(bool active, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        m_circleEmitter.m_active = active;
    }

    public void OnClickPlay()
    {
        if (m_menu != null) //sub menu is active, swallow click
            return;

        //Translate up and fade out title
        GUIImageAnimator titleAnimator = m_title.GetComponent<GUIImageAnimator>();
        titleAnimator.TranslateBy(new Vector3(0, 50, 0), 0.75f, 0);
        titleAnimator.FadeTo(0.0f, 1.0f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        //same thing for touch to play UI elements
        m_fingerIconAnimator.FadeTo(0.0f, 0.3f);        
        m_touchToPlayAnimator.FadeTo(0.0f, 0.3f);

        //Translate right and fade out menu button
        m_menuBtnAnimator.SyncPositionFromTransform();
        m_menuBtnAnimator.TranslateBy(new Vector3(150, 0, 0), 0.4f);
        m_menuBtnAnimator.FadeTo(0.0f, 0.2f);

        //stop the emitter
        StartCoroutine(SetCircleEmitterActiveAfterDelay(false, 0));

        //start the next scene
        StartCoroutine(GameController.GetInstance().StartLevels(1.25f));
    }

    public void OnClickSubMenu()
    {
        if (m_menu == null)
        {
            m_menu = Instantiate(m_menuPfb);
            m_menu.transform.SetParent(GameController.GetInstance().GetGUIManager().m_canvas.transform, false);
            m_menu.Show();
        }
    }
}