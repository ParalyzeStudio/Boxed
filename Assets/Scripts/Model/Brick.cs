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

    public CoveredTiles m_coveredTiles { get; set; } //the tiles that are covered by this brick (max 2)
    public class CoveredTiles
    {
        private Tile[] m_tiles;

        public CoveredTiles()
        {
            m_tiles = new Tile[2];
        }

        public CoveredTiles(Tile[] tiles) : this()
        {
            SetTiles(tiles[0], tiles[1]);
        }

        public CoveredTiles(Tile tile1, Tile tile2) : this()
        {
            SetTiles(tile1, tile2);
        }

        public Tile GetTileAtIndex(int index)
        {
            return m_tiles[index];
        }

        public Tile[] GetAsTruncatedArray()
        {
            Tile[] tiles = new Tile[GetCount()];

            for (int i = 0; i != tiles.Length; i++)
            {
                tiles[i] = m_tiles[i];
            }

            return tiles;
        }

        public void SetTiles(Tile tile1, Tile tile2)
        {
            m_tiles[0] = tile1;
            m_tiles[1] = tile2;
        }

        public int GetCount()
        {
            return (m_tiles[1] == null) ? 1 : 2;
        }

        public bool ContainsBonus()
        {
            return m_tiles[0].AttachedBonus != null || (m_tiles[1] != null && m_tiles[1].AttachedBonus != null);
        }

        public bool ContainsBonus(Bonus bonus)
        {
            if (m_tiles[0].AttachedBonus != null && m_tiles[0].AttachedBonus == bonus)
                return true;
            else if (m_tiles[1] != null && m_tiles[1].AttachedBonus != null && m_tiles[1].AttachedBonus == bonus)
                return true;

            return false;
        }

        public bool ShareSameTiles(CoveredTiles other)
        {
            if (GetCount() != other.GetCount())
                return false;

            if (GetCount() == 1)
                return m_tiles[0] == other.m_tiles[0];
            else
                return m_tiles[0] == other.m_tiles[0] && m_tiles[1] == other.m_tiles[1] || m_tiles[0] == other.m_tiles[1] && m_tiles[1] == other.m_tiles[0];
        }

        public CoveredTiles GetNextCoveredTilesForDirection(Floor floor, Brick.RollDirection direction)
        {
            CoveredTiles nextCoveredTiles = new CoveredTiles();

            if (GetCount() == 1)
            {
                //find the next 2 tiles in the rolling direction
                Tile coveredTile = GetTileAtIndex(0);
                Tile nextTile1 = floor.GetNextTileForDirection(coveredTile, direction);

                if (nextTile1 != null)
                {
                    Tile nextTile2 = floor.GetNextTileForDirection(nextTile1, direction);
                    if (nextTile2 != null)
                        nextCoveredTiles.SetTiles(nextTile1, nextTile2);
                    else
                        return null;
                }
                else
                    return null;
            }
            else
            {
                Tile coveredTile1 = GetTileAtIndex(0);
                Tile coveredTile2 = GetTileAtIndex(1);

                Tile nextTile = floor.GetNextTileForDirection(coveredTile1, direction);
                if (nextTile != null)
                {
                    if (nextTile == coveredTile2)
                    {
                        nextTile = floor.GetNextTileForDirection(coveredTile2, direction);
                        nextCoveredTiles.SetTiles(nextTile, null);
                    }
                    else
                    {
                        Tile nextTile2 = floor.GetNextTileForDirection(coveredTile2, direction);
                        if (nextTile2 == coveredTile1)
                        {
                            nextCoveredTiles.SetTiles(nextTile, null);
                        }
                        else
                        {
                            nextCoveredTiles.SetTiles(nextTile, nextTile2);
                        }
                    }
                }
                else
                    return null;
            }

            return nextCoveredTiles;
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
        public Geometry.Edge GetAdjacentFaceSharedEdge(int adjacentFaceIdx)
        {
            return new Geometry.Edge(m_parentBrick.m_vertices[m_indices[adjacentFaceIdx]],
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
        ROLLING,
        FALLING
    }

    public BrickState m_state { get; set; }

    public Quaternion m_rotation { get; set; }

    private CoveredTiles[] m_rolledOnTiles; //tiles the brick has rolled on
    private int m_rolledOnTilesLastIndex;

    public const float BRICK_BASIS_DIMENSION = 1; //the dimension of the square that serves as the basis of the brick, its height is twice this length

    /**
    * Build a rectangular cuboid that will serve as our main object in the scene.
    * To handle lighting correctly, our cuboid will need 24 vertices (instead of 8) so light is interpolated correctly so one face has one single color.
    * Pass the tile or tiles the brick should be upon
    **/
    public Brick()
    {
        m_vertices = new Vector3[8];
        m_vertices[0] = new Vector3(0, 0, 0);
        m_vertices[1] = new Vector3(BRICK_BASIS_DIMENSION, 0, 0);
        m_vertices[2] = new Vector3(BRICK_BASIS_DIMENSION, 0, BRICK_BASIS_DIMENSION);
        m_vertices[3] = new Vector3(0, 0, BRICK_BASIS_DIMENSION);
        m_vertices[4] = new Vector3(0, 2 * BRICK_BASIS_DIMENSION, 0);
        m_vertices[5] = new Vector3(BRICK_BASIS_DIMENSION, 2 * BRICK_BASIS_DIMENSION, 0);
        m_vertices[6] = new Vector3(BRICK_BASIS_DIMENSION, 2 * BRICK_BASIS_DIMENSION, BRICK_BASIS_DIMENSION);
        m_vertices[7] = new Vector3(0, 2 * BRICK_BASIS_DIMENSION, BRICK_BASIS_DIMENSION);

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
        m_state = BrickState.IDLE;

        m_rotation = Quaternion.identity;

        m_rolledOnTiles = new CoveredTiles[64]; //the maximum length depends on the maximum height of the solution tree (set arbitrarily a big enough number)
        m_rolledOnTilesLastIndex = 0;
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

    public void PlaceOnTiles(Tile[] tiles)
    {
        m_coveredTiles = new CoveredTiles(tiles);

        //set initial rotation of the brick and the index of the face touching the ground depending on the number of covered tiles
        if (m_coveredTiles.GetCount() == 1) //only one tile covered
        {
            m_rotation = Quaternion.identity;
            m_downFaceIndex = 0;
        }
        else //two tiles covered
        {            
            Vector3 brickDirection = m_coveredTiles.GetTileAtIndex(1).GetLocalPosition() - m_coveredTiles.GetTileAtIndex(0).GetLocalPosition();

            if (brickDirection == Vector3.left)
                m_downFaceIndex = 5;
            else if (brickDirection == Vector3.forward)
                m_downFaceIndex = 4;
            else if (brickDirection == Vector3.right)
                m_downFaceIndex = 3;
            else if (brickDirection == Vector3.back)
                m_downFaceIndex = 2;

            m_rotation = Quaternion.FromToRotation(Vector3.up, brickDirection);
        }
    }

    public enum RollDirection
    {
        NONE = 0,
        BOTTOM_LEFT = 1,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_RIGHT
    }

    public enum RollResult
    {
        NONE,
        VALID, //the rolling operation is valid and brick is now on valid tiles
        NO_TILE_TO_ROLL, //there is no tile to perform the rolling, brick won't move
        FALL, //there are tiles to roll onto, but there are disabled tiles leading the brick to a fall off the floor
        BLOCKED //the brick has been blocked
    }

    /**
    * Make the brick roll on one of the 4 available directions (left, top, right, bottom) and return the axis of the rotation to tell the parent renderer
    * around which axis to perform the 90 degrees rotation
    **/
    public void Roll(RollDirection rollDirection, out RollResult rollResult, out Geometry.Edge rotationEdge)
    {
        if (m_state == BrickState.ROLLING || m_state == BrickState.FALLING) //brick is already rolling do nothing
        {
            rollResult = RollResult.NONE;
            rotationEdge = new Geometry.Edge(Vector3.zero, Vector3.zero);
            return;
        }

        Debug.Log("Roll " + rollDirection);

        //Associate one vector to every direction
        Vector3 direction = GetVector3DirectionForRollingDirection(rollDirection);
        
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

        //Tile[] newCoveredTiles = new Tile[2];
        CoveredTiles newCoveredTiles = new CoveredTiles();
        rollResult = CanRoll(currentFace, rollToFace, rollDirection, out newCoveredTiles);
        if (rollResult == RollResult.VALID || rollResult == RollResult.FALL)
        {
            //set the state of the brick to ROLLING
            //m_state = BrickState.ROLLING;
            m_state = (rollResult == RollResult.VALID) ? BrickState.ROLLING : BrickState.FALLING;

            //block tiles if necessary
            for (int i = 0; i != m_coveredTiles.GetCount(); i++)
            {
                Tile previousCoveredTile = m_coveredTiles.GetTileAtIndex(i);
                if (previousCoveredTile is IceTile)
                {
                    ((IceTile)previousCoveredTile).m_blocked = true;
                }
            }

            if (rollResult == RollResult.VALID)
            {
                //replace the old covered tiles by new ones
                m_coveredTiles = newCoveredTiles;
            }

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

            //if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME ||
            //    GameController.GetInstance().m_gameMode == GameController.GameMode.LEVEL_EDITOR /*&& rollResult == RollResult.VALID*/)
            //{
                Quaternion brickRotation = Quaternion.AngleAxis(90, rotationAxis);
                m_rotation *= brickRotation;
            //}

            //if (rollResult == RollResult.VALID)
            //{
                //set the new index for the face touching the floor
                m_downFaceIndex = rollToFace.m_index;
            //}
        }
        else if (rollResult == RollResult.NO_TILE_TO_ROLL) //there is no tile on which we can land, just interrupt the rolling action
        {
            m_state = BrickState.IDLE;
            rotationEdge = new Geometry.Edge(Vector3.zero, Vector3.zero);
            return;
        }
        //else if (rollResult == RollResult.FALL)
        //{
        //    m_state = BrickState.FALLING;
        //    //rotationEdge = new Geometry.Edge(Vector3.zero, Vector3.zero);
        //    return;
        //}
        else //maintain the brick in IDLE state
        {
            rotationEdge = new Geometry.Edge(Vector3.zero, Vector3.zero);
            return;
        }        
    }

    public static Vector3 GetVector3DirectionForRollingDirection(Brick.RollDirection rollDirection)
    {
        if (rollDirection == RollDirection.BOTTOM_LEFT)
            return new Vector3(-1, 0, 0);
        else if (rollDirection == RollDirection.TOP_LEFT)
            return new Vector3(0, 0, 1);
        else if (rollDirection == RollDirection.TOP_RIGHT)
            return new Vector3(1, 0, 0);
        else
            return new Vector3(0, 0, -1);
    }

    /**
    * Determine if the rolling movement from currentFace to rollToFace of the brick is possible
    * Also set the new covered tiles if movement is declared possible
    **/
    public RollResult CanRoll(BrickFace currentFace, BrickFace rollToFace, RollDirection rollDirection, out CoveredTiles newCoveredTiles)
    {
        Floor floor = GameController.GetInstance().m_floorRenderer.m_floorData;

        newCoveredTiles = new CoveredTiles();

        //Determine if rolling movement is legit (i.e brick will not fall off the floor) and mark the new covered tiles
        RollResult rollResult = RollResult.VALID;
        if (currentFace.GetAreaInTileUnits() == 1)
        {
            //find the next 2 tiles in the rolling direction
            Tile coveredTile = m_coveredTiles.GetTileAtIndex(0);
            Tile nextTile1 = floor.GetNextTileForDirection(coveredTile, rollDirection);            

            if (nextTile1 != null)
            {
                if (nextTile1.IsBlocking())
                    return RollResult.NONE;

                Tile nextTile2 = floor.GetNextTileForDirection(nextTile1, rollDirection);
                if (nextTile2 != null)
                {
                    if (nextTile2.CurrentState == Tile.State.DISABLED)
                        rollResult = RollResult.FALL;
                    else if (nextTile1.IsBlocking())
                        return RollResult.NONE;

                    newCoveredTiles.SetTiles(nextTile1, nextTile2);
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
                Tile coveredTile1 = m_coveredTiles.GetTileAtIndex(0);
                Tile coveredTile2 = m_coveredTiles.GetTileAtIndex(1);
                Tile nextTile = floor.GetNextTileForDirection(coveredTile1, rollDirection);
                if (nextTile != null)
                {
                    if (nextTile == coveredTile2)
                    {
                        nextTile = floor.GetNextTileForDirection(coveredTile2, rollDirection);
                        if (nextTile != null)
                        {
                            if (nextTile.IsBlocking())
                                return RollResult.NONE;
                            else if (nextTile.CurrentState == Tile.State.DISABLED)
                                rollResult = RollResult.FALL;

                            newCoveredTiles.SetTiles(nextTile, null);
                        }
                        else
                            rollResult = RollResult.NO_TILE_TO_ROLL;
                    }
                    else
                    {
                        if (nextTile.IsBlocking())
                            return RollResult.NONE;
                        else if (nextTile.CurrentState == Tile.State.DISABLED)
                            return RollResult.FALL;

                        newCoveredTiles.SetTiles(nextTile, null);
                    }
                }
                else
                    rollResult = RollResult.NO_TILE_TO_ROLL;
            }
            else
            {
                Tile nextTile1 = floor.GetNextTileForDirection(m_coveredTiles.GetTileAtIndex(0), rollDirection);
                Tile nextTile2 = floor.GetNextTileForDirection(m_coveredTiles.GetTileAtIndex(1), rollDirection);
                if (nextTile1 != null && nextTile2 != null)
                {
                    if (nextTile1.IsBlocking() || nextTile2.IsBlocking())
                        return RollResult.NONE;
                    else if (nextTile1.CurrentState == Tile.State.DISABLED || nextTile2.CurrentState == Tile.State.DISABLED)
                        return RollResult.FALL;

                    newCoveredTiles.SetTiles(nextTile1, nextTile2);
                }
                else
                    rollResult = RollResult.NO_TILE_TO_ROLL;
            }
        }
        
        return rollResult;
    }

    public void OnFinishRolling()
    {
        if (m_state == BrickState.FALLING)
            return;

        //switches
        if (m_coveredTiles.GetTileAtIndex(0) != null && m_coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.SWITCH)
        {
            ((SwitchTile)m_coveredTiles.GetTileAtIndex(0)).Toggle();
        }
        else
        {
            if (m_coveredTiles.GetTileAtIndex(1) != null && m_coveredTiles.GetTileAtIndex(1).CurrentState == Tile.State.SWITCH)
            {
                ((SwitchTile)m_coveredTiles.GetTileAtIndex(1)).Toggle();
            }
        }
        
        //reset the tile state to IDLE
        m_state = Brick.BrickState.IDLE;

        //Change the state of new covered tiles
        //if (m_coveredTiles[0] != null && m_coveredTiles[0].CurrentState == Tile.State.DISABLED)
        //    m_coveredTiles[0].CurrentState = Tile.State.SELECTED;
        //if (m_coveredTiles[1] != null && m_coveredTiles[1].CurrentState == Tile.State.DISABLED)
        //    m_coveredTiles[1].CurrentState = Tile.State.SELECTED;
    }

    public void AddCurrentCoveredTilesToRolledOnTiles()
    {
        m_rolledOnTiles[m_rolledOnTilesLastIndex++] = m_coveredTiles;
    }

    public void RemoveLastRolledOnTiles()
    {
        m_rolledOnTiles[--m_rolledOnTilesLastIndex] = null;
    }

    public CoveredTiles GetRolledOnTilesAtDistanceFromLast(int distance)
    {
        int lastTilesIndex = -1;
        for (int i = 0; i != m_rolledOnTiles.Length; i++)
        {
            if (m_rolledOnTiles[i] == null)
            {
                lastTilesIndex = i - 1;
                break;
            }
        }

        if (lastTilesIndex >= distance)
            return m_rolledOnTiles[lastTilesIndex - distance];
        else
            return null;
    }

    /**
    * To optimize the algorithm we can skip paths that contain cycles in it.
    * There is an exception to this rule, as we are allowed to cycle if we rolled on a tile that adds something to the gameplay (bonus, switch...)
    * So we need to check if the covered tiles inside a cycle contains such a tile
    **/
    public bool IsCycling(int distanceFromRoot)
    {
        if (distanceFromRoot < 2)
            return false;

        int index = distanceFromRoot;
        CoveredTiles coveredTiles = m_rolledOnTiles[index];
        
        int minDistanceForCycling = 2; //we need at least to move the brick twice to obtain a minimum cycle
        bool bCycleContainsBonuses = false;
        while (index >= 0)
        {
            if (minDistanceForCycling > 0)
            {
                minDistanceForCycling--;
                index--;
                continue;
            }

            CoveredTiles tiles = m_rolledOnTiles[index];

            if (!bCycleContainsBonuses && tiles.ContainsBonus()) //no need to check if this node contains a bonus if we already checked it positively before
                bCycleContainsBonuses = true;

            bool bCycling = coveredTiles.ShareSameTiles(tiles);
            if (bCycling)
            {
                //check if it contains bonus. If not we consider that the cycle is useless and the path containing this cycle can be removed
                return !bCycleContainsBonuses;
            }

            index--;
        }

        return false;

        //SolutionNode node = this.m_parentNode;
        //int minDistanceForCycling = 1; //we need at least to move the brick twice to obtain a minimum cycle
        //bool bCycleContainsBonuses = false;
        //while (node != null)
        //{
        //    if (minDistanceForCycling > 0)
        //    {
        //        minDistanceForCycling--;
        //        node = node.m_parentNode;
        //        continue;
        //    }

        //    if (!bCycleContainsBonuses && node.ContainsBonus()) //no need to check if this node contains a bonus if we already checked it positively before
        //        bCycleContainsBonuses = true;

        //    bool bCycling = this.CoversSameTiles(node);
        //    if (bCycling)
        //    {
        //        //check if it contains bonus. If not we consider that the cycle is useless and the path containing this cycle can be removed
        //        return !bCycleContainsBonuses;
        //    }

        //    node = node.m_parentNode;
        //}

        //return false;
    }
}