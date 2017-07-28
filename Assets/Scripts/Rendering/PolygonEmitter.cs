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

    public bool m_active = true; //is this emitter active
    public float m_emissionPeriod = 0.5f;
    public float m_lifespan = 2.0f;
    public float m_emissionDelay = 0.0f;
    public float m_startScale = 1.0f;
    public float m_endScale = 2.0f;

    public PolygonType m_polygonType;
    public enum PolygonType
    {
        CIRCLE,
        SQUARE
    }

    //private const float BASE_POLYGON_FADEIN_DURATION = 0.75f;
    //private const float POLYGON_EMISSION_PERIOD = 0.5f;
    //private const float POLYGON_LIFESPAN = 2.0f;

    public void Start()
    {
        IEnumerator loopSquaresCoroutine = LoopSquares(m_emissionDelay);
        StartCoroutine(loopSquaresCoroutine);
    }

    private void EmitPolygon()
    {
        Image pfb;
        switch (m_polygonType)
        {
            case PolygonType.CIRCLE:
                pfb = m_circlePolygonPfb;
                break;
            case PolygonType.SQUARE:
                pfb = m_squarePolygonPfb;
                break;
            default:
                pfb = m_circlePolygonPfb;
                break;
        }

        Image poly = Instantiate(pfb);
        poly.gameObject.SetActive(true);
        poly.gameObject.SetActive(true);
        poly.transform.SetParent(this.transform, false);
        GUIElementAnimator animator = poly.GetComponent<GUIElementAnimator>();
        animator.SetScale(m_startScale * Vector3.one);
        animator.FadeTo(0.0f, m_lifespan);
        animator.ScaleTo(m_endScale * Vector3.one, m_lifespan, 0, ValueAnimator.InterpolationType.LINEAR, true); //delete the polygon when animation ends
    }

    /**
    * Animate polygons that grow and fade out in an infinite loop
    **/
    private IEnumerator LoopSquares(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        while (true)
        {
            if (m_active)
            {
                EmitPolygon();
                yield return new WaitForSeconds(m_emissionPeriod);
            }
            else
                yield return null;
        }
    }
}