using UnityEngine;
using UnityEngine.UI;

public class GUITextAnimator : ValueAnimator
{
    private Text m_text;

    public override void OnOpacityChanged()
    {
        base.OnOpacityChanged();
        Color textColor = GetTextComponent().color;
        textColor.a = m_opacity;
        m_text.color = textColor;
    }

    public override void OnPositionChanged()
    {
        base.OnPositionChanged();
        m_text.rectTransform.anchoredPosition = new Vector2(m_position.x, m_position.y);
    }

    private Text GetTextComponent()
    {
        if (m_text == null)
            m_text = GetComponent<Text>();
        return m_text;
    }
}
