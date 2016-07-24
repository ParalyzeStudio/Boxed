using UnityEngine;

/**
* Use this class to render a background image with vertical gradient
**/
public class GradientBackground : BillboardSprite
{
    public Color m_startColor { get; set; }
    public Color m_endColor { get; set; }

    private bool m_colorWheel;

    private Vector3 m_startTSB;
    private Vector3 m_endTSB;
    
    private const float HUE_VARIATION_SPEED = 60.0f;

    public void Init(Color startColor, Color endColor, bool bColorWheel = false)
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        base.Init(camera, null, false);
        
        this.name = "Background";

        m_colorWheel = bColorWheel;
        if (bColorWheel)
        {
            m_startTSB = new Vector3(2, 0.5f, 0.75f);
            m_endTSB = new Vector3(332, 0.5f, 0.5f);

            startColor = ColorUtils.GetRGBAColorFromTSB(m_startTSB, 1);
            endColor = ColorUtils.GetRGBAColorFromTSB(m_endTSB, 1);

            m_startColor = startColor;
            m_endColor = endColor;
        }
        else
        {
            m_startColor = startColor;
            m_endColor = endColor;
        }

        InvalidateColors();

        m_size = ScreenUtils.GetScreenSize();

        //set the background at a long distance from camera so it is behind all scene elements
        Vector3 cameraPosition = m_camera.gameObject.transform.position;
        float distanceFromCamera = camera.farClipPlane;
        QuadAnimator backgroundAnimator = this.GetComponent<QuadAnimator>();
        backgroundAnimator.SetPosition(cameraPosition + distanceFromCamera * m_camera.transform.forward);
    }

    private void InvalidateColors()
    {
        SetColors(new Color[4] { m_startColor, m_startColor, m_endColor, m_endColor });
    }

    public void Update()
    {
        if (m_colorWheel)
        {
            float dt = Time.deltaTime;
            float dHue = HUE_VARIATION_SPEED * dt;

            m_startTSB += new Vector3(dHue, 0, 0);
            m_endTSB += new Vector3(dHue, 0, 0);

            m_startColor = ColorUtils.GetRGBAColorFromTSB(m_startTSB, 1);
            m_endColor = ColorUtils.GetRGBAColorFromTSB(m_endTSB, 1);

            InvalidateColors();
        }
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
