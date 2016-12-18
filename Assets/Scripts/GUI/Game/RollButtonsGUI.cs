using UnityEngine;

public class RollButtonsGUI : MonoBehaviour
{
    public void OnGUI()
    {
        GUI.depth = 5;
        //Debug.Log("RollButtonsGUI >>> OnGUI");
        //Debug.Log("RollButtonsGUI depth:" + GUI.depth);
    }



    public void OnClickRollBottomLeft()
    {
        GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollBottomLeft();
    }

    public void OnClickRollTopRight()
    {
        GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollTopRight();
    }

    public void OnClickRollTopLeft()
    {
        GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollTopLeft();
    }

    public void OnClickRollBottomRight()
    {
        GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollBottomRight();
    }
}
