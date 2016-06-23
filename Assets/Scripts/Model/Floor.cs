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

    public List<Bonus> GetBonuses()
    {
        List<Bonus> bonuses = new List<Bonus>();
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].AttachedBonus != null)
                bonuses.Add(m_tiles[i].AttachedBonus);
        }

        return bonuses;
    }

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
    public void FindVisibleContours(out List<Vector3> frontLeftContour, out List<Vector3> frontRightContour)
    {
        //traverse the floor and find the first tile with state NORMAL on each line
        frontLeftContour = new List<Vector3>();
        int tileColumnIndex = -1;
        Vector3 edgeEndpoint = Vector3.zero;
        for (int i = m_gridHeight - 1; i >= 0; i--) //from top to bottom
        {
            Tile tile = FindFirstTileOnLine(i);

            Vector3 tileEdgePoint1 = tile.GetLocalPosition() + new Vector3(-0.5f * tile.m_size, 0, 0.5f * tile.m_size);
            Vector3 tileEdgePoint2 = tile.GetLocalPosition() + new Vector3(-0.5f * tile.m_size, 0, -0.5f * tile.m_size);

            if (tile.m_columnIndex == tileColumnIndex) //same line as previous tile, just make the segment longer
                edgeEndpoint = tileEdgePoint2;
            else
            {
                if (i < m_gridHeight - 1)
                    frontLeftContour.Add(edgeEndpoint); //close the previous segment
                frontLeftContour.Add(tileEdgePoint1);
                edgeEndpoint = tileEdgePoint2;
                tileColumnIndex = tile.m_columnIndex;
            }

            if (i == 0)
                frontLeftContour.Add(edgeEndpoint);
        }

        
        //do the same for each column
        frontRightContour = new List<Vector3>();
        int tileLineIndex = -1;
        edgeEndpoint = Vector3.zero;
        for (int i = 0; i != m_gridWidth; i++)
        {
            Tile tile = FindFirstTileOnColumn(i);

            Vector3 tileEdgePoint1 = tile.GetLocalPosition() + new Vector3(-0.5f * tile.m_size, 0, -0.5f * tile.m_size);
            Vector3 tileEdgePoint2 = tile.GetLocalPosition() + new Vector3(0.5f * tile.m_size, 0, -0.5f * tile.m_size);

            if (tile.m_lineIndex == tileLineIndex) //same line as previous tile, just make the segment longer
                edgeEndpoint = tileEdgePoint2;
            else
            {
                if (i > 0)
                    frontRightContour.Add(edgeEndpoint); //close the previous segment
                frontRightContour.Add(tileEdgePoint1);
                edgeEndpoint = tileEdgePoint2;
                tileLineIndex = tile.m_lineIndex;
            }

            if (i == m_gridWidth - 1)
                frontRightContour.Add(edgeEndpoint);
        }
    }

    private Tile FindFirstTileOnLine(int lineIndex)
    {
        int i = 0; 
        while (i < m_gridWidth)
        {
            int tileIndex = i * m_gridHeight + lineIndex;
            Tile tile = m_tiles[tileIndex];
            
            //if (tile.CurrentState == Tile.State.NORMAL)
                return tile;

            i++;
        }

        return null;
    }

    private Tile FindFirstTileOnColumn(int columnIndex)
    {
        int i = 0;
        while (i < m_gridHeight)
        {
            int tileIndex = columnIndex * m_gridHeight + i;
            Tile tile = m_tiles[tileIndex];
            
            //if (tile.CurrentState == Tile.State.NORMAL)
            return tile;

            i++;
        }

        return null;
    }
}