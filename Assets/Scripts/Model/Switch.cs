using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Switch
{
    public Tile m_switchTile; //the tile used as a switch
    public List<Tile> m_triggeredTiles; //the tiles triggered by the switch
    public bool m_isOn;

    public Switch()
    {
        m_triggeredTiles = new List<Tile>();
    }

    public Switch(Tile switchTileRenderer, List<Tile> triggeredTilesRenderers)
    {
        m_switchTile = switchTileRenderer;
        m_triggeredTiles = triggeredTilesRenderers;
        m_isOn = false;
        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].CurrentState = Tile.State.BLOCKED;
        }
    }

    public void SetOnOff(bool bSetOn)
    {
        m_isOn = bSetOn;

        if (bSetOn)
        {
            for (int i = 0; i != m_triggeredTiles.Count; i++)
            {
                m_triggeredTiles[i].CurrentState = Tile.State.TRIGGERED_BY_SWITCH;
            }
        }
        else
        {
            for (int i = 0; i != m_triggeredTiles.Count; i++)
            {
                m_triggeredTiles[i].CurrentState = Tile.State.BLOCKED;
            }
        }
    }

    public void Toggle()
    {
        SetOnOff(!m_isOn);
    }

    public void ReplaceTile(Tile oldTile, Tile newTile)
    {
        if (m_switchTile == oldTile)
            m_switchTile = newTile;

        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            if (m_triggeredTiles[i] == oldTile)
                m_triggeredTiles[i] = newTile;
        }
    }

    public void OnSelect()
    {
        if (m_switchTile != null)
        {
            //change the color of the switch tile to show that it is selected
            TileRenderer switchTileRenderer = GameController.GetInstance().m_floor.GetRendererForTile(m_switchTile);
            TileColors colors = switchTileRenderer.TileColors;
            colors.Darken(0.5f);
            switchTileRenderer.SetColors(colors);
        }

        //if (m_triggeredTiles != null)
        //{
        //    //show triggered tiles
        //    for (int i = 0; i != m_triggeredTiles.Count; i++)
        //    {
        //        m_triggeredTiles[i].CurrentState = m_isOn ? Tile.State.BLOCKED : Tile.State.TRIGGERED_BY_SWITCH;
        //    }
        //}
    }

    public void OnDeselect()
    {
        if (m_switchTile != null)
        {
            //reset the colors of that tile by resetting its state
            m_switchTile.CurrentState = Tile.State.SWITCH;
        }

        //if (m_triggeredTiles != null)
        //{
        //    //show triggered tiles
        //    for (int i = 0; i != m_triggeredTiles.Count; i++)
        //    {
        //        m_triggeredTiles[i].CurrentState = Tile.State.NORMAL;
        //    }
        //}
    }

    public void OnRemove()
    {
        m_switchTile.CurrentState = Tile.State.NORMAL;

        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].CurrentState = Tile.State.NORMAL;
        }
    }
}
