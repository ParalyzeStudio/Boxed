using UnityEngine;

public class Floor : MonoBehaviour
{
    public GameObject m_tilePfb; //a cuboid tile

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

    private GameController m_gameController;

    public void Build()
    {
        //Build a square grid with odd dimensions around the origin
        m_gridWidth = 50;
        m_gridHeight = 50;
        m_tiles = new Tile[m_gridWidth * m_gridHeight];
        for (int i = 0; i != m_gridWidth; i++)
        {
            float x = i - m_gridWidth / 2;
            for (int j = 0; j != m_gridHeight; j++)
            {
                float z = j - m_gridHeight / 2;
                GameObject tileObject = (GameObject)Instantiate(m_tilePfb);
                tileObject.transform.parent = this.transform;
                
                Tile tile = tileObject.GetComponent<Tile>();
                tile.Init(this, i, j, Tile.State.NORMAL);

                tileObject.transform.localPosition = new Vector3(x * tile.m_size, 0, z * tile.m_size);

                m_tiles[GetTileIndexForColumnLine(i, j)] = tile;
            }
        }
    }

    /**
    * Return the index of the tile in the global tiles array given the line and the column of this tile inside the grid
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


    /**
    * Bake tiles in one single mesh for performance
    **/
    public void BakeTiles()
    {

    }

    /**
    * When in level editor mode, we can save the current state of the floor by removing useless tiles
    **/
    public void PrepareForSaving()
    {
        //first determine the minimum/maximum x and z coordinates of our selected tiles
        int minColumnIndex = int.MaxValue;
        int maxColumnIndex = int.MinValue;
        int minLineIndex = int.MaxValue;
        int maxLineIndex = int.MinValue;

        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].m_state == Tile.State.SELECTED)
            {
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
        int newFloorWidth = (maxColumnIndex - minColumnIndex + 1) + 4;
        int newFloorHeight = (maxLineIndex - minLineIndex + 1) + 4;

        Tile[] newTiles = new Tile[newFloorWidth * newFloorHeight];

        for (int i = 0; i != newFloorWidth; i++)
        {
            float x = i - newFloorWidth / 2;
            for (int j = 0; j != newFloorHeight; j++)
            {
                float z = j - newFloorHeight / 2;

                GameObject tileObject = (GameObject)Instantiate(m_tilePfb);
                tileObject.transform.parent = this.transform;
                tileObject.transform.localPosition = new Vector3(x, 0, z);

                Tile tile = tileObject.GetComponent<Tile>();

                if (i < 2 || i > newFloorWidth - 3 || j < 2 || j > newFloorHeight - 3)
                    tile.Init(this, i, j, Tile.State.DISABLED);
                else
                {
                    Tile replacedTile = m_tiles[GetTileIndexForColumnLine(i + minColumnIndex - 2, j + minLineIndex - 2)]; //the tile that is replaced by a new one
                    if (replacedTile.m_state == Tile.State.SELECTED)
                        tile.Init(this, i, j, Tile.State.NORMAL);
                    else if (replacedTile.m_state == Tile.State.NORMAL)
                        tile.Init(this, i, j, Tile.State.DISABLED);
                }

                int newTileIndex = i * newFloorHeight + j;
                newTiles[newTileIndex] = tile;
            }
        }       

        //Delete old tiles
        for (int i = 0; i != m_tiles.Length; i++)
        {
            Destroy(m_tiles[i].gameObject);
        }
        
        //Assign new ones
        m_tiles = newTiles;
        m_gridWidth = newFloorWidth;
        m_gridHeight = newFloorHeight;     
    }

    public GameController GetGameController()
    {
        if (m_gameController == null)
            m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        return m_gameController;
    }
}
