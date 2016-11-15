using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : TileRenderer
{
    public int m_index { get; set; }
    public Level m_level { get; set; }

    private LevelsGUI m_parentGUI;

    private bool m_interactable;
    
    //private Color[] m_colors;
    //private Vector3[] m_vertices;
    //private bool m_colorsArrayDirty;
    //private bool m_verticesArrayDirty;

    public void Init(LevelsGUI parentGUI, int index)
    {
        m_parentGUI = parentGUI;
        
        //zero-initialization of a tile object
        Tile tile = new Tile(0, 0, Tile.State.NORMAL, null);
        tile.m_size = 1.3f;
        base.Init(tile);

        float slotHeight = 0.3f;
        BuildFaces(slotHeight, 0);

        //this.GetComponent<Button>().onClick.AddListener(delegate { parentGUI.OnSlotClick(this); });
        m_index = index;

        InvalidateLevel();
    }

    public void InvalidateLevel()
    {
        int localLevelNumber = m_index + 1;
        int absoluteLevelNumber = (m_parentGUI.m_chapterNumber - 1) * LevelManager.NUM_LEVELS_PER_CHAPTER + localLevelNumber;
        m_level = GameController.GetInstance().GetComponent<LevelManager>().GetLevelForNumber(absoluteLevelNumber);
        Invalidate();
    }

    private void Disable(bool bNullLevel)
    {
        m_interactable = bNullLevel;

        //TileColors colors = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme.m_defaultTileColors;

        //if (bNullLevel)
        //{
        //    colors.Darken(0.5f);
        //    colors.Fade(0.5f);
        //    ChangeColorsTo(colors, 0.5f);
        //    m_interactable = false;
        //}
        //else
        //{
        //    colors.Darken(0.35f);
        //    ChangeColorsTo(colors, 0.5f);
        //    m_interactable = true;
        //}
    }

    private void Enable()
    {
        //TileColors colors = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme.m_defaultTileColors;
        //ChangeColorsTo(colors, 0.5f);
        m_interactable = true;
    }

    public new void Invalidate()
    {
        if (m_level == null)
            Disable(true);
        else
        {
            LevelData levelData = LevelData.LoadFromFile(m_level.m_number);
            if (levelData == null)
                Disable(true);
            else if (!levelData.m_done)
                Disable(false);
            else
                Enable();
        }
    }    

    public void OnClick()
    {
        if (m_interactable)
        {
            LevelsGUI levelsGUI = (LevelsGUI)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
            levelsGUI.OnSlotClick(this);
        }
    }
}
