using UnityEngine;

/**
* Use this class to render a full screen, camera-facing quad mesh
**/
public class FSGradientBillboardQuad : BillboardQuad
{
    public Color m_topColor;
    public Color m_bottomColor;
    private Color m_prevTopColor;
    private Color m_prevBottomColor;

    private bool m_colorWheel;

    private HSVColor m_startHSV;
    private HSVColor m_endHSV;
    
    private const float HUE_VARIATION_SPEED = 60.0f;

    QuadAnimator m_animator;
    Transform m_cameraTransform;

    //Color variation
    private bool m_colorVariating;
    private Color m_topFromColor;
    private Color m_bottomFromColor;
    private Color m_topToColor;
    private Color m_bottomToColor;
    private float m_duration;
    private float m_elapsedTime;
    private float m_delay;

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

        m_animator = this.GetComponent<QuadAnimator>();
        m_animator.SetOpacity(1);
        m_cameraTransform = m_camera.transform;
    }

    public virtual void InvalidateColors()
    {
        SetColors(new Color[4] { m_topColor, m_topColor, m_bottomColor, m_bottomColor });

        m_prevTopColor = m_topColor;
        m_prevBottomColor = m_bottomColor;
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

    //public void FadeIn(float duration, float delay = 0.0f)
    //{
    //    m_colorVariating = true;
    //    m_topFromColor = m_topColor;
    //    m_bottomFromColor = m_bottomColor;
    //    m_topToColor = new Color(m_topColor.r, m_topColor.g, m_topColor.b, 1);
    //    m_bottomToColor = new Color(m_bottomColor.r, m_bottomColor.g, m_bottomColor.b, 1);
    //    m_duration = duration;
    //    m_delay = delay;
    //    m_elapsedTime = 0;
    //}

    //public void FadeOut(float duration, float delay = 0.0f)
    //{
    //    m_colorVariating = true;
    //    m_topFromColor = m_topColor;
    //    m_bottomFromColor = m_bottomColor;
    //    m_topToColor = new Color(m_topColor.r, m_topColor.g, m_topColor.b, 0);
    //    m_bottomToColor = new Color(m_bottomColor.r, m_bottomColor.g, m_bottomColor.b, 0);
    //    m_duration = duration;
    //    m_delay = delay;
    //    m_elapsedTime = 0;
    //}

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

    public override void Update()
    {
        float dt = Time.deltaTime;
        
        if (m_colorWheel)
        {           
            float dHue = HUE_VARIATION_SPEED * dt;

            m_startHSV.TranslateHue(dHue);
            m_endHSV.TranslateHue(dHue);

            m_topColor = m_startHSV.ToRGBA();
            m_bottomColor = m_endHSV.ToRGBA();
        }

        UpdateColor(dt);

       
        if (m_prevTopColor != m_topColor || m_prevBottomColor != m_bottomColor)
            InvalidateColors();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

        if (m_animator != null)
        {           
            Vector2 screenSize = ScreenUtils.GetScreenSize();
            if (screenSize != m_size)
            {
                m_size = screenSize;
                m_animator.SetScale(new Vector3(m_size.x, m_size.y, 1));
            }

            //Vector3 cameraPosition = m_cameraTransform.position;
            //float distanceFromCamera = m_camera.farClipPlane - 1;
            //m_animator.SetPosition(cameraPosition + distanceFromCamera * m_camera.transform.forward);
        }
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
