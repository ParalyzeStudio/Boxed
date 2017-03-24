using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public CanvasGroupFade m_backButton;
    public CanvasGroupFade m_homeButton;
    public CanvasGroupFade m_solutionButton;
    public CanvasGroupFade m_musicButton;
    public CanvasGroupFade m_soundButton;
    public CanvasGroupFade m_gameCenterButton;

    public CanvasGroupFade m_confirmHomeWindow;

    private State m_state;
    private enum State
    {
        ENTERING,
        SET,
        LEAVING,
        DISMISSED
    }

    private const float ELEMENT_ANIMATION_DURATION = 0.2f;
    private const float BACKGROUND_FADE_DURATION = 0.2f;
    private const float ELEMENT_TRANSLATION_LENGTH = 100.0f;

    public void Awake()
    {
        m_state = State.DISMISSED;
    }

    public void Show()
    {
        if (m_state == State.ENTERING || m_state == State.LEAVING)
            return;
        
        m_state = State.ENTERING;

        //display buttons
        IEnumerator showButtonsCoroutine = ShowButtonsAfterDelay(0.2f);
        StartCoroutine(showButtonsCoroutine);

        //also display a gradient background for this menu
        GUIManager guiManager = GameController.GetInstance().GetComponent<GUIManager>();
        guiManager.BuildGradientOverlay();
        QuadAnimator overlayAnimator = guiManager.m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);
        overlayAnimator.FadeTo(1.0f, BACKGROUND_FADE_DURATION);

        //fade out the gameGUI canvas group
        CanvasGroupFade gameGUICanvasGroupAnimator = ((GameGUI)guiManager.m_currentGUI).GetComponent<CanvasGroupFade>();
        gameGUICanvasGroupAnimator.FadeTo(0.0f, BACKGROUND_FADE_DURATION);
    }

    public void Dismiss()
    {
        if (m_state == State.ENTERING || m_state == State.LEAVING)
            return;

        m_state = State.LEAVING;
        StartCoroutine("DismissButtons");

        //dismiss the gradient background for this menu
        GUIManager guiManager = GameController.GetInstance().GetGUIManager();
        QuadAnimator overlayAnimator = guiManager.m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.FadeTo(0.0f, 0.45f);

        //fade in the gameGUI canvas group
        CanvasGroupFade gameGUICanvasGroupAnimator = ((GameGUI)guiManager.m_currentGUI).GetComponent<CanvasGroupFade>();
        gameGUICanvasGroupAnimator.FadeTo(1.0f, 0.45f);

        //deactivate after some time
        IEnumerator destroyCoroutine = DestroyAfterDelay(0.45f);
        StartCoroutine(destroyCoroutine);
    }

    public void OnClickHome()
    {
        //simply dismiss buttons
        StartCoroutine("DismissButtons");

        //display confirmation
        m_confirmHomeWindow.gameObject.SetActive(true);
        m_confirmHomeWindow.SetOpacity(0);
        ShowElement(m_confirmHomeWindow, 0.45f);
    }

    public void OnClickSolution()
    {
        Debug.Log(">>>OnClickSolution");
    }

    public void OnClickMusic()
    {
        Debug.Log(">>>OnClickMusic");
    }

    public void OnClickSound()
    {
        Debug.Log(">>>OnClickSound");
    }

    public void OnClickGameCenter()
    {
        Debug.Log(">>>OnClickGameCenter");
    }

    public void OnClickBack()
    {
        Dismiss();

        GameController.GetInstance().m_gameStatus = GameController.GameStatus.RUNNING;
    }

    public void OnClickConfirmHome()
    {
        GameController.GetInstance().ClearLevel();

        DismissElement(m_confirmHomeWindow);

        GUIManager guiManager = GameController.GetInstance().GetGUIManager();
        QuadAnimator overlayAnimator = guiManager.m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.FadeTo(0.0f, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        DestroyAfterDelay(0.5f);
        //GameController.GetInstance().GetGUIManager().DestroyCurrentGUI();
                
        StartCoroutine(GameController.GetInstance().StartMainMenu(0.5f));
    }

    public void OnClickCancelHome()
    {
        DismissElement(m_confirmHomeWindow);
        IEnumerator resetCoroutine = ResetElementAfterDelay(m_confirmHomeWindow, ELEMENT_ANIMATION_DURATION);
        StartCoroutine(resetCoroutine);

        IEnumerator showButtonsCoroutine = ShowButtonsAfterDelay(0.2f);
        StartCoroutine(showButtonsCoroutine);
    }

    private IEnumerator ShowButtonsAfterDelay(float initialDelay)
    {
        m_backButton.gameObject.SetActive(true);
        m_homeButton.gameObject.SetActive(true);
        m_solutionButton.gameObject.SetActive(true);
        m_musicButton.gameObject.SetActive(true);
        m_soundButton.gameObject.SetActive(true);
        m_gameCenterButton.gameObject.SetActive(true);

        m_backButton.SetOpacity(0);
        m_homeButton.SetOpacity(0);
        m_solutionButton.SetOpacity(0);
        m_musicButton.SetOpacity(0);
        m_soundButton.SetOpacity(0);
        m_gameCenterButton.SetOpacity(0);

        float buttonDelay = 0.05f;
        yield return new WaitForSeconds(initialDelay);
        ShowElement(m_backButton);
        yield return new WaitForSeconds(buttonDelay);
        ShowElement(m_homeButton);
        yield return new WaitForSeconds(buttonDelay);
        ShowElement(m_solutionButton);
        yield return new WaitForSeconds(buttonDelay);
        ShowElement(m_musicButton);
        yield return new WaitForSeconds(buttonDelay);
        ShowElement(m_soundButton);
        yield return new WaitForSeconds(buttonDelay);
        ShowElement(m_gameCenterButton);

        m_state = State.SET;

        yield return null;
    }

    private IEnumerator DismissButtons()
    {
        float delay = 0.05f;

        DismissElement(m_backButton);
        yield return new WaitForSeconds(delay);        
        DismissElement(m_homeButton);
        yield return new WaitForSeconds(delay);
        DismissElement(m_solutionButton);
        yield return new WaitForSeconds(delay);
        DismissElement(m_musicButton);
        yield return new WaitForSeconds(delay);
        DismissElement(m_soundButton);
        yield return new WaitForSeconds(delay);
        DismissElement(m_gameCenterButton);

        yield return new WaitForSeconds(ELEMENT_ANIMATION_DURATION);

        //reset buttons initial positions and deactivate them all at once because
        //deactivating an element could modify other element size. Therefore extend 
        //the duration so all elements are invisible before deactivating them
        IEnumerator resetCoroutine = ResetElementAfterDelay(m_backButton, 0);
        StartCoroutine(resetCoroutine);
        resetCoroutine = ResetElementAfterDelay(m_homeButton, 0);
        StartCoroutine(resetCoroutine);
        resetCoroutine = ResetElementAfterDelay(m_solutionButton, 0);
        StartCoroutine(resetCoroutine);
        resetCoroutine = ResetElementAfterDelay(m_musicButton, 0);
        StartCoroutine(resetCoroutine);
        resetCoroutine = ResetElementAfterDelay(m_soundButton, 0);
        StartCoroutine(resetCoroutine);
        resetCoroutine = ResetElementAfterDelay(m_gameCenterButton, 0);
        StartCoroutine(resetCoroutine);

        yield return null;
    }

    private void ShowElement(CanvasGroupFade element, float delay = 0.0f)
    {
        element.gameObject.SetActive(true);

        //translate the button
        GUIImageAnimator buttonAnimator = element.GetComponent<GUIImageAnimator>();
        buttonAnimator.SyncPositionFromTransform();
        buttonAnimator.SetPosition(buttonAnimator.GetPosition() - new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0));
        buttonAnimator.TranslateBy(new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0), ELEMENT_ANIMATION_DURATION, delay);

        //fade in the button
        element.FadeTo(1.0f, ELEMENT_ANIMATION_DURATION, delay);
    }

    private void DismissElement(CanvasGroupFade element, float delay = 0.0f)
    {
        //translate the button
        GUIImageAnimator buttonAnimator = element.GetComponent<GUIImageAnimator>();
        buttonAnimator.TranslateBy(new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0), ELEMENT_ANIMATION_DURATION, delay);

        //fade in the button
        element.FadeTo(0.0f, ELEMENT_ANIMATION_DURATION, delay);
    }

    private IEnumerator ResetElementAfterDelay(CanvasGroupFade element, float delay)
    {
        yield return new WaitForSeconds(delay);

        GUIImageAnimator elementAnimator = element.GetComponent<GUIImageAnimator>();
        elementAnimator.SetPosition(elementAnimator.GetPosition() - new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0));
        element.gameObject.SetActive(false);

        yield return null;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(this.gameObject);

        yield return null;
    }
}
