using UnityEngine.UI;

public class GUIImageAnimator : ValueAnimator
{
    private Image m_image;

    public override void OnScaleChanged()
    {
        GetImageComponent().rectTransform.localScale = m_scale;
    }

    private Image GetImageComponent()
    {
        if (m_image == null)
            m_image = GetComponent<Image>();
        return m_image;
    }
}
