using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public PolygonEmitter m_polygonEmitter;

    private const float BASE_POLYGON_FADEIN_DURATION = 0.75f;
    //private const float POLYGON_EMISSION_PERIOD = 0.5f;
    //private const float POLYGON_LIFESPAN = 2.0f;

    public void Show(float delay)
    {
        //fade in the base polygon before emitting new ones
        GUIElementAnimator basePolygonAnimator = m_polygonEmitter.GetComponent<GUIElementAnimator>();
        basePolygonAnimator.SetOpacity(0);
        basePolygonAnimator.FadeTo(1.0f, BASE_POLYGON_FADEIN_DURATION, delay);

        ////the emission is delayed by (delay + BASE_POLYGON_FADEIN_DURATION) seconds
        //m_emittingPolygons = true;
        //IEnumerator LoopSquaresCoroutine = LoopSquares(delay);
        //StartCoroutine(LoopSquaresCoroutine);
    }

    public void Dismiss()
    {
        GetComponent<Button>().interactable = false;

        //stop polygon emission
        //m_emittingPolygons = false;
        m_polygonEmitter.m_active = false;

        //fade out the base polygon before emitting new ones
        GUIElementAnimator basePolygonAnimator = m_polygonEmitter.GetComponent<GUIElementAnimator>();
        basePolygonAnimator.FadeTo(0.0f, BASE_POLYGON_FADEIN_DURATION, 0, ValueAnimator.InterpolationType.LINEAR, true);
    }

    public void OnClick()
    {
        m_polygonEmitter.m_active = false;
    }
}
