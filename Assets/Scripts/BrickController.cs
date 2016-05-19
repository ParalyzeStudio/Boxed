using UnityEngine;

/**
* This class will be used to control the 2x1 brick over the floor
**/
public class BrickController : MonoBehaviour
{
    private Brick m_brick;

    public void Start()
    {
        m_brick = this.GetComponent<Brick>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_brick.Roll(Brick.RollDirection.LEFT);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            m_brick.Roll(Brick.RollDirection.TOP);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {            
            m_brick.Roll(Brick.RollDirection.RIGHT);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            m_brick.Roll(Brick.RollDirection.BOTTOM);
        }
    }
}
