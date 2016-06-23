using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileRenderer : MonoBehaviour
{
    public const float TILE_HEIGHT = 0.5f;

    public Tile m_tile; //the tile data used to render a cuboid tile

    public Material m_tileMaterial;

    //Bonus
    public GameObject m_bonusPfb;
    private GameObject m_bonusObject;

    //as we want to modify the color of a tile quickly, store the vertex of colors here
    private Color[] m_colors;
    private bool m_colorsArrayDirty;

    //when modifying the thickness of the contour, we want to modify the vertices array. So like colors array, store it here
    private Vector3[] m_vertices;
    private bool m_verticesArrayDirty;

    public void Init(Tile tile)
    {
        m_tile = tile;

        BuildFaces(0.02f);

        m_colorsArrayDirty = false;
        m_verticesArrayDirty = false;

        //Assign a vertex-color unlit shader to this tile object
        this.GetComponent<MeshRenderer>().material = m_tileMaterial;

        if (tile.AttachedBonus != null)
            BuildBonusObject();
    }

    /**
    * Build faces for this tile. The top face can have a contour on it, just set bDrawContourOnTopFace to true in this case and set a contourThicknessRatio
    * between 0 and 1, 0 meaning zero-thickness (no contour) and 1 meaning 0.5f * tile.m_size thickness
    **/
    private void BuildFaces(float contourThicknessRatio = 0.2f)
    {
        //build only two faces as the 3 other wont be visible
        m_vertices = new Vector3[20];
        m_vertices[0] = new Vector3(-0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, 0.5f * m_tile.m_size);
        m_vertices[1] = new Vector3(-0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[2] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[3] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, 0.5f * m_tile.m_size);
        m_vertices[4] = new Vector3(-0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[5] = new Vector3(0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[6] = new Vector3(0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[7] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);

        float innerSquareSize = (1 - contourThicknessRatio) * m_tile.m_size;
        m_vertices[8] = new Vector3(-0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, 0.5f * innerSquareSize);
        m_vertices[9] = new Vector3(-0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, -0.5f * innerSquareSize);
        m_vertices[10] = new Vector3(0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, -0.5f * innerSquareSize);
        m_vertices[11] = new Vector3(0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, 0.5f * innerSquareSize);

        //build actual contour
        m_vertices[12] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, 0.5f * m_tile.m_size);
        m_vertices[13] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[14] = new Vector3(0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
        m_vertices[15] = new Vector3(0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, 0.5f * m_tile.m_size);
        m_vertices[16] = new Vector3(-0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, 0.5f * innerSquareSize);
        m_vertices[17] = new Vector3(-0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, -0.5f * innerSquareSize);
        m_vertices[18] = new Vector3(0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, -0.5f * innerSquareSize);
        m_vertices[19] = new Vector3(0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, 0.5f * innerSquareSize);

        int[] triangles = new int[] { 0, 2, 1, 0, 3, 2, //face 1
                                    4, 6, 5, 4, 7, 6, //face 2
                                    8, 10, 9, 8, 11, 10, //inner square                                    
                                    12, 17, 13, 12, 16, 17, //contour
                                    13, 18, 14, 13, 17, 18, //contour 
                                    14, 19, 15, 14, 18, 19, //contour
                                    15, 16, 12, 15, 19, 16, //contour
            };

        m_colors = new Color[20];

        Mesh facesMesh = new Mesh();
        facesMesh.name = "TileMesh";
        facesMesh.vertices = m_vertices;
        facesMesh.triangles = triangles;
        facesMesh.colors = m_colors;

        this.GetComponent<MeshFilter>().sharedMesh = facesMesh;
    }

    /**
    * Build every face of this tile except the top one
    **/
    //private GameObject BuildSideFaces()
    //{
    //    //build only two faces as the 3 other wont be visible
    //    Vector3[] vertices = new Vector3[8];
    //    vertices[0] = new Vector3(-0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, 0.5f * m_tile.m_size);
    //    vertices[1] = new Vector3(-0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
    //    vertices[2] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
    //    vertices[3] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, 0.5f * m_tile.m_size);
    //    vertices[4] = new Vector3(-0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
    //    vertices[5] = new Vector3(0.5f * m_tile.m_size, -0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
    //    vertices[6] = new Vector3(0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);
    //    vertices[7] = new Vector3(-0.5f * m_tile.m_size, 0.5f * TILE_HEIGHT, -0.5f * m_tile.m_size);

    //    int[] triangles = new int[] { 0, 2, 1, 0, 3, 2, 4, 6, 5, 4, 7, 6 };

    //    Vector3[] normals = new Vector3[8];
    //    normals[0] = new Vector3(-1, 0, 0);
    //    normals[1] = new Vector3(-1, 0, 0);
    //    normals[2] = new Vector3(-1, 0, 0);
    //    normals[3] = new Vector3(-1, 0, 0);
    //    normals[4] = new Vector3(0, 0, -1);
    //    normals[5] = new Vector3(0, 0, -1);
    //    normals[6] = new Vector3(0, 0, -1);
    //    normals[7] = new Vector3(0, 0, -1);

    //    Mesh facesMesh = new Mesh();
    //    facesMesh.vertices = vertices;
    //    facesMesh.triangles = triangles;
    //    facesMesh.normals = normals;

    //    GameObject facesObject = new GameObject();
    //    MeshFilter meshFilter = facesObject.AddComponent<MeshFilter>();
    //    meshFilter.sharedMesh = facesMesh;
    //    MeshRenderer meshRenderer = facesObject.AddComponent<MeshRenderer>();
    //    meshRenderer.material = m_sideFacesMaterial;

    //    return facesObject;
    //}

    /**
    * Build top face of this tile
    **/
    //private GameObject BuildTopFace()
    //{
    //    Quad topFace = Instantiate(m_menuTileTopFacePfb);
    //    topFace.Init(null, true);
    //    topFace.transform.localPosition = new Vector3(0, 0.5f * TILE_HEIGHT, 0);
    //    topFace.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);

    //    return topFace.gameObject;
    //}
    
   

    public void SetLeftFaceColor(Color color, bool bUpdateMeshDirectly = true)
    {
        int i = 0;
        while (i < 4)
        {
            m_colors[i] = color;
            i++;
        }        

        if (bUpdateMeshDirectly)
            GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
        else
            m_colorsArrayDirty = true;
    }

    public void SetRightFaceColor(Color color, bool bUpdateMeshDirectly = true)
    {
        int i = 4;
        while (i < 8)
        {
            m_colors[i] = color;
            i++;
        }

        if (bUpdateMeshDirectly)
            GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
        else
            m_colorsArrayDirty = true;
    }

    public void SetTopFaceColor(Color color, bool bUpdateMeshDirectly = true)
    {
        int i = 8;
        while (i < 12)
        {
            m_colors[i] = color;
            i++;
        }

        if (bUpdateMeshDirectly)
            GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
        else
            m_colorsArrayDirty = true;
    }

    public void SetContourColor(Color color, bool bUpdateMeshDirectly = true)
    {
        int i = 12;
        while (i < 20)
        {
            m_colors[i] = color;
            i++;
        }

        if (bUpdateMeshDirectly)
            GetComponent<MeshFilter>().sharedMesh.colors = m_colors;
        else
            m_colorsArrayDirty = true;
    }

    public void SetContourThicknessRatio(float ratio, bool bUpdateMeshDirectly = true)
    {
        float innerSquareSize = (1 - ratio) * m_tile.m_size;
        m_vertices[16] = new Vector3(-0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, 0.5f * innerSquareSize);
        m_vertices[17] = new Vector3(-0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, -0.5f * innerSquareSize);
        m_vertices[18] = new Vector3(0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, -0.5f * innerSquareSize);
        m_vertices[19] = new Vector3(0.5f * innerSquareSize, 0.5f * TILE_HEIGHT, 0.5f * innerSquareSize);

        if (bUpdateMeshDirectly)
            GetComponent<MeshFilter>().sharedMesh.vertices = m_vertices;
        else
            m_verticesArrayDirty = true;
    }

    public void UpdateTileColors()
    {
        ColorTheme currentTheme = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme;

        TileColors colors = currentTheme.GetTileColorsForTileState(m_tile.CurrentState);

        SetLeftFaceColor(colors.m_tileLeftFaceColor);
        SetRightFaceColor(colors.m_tileRightFaceColor);
        SetTopFaceColor(colors.m_tileTopFaceColor);
        SetContourColor(colors.m_tileContourColor);
    }

    /**
    * Build a bonus element over this tile
    **/
    public void BuildBonusObject()
    {
        m_bonusObject = (GameObject)Instantiate(m_bonusPfb);
        m_bonusObject.name = "Bonus";

        GameObject bonusContainer = GameController.GetInstance().m_bonuses;
        m_bonusObject.transform.parent = bonusContainer.transform;

        GameObjectAnimator bonusAnimator = m_bonusObject.GetComponent<GameObjectAnimator>();
        bonusAnimator.SetPosition(m_tile.GetWorldPosition() + new Vector3(0, 0.3f, 0));
    }

    public void DestroyBonusObject()
    {
        Destroy(m_bonusObject.gameObject);
    }

    /**
    * Called when brick covers this tile and a bonus is attached to it
    **/
    public void OnCaptureBonus()
    {
        DestroyBonusObject();
    }

    public void Update()
    {
        if (m_tile.m_tileStateDirty)
        {
            UpdateTileColors();
            m_tile.m_tileStateDirty = false;
        }

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (m_colorsArrayDirty)
        {
            mesh.colors = m_colors;
            m_colorsArrayDirty = false;
        }
        if (m_verticesArrayDirty)
        {
            mesh.vertices = m_vertices;
            m_verticesArrayDirty = false;
        }
    }
}