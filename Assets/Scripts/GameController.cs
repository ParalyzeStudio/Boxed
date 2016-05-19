using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_brickPfb;

    public void Start()
    {
        GameObject brickObject = (GameObject)Instantiate(m_brickPfb);

        Brick brick = brickObject.GetComponent<Brick>();
        brick.Build();
    }
}
