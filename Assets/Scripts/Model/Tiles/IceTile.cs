using System;

[Serializable]
public class IceTile : Tile
{
    public int m_hitPoints; //the tile will destroy itself after the brick has rolled onto it m_hitPoints times
    public bool m_blocked { get; set; }

    public IceTile(int columnIndex, int lineIndex, Bonus bonus, int hitPoints) : base(columnIndex, lineIndex, Tile.State.ICE, bonus)
    {
        m_hitPoints = hitPoints;
    }

    public IceTile(Tile tile, int hitPoints) : base(tile)
    {
        CurrentState = State.ICE;
        m_hitPoints = hitPoints;
    }

    public IceTile(IceTile other) : this(other, other.m_hitPoints)
    {
        
    }
}
