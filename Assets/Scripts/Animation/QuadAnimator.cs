using UnityEngine;

public class QuadAnimator : GameObjectAnimator
{
    protected Quad m_quad;

    public override void OnOpacityChanged()
    {
        Color[] newQuadColors = new Color[GetQuadComponent().Colors.Length];
        for (int i = 0; i != newQuadColors.Length; i++)
        {
            newQuadColors[i] = m_quad.Colors[i];
            newQuadColors[i].a = m_opacity;
        }

        m_quad.SetColors(newQuadColors);
    }

    private Quad GetQuadComponent()
    {
        if (m_quad == null)
            m_quad = GetComponent<Quad>();
        return m_quad;
    }
}
