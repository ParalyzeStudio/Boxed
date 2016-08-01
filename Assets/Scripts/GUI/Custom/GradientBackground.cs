using UnityEngine;

/**
* Use this class to render a background image with vertical gradient
**/
public class GradientBackground : BillboardSprite
{
    public Color m_startColor { get; set; }
    public Color m_endColor { get; set; }

    private bool m_colorWheel;

    private HSVColor m_startHSV;
    private HSVColor m_endHSV;
    
    private const float HUE_VARIATION_SPEED = 60.0f;

    public void Init(Color startColor, Color endColor, bool bColorWheel = false)
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        base.Init(camera, null, false);
        
        this.name = "Background";

        m_colorWheel = bColorWheel;
        if (bColorWheel)
        {
            m_startHSV = new HSVColor(2, 0.5f, 0.75f);
            m_endHSV = new HSVColor(332, 0.5f, 0.5f);

            startColor = m_startHSV.ToRGBA();
            endColor = m_endHSV.ToRGBA();

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

            m_startHSV.TranslateHue(dHue);
            m_endHSV.TranslateHue(dHue);

            m_startColor = m_startHSV.ToRGBA();
            m_endColor = m_endHSV.ToRGBA();

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
