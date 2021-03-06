﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : GameWindow
{
    public GameWindowContent m_mainWindowContent;
    public GameWindowContent m_confirmHomeWindowContent;
    public Shop m_shopWindowContentPfb;
    private Shop m_shopWindowContent;
    public Settings m_settingsContentPfb;
    private Settings m_settingsContent;

    //Popup windows that show when clicking the solution button
    public Image m_darkFullScreenBackground;
    public SolutionReveal m_solutionRevealPfb;
    public SolutionConfirmPurchase m_solutionConfirmPurchasePfb;

    public override bool Show()
    {
        if (base.ShowContent(m_mainWindowContent))
        {
            ShowCreditsAmount(BACKGROUND_FADE_DURATION);

            return true;
        }

        return false;
    }

    public override bool Dismiss()
    {
        if (base.Dismiss())
        {
            GameController.GetInstance().m_gameStatus = GameController.GameStatus.RUNNING;
            DismissCreditsAmount();
            return true;
        }

        return false;
    }

    public void ShowDarkBackground()
    {
        //fade in dark background
        GUIElementAnimator backgroundAnimator = m_darkFullScreenBackground.GetComponent<GUIElementAnimator>();
        backgroundAnimator.gameObject.SetActive(true);
        backgroundAnimator.SetOpacity(0);
        backgroundAnimator.FadeTo(0.75f, 0.3f);
    }

    public void DismissDarkBackground()
    {
        GUIElementAnimator backgroundAnimator = m_darkFullScreenBackground.GetComponent<GUIElementAnimator>();
        backgroundAnimator.FadeTo(0, 0.3f);
        StartCoroutine(DeactivateBackgroundAfterDelay(0.3f));
    }

    public void OnClickHome()
    {
        //simply dismiss buttons
        StartCoroutine(DismissCurrentContent());
        DismissBackButton();

        //display confirmation
        StartCoroutine(ShowContentAfterDelay(m_confirmHomeWindowContent, GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
    }

    public void OnClickSolution()
    {
        ShowDarkBackground();

        //display correct solution popup
        ShowSolution();
    }

    public void ShowSolution(float delay = 0)
    {
        LevelData levelData = GameController.GetInstance().GetLevelManager().GetLevelDataForCurrentLevel();
        GameWindowElement solutionPopup;
        if (levelData.m_solutionPurchased)
            solutionPopup = Instantiate(m_solutionRevealPfb);
        else
            solutionPopup = Instantiate(m_solutionConfirmPurchasePfb);
        solutionPopup.transform.SetParent(this.transform, false);
        solutionPopup.gameObject.SetActive(true);
        solutionPopup.Show(delay);
    }

    public void OnClickSettings()
    {
        m_settingsContent = Instantiate(m_settingsContentPfb);
        m_settingsContent.transform.SetParent(this.transform, false);
        StartCoroutine(DismissCurrentContent());
        m_settingsContent.gameObject.SetActive(false);
        StartCoroutine(ShowContentAfterDelay(m_settingsContent, GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
    }

    public void OnClickShop()
    {
        m_shopWindowContent = Instantiate(m_shopWindowContentPfb);
        m_shopWindowContent.transform.SetParent(this.transform, false);
        StartCoroutine(DismissCurrentContent());
        StartCoroutine(ShowContentAfterDelay(m_shopWindowContent, GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
    }

    public void OnClickGameCenter()
    {
        Debug.Log(">>>OnClickGameCenter");
    }

    public override void OnClickBack()
    {
        if (m_content != null)
        {
            if (m_content == m_shopWindowContent || m_content == m_settingsContent)
            {
                if (m_content == m_settingsContent)
                    m_settingsContent.SaveSettings();

                StartCoroutine(ShowContentAfterDelay(m_mainWindowContent, GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
                StartCoroutine(DismissCurrentContent());
            }
            else
                base.OnClickBack();
        }

        //if (m_content == m_mainWindowContent)
        //    Dismiss();
        //else if (m_content == m_shopWindowContent)
        //    StartCoroutine(ShowContentAfterDelay(m_mainWindowContent, GameWindowElement.ELEMENT_ANIMATION_DURATION));

        //StartCoroutine(DismissCurrentContent());
    }

    public void OnClickConfirmHome()
    {
        GameController.GetInstance().ClearLevel();

        StartCoroutine(DismissCurrentContent());

        GUIManager guiManager = GameController.GetInstance().GetGUIManager();
        QuadAnimator overlayAnimator = guiManager.m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.FadeTo(0.0f, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, true);

        StartCoroutine(DestroyAfterDelay(0.5f));
        GameController.GetInstance().GetGUIManager().DestroyCurrentGUI();
        //GameController.GetInstance().GetGUIManager().DestroyInterLevelScreen();

        StartCoroutine(GameController.GetInstance().StartMainMenu(0.5f));
    }

    public void OnClickCancelHome()
    {
        StartCoroutine(DismissCurrentContent());
        ShowBackButton(GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION);
        StartCoroutine(ShowContentAfterDelay(m_mainWindowContent, GameWindowElement.DEFAULT_ELEMENT_ANIMATION_DURATION));
    }

    private IEnumerator DeactivateBackgroundAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        m_darkFullScreenBackground.gameObject.SetActive(false);
    }

    //protected override IEnumerator ShowContentAfterDelay(GameWindowContent content, float delay)
    //{
    //    content.MakeInvisible();

    //    yield return base.ShowContentAfterDelay(content, delay);

    //    yield return content.Show();

    //    m_state = State.SET;

    //    yield return null;
    //}   
}
