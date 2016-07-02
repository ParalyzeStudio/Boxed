using System;
using System.Collections.Generic;
using UnityEngine;

/**
* A rectangular grid of tiles
**/
[Serializable]
public class Floor
{
    private Tile[] m_tiles;
    public Tile[] Tiles
    {
        get
        {
            return m_tiles;
        }
    }
    public int m_gridWidth { get; set; }
    public int m_gridHeight { get; set; }

    //cache values for start, finish and bonus tiles
    private Tile m_startTile;
    private Tile m_finishTile;
    private List<Tile> m_bonusTiles;

    public Floor(int width, int height, Tile[] tiles = null)
    {
        m_gridWidth = width;
        m_gridHeight = height;

        if (tiles == null)
            BuildDefault();
        else
            m_tiles = tiles;
    }

    /**
    * Build a squared grid
    **/
    public void BuildDefault()
    {
        //Build a square grid with odd dimensions around the origin
        m_tiles = new Tile[m_gridWidth * m_gridHeight];
        for (int i = 0; i != m_gridWidth; i++)
        {
            for (int j = 0; j != m_gridHeight; j++)
            {
                Tile tile = new Tile(i, j, Tile.State.DISABLED, null);

                m_tiles[GetTileIndexForColumnLine(i, j)] = tile;
            }
        }
    }
    
    /**
    * Return the index of the tile in the global tiles array, given the line and the column of this tile inside the grid
    **/
    public int GetTileIndexForColumnLine(int columnIndex, int lineIndex)
    {
        return columnIndex * m_gridHeight + lineIndex;
    }

    public Tile GetCenterTile()
    {
        int index = GetTileIndexForColumnLine(m_gridWidth / 2, m_gridHeight / 2);
        return Tiles[index];
    }
    
    public Tile GetStartTile()
    {
        if (m_startTile != null) //use the cached value
            return m_startTile;

        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].CurrentState == Tile.State.START)
            {
                m_startTile = m_tiles[i];
                return m_tiles[i];
            }               
        }

        return null;
    }

    public Tile GetFinishTile()
    {
        if (m_finishTile != null) //use the cached value
            return m_finishTile;

        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].CurrentState == Tile.State.FINISH)
            {
                m_finishTile = m_tiles[i];
                return m_tiles[i];
            }
        }

        return null;
    }

    public void SetStartTile(Tile tile)
    {
        m_startTile = tile;
    }

    public void SetFinishTile(Tile tile)
    {
        m_finishTile = tile;
    }

    //public List<Bonus> GetBonuses()
    //{
    //    List<Bonus> bonuses = new List<Bonus>();
    //    for (int i = 0; i != m_tiles.Length; i++)
    //    {
    //        if (m_tiles[i].AttachedBonus != null)
    //            bonuses.Add(m_tiles[i].AttachedBonus);
    //    }

    //    return bonuses;
    //}

    public List<Tile> GetBonusTiles()
    {
        if (m_bonusTiles != null)
            return m_bonusTiles;

        List<Tile> bonusTiles = new List<Tile>();
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].AttachedBonus != null)
                bonusTiles.Add(m_tiles[i]);
        }

        m_bonusTiles = bonusTiles; //cache value
        return bonusTiles;
    }
    
    /**
    * Return the index of the tile next to this tile given a direction (left, top, right or bottom)
    * The tile should never be null because we will design our grid in a way the brick will never land onto a null tile but either on a normal 
    * or a disabled one (a hole) leading the part of the brick to be destroyed/dismantled/exploded
    * We can obtain a null tile only during development time for testing purposes, therefore this method can actually return null.
    **/
    public Tile GetNextTileForDirection(Tile tile, Brick.RollDirection direction)
    {
        int nextTileColumnIndex = -1;
        int nextTileLineIndex = -1;
        if (direction == Brick.RollDirection.LEFT)
        {
            nextTileColumnIndex = tile.m_columnIndex - 1;
            nextTileLineIndex = tile.m_lineIndex;
        }
        else if (direction == Brick.RollDirection.TOP)
        {
            nextTileColumnIndex = tile.m_columnIndex;
            nextTileLineIndex = tile.m_lineIndex + 1;
        }
        else if (direction == Brick.RollDirection.RIGHT)
        {
            nextTileColumnIndex = tile.m_columnIndex + 1;
            nextTileLineIndex = tile.m_lineIndex;
        }
        else
        {
            nextTileColumnIndex = tile.m_columnIndex;
            nextTileLineIndex = tile.m_lineIndex - 1;
        }

        if (nextTileColumnIndex >= 0 && nextTileColumnIndex < m_gridWidth
            &&
            nextTileLineIndex >= 0 && nextTileLineIndex < m_gridHeight)
        {

            int nextTileIndex = GetTileIndexForColumnLine(nextTileColumnIndex, nextTileLineIndex);
            return Tiles[nextTileIndex];
        }
        else
            return null;
    }

    /**
    * Return the edge shared by two consecutive tiles
    **/
    static public Geometry.Edge GetCommonEdgeForConsecutiveTiles(Tile tile1, Tile tile2)
    {
        Vector3 edgeMiddle = 0.5f * (tile2.GetWorldPosition() + tile1.GetWorldPosition()) + new Vector3(0, 0.5f * TileRenderer.TILE_HEIGHT, 0);
        Vector3 edgeDirection = tile2.GetWorldPosition() - tile1.GetWorldPosition();
        edgeDirection = Quaternion.AngleAxis(90, Vector3.up) * edgeDirection;

        Vector3 edgePointA = edgeMiddle - 0.5f * edgeDirection;
        Vector3 edgePointB = edgeMiddle + 0.5f * edgeDirection;

        return new Geometry.Edge(edgePointA, edgePointB);
    }

    /**
    * When we are about to save a level, call this function to recompute a new floor that will fit more our needs than the default one
    **/
    public Floor Clamp()
    {
        //first determine the minimum/maximum x and z coordinates of our selected tiles
        int minColumnIndex = int.MaxValue;
        int maxColumnIndex = int.MinValue;
        int minLineIndex = int.MaxValue;
        int maxLineIndex = int.MinValue;

        bool bEmptyFloor = true;
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].CurrentState == Tile.State.NORMAL || m_tiles[i].CurrentState == Tile.State.START || m_tiles[i].CurrentState == Tile.State.FINISH)
            {
                bEmptyFloor = false;
                int columnIndex = m_tiles[i].m_columnIndex;
                int lineIndex = m_tiles[i].m_lineIndex;
                if (columnIndex < minColumnIndex)
                    minColumnIndex = columnIndex;
                if (columnIndex > maxColumnIndex)
                    maxColumnIndex = columnIndex;
                if (lineIndex < minLineIndex)
                    minLineIndex = lineIndex;
                if (lineIndex > maxLineIndex)
                    maxLineIndex = lineIndex;
            }
        }

        //Build a new grid of tiles that is formed by a bounding box containing all selected tiles + a 2-tiles border of disabled tiles
        int newFloorWidth;
        int newFloorHeight;
        if (bEmptyFloor)
        {
            newFloorWidth = 4;
            newFloorHeight = 4;
        }
        else
        {
            newFloorWidth = (maxColumnIndex - minColumnIndex + 1) + 4;
            newFloorHeight = (maxLineIndex - minLineIndex + 1) + 4;
        }

        Tile[] newTiles = new Tile[newFloorWidth * newFloorHeight];

        for (int i = 0; i != newFloorWidth; i++)
        {
            for (int j = 0; j != newFloorHeight; j++)
            {
                //TileRenderer tileRenderer = tileObject.GetComponent<TileRenderer>();
                Tile tile;
                if (i < 2 || i > newFloorWidth - 3 || j < 2 || j > newFloorHeight - 3)
                {
                    tile = new Tile(i, j, Tile.State.DISABLED, null);
                }
                else
                {
                    Tile replacedTile = m_tiles[GetTileIndexForColumnLine(i + minColumnIndex - 2, j + minLineIndex - 2)];
                    tile = new Tile(i, j, replacedTile.CurrentState, replacedTile.AttachedBonus);
                }

                int newTileIndex = i * newFloorHeight + j;
                newTiles[newTileIndex] = tile;
            }
        }

        Floor clampedFloor = new Floor(newFloorWidth, newFloorHeight, newTiles);
        return clampedFloor;
    }

    /**
    * Do the reverse operation of Clamp(). Generates a square floor of size 'floorSize'
    **/
    public Floor Unclamp(int floorSize)
    {
        if (floorSize < m_gridWidth || floorSize < m_gridHeight)
        {
            throw new System.Exception("Set a bigger floor size to encapsulate current floor bounds");
        }

        Floor floor = new Floor(floorSize, floorSize);

        int minColumnIndex = (floorSize - m_gridWidth) / 2;
        int maxColumnIndex = minColumnIndex + m_gridWidth;
        int minLineIndex = (floorSize - m_gridHeight) / 2;
        int maxLineIndex = minLineIndex + m_gridHeight;

        for (int i = minColumnIndex; i != maxColumnIndex; i++)
        {
            for (int j = minLineIndex; j != maxLineIndex; j++)
            {
                Tile replacedTile = m_tiles[(i - minColumnIndex) * m_gridHeight + (j - minLineIndex)];

                int tileIndex = i * floorSize + j;
                floor.m_tiles[tileIndex] = new Tile(i, j, replacedTile.CurrentState, replacedTile.AttachedBonus);
            }
        }

        return floor;
    }

    /**
    * As a bonus has no reference to its parent tile, use this method to find the tile that has the parameter 'bonus' as a bonus
    **/
    public Tile FindTileForBonus(Bonus bonus)
    {
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].AttachedBonus == bonus)
                return m_tiles[i];
        }

        return null;
    }

    /**
    * Traverse the floor tiles and find those who are blocked i.e disabled and surrounded by 2 normal tiles either horizontally or vertically
    **/
    public void AssignBlockedTiles()
    {
        for (int i = 0; i != m_tiles.Length; i++)
        {
            Tile tile = m_tiles[i];
            if (tile.CurrentState != Tile.State.DISABLED)
                continue;
            if (tile.m_columnIndex > 0 && tile.m_columnIndex < this.m_gridWidth - 1 && tile.m_lineIndex > 0 && tile.m_lineIndex < this.m_gridHeight - 1)
            {
                int linePreviousTileIndex = (tile.m_columnIndex - 1) * m_gridHeight + tile.m_lineIndex;
                int lineNextTileIndex = (tile.m_columnIndex + 1) * m_gridHeight + tile.m_lineIndex;
                Tile linePreviousTile = m_tiles[linePreviousTileIndex];
                Tile lineNextTile = m_tiles[lineNextTileIndex];

                if (linePreviousTile.CurrentState == Tile.State.NORMAL && lineNextTile.CurrentState == Tile.State.NORMAL)
                    tile.CurrentState = Tile.State.BLOCKED;
                else
                {
                    Tile columnPreviousTile = m_tiles[i - 1];
                    Tile columnNextTile = m_tiles[i + 1];

                    if (columnPreviousTile.CurrentState == Tile.State.NORMAL && columnNextTile.CurrentState == Tile.State.NORMAL)
                        tile.CurrentState = Tile.State.BLOCKED;
                }
            }
        }
    }

    /**
    * Clear the cached values
    **/
    public void ClearCachedValues()
    {
        m_startTile = null;
        m_finishTile = null;
        m_bonusTiles = null;
    }

    /**
    * Return floor edges that are visible from camera point of view and create a contour from them
    **/
    public void FindVisibleContours(out List<Geometry.Edge> frontLeftContour, out List<Geometry.Edge> frontRightContour)
    {
        //traverse the floor and find the first tile with state NORMAL on each line
        frontLeftContour = new List<Geometry.Edge>();
        for (int i = 0; i != m_gridHeight; i++)
        {
            List<Tile> tiles = FindTilesWithVisibleLeftFaceOnLine(i);

            for (int j = 0; j != tiles.Count; j++)
            {
                Tile tile = tiles[j];

                if (tile == null)
                    continue;

                Vector3 tileEdgePoint1 = tile.GetLocalPosition() + new Vector3(-0.5f * tile.m_size, 0, 0.5f * tile.m_size);
                Vector3 tileEdgePoint2 = tile.GetLocalPosition() + new Vector3(-0.5f * tile.m_size, 0, -0.5f * tile.m_size);

                frontLeftContour.Add(new Geometry.Edge(tileEdgePoint1, tileEdgePoint2));
            }
        }

        
        //do the same for each column
        frontRightContour = new List<Geometry.Edge>();
        for (int i = 0; i != m_gridWidth; i++)
        {
            List<Tile> tiles = FindTilesWithVisibleRightFaceOnColumn(i);

            for (int j = 0; j != tiles.Count; j++)
            {
                Tile tile = tiles[j];

                if (tile == null)
                    continue;

                Vector3 tileEdgePoint1 = tile.GetLocalPosition() + new Vector3(-0.5f * tile.m_size, 0, -0.5f * tile.m_size);
                Vector3 tileEdgePoint2 = tile.GetLocalPosition() + new Vector3(0.5f * tile.m_size, 0, -0.5f * tile.m_size);

                frontRightContour.Add(new Geometry.Edge(tileEdgePoint1, tileEdgePoint2));
            }
        }
    }

    private List<Tile> FindTilesWithVisibleLeftFaceOnLine(int lineIndex)
    {
        List<Tile> tiles = new List<Tile>();

        int i = 0; 
        while (i < m_gridWidth)
        {
            int tileIndex = i * m_gridHeight + lineIndex;
            Tile tile = m_tiles[tileIndex];

            if (tile.CurrentState != Tile.State.DISABLED)
            {
                if (i == 0)
                    tiles.Add(tile);
                else
                {
                    int previousTileIndex = (i - 1) * m_gridHeight + lineIndex;
                    Tile previousTile = m_tiles[previousTileIndex];
                    if (previousTile.CurrentState == Tile.State.DISABLED)
                        tiles.Add(tile);
                }
            }

            i++;
        }

        return tiles;
    }

    private List<Tile> FindTilesWithVisibleRightFaceOnColumn(int columnIndex)
    {
        List<Tile> tiles = new List<Tile>();

        int i = 0;
        while (i < m_gridHeight)
        {
            int tileIndex = columnIndex * m_gridHeight + i;
            Tile tile = m_tiles[tileIndex];

            if (tile.CurrentState != Tile.State.DISABLED)
            {
                if (i == 0)
                    tiles.Add(tile);
                else
                {
                    int previousTileIndex = columnIndex * m_gridHeight + (i - 1);
                    Tile previousTile = m_tiles[previousTileIndex];
                    if (previousTile.CurrentState == Tile.State.DISABLED)
                        tiles.Add(tile);
                }
            }

            i++;
        }

        return tiles;
    }
}