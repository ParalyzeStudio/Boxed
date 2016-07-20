using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(CanvasGroupFade))]
public class BaseGUI : MonoBehaviour
{
    public virtual void Show()
    {
        CanvasGroup canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        canvasGroupFade.m_opacity = 0;
        canvasGroupFade.FadeIn();
    }

    public virtual void Dismiss()
    {
        CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        canvasGroupFade.FadeOut();
    }
}
