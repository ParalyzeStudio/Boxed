using UnityEngine;
using UnityEngine.UI;

public class InterLevelScreen : MonoBehaviour
{
    public GameObject m_circleEmitter; //the empty game object used to emit circles from the finger icon
    public Text m_nextLevelTitle; //text containing the number of the next level to play

    /**
    * When the level is done, show some statistics (collected bonuses, par score)
    **/
    public void ShowResults()
    {

    }

    /**
    * Dismiss the previously shown results
    **/
    public void DismissResults()
    {

    }

    public void OnClickTouchToPlay()
    {
        GameController.GetInstance().ClearLevel();
        GameController.GetInstance().StartNextLevel();

        GameGUI gameGUI = this.transform.parent.GetComponent<GameGUI>();
        gameGUI.DismissInterLevelScreen();
    }
}
