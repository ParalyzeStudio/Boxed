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
        if (brickCoveredTiles.GetFirstTile().ContainsXZPoint(touchPlaneProjection))
        {
            return;
        }
        if (brickCoveredTiles.GetSecondTile() != null)
        {
            if (brickCoveredTiles.GetSecondTile().ContainsXZPoint(touchPlaneProjection))
                return;
        }

        Brick.RollDirection[] directions = new Brick.RollDirection[4];
        directions[0] = Brick.RollDirection.LEFT;
        directions[1] = Brick.RollDirection.TOP;
        directions[2] = Brick.RollDirection.RIGHT;
        directions[3] = Brick.RollDirection.BOTTOM;

        Vector2[] vectorDirections = new Vector2[4];
        vectorDirections[0] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[0]));
        vectorDirections[1] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[1]));
        vectorDirections[2] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[2]));
        vectorDirections[3] = Geometry.RemoveYComponent(Brick.GetVector3DirectionForRollingDirection(directions[3]));

        //find the minimal dot product between the vector joining the brick and the touchPlaneProjection point and each of the four direction vectors
        Vector2 brickGroundPosition;
        if (brickCoveredTiles.GetSecondTile() == null)
            brickGroundPosition = Geometry.RemoveYComponent(brickCoveredTiles.GetFirstTile().GetWorldPosition());
        else
            brickGroundPosition = 0.5f * (Geometry.RemoveYComponent(brickCoveredTiles.GetFirstTile().GetWorldPosition()) + Geometry.RemoveYComponent(brickCoveredTiles.GetSecondTile().GetWorldPosition()));
        
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

    public void RollLeft()
    {
        m_brickRenderer.Roll(Brick.RollDirection.LEFT);
    }

    public void RollRight()
    {
        m_brickRenderer.Roll(Brick.RollDirection.RIGHT);
    }

    public void RollTop()
    {
        m_brickRenderer.Roll(Brick.RollDirection.TOP);
    }

    public void RollBottom()
    {
        m_brickRenderer.Roll(Brick.RollDirection.BOTTOM);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RollLeft();
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            RollTop();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RollRight();
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            RollBottom();
        }
    }
}
