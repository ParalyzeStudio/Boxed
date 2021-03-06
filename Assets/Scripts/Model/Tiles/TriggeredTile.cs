﻿using System;

[Serializable]
public class TriggeredTile : Tile
{
    public bool m_isLiftUp;

    public TriggeredTile(int columnIndex, int lineIndex, Bonus bonus) : base(columnIndex, lineIndex, Tile.State.TRIGGERED_BY_SWITCH, bonus)
    {
        
    }

    public TriggeredTile(Tile tile) : this(tile.m_columnIndex, tile.m_lineIndex, tile.AttachedBonus)
    {

    }

    public TriggeredTile(TriggeredTile other) : this((Tile)other)
    {
        m_isLiftUp = other.m_isLiftUp;
    }
}
