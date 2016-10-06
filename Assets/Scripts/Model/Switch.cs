using System;
using UnityEngine;

[Serializable]
public class Switch
{
    public Tile m_switchTile; //the tile used as a switch
    private Tile[] m_triggeredTiles; //the tiles triggered by the switch

    public Switch()
    {
        
    }

    public Switch(Tile switchTileRenderer, Tile[] triggeredTilesRenderers)
    {
        m_switchTile = switchTileRenderer;
        m_triggeredTiles = triggeredTilesRenderers;
    }

    //private TileRenderer[] m_triggeredTiles;
    private bool m_isOn;

    //public SwitchTile(int columnIndex, int lineIndex, Bonus bonus, TileRenderer[] triggeredTiles) : base(columnIndex, lineIndex, Tile.State.TRIGGER, bonus)
    //{
    //    m_triggeredTiles = triggeredTiles;
    //}

    public void Toggle()
    {
        m_isOn = !m_isOn;

        if (m_isOn)
        {
            for (int i = 0; i != m_triggeredTiles.Length; i++)
            {
                m_triggeredTiles[i].CurrentState = Tile.State.NORMAL;
            }
        }
        else
        {
            for (int i = 0; i != m_triggeredTiles.Length; i++)
            {
                m_triggeredTiles[i].CurrentState = Tile.State.BLOCKED;
            }
        }
    }
}
