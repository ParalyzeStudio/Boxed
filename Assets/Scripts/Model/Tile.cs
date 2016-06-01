using System;
using UnityEngine;

[Serializable]
public class Tile
{
    public float m_size { get; set; }

    private const float TILE_SIZE = 1.0f;

    public enum State
    {
        DISABLED,
        NORMAL,
        SELECTED,
        START, //tile used to show the start tile on the floor (where the brick starts)
        FINISH //tile used to show the finish tile on the floor (where the brick has to end)
    }

    private State m_currentState;
    public State CurrentState
    {
        get
        {
            return m_currentState;
        }

        set
        {
            m_currentState = value;
            m_tileMaterialDirty = true;
        }
    }

    public bool m_tileMaterialDirty { get; set; }

    //the (column,line) coordinates of the tile inside the rectangular grid floor
    public int m_lineIndex { get; set; }
    public int m_columnIndex { get; set; }

    //Bonus attached to this tile
    private Bonus m_attachedBonus;
    public Bonus AttachedBonus
    {
        get
        {
            return m_attachedBonus;
        }

        set
        {
            m_attachedBonus = value;
        }
    }


    public Tile(int columnIndex, int lineIndex, State state, Bonus bonus)
    {
        m_size = TILE_SIZE;
        m_lineIndex = lineIndex;
        m_columnIndex = columnIndex;
        m_currentState = state;
        m_attachedBonus = bonus;
    }    

    /**
    * Get the local position of the tile depending on its column/line values
    **/
    public Vector3 GetLocalPosition()
    {
        return new Vector3(m_columnIndex * m_size, 0, m_lineIndex * m_size);
    }

    /**
    * Get the world position of the tile
    **/
    public Vector3 GetWorldPosition()
    {
        Vector3 parentFloorPosition = GameController.GetInstance().m_floor.transform.position;
        return parentFloorPosition + GetLocalPosition();
    }

    /**
    * Tell if this tile contains a point without taking account of its y coordinate
    **/
    public bool ContainsXZPoint(Vector3 point)
    {
        Vector3 tileWorldPosition = GetWorldPosition();
        float tileMinX = tileWorldPosition.x - 0.5f * m_size;
        float tileMaxX = tileWorldPosition.x + 0.5f * m_size;
        float tileMinZ = tileWorldPosition.z - 0.5f * m_size;
        float tileMaxZ = tileWorldPosition.z + 0.5f * m_size;

        return (point.x >= tileMinX && point.x <= tileMaxX && point.z >= tileMinZ && point.z <= tileMaxZ);
    }

}