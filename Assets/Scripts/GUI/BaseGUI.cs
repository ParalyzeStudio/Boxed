using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(CanvasGroupFade))]
public class BaseGUI : MonoBehaviour
{
    public virtual void Show()
    {
        //CanvasGroup canvasGroup = this.GetComponent<CanvasGroup>();
        //canvasGroup.alpha = 0;

        //CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        //canvasGroupFade.m_opacity = 0;
        //canvasGroupFade.FadeIn();

        //FSGradientBillboardQuad overlay = GameController.GetInstance().GetComponent<GUIManager>().m_overlay;
        //overlay.GetComponent<QuadAnimator>().FadeTo(0.0f, 1, 0.3f);
    }

    public virtual void Dismiss(bool bDestroyOnFinish = false)
    {
        //CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        //canvasGroupFade.FadeOut(bDestroyOnFinish);

        //FSGradientBillboardQuad overlay = GameController.GetInstance().GetComponent<GUIManager>().m_overlay;
        //overlay.GetComponent<QuadAnimator>().FadeTo(1.0f, 0.5f);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
