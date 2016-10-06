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

        GradientBackground overlay = GameController.GetInstance().GetComponent<GUIManager>().m_overlay;
        overlay.FadeOut(0.5f);
    }

    public virtual void Dismiss(bool bDestroyOnFinish = false)
    {
        CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        canvasGroupFade.FadeOut(bDestroyOnFinish);

        GradientBackground overlay = GameController.GetInstance().GetComponent<GUIManager>().m_overlay;
        overlay.FadeIn(0.5f);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
