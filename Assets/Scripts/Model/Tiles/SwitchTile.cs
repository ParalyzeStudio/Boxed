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
    }

    public SwitchTile(Tile tile, List<TriggeredTile> triggeredTiles) : this(tile.m_columnIndex, tile.m_lineIndex, null, triggeredTiles)
    {
        if (tile.AttachedBonus != null)
            AttachedBonus = new Bonus(tile.AttachedBonus);

        if (triggeredTiles == null)
            triggeredTiles = new List<TriggeredTile>();
    }

    public SwitchTile(SwitchTile other) : this(other, null)
    {
        m_isOn = other.m_isOn;

        if (other.m_triggeredTiles != null)
        {
            m_triggeredTiles = new List<TriggeredTile>(other.m_triggeredTiles.Count);
            for (int p = 0; p != other.m_triggeredTiles.Count; p++)
            {
                TriggeredTile copiedTriggeredTile = new TriggeredTile(other.m_triggeredTiles[p]);
                m_triggeredTiles.Add(copiedTriggeredTile);
            }
        }

        ////deep copy triggered tiles
        //for (int p = 0; p != other.m_triggeredTiles.Count; p++)
        //{
        //    TriggeredTile triggeredTile = other.m_triggeredTiles[p];
        //    //int triggeredTileIndex = GetTileIndex(triggeredTile);
        //    TriggeredTile copiedTriggeredTile = new TriggeredTile(triggeredTile);
        //    //copy it both to floor and switch tiles referenced triggered tiles
        //    //m_tiles[triggeredTileIndex] = copiedTriggeredTile;
        //    m_triggeredTiles[p] = copiedTriggeredTile;
        //}
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

    public void OnSelect()
    {
        //change the color of the switch tile to show that it is selected
        TileRenderer tileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(this);
        //ThemeManager thememanager currentTheme = GameController.GetInstance().GetComponent<ThemeManager>().m_selectedTheme;
        //Color switchTileColor = currentTheme.m_switchTileColor;
        //Color selectedColor = ColorUtils.LightenColor(switchTileColor, 0.25f);
        tileRenderer.SetTileColor(Color.yellow);


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
        Tile normalTile = new Tile(this);
        Level editedLevel = ((LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI).m_editedLevel;
        editedLevel.m_floor.InsertTile(normalTile);

        GameController.GetInstance().m_floorRenderer.ReplaceTileOnRenderer(normalTile);

        if (m_triggeredTiles != null)
        {
            for (int i = 0; i != m_triggeredTiles.Count; i++)
            {
                m_triggeredTiles[i].CurrentState = Tile.State.NORMAL;
            }
        }
    }
}
