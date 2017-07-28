using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIElementAnimator))]
public class GameWindowElement : CanvasGroupFade
{
    public float m_animationDuration = DEFAULT_ELEMENT_ANIMATION_DURATION;

    private const float DEFAULT_ELEMENT_TRANSLATION_LENGTH = 100.0f;
    public const float DEFAULT_ELEMENT_ANIMATION_DURATION = 0.4f;

    private Button m_button; //a possible button associated to this element

    public virtual void Show(float delay = 0.0f)
    {
        //translate the button
        //GUIImageAnimator elementAnimator = GetComponent<GUIImageAnimator>();
        //elementAnimator.SyncPositionFromTransform();
        //elementAnimator.SetPosition(elementAnimator.GetPosition() - new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0));
        //elementAnimator.TranslateBy(new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0), m_animationDuration, m_animationDelay);

        //fade in the button
        FadeTo(1.0f, 1.25f * m_animationDuration, delay);
    }

    public virtual void Dismiss(float delay = 0.0f)
    {
        //translate the button
        GUIElementAnimator elementAnimator = GetComponent<GUIElementAnimator>();
        elementAnimator.SyncPositionFromTransform();
        elementAnimator.TranslateBy(new Vector3(0, DEFAULT_ELEMENT_TRANSLATION_LENGTH, 0), m_animationDuration, delay);

        //fade in the button
        FadeTo(0.0f, m_animationDuration, delay);
    }

    public IEnumerator ActivateAfterDelay(float delay, float initialOpacity)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        gameObject.SetActive(true);
        SetOpacity(initialOpacity);
    }

    public IEnumerator ShowAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        Show();
    }

    /**
    * Reset element initial position and deactivate it
    **/
    public IEnumerator ResetPositionAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        GUIElementAnimator elementAnimator = GetComponent<GUIElementAnimator>();
        elementAnimator.SyncPositionFromTransform();
        elementAnimator.SetPosition(elementAnimator.GetPosition() - new Vector3(0, DEFAULT_ELEMENT_TRANSLATION_LENGTH, 0));
    }

    public IEnumerator DeactivateAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }

    /**
    * Return a Button if such an component is attached to this GameWindowElement
    * Return null otherwise
    **/
    public Button GetButton()
    {
        if (m_button == null)
            m_button = GetComponentInChildren<Button>();

        return m_button;
    }
}
