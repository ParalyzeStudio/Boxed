using System;
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
                Tile tile = new Tile(i, j, Tile.State.NORMAL, null);

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
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].CurrentState == Tile.State.START)
                return m_tiles[i];
        }

        return null;
    }

    public Tile GetFinishTile()
    {
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].CurrentState == Tile.State.FINISH)
                return m_tiles[i];
        }

        return null;
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
    public Floor ClampFloor()
    {
        //first determine the minimum/maximum x and z coordinates of our selected tiles
        int minColumnIndex = int.MaxValue;
        int maxColumnIndex = int.MinValue;
        int minLineIndex = int.MaxValue;
        int maxLineIndex = int.MinValue;

        bool bEmptyFloor = true;
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].CurrentState == Tile.State.SELECTED)
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
                    Tile replacedTile = m_tiles[GetTileIndexForColumnLine(i + minColumnIndex - 2, j + minLineIndex - 2)]; //the tile that is replaced by a new one
                    if (replacedTile.CurrentState == Tile.State.SELECTED)
                        tile = new Tile(i, j, Tile.State.NORMAL, replacedTile.AttachedBonus);
                    else if (replacedTile.CurrentState == Tile.State.NORMAL)
                        tile = new Tile(i, j, Tile.State.DISABLED, replacedTile.AttachedBonus);
                    else
                        tile = new Tile(i, j, replacedTile.CurrentState, replacedTile.AttachedBonus);
                }

                int newTileIndex = i * newFloorHeight + j;
                newTiles[newTileIndex] = tile;
            }
        }

        Floor clampedFloor = new Floor(newFloorWidth, newFloorHeight, newTiles);
        return clampedFloor;
    }
}