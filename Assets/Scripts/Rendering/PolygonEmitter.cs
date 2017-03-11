using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/**
* Helper class to generate polygons (circle, square, triangle) emitted from a single point at a constant frequency
**/
public class PolygonEmitter : MonoBehaviour
{
    public Image m_squarePolygonPfb;
    public Image m_circlePolygonPfb;

    public bool m_active; //is this emitter active
    public float m_fadeInDuration = 0.75f;
    public float m_emissionPeriod = 0.5f;
    public float m_lifespan = 2.0f;
    public bool m_warmUp; //use this if you want to some polygons on the field before actually emitting new ones

    private List<Image> m_polygons; //the polygons currently active

    //private const float BASE_POLYGON_FADEIN_DURATION = 0.75f;
    //private const float POLYGON_EMISSION_PERIOD = 0.5f;
    //private const float POLYGON_LIFESPAN = 2.0f;

    public void Reset()
    {
        m_active = false;
    
        //destroy active
    }

    private void EmitPolygon()
    {
        Image poly = Instantiate(m_squarePolygonPfb);
        poly.gameObject.SetActive(true);
        poly.transform.SetParent(this.transform, false);
        GUIImageAnimator animator = poly.GetComponent<GUIImageAnimator>();
        animator.SetScale(0.5f * Vector3.one);
        animator.FadeTo(0.0f, m_lifespan);
        animator.ScaleTo(2.0f * Vector3.one, m_lifespan, 0, ValueAnimator.InterpolationType.LINEAR, true); //delete the polygon when animation ends
    }

    /**
    * Animate polygons that grow and fade out in an infinite loop
    **/
    private IEnumerator LoopSquares(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (m_active)
        {
            EmitPolygon();

            yield return new WaitForSeconds(m_emissionPeriod);
        }

        yield return null;
    }
}