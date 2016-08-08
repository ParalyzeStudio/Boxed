using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : TileRenderer
{
    public int m_index { get; set; }
    public Level m_level { get; set; }

    private LevelsGUI m_parentGUI;
    
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

    /**
   * Build faces for this tile. The top face can have a contour on it, just set bDrawContourOnTopFace to true in this case and set a contourThicknessRatio
   * between 0 and 1, 0 meaning zero-thickness (no contour) and 1 meaning 0.5f * tile.m_size thickness
   **/
    //private void BuildFaces()
    //{
    //    float tileHeight = 0.3f;
    //    float tileSize = 1.0f;

    //    //build only two faces as the 3 other wont be visible
    //    m_vertices = new Vector3[20];
    //    m_vertices[0] = new Vector3(-0.5f * tileSize, -0.5f * tileHeight, 0.5f * tileSize);
    //    m_vertices[1] = new Vector3(-0.5f * tileSize, -0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[2] = new Vector3(-0.5f * tileSize, 0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[3] = new Vector3(-0.5f * tileSize, 0.5f * tileHeight, 0.5f * tileSize);
    //    m_vertices[4] = new Vector3(-0.5f * tileSize, -0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[5] = new Vector3(0.5f * tileSize, -0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[6] = new Vector3(0.5f * tileSize, 0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[7] = new Vector3(-0.5f * tileSize, 0.5f * tileHeight, -0.5f * tileSize);

    //    float innerSquareSize = 0.2f * tileSize;
    //    m_vertices[8] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);
    //    m_vertices[9] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
    //    m_vertices[10] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
    //    m_vertices[11] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);

    //    //build actual contour
    //    m_vertices[12] = new Vector3(-0.5f * tileSize, 0.5f * tileHeight, 0.5f * tileSize);
    //    m_vertices[13] = new Vector3(-0.5f * tileSize, 0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[14] = new Vector3(0.5f * tileSize, 0.5f * tileHeight, -0.5f * tileSize);
    //    m_vertices[15] = new Vector3(0.5f * tileSize, 0.5f * tileHeight, 0.5f * tileSize);
    //    m_vertices[16] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);
    //    m_vertices[17] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
    //    m_vertices[18] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
    //    m_vertices[19] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);

    //    int[] triangles = new int[] { 0, 2, 1, 0, 3, 2, //face 1
    //                                4, 6, 5, 4, 7, 6, //face 2
    //                                8, 10, 9, 8, 11, 10, //inner square                                    
    //                                12, 17, 13, 12, 16, 17, //contour
    //                                13, 18, 14, 13, 17, 18, //contour 
    //                                14, 19, 15, 14, 18, 19, //contour
    //                                15, 16, 12, 15, 19, 16, //contour
    //        };

    //    m_colors = new Color[20];

    //    Mesh facesMesh = new Mesh();
    //    facesMesh.name = "TileMesh";
    //    facesMesh.vertices = m_vertices;
    //    facesMesh.triangles = triangles;
    //    facesMesh.colors = m_colors;

    //    this.GetComponent<MeshFilter>().sharedMesh = facesMesh;
    //}

    //public void SetLeftFaceColor(Color color, bool bUpdateMeshDirectly = true)
    //{
    //    int i = 0;
    //    while (i < 4)
    //    {
    //        m_colors[i] = color;
    //        i++;
    //    }

    //    if (bUpdateMeshDirectly)
    //        GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
    //    else
    //        m_colorsArrayDirty = true;
    //}

    //public void SetRightFaceColor(Color color, bool bUpdateMeshDirectly = true)
    //{
    //    int i = 4;
    //    while (i < 8)
    //    {
    //        m_colors[i] = color;
    //        i++;
    //    }

    //    if (bUpdateMeshDirectly)
    //        GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
    //    else
    //        m_colorsArrayDirty = true;
    //}

    //public void SetTopFaceColor(Color color, bool bUpdateMeshDirectly = true)
    //{
    //    int i = 8;
    //    while (i < 12)
    //    {
    //        m_colors[i] = color;
    //        i++;
    //    }

    //    if (bUpdateMeshDirectly)
    //        GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
    //    else
    //        m_colorsArrayDirty = true;
    //}

    //public void SetContourColor(Color color, bool bUpdateMeshDirectly = true)
    //{
    //    int i = 12;
    //    while (i < 20)
    //    {
    //        m_colors[i] = color;
    //        i++;
    //    }

    //    if (bUpdateMeshDirectly)
    //        GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
    //    else
    //        m_colorsArrayDirty = true;
    //}

    public void InvalidateLevel()
    {
        int localLevelNumber = m_index + 1;
        int absoluteLevelNumber = (m_parentGUI.m_chapterNumber - 1) * LevelManager.NUM_LEVELS_PER_CHAPTER + localLevelNumber;
        m_level = GameController.GetInstance().GetComponent<LevelManager>().GetLevelForNumber(absoluteLevelNumber);
        Invalidate();
    }

    //private void Disable(bool bNullLevel)
    //{
    //    if (bNullLevel)
    //    {
    //        GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    //        GetComponent<Button>().interactable = false;
    //    }
    //    else
    //    {
    //        GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    //        GetComponent<Button>().interactable = true;
    //    }
    //}

    //private void Enable()
    //{
    //    GetComponent<Image>().color = new Color(1, 1, 1, 1);
    //    GetComponent<Button>().interactable = true;
    //}

    public void Invalidate()
    {
        //this.GetComponentInChildren<Text>().text = (m_index + 1).ToString();

        //if (m_level == null)
        //    Disable(true);
        //else
        //{
        //    LevelData levelData = LevelData.LoadFromFile(m_level.m_number);
        //    if (levelData == null)
        //        Disable(true);
        //    else if (!levelData.m_done)
        //        Disable(false);
        //    else
        //        Enable();
        //}
    }    

    public void OnClick()
    {
        Debug.Log("onClick slot:" + m_index);

        LevelsGUI levelsGUI = (LevelsGUI) GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
        levelsGUI.OnSlotClick(this);


    }
}
