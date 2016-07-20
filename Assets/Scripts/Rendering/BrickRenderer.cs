using UnityEngine;

public class BrickRenderer : MonoBehaviour
{
    public Brick m_brick { get; set; }

    private Geometry.Edge m_fallRotationEdge;
    private bool m_transformRotationEdgeToLocal;
    private Vector3 m_fallDirection;

    private const float DEFAULT_ANGULAR_SPEED = 90 / 0.3f;

    /**
    * Build a rectangular cuboid that will serve as our main object in the scene.
    * To handle lighting correctly, our cuboid will need 24 vertices (instead of 8) so light is interpolated correctly so one face has one single color.
    * Pass the tile or tiles (max 2) the brick should be upon
    **/
    public void BuildOnTiles(Tile[] tiles)
    {
        m_brick = new Brick();

        //build mesh vertices
        Vector3[] vertices = new Vector3[24];
        
        for (int i = 0; i != m_brick.Faces.Length; i++)
        {
            Brick.BrickFace face = m_brick.Faces[i];
            for (int j = 0; j != face.m_indices.Length; j++)
            {
                vertices[i * 4 + j] = m_brick.Vertices[face.m_indices[j]];
            }
        }

        //Construct mesh triangles and normals
        int[] triangles = new int[36];
        Vector3[] normals = new Vector3[24];
        
        for (int i = 0; i != 6; i++)
        {
            triangles[i * 6] = i * 4;
            triangles[i * 6 + 1] = i * 4 + 2;
            triangles[i * 6 + 2] = i * 4 + 1;
            triangles[i * 6 + 3] = i * 4;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 2;

            for (int p = 0; p != 4; p++)
            {
                normals[i * 4 + p] = m_brick.Faces[i].m_normal;
            }
        }       

        Mesh brickMesh = new Mesh();
        brickMesh.name = "BrickMesh";
        brickMesh.vertices = vertices;
        brickMesh.triangles = triangles;
        brickMesh.normals = normals;

        brickMesh.RecalculateBounds();

        MeshFilter brickMeshFilter = this.GetComponent<MeshFilter>();
        brickMeshFilter.sharedMesh = brickMesh;

        //place the brick upon the tile parameter
        PlaceOnTiles(tiles);
    }

    /**
    * Adjust the position and the rotation of the brick so it covers the tiles passed as parameters
    * tile2 can be null if there is only one covered tile
    **/
    public void PlaceOnTiles(Tile[] tiles)
    {
        m_brick.PlaceOnTiles(tiles);

        GameObjectAnimator brickAnimator = this.GetComponent<GameObjectAnimator>();
        brickAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f)); //place the pivot point at the center of the brick     

        if (tiles[1] == null)
        {
            brickAnimator.transform.rotation = Quaternion.identity; //null rotation  
            brickAnimator.SetPosition(tiles[0].GetWorldPosition() + new Vector3(0, 1 + 0.5f * TileRenderer.TILE_HEIGHT, 0));
        }
        else
        {
            brickAnimator.transform.rotation = m_brick.m_rotation;
            brickAnimator.SetPosition(0.5f * (tiles[0].GetWorldPosition() + tiles[1].GetWorldPosition()) + new Vector3(0, 0.5f + 0.5f * TileRenderer.TILE_HEIGHT, 0));
        }
    }

    /**
    * Make the brick roll on one of the 4 available directions (left, top, right, bottom)
    **/
    public void Roll(Brick.RollDirection rollDirection)
    {
        Brick.RollResult rollResult;
        Geometry.Edge rotationEdge;
        m_brick.Roll(rollDirection, out rollResult, out rotationEdge);

        if (rollResult == Brick.RollResult.VALID || rollResult == Brick.RollResult.FALL)
        {
            //Get the rotation axis from the rotation edge
            Vector3 rotationAxis = rotationEdge.m_pointB - rotationEdge.m_pointA;

            if (rollResult == Brick.RollResult.VALID)
            {
                //perform the normal rotation of the brick
                BrickAnimator brickAnimator = GetComponent<BrickAnimator>();
                brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(rotationEdge.GetMiddle()));
                brickAnimator.SetRotationAxis(rotationAxis);
                float rotationDuration = 90 / DEFAULT_ANGULAR_SPEED;
                brickAnimator.RotateBy(90, rotationDuration);
            }
            else if (rollResult == Brick.RollResult.FALL)
            {
                CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();

                Tile[] coveredTiles = m_brick.CoveredTiles;
                float normalRotationAngle = 0;
                bool bPrefall = false;

                if (coveredTiles[1] != null) //2 tiles are covered
                {
                    if (coveredTiles[0].CurrentState == Tile.State.DISABLED && coveredTiles[1].CurrentState == Tile.State.DISABLED) //brick fell on two disabled tiles
                    {
                        m_fallRotationEdge = rotationEdge;
                        m_fallDirection = Brick.GetVector3DirectionForRollingDirection(rollDirection);
                        m_transformRotationEdgeToLocal = false;
                        normalRotationAngle = 135;
                        bPrefall = false;
                    }
                    else if (coveredTiles[0].CurrentState == Tile.State.NORMAL || coveredTiles[0].CurrentState == Tile.State.START || coveredTiles[0].CurrentState == Tile.State.FINISH) //first tile is normal and second one is disabled
                    {
                        normalRotationAngle = 90;
                        m_fallDirection = coveredTiles[1].GetWorldPosition() - coveredTiles[0].GetWorldPosition();
                        m_fallRotationEdge = Floor.GetCommonEdgeForConsecutiveTiles(coveredTiles[0], coveredTiles[1]);
                        m_transformRotationEdgeToLocal = true;
                        bPrefall = true;
                    }
                    else /*if (coveredTiles[1].CurrentState == Tile.State.NORMAL)*/ //second tile is normal and first one is disabled
                    {
                        normalRotationAngle = 90;
                        m_fallDirection = coveredTiles[0].GetWorldPosition() - coveredTiles[1].GetWorldPosition();
                        m_fallRotationEdge = Floor.GetCommonEdgeForConsecutiveTiles(coveredTiles[1], coveredTiles[0]);
                        m_transformRotationEdgeToLocal = true;
                        bPrefall = true;
                    }
                }
                else
                {
                    normalRotationAngle = 135;
                    m_fallRotationEdge = rotationEdge;
                    m_fallDirection = Brick.GetVector3DirectionForRollingDirection(rollDirection);
                    m_transformRotationEdgeToLocal = false;
                    bPrefall = false;
                }

                //perform the normal rotation of the brick
                BrickAnimator brickAnimator = GetComponent<BrickAnimator>();
                brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(rotationEdge.GetMiddle()));
                brickAnimator.SetRotationAxis(rotationAxis);
                float rotationDuration = normalRotationAngle / DEFAULT_ANGULAR_SPEED;
                brickAnimator.RotateBy(normalRotationAngle, rotationDuration);

                //perform the prefall action if needed
                if (bPrefall)
                {
                    //rotate the brick to get to the point where it is going to fall
                    callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(PreFall), rotationDuration);
                }

                //finally make it fall
                callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(Fall), rotationDuration + (bPrefall ? 45 / DEFAULT_ANGULAR_SPEED : 0));
            }
        }
    }

    /**
    * In case the normal rotation of the brick is not enough to make the brick fall, rotate it again by 45 degrees around the floor edge so it reaches the point it will start falling
    **/
    private void PreFall()
    {    
        if (m_transformRotationEdgeToLocal)
        {
            //transform this edge into brick local coordinates
            m_fallRotationEdge.Translate(-this.transform.localPosition);
            m_fallRotationEdge.ApplyRotation(Quaternion.Inverse(m_brick.m_rotation));
        }

        Vector3 rotationAxis = m_fallRotationEdge.m_pointB - m_fallRotationEdge.m_pointA;

        //rotates the brick until the gravity makes it fall
        BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
        brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(m_fallRotationEdge.GetMiddle()));
        brickAnimator.SetRotationAxis(rotationAxis);
        float rotationAngle = 45;
        float rotationDuration = rotationAngle / DEFAULT_ANGULAR_SPEED;
        brickAnimator.RotateBy(rotationAngle, rotationDuration);
    }

    private void Fall()
    {
        Vector3 fallRotationAxis = m_fallRotationEdge.m_pointB - m_fallRotationEdge.m_pointA;

        BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
        brickAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f));

        //make the brick fall
        float fallHeight = 10.0f;
        float fallSpeed = 1.5f  * 0.5f * Mathf.Deg2Rad * DEFAULT_ANGULAR_SPEED;
        float fallDuration = fallHeight / fallSpeed;

        Vector3 brickToPosition = brickAnimator.GetPosition() + new Vector3(0, -fallHeight, 0) + 4.8f * m_fallDirection;
        brickAnimator.TranslateTo(brickToPosition, fallDuration, 0, ValueAnimator.InterpolationType.LINEAR, true);

        brickAnimator.SetRotationAxis(fallRotationAxis);
        brickAnimator.RotateBy(540, fallDuration);
    }

    /**
    * Get the object normalized coordinated for our pivot point given its local coordinates
    **/
    public Vector3 GetPivotPointFromLocalCoordinates(Vector3 coords)
    {
        return new Vector3(coords.x, 0.5f * coords.y, coords.z);
    }

    /**
    * Called when the brick has finished its rolling animation
    **/
    public void OnFinishRolling()
    {
        m_brick.OnFinishRolling();
    }

    /**
    * Tells if the brick is upon the finish tile and only it
    **/
    public bool IsOnFinishTile()
    {
        return (m_brick.GetCoveredTilesCount() == 1) && (m_brick.CoveredTiles[0].CurrentState == Tile.State.FINISH);
    }

    /**
    * Tells if the brick is currently falling
    **/ 
    public bool IsFalling()
    {
        return m_brick.m_state == Brick.BrickState.FALLING;
    }
}