using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public Image m_polygonPfb;

    private const float BASE_POLYGON_FADEIN_DURATION = 0.75f;
    private const float POLYGON_EMISSION_PERIOD = 0.5f;
    private const float POLYGON_LIFESPAN = 2.0f;

    private bool m_emittingPolygons;

    public void Show(float delay)
    {
        //fade in the base polygon before emitting new ones
        GUIImageAnimator basePolygonAnimator = m_polygonPfb.GetComponent<GUIImageAnimator>();
        basePolygonAnimator.SetOpacity(0);
        basePolygonAnimator.FadeTo(1.0f, BASE_POLYGON_FADEIN_DURATION, delay);

        //the emission is delayed by (delay + BASE_POLYGON_FADEIN_DURATION) seconds
        m_emittingPolygons = true;
        IEnumerator LoopSquaresCoroutine = LoopSquares(delay);
        StartCoroutine(LoopSquaresCoroutine);
    }

    public void Dismiss()
    {
        GetComponent<Button>().interactable = false;

        //stop polygon emission
        m_emittingPolygons = false;

        //fade out the base polygon before emitting new ones
        GUIImageAnimator basePolygonAnimator = m_polygonPfb.GetComponent<GUIImageAnimator>();
        basePolygonAnimator.FadeTo(0.0f, BASE_POLYGON_FADEIN_DURATION, 0, ValueAnimator.InterpolationType.LINEAR, true);
    }

    private void EmitPolygon()
    {
        Image poly = Instantiate(m_polygonPfb);
        poly.gameObject.SetActive(true);
        poly.transform.SetParent(this.transform, false);
        GUIImageAnimator animator = poly.GetComponent<GUIImageAnimator>();
        animator.SetScale(0.5f * Vector3.one);
        animator.FadeTo(0.0f, POLYGON_LIFESPAN);
        animator.ScaleTo(2.0f * Vector3.one, POLYGON_LIFESPAN, 0, ValueAnimator.InterpolationType.LINEAR, true); //delete the polygon when animation ends
    }

    /**
    * Animate squares that grow and fade out in an infinite loop
    **/
    private IEnumerator LoopSquares(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (m_emittingPolygons)
        {
            EmitPolygon();

            yield return new WaitForSeconds(POLYGON_EMISSION_PERIOD);
        }

        yield return null;
    }

    public void OnClick()
    {
        m_emittingPolygons = false;
    }
}
