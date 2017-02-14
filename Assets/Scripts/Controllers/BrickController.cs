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

    /**
    * Process the touch that has been raycast to a plane parallel to the floor with y-coordinate equals to zero
    **/
    public void ProcessTouch(Vector2 touchPlaneProjection)
    {
        //we do not process touch on covered tiles
        Brick.CoveredTiles brickCoveredTiles = m_brickRenderer.m_brick.m_coveredTiles;
        if (brickCoveredTiles.GetTileAtIndex(0).ContainsXZPoint(touchPlaneProjection))
        {
            return;
        }
        if (brickCoveredTiles.GetTileAtIndex(1) != null)
        {
            if (brickCoveredTiles.GetTileAtIndex(1).ContainsXZPoint(touchPlaneProjection))
                return;
        }

        Brick.RollDirection[] directions = new Brick.RollDirection[4];
        directions[0] = Brick.RollDirection.BOTTOM_LEFT;
        directions[1] = Brick.RollDirection.TOP_LEFT;
        directions[2] = Brick.RollDirection.TOP_RIGHT;
        directions[3] = Brick.RollDirection.BOTTOM_RIGHT;

        Vector2[] vectorDirections = new Vector2[4];
        vectorDirections[0] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[0]));
        vectorDirections[1] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[1]));
        vectorDirections[2] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[2]));
        vectorDirections[3] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[3]));

        //find the minimal dot product between the vector joining the brick and the touchPlaneProjection point and each of the four direction vectors
        Vector2 brickGroundPosition;
        if (brickCoveredTiles.GetTileAtIndex(1) == null)
            brickGroundPosition = Geometry.RemoveYComponent(brickCoveredTiles.GetTileAtIndex(0).GetWorldPosition());
        else
            brickGroundPosition = 0.5f * (Geometry.RemoveYComponent(brickCoveredTiles.GetTileAtIndex(0).GetWorldPosition()) + Geometry.RemoveYComponent(brickCoveredTiles.GetTileAtIndex(1).GetWorldPosition()));
        
        Vector2 touchVector = touchPlaneProjection - brickGroundPosition;

        float maxDotProduct = float.MinValue;
        int maxDotProductIdx = 0;
        for (int i = 0; i != 4; i++)
        {
            float dotProduct = MathUtils.DotProduct(vectorDirections[i], touchVector);

            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                maxDotProductIdx = i;
            }
        }
        
        m_brickRenderer.Roll(directions[maxDotProductIdx]);
    }

    public void RollBottomLeft()
    {
        m_brickRenderer.Roll(Brick.RollDirection.BOTTOM_LEFT);
    }

    public void RollTopRight()
    {
        m_brickRenderer.Roll(Brick.RollDirection.TOP_RIGHT);
    }

    public void RollTopLeft()
    {
        m_brickRenderer.Roll(Brick.RollDirection.TOP_LEFT);
    }

    public void RollBottomRight()
    {
        m_brickRenderer.Roll(Brick.RollDirection.BOTTOM_RIGHT);
    }

    void Update()
    {
        if (GameController.GetInstance().m_gameStatus == GameController.GameStatus.RUNNING)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                RollBottomLeft();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                RollTopLeft();
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                RollTopRight();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                RollBottomRight();
            }
        }
    }
}
