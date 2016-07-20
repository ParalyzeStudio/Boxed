using UnityEngine;
using UnityEngine.UI;

public class GameGUI : BaseGUI
{
    public Text m_levelNumber;
    public Level m_level { get; set; }

    //fading overlay
    public GradientBackground m_overlayPfb;
    public GradientBackground m_overlay { get; set; }

    public void Init(Level level)
    {
        m_level = level;
        BuildGradientOverlay();
    }

    /**
    * Build a gradient billboard sprite that we put on the near clip plane of the camera to achieve fading effects
    **/
    public void BuildGradientOverlay()
    {
        m_overlay = Instantiate(m_overlayPfb);
        GradientBackground background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        m_overlay.Init(background.m_startColor, background.m_endColor);
        m_overlay.name = "Overlay";

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);

        //set the background at a long distance from camera so it is behind all scene elements
        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.gameObject.transform.position;
        float distanceFromCamera = camera.nearClipPlane + 10;
        m_overlay.GetComponent<QuadAnimator>().SetPosition(cameraPosition + distanceFromCamera * camera.transform.forward);
    }

    public override void Show()
    {
        m_levelNumber.text = m_level.m_number.ToString();
        base.Show();

        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(1);
        overlayAnimator.FadeTo(0.0f, 0.5f);
    }

    public override void Dismiss()
    {
        base.Dismiss();
        QuadAnimator overlayAnimator = m_overlay.GetComponent<QuadAnimator>();
        overlayAnimator.SetOpacity(0);
        overlayAnimator.FadeTo(1.0f, 0.5f, 0, ValueAnimator.InterpolationType.LINEAR, true);
    }

    public void OnClickRestart()
    {
        Dismiss();
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(new CallFuncHandler.CallFunc(GameController.GetInstance().RestartLevel), 0.5f);
    }
}
