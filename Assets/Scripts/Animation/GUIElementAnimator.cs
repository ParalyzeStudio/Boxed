using UnityEngine;
using UnityEngine.UI;

public class GUIElementAnimator : ValueAnimator
{
    private Image m_image; //a possible Image component attached to this GUI element
    private RectTransform m_rectTf;

    public override void SyncPositionFromTransform()
    {       
        m_position = GetRectTransform().anchoredPosition;
    }

    public override void OnScaleChanged()
    {
        GetRectTransform().localScale = m_scale;
    }

    public override void OnPositionChanged()
    {
        base.OnPositionChanged();
        GetRectTransform().anchoredPosition = new Vector2(m_position.x, m_position.y);
    }

    public override void OnOpacityChanged()
    {
        Image image = GetImageComponent();
        Color color = image.color;
        color.a = m_opacity;
        image.color = color;
    }

    private Image GetImageComponent()
    {
        if (m_image == null)
            m_image = GetComponent<Image>();
        return m_image;
    }

    private RectTransform GetRectTransform()
    {
        if (m_rectTf == null)
            m_rectTf = GetComponent<RectTransform>();

        return m_rectTf;
    }
}
