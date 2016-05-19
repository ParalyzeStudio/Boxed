using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum State
    {
        DISABLED,
        NORMAL,
        SELECTED,
    }

    public State m_state { get; set; }

    private Floor m_parentFloor;
    public int m_index { get; set; } //index of the tile inside the floor grid
    
    //materials
    public Material m_defaultTileMaterial;
    public Material m_disabledTileMaterial;
    public Material m_selectedTileMaterial;

    public void Init(Floor parentFloor, int index, State state)
    {
        m_parentFloor = parentFloor;
        m_index = index;

        SetState(state);
    }

    public void SetState(State state)
    {
        m_state = state;
        Material material = null;
        if (state == State.NORMAL)
            material = m_defaultTileMaterial;
        else if (state == State.SELECTED)
            material = m_selectedTileMaterial;
        else if (state == State.DISABLED)
            material = m_disabledTileMaterial;

        this.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /**
    * Return the index of the tile next to this tile given a direction (left, top, right or bottom)
    * The tile should never be null because we will design our grid in a way the brick will never land onto a null tile but either on a normal 
    * or a disabled one (a hole) leading the part of the brick to be destroyed/dismantled/exploded
    * We can obtain a null tile only during development time for testing purposes, therefore this method can actually return null.
    **/
    public Tile GetNextTileForDirection(Brick.RollDirection direction)
    {
        int nextTileIndex = -1;
        int gridSize = m_parentFloor.m_gridSize;
        if (direction == Brick.RollDirection.LEFT)
        {
            if (m_index >= gridSize)
                nextTileIndex = m_index - gridSize;
        }
        else if (direction == Brick.RollDirection.TOP)
        {
            if (m_index % gridSize != gridSize - 1)
                nextTileIndex = m_index + 1;
        }
        else if (direction == Brick.RollDirection.RIGHT)
        {
            if (m_index <= (gridSize * (gridSize - 1)))
                nextTileIndex = m_index + gridSize;
        }
        else
        {
            if (m_index % gridSize != 0)
                nextTileIndex = m_index - 1;
        }

        if (nextTileIndex == -1)
            return null;
        else
            return m_parentFloor.Tiles[nextTileIndex].GetComponent<Tile>();
    }
}