using UnityEngine;

/**
* Use this class to render a background image with vertical gradient
**/
public class GradientBackground : BillboardSprite
{
    public Color m_topColor { get; set; }
    public Color m_bottomColor { get; set; }

    private bool m_colorWheel;

    private HSVColor m_startHSV;
    private HSVColor m_endHSV;
    
    private const float HUE_VARIATION_SPEED = 60.0f;

    //Color variation
    bool m_colorVariating;
    Color m_topFromColor;
    Color m_bottomFromColor;
    Color m_topToColor;
    Color m_bottomToColor;
    float m_duration;
    float m_elapsedTime;
    float m_delay;

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

            m_topColor = startColor;
            m_bottomColor = endColor;
        }
        else
        {
            m_topColor = startColor;
            m_bottomColor = endColor;
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
        SetColors(new Color[4] { m_topColor, m_topColor, m_bottomColor, m_bottomColor });
    }

    public void ChangeColorsTo(Color topToColor, Color bottomToColor, float duration, float delay = 0.0f)
    {
        m_colorVariating = true;
        m_topFromColor = m_topColor;
        m_bottomFromColor = m_bottomColor;
        m_topToColor = topToColor;
        m_bottomToColor = bottomToColor;
        m_duration = duration;
        m_delay = delay;
        m_elapsedTime = 0;
    }

    protected virtual void UpdateColor(float dt)
    {
        if (m_colorVariating)
        {
            bool inDelay = (m_elapsedTime < m_delay);
            m_elapsedTime += dt;
            if (m_elapsedTime > m_delay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_elapsedTime - m_delay;
                float effectiveElapsedTime = m_elapsedTime - m_delay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;

                //Top color variation
                Vector4 colorVariation = m_topToColor - m_topFromColor;
                float deltaColorX = (t2 - t1) / m_duration * colorVariation.x;
                float deltaColorY = (t2 - t1) / m_duration * colorVariation.y;
                float deltaColorZ = (t2 - t1) / m_duration * colorVariation.z;
                float deltaColorW = (t2 - t1) / m_duration * colorVariation.w;
                Color deltaColor = new Color(deltaColorX, deltaColorY, deltaColorZ, deltaColorW);

                m_topColor += deltaColor;

                //Bottom color variation
                colorVariation = m_bottomToColor - m_bottomFromColor;
                deltaColorX = (t2 - t1) / m_duration * colorVariation.x;
                deltaColorY = (t2 - t1) / m_duration * colorVariation.y;
                deltaColorZ = (t2 - t1) / m_duration * colorVariation.z;
                deltaColorW = (t2 - t1) / m_duration * colorVariation.w;
                deltaColor = new Color(deltaColorX, deltaColorY, deltaColorZ, deltaColorW);

                m_bottomColor += deltaColor;

                if (effectiveElapsedTime > m_duration)
                {
                    m_topColor = m_topToColor;
                    m_bottomColor = m_bottomToColor;
                    m_colorVariating = false;
                }

                InvalidateColors();
            }
        }
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        if (m_colorWheel)
        {           
            float dHue = HUE_VARIATION_SPEED * dt;

            m_startHSV.TranslateHue(dHue);
            m_endHSV.TranslateHue(dHue);

            m_topColor = m_startHSV.ToRGBA();
            m_bottomColor = m_endHSV.ToRGBA();

            InvalidateColors();
        }

        UpdateColor(dt);
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
        return Color.Lerp(m_topColor, m_bottomColor, distanceFromTop);
    }
}
