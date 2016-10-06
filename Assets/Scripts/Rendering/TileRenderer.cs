using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileRenderer : MonoBehaviour
{
    private const float TILE_DEFAULT_CONTOUR_THICKNESS = 0.05f;

    public Tile m_tile { get; set; } //the tile data used to render a cuboid tile

    public Material m_tileMaterial;

    //Bonus
    public BonusRenderer m_bonusPfb;
    private BonusRenderer m_bonus;
    public ParticleSystem m_bonusFxPfb;
    //private ParticleSystem m_bonusFx;

    //as we want to modify the color of a tile quickly, store the vertex of colors here
    private Color[] m_colors;
    private bool m_colorsArrayDirty;

    //when modifying the thickness of the contour, we want to modify the vertices array. So like colors array, store it here
    private Vector3[] m_vertices;
    private bool m_verticesArrayDirty;

    //triangles
    private int[] m_triangles;

    //projections of each vertex
    Vector2[] m_vertexProjections;

    //colors of the tile as a TileColors object
    private TileColors m_tileColors;

    //Color variation
    bool m_colorVariating;
    TileColors m_fromColors;
    TileColors m_toColors;
    float m_duration;
    float m_elapsedTime;
    float m_delay;

    public void Init(Tile tile)
    {
        m_tile = tile;

        float tileHeight = GetTileHeight();
        BuildFaces(tileHeight);

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
    protected void BuildFaces(float height, float contourThicknessRatio = TILE_DEFAULT_CONTOUR_THICKNESS)
    {
        //build only two faces as the 3 other wont be visible
        m_vertices = new Vector3[20];
        m_vertices[0] = new Vector3(-0.5f * m_tile.m_size, -0.5f * height, 0.5f * m_tile.m_size);
        m_vertices[1] = new Vector3(-0.5f * m_tile.m_size, -0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[2] = new Vector3(-0.5f * m_tile.m_size, 0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[3] = new Vector3(-0.5f * m_tile.m_size, 0.5f * height, 0.5f * m_tile.m_size);
        m_vertices[4] = new Vector3(-0.5f * m_tile.m_size, -0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[5] = new Vector3(0.5f * m_tile.m_size, -0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[6] = new Vector3(0.5f * m_tile.m_size, 0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[7] = new Vector3(-0.5f * m_tile.m_size, 0.5f * height, -0.5f * m_tile.m_size);

        float innerSquareSize = (1 - contourThicknessRatio) * m_tile.m_size;
        m_vertices[8] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);
        m_vertices[9] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
        m_vertices[10] = new Vector3(0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
        m_vertices[11] = new Vector3(0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);

        //build actual contour
        m_vertices[12] = new Vector3(-0.5f * m_tile.m_size, 0.5f * height, 0.5f * m_tile.m_size);
        m_vertices[13] = new Vector3(-0.5f * m_tile.m_size, 0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[14] = new Vector3(0.5f * m_tile.m_size, 0.5f * height, -0.5f * m_tile.m_size);
        m_vertices[15] = new Vector3(0.5f * m_tile.m_size, 0.5f * height, 0.5f * m_tile.m_size);
        m_vertices[16] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);
        m_vertices[17] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
        m_vertices[18] = new Vector3(0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
        m_vertices[19] = new Vector3(0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);

        m_triangles = new int[] { 0, 2, 1, 0, 3, 2, //face 1
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
        facesMesh.triangles = m_triangles;
        facesMesh.colors = m_colors;

        this.GetComponent<MeshFilter>().sharedMesh = facesMesh;
    }

    public void SetColors(TileColors colors)
    {
        m_tileColors = colors;
        SetLeftFaceColor(colors.m_tileLeftFaceColor, false);
        SetRightFaceColor(colors.m_tileRightFaceColor, false);
        SetTopFaceColor(colors.m_tileTopFaceColor, false);
        SetContourColor(colors.m_tileContourColor, true);
    }

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
        float tileHeight = Tile.TILE_DEFAULT_HEIGHT;
        if (m_tile.CurrentState == Tile.State.BLOCKED)
            tileHeight *= 2;


        float innerSquareSize = (1 - ratio) * m_tile.m_size;
        m_vertices[8] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);
        m_vertices[9] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
        m_vertices[10] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
        m_vertices[11] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);
        m_vertices[16] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);
        m_vertices[17] = new Vector3(-0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
        m_vertices[18] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, -0.5f * innerSquareSize);
        m_vertices[19] = new Vector3(0.5f * innerSquareSize, 0.5f * tileHeight, 0.5f * innerSquareSize);

        if (bUpdateMeshDirectly)
            GetComponent<MeshFilter>().sharedMesh.vertices = m_vertices;
        else
            m_verticesArrayDirty = true;
    }

    public void UpdateTileHeight(float height)
    {
        BuildFaces(GetTileHeight()); //rebuild the tile
    }

    public void UpdateTilePosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void UpdateTileColors()
    {
        ColorTheme currentTheme = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme;

        TileColors colors = currentTheme.GetTileColorsForTileState(m_tile.CurrentState);
        SetColors(colors);
    }

    /**
    * Build a bonus element over this tile
    **/
    public void BuildBonusObject()
    {
        m_bonus = Instantiate(m_bonusPfb);
        m_bonus.name = "Bonus";

        Vector3 bonusPosition = m_tile.GetWorldPosition() + new Vector3(0, 0.5f * GetTileHeight() + 0.25f, 0);

        GameObject bonusContainer = GameController.GetInstance().m_bonuses;
        m_bonus.transform.parent = bonusContainer.transform;
        m_bonus.GetComponent<GameObjectAnimator>().SetPosition(bonusPosition);

        //create the particle effect associated with this bonuss
        //m_bonusFx = Instantiate(m_bonusFxPfb);
        //m_bonusFx.transform.parent = bonusContainer.transform;
        //m_bonusFx.transform.localPosition = bonusPosition;
    }

    public void DestroyBonusObject()
    {
        Destroy(m_bonus.gameObject);
        //if (m_bonusFx != null)
        //    Destroy(m_bonusFx.gameObject);
    }

    /**
    * Called when brick covers this tile and a bonus is attached to it
    **/
    public void OnCaptureBonus()
    {
        m_tile.AttachedBonus = null;
        DestroyBonusObject();
    }

    private float GetTileHeight()
    {
        float tileHeight = Tile.TILE_DEFAULT_HEIGHT;
        if (m_tile.CurrentState == Tile.State.BLOCKED)
            tileHeight *= 2;

        return tileHeight;
    }

    /**
    * Calculate the projections of each vertex of this tile onto the screen
    **/
    private void CalculateVertexProjections()
    {
        m_vertexProjections = new Vector2[m_vertices.Length];

        for (int i = 0; i != m_vertexProjections.Length; i++)
        {
            m_vertexProjections[i] = Camera.main.WorldToScreenPoint(m_vertices[i] + this.transform.position);
        }
    }

    /**
    * Return the vertex projections with recalculating them if necessary
    **/
    public Vector2[] GetVertexProjections(bool bForceRecalculation = false)
    {
        if (m_vertexProjections == null || bForceRecalculation)
             CalculateVertexProjections();

        return m_vertexProjections;
    }

    /**
    * Tells if the projected mesh of this tile (when used as a button) contains the click location
    **/
    public bool ContainsClickAsButton(Vector2 clickLocation)
    {
        GetVertexProjections();

        LevelSlot slot = (LevelSlot)this;

        for (int i = 0; i != m_triangles.Length; i+=3)
        {
            //form a triangle and test if it contains the point
            Geometry.Triangle triangle = new Geometry.Triangle(m_vertexProjections[m_triangles[i]],
                                                               m_vertexProjections[m_triangles[i + 1]],
                                                               m_vertexProjections[m_triangles[i + 2]]);

            if (triangle.ContainsPoint(clickLocation))
                return true;
        }

        return false;
    }

    public void ChangeColorsTo(TileColors toColors, float duration, float delay = 0.0f)
    {
        m_colorVariating = true;
        m_fromColors = m_tileColors;
        m_toColors = toColors;
        m_duration = duration;
        m_delay = delay;
        m_elapsedTime = 0;
    }

    public void Update()
    {
        if (m_colorVariating)
        {
            float dt = Time.deltaTime;

            bool inDelay = (m_elapsedTime < m_delay);
            m_elapsedTime += dt;
            if (m_elapsedTime > m_delay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_elapsedTime - m_delay;
                float effectiveElapsedTime = m_elapsedTime - m_delay;
                float t1 = effectiveElapsedTime - dt;
                float t2 = effectiveElapsedTime;

                //Top color variation
                TileColors colorsVariation = m_toColors;
                colorsVariation.Substract(m_fromColors);
                TileColors deltaColors = colorsVariation;
                deltaColors.Multiply((t2 - t1) / m_duration);

                m_tileColors.Add(deltaColors);

                if (effectiveElapsedTime > m_duration)
                {
                    m_tileColors = m_toColors;
                    m_colorVariating = false;
                }

                SetColors(m_tileColors);
            }
        }

        if (m_tile.m_tileStateDirty)
        {
            UpdateTileHeight(GetTileHeight());
            UpdateTilePosition(m_tile.GetLocalPosition());
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