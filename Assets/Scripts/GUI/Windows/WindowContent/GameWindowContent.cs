using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameWindowContent : MonoBehaviour
{
    public const float DEFAULT_TIME_SPACING = 0.05f; //the default time spent between two elements animations (~2 frames)

    public virtual IEnumerator Show(float timeSpacing = DEFAULT_TIME_SPACING)
    {
        //gameObject.SetActive(true);

        //make elements invisible and buttons interactable       
        GameWindowElement[] elts = GetComponentsInChildren<GameWindowElement>();
        for (int i = 0; i != elts.Length; i++)
        {
            GameWindowElement elt = elts[i];
            //if element has a button, make it interactable
            Button button = elt.GetButton();
            if (button != null)
                button.interactable = true;
            elt.SetOpacity(0);
        }

        //fade them in
        //also store the duration of the whole entering animation including time spacings
        for (int i = 0; i != elts.Length; i++)
        {
            elts[i].Show();
            if (i < elts.Length - 1)
                yield return new WaitForSeconds(timeSpacing);
        }
    }

    public virtual IEnumerator Dismiss(float timeSpacing = DEFAULT_TIME_SPACING)
    {
        GameWindowElement[] elts = GetComponentsInChildren<GameWindowElement>();
        for (int i = 0; i != elts.Length; i++)
        {
            //if element has a button, make it not interactable
            Button button = elts[i].GetButton();
            if (button != null)
                button.interactable = false;
        }

        //also disable back button
        GameWindow parentWindow = this.transform.parent.gameObject.GetComponent<GameWindow>();
        parentWindow.DisableBackButton();
        
        for (int i = 0; i != elts.Length; i++)
        {
            elts[i].Dismiss();
            StartCoroutine(elts[i].ResetPositionAfterDelay(elts[i].m_animationDuration));
            if (i < elts.Length - 1)
                yield return new WaitForSeconds(timeSpacing);
        }

        yield return new WaitForSeconds(elts[elts.Length - 1].m_animationDuration);

        //enable the back button
        parentWindow.EnableBackButton();
    }

    public IEnumerator DeactivateAfterDelay(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        gameObject.SetActive(false);
    }
}

