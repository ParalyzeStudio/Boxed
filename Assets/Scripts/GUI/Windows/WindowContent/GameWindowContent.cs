using System.Collections;
using UnityEngine;

public class GameWindowContent : MonoBehaviour
{
    private GameWindowElement[] m_elements;

    protected const float DEFAULT_TIME_SPACING = 0.032f; //the default time spent between two elements animations (~2 frames)

    public virtual IEnumerator Show(float timeSpacing = DEFAULT_TIME_SPACING)
    {
        gameObject.SetActive(true);

        //make elements invisible
        for (int i = 0; i != GetElements().Length; i++)
        {
            GetElements()[i].SetOpacity(0);
        }

        for (int i = 0; i != GetElements().Length; i++)
        {
            GetElements()[i].Show();
            yield return new WaitForSeconds(timeSpacing);
        }

        yield return null;
    }

    public virtual IEnumerator Dismiss(bool bDestroy = false, float timeSpacing = DEFAULT_TIME_SPACING)
    {
        for (int i = 0; i != GetElements().Length; i++)
        {
            GetElements()[i].Dismiss();
            StartCoroutine(GetElements()[i].ResetPositionAfterDelay(GameWindowElement.ELEMENT_ANIMATION_DURATION));
            //StartCoroutine(GetElements()[i].DeactivateAfterDelay(GameWindowElement.ELEMENT_ANIMATION_DURATION));
            yield return new WaitForSeconds(timeSpacing);
        }

        //for (int i = 0; i != GetElements().Length; i++)
        //{
        //    StartCoroutine(GetElements()[i].ResetPositionAfterDelay(GameWindowElement.ELEMENT_ANIMATION_DURATION));
        //    StartCoroutine(GetElements()[i].DeactivateAfterDelay(GameWindowElement.ELEMENT_ANIMATION_DURATION));
        //}

        if (bDestroy)
            Destroy(this.gameObject, GameWindowElement.ELEMENT_ANIMATION_DURATION);
        else
            StartCoroutine(DeactivateAfterDelay(GameWindowElement.ELEMENT_ANIMATION_DURATION));
    }

    public GameWindowElement[] GetElements()
    {
        if (m_elements == null)
            m_elements = GetComponentsInChildren<GameWindowElement>();

        return m_elements;
    }

    public IEnumerator DeactivateAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        gameObject.SetActive(false);
    }
}

