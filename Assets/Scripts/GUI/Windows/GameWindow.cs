using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
    protected State m_state;
    protected enum State
    {
        ENTERING,
        SET,
        LEAVING,
        DISMISSED
    }

    protected const float BACKGROUND_FADE_DURATION = 0.55f;

    public Color m_backgroundTopColor;
    public Color m_backgroundBottomColor;

    public GameWindowElement m_backBtn; //the back button to dismiss a content or the whole window
    public CreditsAmount m_creditsAmounts; //label displaying the current amount of credits
    private Text m_creditsAmountText;
    protected GameWindowContent m_content; //the content currently displayed in this window

    public void Awake()
    {
        m_state = State.DISMISSED;
    }

    public virtual bool Show()
    {
        return false;
    }

    protected virtual bool ShowContent(GameWindowContent content)
    {
        if (m_state == State.ENTERING || m_state == State.LEAVING)
            return false;

        m_state = State.ENTERING;

        //fade out the GUI canvas group
        CanvasGroupFade GUICanvasGroupAnimator = (GameController.GetInstance().GetGUIManager().m_currentGUI).GetComponent<CanvasGroupFade>();
        GUICanvasGroupAnimator.FadeTo(0.0f, BACKGROUND_FADE_DURATION);

        //also display a gradient background for this menu
        GUIManager guiManager = GameController.GetInstance().GetComponent<GUIManager>();
        guiManager.BuildGradientOverlay();
        guiManager.m_overlay.m_topColor = m_backgroundTopColor;
        guiManager.m_overlay.m_bottomColor = m_backgroundBottomColor;
        guiManager.m_overlay.InvalidateColors();

        QuadAnimator overlayAnimator = guiManager.m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);
        overlayAnimator.FadeTo(1.0f, BACKGROUND_FADE_DURATION);

        //show the content
        StartCoroutine(ShowContentAfterDelay(content, BACKGROUND_FADE_DURATION));

        return true;
    }

    public virtual bool Dismiss()
    {
        if (m_state == State.ENTERING || m_state == State.LEAVING)
            return false;

        m_state = State.LEAVING;

        DismissBackButton();
        DismissCreditsAmount();

        if (m_content != null)
            StartCoroutine(DismissCurrentContent());

        float delay = 0.0f; //delay for removing background
        //fade in the GUI canvas group
        CanvasGroupFade guiCanvasGroupAnimator = (GameController.GetInstance().GetGUIManager().m_currentGUI).GetComponent<CanvasGroupFade>();
        guiCanvasGroupAnimator.FadeTo(1.0f, 2 * BACKGROUND_FADE_DURATION, delay);

        //dismiss the gradient background for this menu
        GUIManager guiManager = GameController.GetInstance().GetGUIManager();
        QuadAnimator overlayAnimator = guiManager.m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.FadeTo(0.0f, 2 * BACKGROUND_FADE_DURATION, delay, ValueAnimator.InterpolationType.LINEAR, true);

        //deactivate after some time
        StartCoroutine(DestroyAfterDelay(2 * BACKGROUND_FADE_DURATION + delay));

        return true;
    }

    protected virtual IEnumerator ShowContentAfterDelay(GameWindowContent content, float delay, float timeSpacing = GameWindowContent.DEFAULT_TIME_SPACING)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        m_content = content;
        content.gameObject.SetActive(true);
        yield return StartCoroutine(content.Show(timeSpacing));

        m_state = State.SET;
    }

    public virtual IEnumerator DismissCurrentContent()
    {
        if (m_content != null)
        {
            GameWindowContent dismissedContent = m_content;
            m_content = null;
            yield return StartCoroutine(dismissedContent.Dismiss());
            dismissedContent.gameObject.SetActive(false);
        }
    }

    protected IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(this.gameObject);
        m_state = State.DISMISSED;
    }

    public void ShowBackButton(float delay = 0.0f)
    {
        EnableBackButton();
        m_backBtn.gameObject.SetActive(true);
        m_backBtn.SetOpacity(0);
        m_backBtn.Show(delay);
    }

    public void DismissBackButton()
    {
        if (m_backBtn != null)
        {
            DisableBackButton();
            m_backBtn.Dismiss();
            StartCoroutine(m_backBtn.ResetPositionAfterDelay(GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
            StartCoroutine(m_backBtn.DeactivateAfterDelay(GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
        }
    }

    public void DisableBackButton()
    {
        m_backBtn.GetComponent<Button>().interactable = false;

        //tmp, visual debug
        //Image arrow = m_backBtn.GetComponentsInChildren<Image>()[1];
        //arrow.color = ColorUtils.FadeColor(arrow.color, 0.3f);
        //Text backText = m_backBtn.GetComponentInChildren<Text>();
        //backText.color = ColorUtils.FadeColor(backText.color, 0.3f);
    }

    public void EnableBackButton()
    {
        m_backBtn.GetComponent<Button>().interactable = true;

        //tmp, visual debug
        //Image arrow = m_backBtn.GetComponentsInChildren<Image>()[1];
        //arrow.color = ColorUtils.FadeColor(arrow.color, 1.0f);
        //Text backText = m_backBtn.GetComponentInChildren<Text>();
        //backText.color = ColorUtils.FadeColor(backText.color, 1.0f);
    }

    public void ShowCreditsAmount(float delay = 0.0f)
    {
        m_creditsAmounts.gameObject.SetActive(true);
        m_creditsAmounts.InvalidateAmount();
        m_creditsAmounts.SetOpacity(0);
        m_creditsAmounts.m_animationDuration = 0.2f;
        m_creditsAmounts.Show(delay);
    }

    public void DismissCreditsAmount()
    {
        if (m_creditsAmounts != null)
        {
            m_creditsAmounts.Dismiss();
            StartCoroutine(m_creditsAmounts.ResetPositionAfterDelay(GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
            StartCoroutine(m_creditsAmounts.DeactivateAfterDelay(GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
        }
    }
    
    public void InvalidateCreditsAmount(int newAmount)
    {
        GetCreditsAmountText().text = newAmount.ToString();
    }

    private Text GetCreditsAmountText()
    {
        if (m_creditsAmountText == null)
            m_creditsAmountText = m_creditsAmounts.GetComponentInChildren<Text>();
        return m_creditsAmountText;
    }

    public virtual void OnClickBack()
    {
        if (m_state != State.SET)
            return;

        if (m_content != null)
        {
            Dismiss();
            //StartCoroutine(DismissCurrentContent());
        }
    }
}
