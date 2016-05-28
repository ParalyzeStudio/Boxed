using UnityEngine;

public class Tile : MonoBehaviour
{
    public float m_size { get; set; }

    public enum State
    {
        DISABLED,
        NORMAL,
        SELECTED,
        START, //tile used to show the start tile on the floor (where the brick starts)
        FINISH //tile used to show the finish tile on the floor (where the brick has to end)
    }

    public State m_state { get; set; }

    private Floor m_parentFloor;

    //the (column,line) coordinates of the tile inside the rectangular grid floor
    public int m_lineIndex { get; set; }
    public int m_columnIndex { get; set; }

    //materials
    public Material m_defaultTileMaterial;
    public Material m_disabledTileMaterial;
    public Material m_selectedTileMaterial;
    public Material m_startTileMaterial;
    public Material m_finishTileMaterial;

    //Bonus
    public GameObject m_bonusPfb;
    private Bonus m_bonus;


    public void Init(Floor parentFloor, int columnIndex, int lineIndex, State state, float size = 1)
    {
        m_size = size;
        m_parentFloor = parentFloor;
        m_lineIndex = lineIndex;
        m_columnIndex = columnIndex;

        SetState(state);

        this.transform.localScale = new Vector3(0.95f * size, this.transform.localScale.y, 0.95f * size);
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
        else if (state == State.START)
            material = m_startTileMaterial;
        else if (state == State.FINISH)
            material = m_finishTileMaterial;

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
        int nextTileColumnIndex = -1;
        int nextTileLineIndex = -1;
        if (direction == Brick.RollDirection.LEFT)
        {
            nextTileColumnIndex = this.m_columnIndex - 1;
            nextTileLineIndex = this.m_lineIndex;
        }
        else if (direction == Brick.RollDirection.TOP)
        {
            nextTileColumnIndex = this.m_columnIndex;
            nextTileLineIndex = this.m_lineIndex + 1;
        }
        else if (direction == Brick.RollDirection.RIGHT)
        {
            nextTileColumnIndex = this.m_columnIndex + 1;
            nextTileLineIndex = this.m_lineIndex;
        }
        else
        {
            nextTileColumnIndex = this.m_columnIndex;
            nextTileLineIndex = this.m_lineIndex - 1;
        }

        if (nextTileColumnIndex >= 0 && nextTileColumnIndex < m_parentFloor.m_gridWidth
            &&
            nextTileLineIndex >= 0 && nextTileLineIndex < m_parentFloor.m_gridHeight)
        {

            int nextTileIndex = m_parentFloor.GetTileIndexForColumnLine(nextTileColumnIndex, nextTileLineIndex);
            return m_parentFloor.Tiles[nextTileIndex];
        }
        else
            return null;        
    }

    /**
    * Tell if this tile contains a point without taking account of its y coordinate
    **/
    public bool ContainsXZPoint(Vector3 point)
    {
        float tileMinX = this.transform.position.x - 0.5f * m_size;
        float tileMaxX = this.transform.position.x + 0.5f * m_size;
        float tileMinZ = this.transform.position.z - 0.5f * m_size;
        float tileMaxZ = this.transform.position.z + 0.5f * m_size;

        return (point.x >= tileMinX && point.x <= tileMaxX && point.z >= tileMinZ && point.z <= tileMaxZ);
    }

    /**
    * Set a bonus on this tile
    **/
    public void AddBonus()
    {
        GameObject bonusObject = (GameObject)Instantiate(m_bonusPfb);
        bonusObject.name = "Bonus";

        GameObject bonusContainer = GameController.GetInstance().m_bonuses;
        bonusObject.transform.parent = bonusContainer.transform;

        GameObjectAnimator bonusAnimator = bonusObject.GetComponent<GameObjectAnimator>();
        bonusAnimator.SetPosition(this.transform.position + new Vector3(0,0.3f,0));
    }

    public Bonus GetBonus()
    {
        return m_bonus;
    }
}