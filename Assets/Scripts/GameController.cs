using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_levelEditorPfb;
    public GameObject m_canvas;

    public GameObject m_brickPfb;
    public GameObject m_floorPfb;

    public Brick m_brick { get; set; }
    public Floor m_floor { get; set; }

    public bool m_levelEditorMode;

    public void Start()
    {
        if (m_levelEditorMode)
        {
            EnterLevelEditor();
        }
        else
        {
            StartLevel(1);
        }
    }

    public void EnterLevelEditor()
    {
        BuildFloor(null);
        GameObject levelEditorObject = (GameObject)Instantiate(m_levelEditorPfb);
        levelEditorObject.name = "LevelEditor";
        levelEditorObject.transform.SetParent(m_canvas.transform, false);
    }

    public void StartLevel(int levelNumber)
    {
        Level level = this.GetComponent<LevelManager>().GetLevelForNumber(levelNumber);
        BuildFloor(level);
        BuildBrick(level);
    }

    private void BuildFloor(Level level)
    {
        GameObject floorObject = (GameObject)Instantiate(m_floorPfb);
        m_floor = floorObject.GetComponent<Floor>();
        m_floor.Build();
    }

    private void BuildBrick(Level level)
    {
        GameObject brickObject = (GameObject)Instantiate(m_brickPfb);
        m_brick = brickObject.GetComponent<Brick>();
        
        //m_brick.BuildOnTiles((level == null) ? m_floor.GetCenterTile() : level.m_startTile, null);
        m_brick.BuildOnTiles(m_floor.Tiles[1], m_floor.Tiles[2]);
    }
}
