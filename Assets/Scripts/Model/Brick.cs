using UnityEngine;

public class Brick
{
    private Vector3[] m_vertices; //brick's 8 vertices
    public Vector3[] Vertices
    {
        get
        {
            return m_vertices;
        }
    }

    private BrickFace[] m_faces; //brick's 6 faces
    public BrickFace[] Faces
    {
        get
        {
            return m_faces;
        }
    }

    private Tile[] m_coveredTiles; //the tiles that are covered by this brick (max 2)
    public Tile[] CoveredTiles
    {
        get
        {
            return m_coveredTiles;
        }
    }

    public struct BrickEdge
    {
        public Vector3 m_pointA;
        public Vector3 m_pointB;

        public BrickEdge(Vector3 pointA, Vector3 pointB)
        {
            m_pointA = pointA;
            m_pointB = pointB;
        }

        public Vector3 GetMiddle()
        {
            return 0.5f * (m_pointA + m_pointB);
        }
    }

    /**
    * A face of the brick d-c
    *                     | |
    *                     a-b
    **/
    public struct BrickFace
    {
        public Brick m_parentBrick;

        public int m_index; //index of the face inside the whole brick
        public int[] m_indices; //array of 4 indices
        public Vector3 m_normal;

        //indices of the 4 faces that are adjacent to this one. Faces are sorted accordint to the vertices order.
        //That means that the face sharing edge ab is numbered 1, face sharing bc is numbered 2 and so on
        public int[] m_adjacentsFaces;

        public BrickFace(Brick parentBrick, int index, int a, int b, int c, int d)
        {
            m_parentBrick = parentBrick;
            m_index = index;
            m_indices = new int[4];
            m_indices[0] = a;
            m_indices[1] = b;
            m_indices[2] = c;
            m_indices[3] = d;

            Vector3 ab = m_parentBrick.m_vertices[b] - m_parentBrick.m_vertices[a];
            Vector3 bc = m_parentBrick.m_vertices[c] - m_parentBrick.m_vertices[b];
            m_normal = -Vector3.Cross(ab, bc);
            m_normal.Normalize();

            //set adjacent faces
            if (index == 0)
                m_adjacentsFaces = new int[4] { 4, 3, 2, 5 };
            else if (index == 1)
                m_adjacentsFaces = new int[4] { 2, 3, 4, 5 };
            else if (index == 2)
                m_adjacentsFaces = new int[4] { 0, 3, 1, 5 };
            else if (index == 3)
                m_adjacentsFaces = new int[4] { 0, 4, 1, 2 };
            else if (index == 4)
                m_adjacentsFaces = new int[4] { 0, 5, 1, 3 };
            else
                m_adjacentsFaces = new int[4] { 0, 2, 1, 4 };
        }

        /**
        * Return the edge shared by both this brick face and the adjacent face defined by its index
        **/
        public BrickEdge GetAdjacentFaceSharedEdge(int adjacentFaceIdx)
        {
            return new BrickEdge(m_parentBrick.m_vertices[m_indices[adjacentFaceIdx]],
                                 m_parentBrick.m_vertices[m_indices[(adjacentFaceIdx == 3) ? 0 : adjacentFaceIdx + 1]]);
        }

        /**
        * Return the area of the face in tile units. It can be either 1 or 2
        **/
        public float GetAreaInTileUnits()
        {
            if (m_index < 2)
                return 1;
            return 2;
        }
    }

    private int m_downFaceIndex; //the index of the face currently touching down the floor

    //current animation state of the brick
    public enum BrickState
    {
        IDLE = 1,
        ROLLING
    }

    public BrickState m_state { get; set; }

    public Quaternion m_rotation { get; set; }

    /**
    * Build a rectangular cuboid that will serve as our main object in the scene.
    * To handle lighting correctly, our cuboid will need 24 vertices (instead of 8) so light is interpolated correctly so one face has one single color.
    * Pass the tile the brick should be upon
    **/
    public Brick(Tile tile = null)
    {
        m_vertices = new Vector3[8];
        m_vertices[0] = new Vector3(0, 0, 0);
        m_vertices[1] = new Vector3(1, 0, 0);
        m_vertices[2] = new Vector3(1, 0, 1);
        m_vertices[3] = new Vector3(0, 0, 1);
        m_vertices[4] = new Vector3(0, 2, 0);
        m_vertices[5] = new Vector3(1, 2, 0);
        m_vertices[6] = new Vector3(1, 2, 1);
        m_vertices[7] = new Vector3(0, 2, 1);

        m_faces = new BrickFace[6];
        //build square faces of the brick
        m_faces[0] = new BrickFace(this, 0, 3, 2, 1, 0);
        m_faces[1] = new BrickFace(this, 1, 4, 5, 6, 7);

        //build rectangle faces of the brick
        for (int i = 2; i != 6; i++)
        {
            int startIdx = i - 2;
            m_faces[i] = new BrickFace(this, i, startIdx, (i == 5) ? 0 : startIdx + 1, (i == 5) ? 4 : startIdx + 5, startIdx + 4);
        }              

        //at start the first face is touching the ground with face 0 and is idle
        m_downFaceIndex = 0;
        m_state = BrickState.IDLE;

        //tiles covered by the brick (only 1 at start)
        m_coveredTiles = new Tile[2];
        m_coveredTiles[0] = tile;
        m_coveredTiles[1] = null;

        //set rotation to identity at start
        m_rotation = Quaternion.identity;
    }

    /**
    * Copy constructor that makes a shallow copy of the brick parameter
    **/
    public Brick(Brick other)
    {
        m_vertices = other.m_vertices;
        m_faces = other.m_faces;
        m_coveredTiles = other.m_coveredTiles;
        m_downFaceIndex = other.m_downFaceIndex;
        m_state = other.m_state;
        m_rotation = other.m_rotation;
    }

    public enum RollDirection
    {
        NONE = 0,
        LEFT = 1,
        TOP,
        RIGHT,
        BOTTOM
    }

    public enum RollResult
    {
        NONE,
        VALID, //the rolling operation is valid and brick is now on valid tiles
        NO_TILE_TO_ROLL, //there is no tile to perform the rolling, brick won't move
        FALL, //there are tiles to roll onto, but there are disabled tiles leading the brick to a fall off the floor
    }

    /**
    * Make the brick roll on one of the 4 available directions (left, top, right, bottom) and return the axis of the rotation to tell the parent renderer
    * around which axis to perform the 90 degrees rotation
    **/
    public void Roll(RollDirection rollDirection, out RollResult rollResult, out BrickEdge rotationEdge)
    {
        if (m_state == BrickState.ROLLING) //brick is already rolling do nothing
        {
            rollResult = RollResult.NONE;
            rotationEdge = new BrickEdge(Vector3.zero, Vector3.zero);
            return;
        }

        m_state = BrickState.ROLLING;

        //Associate one vector to every direction
        Vector3 direction;
        if (rollDirection == RollDirection.LEFT)
            direction = new Vector3(-1, 0, 0);
        else if (rollDirection == RollDirection.TOP)
            direction = new Vector3(0, 0, 1);
        else if (rollDirection == RollDirection.RIGHT)
            direction = new Vector3(1, 0, 0);
        else
            direction = new Vector3(0, 0, -1);

        //Find the face that has the same normal vector as the roll direction
        float maxDotProduct = float.MinValue;
        int rollToFaceIdx = -1;
        for (int i = 0; i != 6; i++)
        {
            Vector3 faceNormal = m_rotation * m_faces[i].m_normal;
            float dotProduct = MathUtils.DotProduct(direction, faceNormal);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                rollToFaceIdx = i;
            }
        }

        BrickFace currentFace = m_faces[m_downFaceIndex];
        BrickFace rollToFace = m_faces[rollToFaceIdx];

        Tile[] newCoveredTiles = new Tile[2];
        rollResult = CanRoll(currentFace, rollToFace, rollDirection, out newCoveredTiles);
        if (rollResult == RollResult.NO_TILE_TO_ROLL) //there is no tile on which we can land, just interrupt the rolling action
        {
            m_state = BrickState.IDLE;
            rotationEdge = new BrickEdge(Vector3.zero, Vector3.zero);
            return;
        }

        //replace the old covered tiles by new ones
        m_coveredTiles = newCoveredTiles;

        //Determine which of the 4 adjacent faces the rollToFace is equal to        
        int adjacentFaceIdx = -1;
        for (int i = 0; i != currentFace.m_adjacentsFaces.Length; i++)
        {
            BrickFace adjacentFace = m_faces[currentFace.m_adjacentsFaces[i]];
            if (adjacentFace.m_index == rollToFace.m_index)
            {
                adjacentFaceIdx = i;
                break;
            }
        }
        
        rotationEdge = currentFace.GetAdjacentFaceSharedEdge(adjacentFaceIdx);
        Vector3 rotationAxis = rotationEdge.m_pointB - rotationEdge.m_pointA;

        Quaternion brickRotation = Quaternion.AngleAxis(90, rotationAxis);
        m_rotation *= brickRotation;

        //set the new index for the face touching the floor
        m_downFaceIndex = rollToFace.m_index;
    }

    /**
    * Determine if the rolling movement from currentFace to rollToFace of the brick is possible
    * Also set the new covered tiles if movement is declared possible
    **/
    public RollResult CanRoll(BrickFace currentFace, BrickFace rollToFace, RollDirection rollDirection, out Tile[] newCoveredTiles)
    {
        Floor floor = GameController.GetInstance().m_floor.m_floorData;

        newCoveredTiles = new Tile[2];

        //Determine if rolling movement is legit (i.e brick will not fall off the floor) and mark the new covered tiles
        RollResult rollResult = RollResult.VALID;
        if (currentFace.GetAreaInTileUnits() == 1)
        {
            //find the next 2 tiles in the rolling direction
            Tile coveredTile = m_coveredTiles[0];
            Tile nextTile1 = floor.GetNextTileForDirection(coveredTile, rollDirection);            

            if (nextTile1 != null)
            {
                if (nextTile1.CurrentState == Tile.State.DISABLED)
                    rollResult = RollResult.FALL;

                Tile nextTile2 = floor.GetNextTileForDirection(nextTile1, rollDirection);
                if (nextTile2 != null)
                {
                    if (nextTile2.CurrentState == Tile.State.DISABLED)
                        rollResult = RollResult.FALL;

                    newCoveredTiles[0] = nextTile1;
                    newCoveredTiles[1] = nextTile2;
                }
                else
                    rollResult = RollResult.NO_TILE_TO_ROLL;
            }
            else
                rollResult = RollResult.NO_TILE_TO_ROLL;
        }
        else
        {
            if (rollToFace.GetAreaInTileUnits() == 1)
            {
                //find the tile next to m_coveredTile[0] or m_coveredTile[1] in the rolling direction
                Tile coveredTile1 = m_coveredTiles[0];
                Tile coveredTile2 = m_coveredTiles[1];
                Tile nextTile = floor.GetNextTileForDirection(coveredTile1, rollDirection);
                if (nextTile != null)
                {
                    if (nextTile == coveredTile2)
                    {
                        nextTile = floor.GetNextTileForDirection(coveredTile2, rollDirection);
                        if (nextTile != null)
                        {
                            if (nextTile.CurrentState == Tile.State.DISABLED)
                                rollResult = RollResult.FALL;

                            newCoveredTiles[0] = nextTile;
                            newCoveredTiles[1] = null;
                        }
                        else
                            rollResult = RollResult.NO_TILE_TO_ROLL;
                    }
                    else
                    {
                        if (nextTile.CurrentState == Tile.State.DISABLED)
                            rollResult = RollResult.FALL;

                        newCoveredTiles[0] = nextTile;
                        newCoveredTiles[1] = null;
                    }
                }
                else
                    rollResult = RollResult.NO_TILE_TO_ROLL;
            }
            else
            {
                Tile nextTile1 = floor.GetNextTileForDirection(m_coveredTiles[0], rollDirection);
                Tile nextTile2 = floor.GetNextTileForDirection(m_coveredTiles[1], rollDirection);
                if (nextTile1 != null && nextTile2 != null)
                {
                    if (nextTile1.CurrentState == Tile.State.DISABLED || nextTile2.CurrentState == Tile.State.DISABLED)
                        rollResult = RollResult.FALL;

                    newCoveredTiles[0] = nextTile1;
                    newCoveredTiles[1] = nextTile2;
                }
                else
                    rollResult = RollResult.NO_TILE_TO_ROLL;
            }
        }

        return rollResult;
    }

    /**
    * Return the number of tiles covered by the brick (1 or 2)
    **/
    public int GetCoveredTilesCount()
    {
        if (m_coveredTiles[1] == null)
            return 1;

        return 2;
    }

    public void OnFinishRolling()
    {
        m_state = Brick.BrickState.IDLE;

        //Capture bonuses
        if (m_coveredTiles[0] != null && m_coveredTiles[0].AttachedBonus != null)
        {
            TileRenderer tileRenderer = GameController.GetInstance().m_floor.GetRendererForTile(m_coveredTiles[0]);
            tileRenderer.OnCaptureBonus();
        }

        if (m_coveredTiles[1] != null && m_coveredTiles[1].AttachedBonus != null)
        {
            TileRenderer tileRenderer = GameController.GetInstance().m_floor.GetRendererForTile(m_coveredTiles[1]);
            tileRenderer.OnCaptureBonus();
        }

        //Change the state of new covered tiles
        //if (m_coveredTiles[0] != null && m_coveredTiles[0].CurrentState == Tile.State.DISABLED)
        //    m_coveredTiles[0].CurrentState = Tile.State.SELECTED;
        //if (m_coveredTiles[1] != null && m_coveredTiles[1].CurrentState == Tile.State.DISABLED)
        //    m_coveredTiles[1].CurrentState = Tile.State.SELECTED;
    }
}