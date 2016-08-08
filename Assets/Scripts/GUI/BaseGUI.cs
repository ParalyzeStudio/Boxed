using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(CanvasGroupFade))]
public class BaseGUI : MonoBehaviour
{
    //fading overlay
    public GradientBackground m_overlayPfb;
    public GradientBackground m_overlay { get; set; }

    public void Init()
    {
        if (m_overlay == null)
            BuildGradientOverlay();
    }

    /**
    * Build a gradient billboard sprite that we put on the near clip plane of the camera to achieve fading effects
    **/
    public void BuildGradientOverlay()
    {
        m_overlay = Instantiate(m_overlayPfb);
        GradientBackground background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        m_overlay.Init(background.m_topColor, background.m_bottomColor);
        m_overlay.name = "Overlay";

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);

        //set the background at a long distance from camera so it is behind all scene elements
        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.gameObject.transform.position;
        float distanceFromCamera = camera.nearClipPlane + 10;
        m_overlay.GetComponent<QuadAnimator>().SetPosition(cameraPosition + distanceFromCamera * camera.transform.forward);
    }

    public virtual void Show()
    {
        CanvasGroup canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        canvasGroupFade.m_opacity = 0;
        canvasGroupFade.FadeIn();

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(1);
        overlayAnimator.FadeTo(0.0f, 0.5f);
    }

    public virtual void Dismiss()
    {
        CanvasGroupFade canvasGroupFade = this.GetComponent<CanvasGroupFade>();
        canvasGroupFade.FadeOut();

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);
        overlayAnimator.FadeTo(1.0f, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, false);
    }

    public void DestroyOverlay()
    {
        Destroy(m_overlay.gameObject);
    }
}
