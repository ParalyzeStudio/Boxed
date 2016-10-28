using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SwitchTile : Tile
{
    public List<Tile> m_triggeredTiles; //the tiles triggered by the switch
    public bool m_isOn;

    public SwitchTile(int columnIndex, int lineIndex, Bonus bonus) : base(columnIndex, lineIndex, Tile.State.SWITCH, bonus)
    {
        m_triggeredTiles = new List<Tile>();
    }

    public SwitchTile(int columnIndex, int lineIndex, Bonus bonus, List<Tile> triggeredTiles) : this(columnIndex, lineIndex, bonus)
    {
        if (triggeredTiles != null)
            m_triggeredTiles = triggeredTiles;

        m_isOn = false;
        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].CurrentState = Tile.State.BLOCKED;
        }
    }

    public SwitchTile(Tile tile, List<Tile> triggeredTiles) : this(tile.m_columnIndex, tile.m_lineIndex, tile.AttachedBonus, triggeredTiles)
    {
        if (triggeredTiles == null)
            triggeredTiles = new List<Tile>();
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
        //if (m_switchTile == oldTile)
        //    m_switchTile = newTile;

        //for (int i = 0; i != m_triggeredTiles.Count; i++)
        //{
        //    if (m_triggeredTiles[i] == oldTile)
        //        m_triggeredTiles[i] = newTile;
        //}
    }

    public void OnSelect()
    {
        //change the color of the switch tile to show that it is selected
        TileRenderer tileRenderer = GameController.GetInstance().m_floor.GetRendererForTile(this);
        TileColors colors = tileRenderer.TileColors;
        colors.Darken(0.5f);
        tileRenderer.SetColors(colors);


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
        CurrentState = Tile.State.SWITCH;
    }

    public void OnRemove()
    {
        CurrentState = Tile.State.NORMAL;

        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].CurrentState = Tile.State.NORMAL;
        }
    }
}
