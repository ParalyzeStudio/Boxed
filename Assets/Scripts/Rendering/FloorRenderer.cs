﻿using UnityEngine;

public class FloorRenderer : MonoBehaviour
{
    public Floor m_floorData { get; set; }
    public FloorSupportRenderer m_floorSupportPfb;
    public TileRenderer m_tilePfb; //a cuboid tile

    public GameObject m_tilesHolder;

    private GameController m_gameController;

    private TileRenderer[] m_tileRenderers;

    public struct TileColors
    {
        public Color m_leftFaceColor;
        public Color m_rightFaceColor;
        public Color m_topFaceColor;
        public Color m_contourColor;

        public TileColors(Color leftFaceColor, Color rightFaceColor, Color topFaceColor, Color contourColor)
        {
            m_leftFaceColor = leftFaceColor;
            m_rightFaceColor = rightFaceColor;
            m_topFaceColor = topFaceColor;
            m_contourColor = contourColor;
        }
    }

    public void Render(Floor floor, bool bStripDisabledTiles = false)
    {
        m_floorData = floor;

        //TMP Offset the floor so it is centered in the screen
        this.transform.position = new Vector3(-0.5f * floor.m_gridWidth, -0.5f * Tile.TILE_DEFAULT_HEIGHT, -0.5f * floor.m_gridHeight);

        //Build a square grid with odd dimensions around the origin
        m_tileRenderers = new TileRenderer[floor.Tiles.Length];
        for (int i = 0; i != floor.Tiles.Length; i++)
        {
            Tile tile = floor.Tiles[i];

            //remove disabled tiles only in game mode
            if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME && tile.CurrentState == Tile.State.DISABLED)
                continue;

            //Tile renderer
            TileRenderer tileRenderer = Instantiate(m_tilePfb);
            tileRenderer.transform.parent = m_tilesHolder.transform;
            tileRenderer.Init(tile);

            //tileRenderer.UpdateTileColor();
            //tileRenderer.UpdateTileDecal();

            //set correct positions for tile and its support
            tileRenderer.transform.localPosition = tile.GetLocalPosition();

            m_tileRenderers[i] = tileRenderer;
        }

        //render the floor support (only in game mode)
        if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME)
        {
            FloorSupportRenderer supportRenderer = Instantiate(m_floorSupportPfb);
            supportRenderer.name = "Support";
            supportRenderer.transform.parent = this.transform;
            supportRenderer.transform.localPosition = new Vector3(0, -0.5f * Tile.TILE_DEFAULT_HEIGHT, 0);
            supportRenderer.Render(floor);
        }
    }

    public TileRenderer GetRendererForTile(Tile tile)
    {
        return GetRendererForTileColumnLine(tile.m_columnIndex, tile.m_lineIndex);
    }

    public TileRenderer GetRendererForTileColumnLine(int column, int line)
    {
        return m_tileRenderers[m_floorData.GetTileIndexForColumnLine(column, line)];
    }

    public void ReplaceTileOnRenderer(Tile newTile)
    {
        TileRenderer renderer = GetRendererForTileColumnLine(newTile.m_columnIndex, newTile.m_lineIndex);
        renderer.m_tile = newTile;
        renderer.Invalidate();
    }

    public void Invalidate()
    {
        for (int i = 0; i != m_tileRenderers.Length; i++)
        {
            m_tileRenderers[i].m_tile = m_floorData.Tiles[i];
            m_tileRenderers[i].Invalidate();
        }
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
