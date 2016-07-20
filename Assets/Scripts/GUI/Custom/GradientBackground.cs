using UnityEngine;

/**
* Use this class to render a background image with vertical gradient
**/
public class GradientBackground : BillboardSprite
{
    public Color m_startColor { get; set; }
    public Color m_endColor { get; set; }

    public void Init(Color startColor, Color endColor)
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        base.Init(camera, null, false);

        m_startColor = startColor;
        m_endColor = endColor;
        m_size = ScreenUtils.GetScreenSize();

        this.name = "Background";

        SetColors(new Color[4] { startColor, startColor, endColor, endColor});
        
        //set the background at a long distance from camera so it is behind all scene elements
        Vector3 cameraPosition = m_camera.gameObject.transform.position;
        float distanceFromCamera = camera.farClipPlane;
        QuadAnimator backgroundAnimator = this.GetComponent<QuadAnimator>();
        backgroundAnimator.SetPosition(cameraPosition + distanceFromCamera * m_camera.transform.forward);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        
        QuadAnimator backgroundAnimator = this.GetComponent<QuadAnimator>();
        backgroundAnimator.SetScale(new Vector3(m_size.x, m_size.y, 1));
    }

    /**
    * Return the color of a point at position 'viewportPosition' inside the viewport
    **/
    public Color GetColorAtViewportPosition(Vector2 viewportPosition)
    {
        float distanceFromTop = Mathf.Abs(viewportPosition.y - 1.0f);
        return Color.Lerp(m_startColor, m_endColor, distanceFromTop);
    }
}
