using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SwitchTile : Tile
{
    public List<TriggeredTile> m_triggeredTiles; //the tiles triggered by the switch
    public bool m_isOn;

    public SwitchTile(int columnIndex, int lineIndex, Bonus bonus) : base(columnIndex, lineIndex, Tile.State.SWITCH, bonus)
    {
        m_triggeredTiles = new List<TriggeredTile>();
    }

    public SwitchTile(int columnIndex, int lineIndex, Bonus bonus, List<TriggeredTile> triggeredTiles) : this(columnIndex, lineIndex, bonus)
    {
        if (triggeredTiles != null)
            m_triggeredTiles = triggeredTiles;

        m_isOn = false;
        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].CurrentState = Tile.State.BLOCKED;
        }
    }

    public SwitchTile(Tile tile, List<TriggeredTile> triggeredTiles) : this(tile.m_columnIndex, tile.m_lineIndex, tile.AttachedBonus, triggeredTiles)
    {
        if (triggeredTiles == null)
            triggeredTiles = new List<TriggeredTile>();
    }

    public SwitchTile(SwitchTile other) : this(other, other.m_triggeredTiles)
    {

    }

    public void SetOnOff(bool bSetOn)
    {
        m_isOn = bSetOn;

        //invert the position of triggered tiles (blocked/unblocked)
        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].m_isLiftUp = !m_triggeredTiles[i].m_isLiftUp;
            m_triggeredTiles[i].m_tileStateDirty = true;
        }
    }

    public void Toggle()
    {
        SetOnOff(!m_isOn);
    }

    public bool ReplaceTriggerTile(TriggeredTile oldTile, TriggeredTile newTile)
    {
        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            if (m_triggeredTiles[i] == oldTile)
            {
                m_triggeredTiles[i] = newTile;
                return true;
            }
        }

        return false;
    }

    public bool ReplaceTriggerTile(TriggeredTile newTile)
    {
        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            Tile triggeredTile = m_triggeredTiles[i];
            if (triggeredTile.m_columnIndex == newTile.m_columnIndex && triggeredTile.m_lineIndex == newTile.m_lineIndex)
            {
                m_triggeredTiles[i] = newTile;
                return true;
            }
        }

        return false;
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
        Debug.Log("onRemove");
        //CurrentState = Tile.State.NORMAL; //TODO replace the tile with a normal one
        Tile normalTile = new Tile(this);
        Level editedLevel = ((LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI).m_editedLevel;
        editedLevel.m_floor.InsertTile(normalTile);

        GameController.GetInstance().m_floor.ReplaceTileOnRenderer(normalTile);

        if (m_triggeredTiles != null)
        {
            for (int i = 0; i != m_triggeredTiles.Count; i++)
            {
                m_triggeredTiles[i].CurrentState = Tile.State.NORMAL;
            }
        }
    }
}
