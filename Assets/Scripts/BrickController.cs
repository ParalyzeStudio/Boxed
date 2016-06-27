using UnityEngine;

/**
* This class will be used to control the 2x1 brick over the floor
**/
public class BrickController : MonoBehaviour
{
    private BrickRenderer m_brickRenderer;

    public void Start()
    {
        m_brickRenderer = this.GetComponent<BrickRenderer>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_brickRenderer.Roll(Brick.RollDirection.LEFT);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            m_brickRenderer.Roll(Brick.RollDirection.TOP);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_brickRenderer.Roll(Brick.RollDirection.RIGHT);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            m_brickRenderer.Roll(Brick.RollDirection.BOTTOM);
        }
    }
}
