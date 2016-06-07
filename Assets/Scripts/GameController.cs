using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject m_levelEditorPfb;
    public GameObject m_canvas;

    public GameObject m_brickPfb;
    public GameObject m_floorPfb;

    public BrickRenderer m_brick { get; set; }
    public FloorRenderer m_floor { get; set; }
    public GameObject m_bonuses { get; set; } //use this object to hold bonus objects

    public bool m_levelEditorMode;

    private static GameController s_instance;

    public void Start()
    {
        if (m_levelEditorMode)
        {
            EnterLevelEditor();
        }
        else
        {
            //LevelManager levelManager = this.GetComponent<LevelManager>();
            //levelManager.CacheLevels();
            //StartLevel(levelManager.GetLevelForNumber(4));
        }
    }

    public static GameController GetInstance()
    {
        if (s_instance == null)
            s_instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        return s_instance;
    }

    public void EnterLevelEditor()
    {        
        //build the level editor object
        GameObject levelEditorObject = (GameObject)Instantiate(m_levelEditorPfb);
        levelEditorObject.name = "LevelEditor";
        levelEditorObject.transform.SetParent(m_canvas.transform, false);

        levelEditorObject.GetComponent<LevelEditor>().Init();
    }

    public void ClearLevel()
    {
        RemoveFloor();
        RemoveBrick();
        RemoveBonuses();
    }

    public void RemoveFloor()
    {
        if (m_floor != null)
        {
            Destroy(m_floor.gameObject);
            m_floor = null;
        }
    }

    public void RemoveBrick()
    {
        if (m_brick != null)
        {
            Destroy(m_brick.gameObject);
            m_brick = null;
        }
    }

    public void RemoveBonuses()
    {
        if (m_bonuses != null)
        {
            Destroy(m_bonuses.gameObject);
            m_bonuses = null;
        }
    }

    public void StartLevel(Level level)
    {
        //clear old displayed level if any
        ClearLevel();

        BuildBonusesHolder();

        Floor clampedFloor = level.m_floor.Clamp();
        RenderFloor(clampedFloor);
        BuildBrick(level);
    }

    public void RenderFloor(Floor floor)
    {
        if (m_floor != null)
        {
            Destroy(m_floor.gameObject);
            m_floor = null;
        }
        //Render the floor
        GameObject floorObject = (GameObject)Instantiate(m_floorPfb);
        m_floor = floorObject.GetComponent<FloorRenderer>();
        m_floor.Render(floor);
    }

    private void BuildBrick(Level level)
    {
        if (level != null)
        {
            GameObject brickObject = (GameObject)Instantiate(m_brickPfb);
            m_brick = brickObject.GetComponent<BrickRenderer>();

            m_brick.BuildOnTile((level == null) ? m_floor.m_floorData.GetCenterTile() : level.m_floor.GetStartTile());
            //m_brick.BuildOnTile(m_floor.m_floorData.Tiles[1]);
        }
    }

    public void BuildBonusesHolder()
    {
        m_bonuses = new GameObject("Bonuses");
    }
}
