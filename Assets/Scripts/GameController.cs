using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_brickPfb;
    public GameObject m_floorPfb;

    public Brick m_brick { get; set; }
    public Floor m_floor { get; set; }

    public void Start()
    {
        GameObject floorObject = (GameObject)Instantiate(m_floorPfb);
        m_floor = floorObject.GetComponent<Floor>();
        m_floor.Build();

        GameObject brickObject = (GameObject)Instantiate(m_brickPfb);
        m_brick = brickObject.GetComponent<Brick>();
        m_brick.Build();

        //TODO Make the brick start on one or 2 tiles instead of an absolute 3d position like Vector3.zero for instance
    }
}
