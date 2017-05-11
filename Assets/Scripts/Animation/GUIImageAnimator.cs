using UnityEngine;
using UnityEngine.UI;

public class GUIImageAnimator : ValueAnimator
{
    private Image m_image;

    public override void SyncPositionFromTransform()
    {       
        m_position = GetComponent<RectTransform>().anchoredPosition;
    }

    public override void OnScaleChanged()
    {
        GetComponent<RectTransform>().localScale = m_scale;
    }

    public override void OnOpacityChanged()
    {
        Image image = GetImageComponent();
        Color color = image.color;
        color.a = m_opacity;
        image.color = color;
    }

    public override void OnPositionChanged()
    {
        base.OnPositionChanged();
        GetComponent<RectTransform>().anchoredPosition = new Vector2(m_position.x, m_position.y);
    }

    private Image GetImageComponent()
    {
        if (m_image == null)
            m_image = GetComponent<Image>();
        return m_image;
    }
}
