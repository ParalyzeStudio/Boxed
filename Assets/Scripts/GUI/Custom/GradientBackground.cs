using UnityEngine;

/**
* Use this class to render a background image with vertical gradient
**/
public class GradientBackground : BillboardSprite
{
    public void Init(Color startColor, Color endColor)
    {
        base.Init(null, false);

        this.name = "Background";

        SetColors(new Color[4] { startColor, startColor, endColor, endColor});
        
        QuadAnimator backgroundAnimator = this.GetComponent<QuadAnimator>();
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        backgroundAnimator.SetScale(new Vector3(screenSize.x, screenSize.y, 1));
    }

    public override void LateUpdate()
    {
        //set the background at a long distance from camera so it is behind all scene elements
        Vector3 cameraPosition = m_camera.gameObject.transform.position;
        float distanceFromCamera = 1000;
        this.transform.position = cameraPosition + distanceFromCamera * m_camera.transform.forward;

        base.LateUpdate();
    }
}
