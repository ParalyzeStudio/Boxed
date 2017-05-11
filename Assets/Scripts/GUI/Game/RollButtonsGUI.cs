using UnityEngine;

public class RollButtonsGUI : MonoBehaviour
{
    public void OnClickRollBottomLeft()
    {
        if (GameController.GetInstance().m_gameStatus == GameController.GameStatus.RUNNING)
            GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollBottomLeft();
    }

    public void OnClickRollTopRight()
    {
        if (GameController.GetInstance().m_gameStatus == GameController.GameStatus.RUNNING)
            GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollTopRight();
    }

    public void OnClickRollTopLeft()
    {
        if (GameController.GetInstance().m_gameStatus == GameController.GameStatus.RUNNING)
            GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollTopLeft();
    }

    public void OnClickRollBottomRight()
    {
        if (GameController.GetInstance().m_gameStatus == GameController.GameStatus.RUNNING)
            GameController.GetInstance().m_brickRenderer.GetComponent<BrickController>().RollBottomRight();
    }
}
