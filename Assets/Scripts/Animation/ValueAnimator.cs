﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ValueAnimator : MonoBehaviour
{
    public enum InterpolationType
    {
        LINEAR = 1,
        SINUSOIDAL,
        HERMITE1,
        HERMITE2
    }

    //Variables to handle fading
    protected bool m_fading;
    public float m_opacity { get; set; }
    protected float m_fromOpacity;
    protected float m_toOpacity;
    protected float m_fadingDuration;
    protected float m_fadingDelay;
    protected float m_fadingElapsedTime;
    protected InterpolationType m_fadingInterpolationType;
    protected bool m_destroyObjectOnFinishFading;

    //Variables to handle scaling
    protected bool m_scaling;
    protected Vector3 m_scale;
    protected Vector3 m_fromScale;
    protected Vector3 m_toScale;
    protected float m_scalingDuration;
    protected float m_scalingDelay;
    protected float m_scalingElapsedTime;
    protected InterpolationType m_scalingInterpolationType;

    //Variables to handle translating
    protected bool m_translating;
    protected Vector3 m_position;
    protected Vector3 m_fromPosition;
    protected Vector3 m_toPosition;
    protected Vector3 m_translationDirection;
    protected float m_translationLength;
    protected float m_translatingDuration;
    protected float m_translatingDelay;
    protected float m_translatingElapsedTime;
    protected InterpolationType m_translatingInterpolationType;
    protected bool m_destroyObjectOnFinishTranslating;

    //Variables to handle rotating
    protected bool m_rotating;
    protected Vector3 m_rotationAxis;
    protected float m_angle;
    //protected float m_fromAngle;
    protected float m_byAngle;
    protected float m_rotatingDuration;
    protected float m_rotatingDelay;
    protected float m_rotatingElapsedTime;
    protected InterpolationType m_rotatingInterpolationType;

    //Variables to handle color variation
    protected bool m_colorChanging;
    protected Color m_color;
    protected Color m_fromColor;
    protected Color m_toColor;
    protected float m_colorChangingDuration;
    protected float m_colorChangingDelay;
    protected float m_colorChangingElapsedTime;
    protected InterpolationType m_colorChangingInterpolationType;

    public virtual void Awake()
    {
        m_opacity = 1;
        m_color = Color.white;
    }

    public void FadeTo(float toOpacity, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyObjectOnFinish = false)
    {
        if (m_opacity == toOpacity)
            return;

        m_fading = true;
        m_fromOpacity = m_opacity;
        m_toOpacity = toOpacity;
        m_fadingDuration = duration;
        m_fadingDelay = delay;
        m_fadingElapsedTime = 0;
        m_fadingInterpolationType = interpolType;
        m_destroyObjectOnFinishFading = bDestroyObjectOnFinish;
    }

    public void ScaleTo(Vector3 toScale, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
    {
        m_scaling = true;
        m_fromScale = this.transform.localScale;
        m_toScale = toScale;
        m_scalingDuration = duration;
        m_scalingDelay = delay;
        m_scalingElapsedTime = 0;
        m_scalingInterpolationType = interpolType;
        m_destroyObjectOnFinishFading = bDestroyOnFinish;
    }

    public void TranslateTo(Vector3 toPosition, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
    {
        m_translating = true;
        m_fromPosition = m_position;
        m_toPosition = toPosition;
        m_translationLength = (m_toPosition - m_fromPosition).magnitude;
        m_translationDirection = (m_toPosition - m_fromPosition);
        m_translationDirection /= m_translationLength;
        m_translatingDuration = duration;
        m_translatingDelay = delay;
        m_translatingElapsedTime = 0;
        m_translatingInterpolationType = interpolType;
        m_destroyObjectOnFinishTranslating = bDestroyOnFinish;
    }

    public void TranslateBy(Vector3 positionDelta, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
    {
        TranslateTo(m_position + positionDelta, duration, delay, interpolType, bDestroyOnFinish);
    }

    public void RotateBy(float byAngle, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_rotating = true;
        m_angle = 0;
        m_byAngle = byAngle;
        m_rotatingDuration = duration;
        m_rotatingDelay = delay;
        m_rotatingElapsedTime = 0;
        m_rotatingInterpolationType = interpolType;
    }

    public void ColorChangeTo(Color toColor, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_colorChanging = true;
        m_fromColor = m_color;
        m_toColor = toColor;
        m_colorChangingDuration = duration;
        m_colorChangingDelay = delay;
        m_colorChangingElapsedTime = 0;
        m_colorChangingInterpolationType = interpolType;
    }

    public virtual void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        fOpacity = Mathf.Clamp(fOpacity, 0, 1);

        m_opacity = fOpacity;
        m_color.a = fOpacity;
        OnOpacityChanged();

        ValueAnimator[] childAnimators = this.GetComponentsInChildren<ValueAnimator>();

        if (bPassOnChildren && childAnimators != null)
        {
            for (int i = 0; i != childAnimators.Length; i++)
            {
                ValueAnimator childAnimator = childAnimators[i];
                if (childAnimator != this)
                {
                    childAnimator.SetOpacity(fOpacity, false); //do not pass to this object's children because they are already in the list of childAnimators
                }
            }
        }
    }

    public virtual void SetScale(Vector3 scale)
    {
        m_scale = scale;
        OnScaleChanged();
    }

    public virtual void SyncPositionFromTransform()
    {
        m_position = gameObject.transform.localPosition;
    }

    public virtual void SetPosition(Vector3 position)
    {
        m_position = position;
        OnPositionChanged();
    }

    public virtual Vector3 GetPosition()
    {
        return m_position;
    }

    public virtual void ApplyRotationAngle(float angle)
    {
        m_angle += angle;
        OnRotationChanged();
    }

    /**
    * Define the rotation axis on this object.
    * It can be defined as a global vector or a local one 
    **/
    public virtual void SetRotationAxis(Vector3 axis, bool local = true)
    {
        if (!local)
            axis = Quaternion.Inverse(this.transform.rotation) * axis;

        m_rotationAxis = axis;        
    }

    public virtual void SetColor(Color color)
    {
        m_color = color;
        m_opacity = color.a;

        OnColorChanged();
    }

    public virtual void IncOpacity(float deltaOpacity)
    {
        float fOpacity = m_opacity + deltaOpacity;
        SetOpacity(fOpacity);
    }

    public virtual void IncScale(Vector3 deltaScale)
    {
        Vector3 fScale = this.transform.localScale + deltaScale;
        SetScale(fScale);
    }

    public virtual void IncPosition(Vector3 deltaPosition)
    {
        Vector3 fPosition = m_position + deltaPosition;
        SetPosition(fPosition);
    }

    public virtual void IncColor(Color deltaColor)
    {
        Color color = m_color + deltaColor;
        SetColor(color);
    }

    public virtual void OnOpacityChanged()
    {

    }

    public virtual void OnPositionChanged()
    {

    }

    public virtual void OnScaleChanged()
    {

    }

    public virtual void OnRotationChanged()
    {

    }

    public virtual void OnColorChanged()
    {

    }

    public virtual void OnFinishFading()
    {
        if (m_destroyObjectOnFinishFading)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void OnFinishTranslating()
    {
        if (m_destroyObjectOnFinishTranslating)
        {
            Destroy(this.gameObject);
        }
    }

    public virtual void OnFinishScaling()
    {

    }

    public virtual void OnFinishRotating()
    {

    }

    public virtual void OnFinishColorChanging()
    {

    }

    protected virtual void UpdateOpacity(float dt)
    {
        if (m_fading)
        {
            bool inDelay = (m_fadingElapsedTime < m_fadingDelay);
            m_fadingElapsedTime += dt;
            if (m_fadingElapsedTime >= m_fadingDelay)
            {
                if (inDelay) //we were in delay previously
                {
                    dt = m_fadingElapsedTime - m_fadingDelay;
                }
                float effectiveElapsedTime = m_fadingElapsedTime - m_fadingDelay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;
                float opacityVariation = m_toOpacity - m_fromOpacity;
                float deltaOpacity = CalculateDeltaForInterpolationType(t1, t2, m_fadingDuration, opacityVariation, m_fadingInterpolationType);

                if (effectiveElapsedTime > m_fadingDuration)
                {
                    SetOpacity(m_toOpacity);
                    m_fading = false;
                    OnFinishFading();
                }
                else
                    IncOpacity(deltaOpacity);
            }
        }
    }

    protected virtual void UpdatePosition(float dt)
    {
        if (m_translating)
        {
            bool inDelay = (m_translatingElapsedTime < m_translatingDelay);
            m_translatingElapsedTime += dt;
            if (m_translatingElapsedTime >= m_translatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_translatingElapsedTime - m_translatingDelay;
                float effectiveElapsedTime = m_translatingElapsedTime - m_translatingDelay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;
                Vector3 positionVariation = m_toPosition - m_fromPosition;
                float deltaPositionX = CalculateDeltaForInterpolationType(t1, t2, m_translatingDuration, positionVariation.x, m_translatingInterpolationType);
                float deltaPositionY = CalculateDeltaForInterpolationType(t1, t2, m_translatingDuration, positionVariation.y, m_translatingInterpolationType);
                float deltaPositionZ = CalculateDeltaForInterpolationType(t1, t2, m_translatingDuration, positionVariation.z, m_translatingInterpolationType);
                Vector3 deltaPosition = new Vector3(deltaPositionX, deltaPositionY, deltaPositionZ);

                if (effectiveElapsedTime > m_translatingDuration)
                {
                    SetPosition(m_toPosition);
                    m_translating = false;
                    OnFinishTranslating();
                }
                else
                {
                    IncPosition(deltaPosition);
                }
            }
        }
    }

    protected void UpdateRotation(float dt)
    {
        if (m_rotating)
        {
            bool inDelay = (m_rotatingElapsedTime < m_rotatingDelay);
            m_rotatingElapsedTime += dt;
            if (m_rotatingElapsedTime > m_rotatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_rotatingElapsedTime - m_rotatingDelay;
                float effectiveElapsedTime = m_rotatingElapsedTime - m_rotatingDelay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;
                float deltaAngle = CalculateDeltaForInterpolationType(t1, t2, m_rotatingDuration, m_byAngle, m_rotatingInterpolationType);

                if (effectiveElapsedTime > m_rotatingDuration)
                {
                    deltaAngle = m_byAngle - m_angle;
                    ApplyRotationAngle(deltaAngle);
                    m_rotating = false;
                    OnFinishRotating();
                }
                else
                    ApplyRotationAngle(deltaAngle);
            }
        }
    }

    protected virtual void UpdateScale(float dt)
    {
        if (m_scaling)
        {
            bool inDelay = (m_scalingElapsedTime < m_scalingDelay);
            m_scalingElapsedTime += dt;
            if (m_scalingElapsedTime > m_scalingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_scalingElapsedTime - m_scalingDelay;
                float effectiveElapsedTime = m_scalingElapsedTime - m_scalingDelay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;
                Vector3 scaleVariation = m_toScale - m_fromScale;
                float deltaScaleX = CalculateDeltaForInterpolationType(t1, t2, m_scalingDuration, scaleVariation.x, m_scalingInterpolationType);
                float deltaScaleY = CalculateDeltaForInterpolationType(t1, t2, m_scalingDuration, scaleVariation.y, m_scalingInterpolationType);
                float deltaScaleZ = CalculateDeltaForInterpolationType(t1, t2, m_scalingDuration, scaleVariation.z, m_scalingInterpolationType);
                Vector3 deltaScale = new Vector3(deltaScaleX, deltaScaleY, deltaScaleZ);

                if (effectiveElapsedTime > m_scalingDuration)
                {
                    SetScale(m_toScale);
                    m_scaling = false;
                    OnFinishScaling();
                }
                else
                    IncScale(deltaScale);
            }
        }
    }

    protected virtual void UpdateColor(float dt)
    {
        if (m_colorChanging)
        {
            bool inDelay = (m_colorChangingElapsedTime < m_colorChangingDelay);
            m_colorChangingElapsedTime += dt;
            if (m_colorChangingElapsedTime > m_colorChangingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_colorChangingElapsedTime - m_colorChangingDelay;
                float effectiveElapsedTime = m_colorChangingElapsedTime - m_colorChangingDelay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;
                Vector4 colorVariation = m_toColor - m_fromColor;
                float deltaColorX = CalculateDeltaForInterpolationType(t1, t2, m_colorChangingDuration, colorVariation.x, m_colorChangingInterpolationType);
                float deltaColorY = CalculateDeltaForInterpolationType(t1, t2, m_colorChangingDuration, colorVariation.y, m_colorChangingInterpolationType);
                float deltaColorZ = CalculateDeltaForInterpolationType(t1, t2, m_colorChangingDuration, colorVariation.z, m_colorChangingInterpolationType);
                float deltaColorW = CalculateDeltaForInterpolationType(t1, t2, m_colorChangingDuration, colorVariation.w, m_colorChangingInterpolationType);
                Vector4 deltaColor = new Color(deltaColorX, deltaColorY, deltaColorZ, deltaColorW);

                if (effectiveElapsedTime > m_colorChangingDuration)
                {
                    SetColor(m_toColor);
                    m_colorChanging = false;
                    OnFinishColorChanging();
                }
                else
                    IncColor(deltaColor);
            }
        }
    }

    /**
    * Calculate the infinitesimal variation of the interpolation between t1 and t2 (t2 = t1 + dt)
    **/
    private float CalculateDeltaForInterpolationType(float t1, float t2, float L, float amp, InterpolationType interpolationType)
    {
        if (interpolationType == InterpolationType.LINEAR)
            return amp * (Linear(t2 / L) - Linear(t1 / L));
        else if (interpolationType == InterpolationType.SINUSOIDAL)
            return amp * (Sinusoidal(t2 / L) - Sinusoidal(t1 / L));
        else if (interpolationType == InterpolationType.HERMITE1)
            return amp * (SmoothStep(t2 / L) - SmoothStep(t1 / L));
        else if (interpolationType == InterpolationType.HERMITE2)
            return amp * (SmootherStep(t2 / L) - SmootherStep(t1 / L));
        return 0;
    }

    /**
    * f(x) =  = x
    **/
    private float Linear(float x)
    {
        return x;
    }

    /**
    * f(x) = sin(pi/2 * x)
    **/
    private float Sinusoidal(float x)
    {
        return Mathf.Sin(Mathf.PI / 2 * x);
    }

    /**
    * f(x) = 3x^2 - 2x^3
    **/
    private float SmoothStep(float x)
    {
        return 3 * x * x - 2 * x * x * x;
    }

    /**
    * f(x) = 6x^5 - 15x^4 + 10x^3
    **/
    private float SmootherStep(float x)
    {
        return 6 * x * x * x * x * x - 15 * x * x * x * x + 10 * x * x * x;
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;

        //update values that have to be modified through time
        UpdateOpacity(dt);
        UpdatePosition(dt);
        UpdateRotation(dt);
        UpdateScale(dt);
        UpdateColor(dt);
    }
}