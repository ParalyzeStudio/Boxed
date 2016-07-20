using UnityEngine;

public class CanvasGroupFade : ValueAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, false);
        GetComponent<CanvasGroup>().alpha = fOpacity;
    }

    public void FadeOut()
    {
        this.FadeTo(0.0f, 0.5f, 0, InterpolationType.LINEAR, true);
    }
     
    public void FadeIn()
    {
        this.FadeTo(1.0f, 0.5f);
    }
}
