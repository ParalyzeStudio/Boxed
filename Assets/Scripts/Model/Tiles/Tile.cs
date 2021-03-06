﻿using System;
using UnityEngine;

[Serializable]
public class Tile
{
    public float m_size { get; set; } //tile dimension along x and z

    private const float TILE_SIZE = 1.0f; //tile dimension along x and z
    public const float TILE_HEIGHT = 1.0f; //tile dimension along y

    public enum State
    {
        DISABLED,
        NORMAL,
        START, //tile used to show the start tile on the floor (where the brick starts)
        FINISH, //tile used to show the finish tile on the floor (where the brick has to end)
        BLOCKED, //we cannot land on this tile, the rotation movement of the brick is blocked
        TRAP, //brick will explode on this brick
        SWITCH, //switch that will make some tiles appear or disappear on the floor
        TRIGGERED_BY_SWITCH, //tiles that are triggered by a switch
        ICE, //tiles that can be destroyed after the brick has rolled on them
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
            m_tileStateDirty = true;
        }
    }

    public bool m_tileStateDirty { get; set; }

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

    public Tile(Tile other) : this(other.m_columnIndex, other.m_lineIndex, other.m_currentState, null)
    {
        if (other.AttachedBonus != null)
            m_attachedBonus = new Bonus(other.AttachedBonus);
    }

    /**
    * Get the local position of the tile depending on its column/line values
    **/
    public Vector3 GetLocalPosition()
    {
        float tilePositionY;
        if (CurrentState == State.BLOCKED)
            tilePositionY = 0.5f * TILE_HEIGHT;
        else if (CurrentState == State.TRIGGERED_BY_SWITCH)
        {
            if (((TriggeredTile)this).m_isLiftUp)
                tilePositionY = 0.5f * TILE_HEIGHT;
            else
                tilePositionY = 0;
        }
        else
            tilePositionY = 0;

        return new Vector3(m_columnIndex * m_size, tilePositionY, m_lineIndex * m_size);
    }

    /**
    * Get the world position of the tile
    **/
    public Vector3 GetWorldPosition()
    {
        Vector3 parentFloorPosition = GameController.GetInstance().m_floorRenderer.transform.position;
        return parentFloorPosition + GetLocalPosition();
    }

    /**
    * Same as GetLocalPosition() but with y-component removed
    **/
    public Vector3 GetXZLocalPosition()
    {
        Vector3 localPosition = GetLocalPosition();
        return new Vector3(localPosition.x, 0, localPosition.z);
    }

    /**
    * Same as GetWorldPosition() but with y-component removed
    **/
    public Vector3 GetXZWorldPosition()
    {
        Vector3 worldPosition = GetWorldPosition();
        return new Vector3(worldPosition.x, 0, worldPosition.z);
    }

    /**
    * Tell if this tile contains a point without taking account of its y coordinate
    **/
    public bool ContainsXZPoint(Vector2 point)
    {
        Vector3 tileWorldPosition = GetWorldPosition();
        float tileMinX = tileWorldPosition.x - 0.5f * m_size;
        float tileMaxX = tileWorldPosition.x + 0.5f * m_size;
        float tileMinZ = tileWorldPosition.z - 0.5f * m_size;
        float tileMaxZ = tileWorldPosition.z + 0.5f * m_size;

        return (point.x >= tileMinX && point.x <= tileMaxX && point.y >= tileMinZ && point.y <= tileMaxZ);
    }

    public bool HasSameFloorPosition(Tile other)
    {
        return (this.m_columnIndex == other.m_columnIndex) && (this.m_lineIndex == other.m_lineIndex);
    }

    public bool IsBlocking()
    {
        if (CurrentState == State.BLOCKED)
            return true;

        if (CurrentState == State.TRIGGERED_BY_SWITCH && ((TriggeredTile)this).m_isLiftUp)
            return true;

        if (CurrentState == State.ICE && ((IceTile)this).m_blocked)
            return true;

        return false;
    }

    public override string ToString()
    {
        return "l:" + m_lineIndex + " c:" + m_columnIndex + (m_currentState == State.DISABLED ? "" : " s:" + m_currentState);
    }
}