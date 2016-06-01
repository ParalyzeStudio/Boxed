using UnityEngine;

public class FloorRenderer : MonoBehaviour
{
    public Floor m_floorData { get; set; }
    public GameObject m_tilePfb; //a cuboid tile

    private GameController m_gameController;

    public TileRenderer[] m_tileRenderers;

    public void Render(Floor floor)
    {
        m_floorData = floor;

        //TMP Offset the floor so it is centered in the screen
        this.transform.position = new Vector3(-0.5f * floor.m_gridWidth, 0, -0.5f * floor.m_gridHeight);

        //Build a square grid with odd dimensions around the origin
        m_tileRenderers = new TileRenderer[floor.Tiles.Length];
        for (int i = 0; i != floor.Tiles.Length; i++)
        {
            Tile tile = floor.Tiles[i];
            //float x = tile.m_columnIndex - floor.m_gridWidth / 2;
            //float z = tile.m_lineIndex - floor.m_gridHeight / 2;

            GameObject tileObject = (GameObject)Instantiate(m_tilePfb);
            tileObject.transform.parent = this.transform;

            TileRenderer tileRenderer = tileObject.GetComponent<TileRenderer>();
            tileRenderer.Init(tile);

            //tileObject.transform.localPosition = new Vector3(x * tile.m_size, 0, z * tile.m_size);
            tileObject.transform.localPosition = tile.GetLocalPosition();

            m_tileRenderers[i] = tileRenderer;
        }
    }

    public TileRenderer GetRendererForTile(Tile tile)
    {
        return m_tileRenderers[m_floorData.GetTileIndexForColumnLine(tile.m_columnIndex, tile.m_lineIndex)];
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
