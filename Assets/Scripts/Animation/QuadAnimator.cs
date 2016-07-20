using UnityEngine;
using System.Collections.Generic;

public class QuadAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        Quad quad = this.GetComponent<Quad>();
        Color[] quadColors = quad.m_colors;
        for (int i = 0; i != 4; i++)
        {
            Color color = quadColors[i];
            quadColors[i] = new Color(color.r, color.g, color.b, fOpacity);
        }

        quad.SetColors(quadColors);
    }
}