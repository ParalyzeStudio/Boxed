using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GUIImageAnimator))]
public class GameWindowElement : CanvasGroupFade
{
    public float m_animationDuration = ELEMENT_ANIMATION_DURATION;
    public float m_animationDelay = 0;

    private const float ELEMENT_TRANSLATION_LENGTH = 100.0f;
    public const float ELEMENT_ANIMATION_DURATION = 0.2f;

    public virtual void Show()
    {
        //translate the button
        GUIImageAnimator elementAnimator = GetComponent<GUIImageAnimator>();
        elementAnimator.SyncPositionFromTransform();
        elementAnimator.SetPosition(elementAnimator.GetPosition() - new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0));
        elementAnimator.TranslateBy(new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0), m_animationDuration, m_animationDelay);

        //fade in the button
        SetOpacity(0);
        FadeTo(1.0f, m_animationDuration, m_animationDelay);
    }

    public virtual void Dismiss(float duration = ELEMENT_ANIMATION_DURATION, float delay = 0.0f)
    {
        //translate the button
        GUIImageAnimator elementAnimator = GetComponent<GUIImageAnimator>();
        elementAnimator.SyncPositionFromTransform();
        elementAnimator.TranslateBy(new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0), duration, delay);

        //fade in the button
        FadeTo(0.0f, duration, delay);
    }

    /**
    * Reset element initial position and deactivate it
    **/
    public IEnumerator ResetPositionAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        GUIImageAnimator elementAnimator = GetComponent<GUIImageAnimator>();
        elementAnimator.SyncPositionFromTransform();
        elementAnimator.SetPosition(elementAnimator.GetPosition() - new Vector3(0, ELEMENT_TRANSLATION_LENGTH, 0));
    }

    public IEnumerator DeactivateAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }
}
