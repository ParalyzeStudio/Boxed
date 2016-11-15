using UnityEngine;

public class GlowSquareAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, false);

        GlowSquare glowSquare = this.GetComponent<GlowSquare>();
        glowSquare.Invalidate();
    }
}
