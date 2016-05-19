using UnityEngine;

public class Floor : MonoBehaviour
{
    public GameObject m_tilePfb; //a cuboid tile

    private GameObject[] m_tiles;
    public GameObject[] Tiles
    {
        get
        {
            return m_tiles;
        }
    }
    public int m_gridSize { get; set; }

    public Tile[] m_activeTiles; //tiles under the brick

    private GameController m_gameController;

    public void Build()
    {
        GameObject tilesHolder = new GameObject("Tiles");
        tilesHolder.transform.parent = this.transform;
        tilesHolder.transform.localPosition = Vector3.zero;

        //Build a square grid with odd dimensions around the origin
        m_gridSize = 50;
        m_tiles = new GameObject[m_gridSize * m_gridSize];
        for (int i = 0; i != m_gridSize; i++)
        {
            float x = i - m_gridSize / 2;
            for (int j = 0; j != m_gridSize; j++)
            {
                float z = j - m_gridSize / 2;
                GameObject tileObject = (GameObject)Instantiate(m_tilePfb);
                tileObject.transform.parent = tilesHolder.transform;
                tileObject.transform.localPosition = new Vector3(x, 0, z);

                int index = i * m_gridSize + j;
                Tile tile = tileObject.GetComponent<Tile>();
                tile.Init(this, i * m_gridSize + j, Tile.State.NORMAL);

                m_tiles[index] = tileObject;
            }
        }

        m_activeTiles = new Tile[2];
    }
       
    /**
    * Return the tile at the given position
    **/
    public GameObject GetTileForPosition(Vector3 position)
    {
        for (int i = 0; i != m_tiles.Length; i++)
        {
            if (m_tiles[i].transform.position == position)
                return m_tiles[i];
        }

        return null;
    }

    /**
    * Bake tiles in one single mesh for performance
    **/
    public void BakeTiles()
    {

    }

    public GameController GetGameController()
    {
        if (m_gameController == null)
            m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        return m_gameController;
    }
}
