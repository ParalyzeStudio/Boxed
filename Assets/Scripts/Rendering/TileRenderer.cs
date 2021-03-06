﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileRenderer : MonoBehaviour
{
    private const float TILE_DEFAULT_CONTOUR_THICKNESS = 0.05f;

    public Tile m_tile { get; set; } //the tile data used to render a cuboid tile

    public Material m_defaultTileMaterial;
    public Material m_finishTileMaterial;

    private float m_renderSize;

    //Bonus
    public BonusRenderer m_bonusPfb;
    private BonusRenderer m_bonus;
    public ParticleSystem m_bonusFxPfb;
    //private ParticleSystem m_bonusFx;

    //as we want to modify the color of a tile quickly, store the vertex of colors here
    private Color[] m_colors;
    private bool m_colorsArrayDirty;

    //use uv mixed with colors
    private Vector2[] m_uv;
    //private bool m_uvArrayDirty;

    //when modifying the thickness of the contour, we want to modify the vertices array. So like colors array, store it here
    private Vector3[] m_vertices;
    private bool m_verticesArrayDirty;

    //triangles
    private int[] m_triangles;

    //projections of each vertex
    //Vector2[] m_vertexProjections;

    //colors of the tile as a TileColors object
    //private TileColors m_tileColors;
    //public TileColors TileColors
    //{
    //    get
    //    {
    //        return m_tileColors;
    //    }
    //}

    //has the tile a contour on its top face
    private bool m_hasContour;

    //Color variation
    //bool m_colorVariating;
    //TileColors m_fromColors;
    //TileColors m_toColors;
    //float m_duration;
    //float m_elapsedTime;
    //float m_delay;

    //decals
    private GameObject m_decalObject;
    public Material m_testDecalMaterial;

    //various animations
    public GlowSquare m_glowSquarePfb;
    private bool m_generatingGlowSquares;
    private float m_gsGenerationPeriod;
    private float m_gsGenerationElapsedTime;

    public void Init(Tile tile)
    {
        m_tile = tile;

        //Assign a vertex-color unlit shader to this tile object
        if (tile.CurrentState == Tile.State.FINISH)
            GetComponent<MeshRenderer>().sharedMaterial = m_finishTileMaterial;
        else
            GetComponent<MeshRenderer>().sharedMaterial = m_defaultTileMaterial;

        float contourThicknessRatio;
        if (tile.CurrentState == Tile.State.FINISH)
            contourThicknessRatio = 0;
        else
            contourThicknessRatio = 0.06f;
        BuildFaces(Tile.TILE_HEIGHT, contourThicknessRatio);

        Invalidate();

        if (tile.AttachedBonus != null)
            BuildBonusObject();
    }

    /**
    * Build faces for this tile. The top face can have a contour on it, just set bDrawContourOnTopFace to true in this case and set a contourThicknessRatio
    * between 0 and 1, 0 meaning zero-thickness (no contour) and 1 meaning 0.5f * tile.m_size thickness
    **/
    protected void BuildFaces(float height = Tile.TILE_HEIGHT, float contourThicknessRatio = TILE_DEFAULT_CONTOUR_THICKNESS)
    {
        m_hasContour = (contourThicknessRatio > 0);
        m_hasContour = false; //TODO remove contour on textured tiles
        
        m_renderSize = 0.95f * m_tile.m_size;

        //VERTICES
        //build only two faces as the 3 other wont be visible
        if (m_hasContour)
            m_vertices = new Vector3[20];
        else
            m_vertices = new Vector3[12];

        m_vertices[0] = new Vector3(-0.5f * m_renderSize, -0.5f * height, 0.5f * m_renderSize);
        m_vertices[1] = new Vector3(-0.5f * m_renderSize, -0.5f * height, -0.5f * m_renderSize);
        m_vertices[2] = new Vector3(-0.5f * m_renderSize, 0.5f * height, -0.5f * m_renderSize);
        m_vertices[3] = new Vector3(-0.5f * m_renderSize, 0.5f * height, 0.5f * m_renderSize);
        m_vertices[4] = new Vector3(-0.5f * m_renderSize, -0.5f * height, -0.5f * m_renderSize);
        m_vertices[5] = new Vector3(0.5f * m_renderSize, -0.5f * height, -0.5f * m_renderSize);
        m_vertices[6] = new Vector3(0.5f * m_renderSize, 0.5f * height, -0.5f * m_renderSize);
        m_vertices[7] = new Vector3(-0.5f * m_renderSize, 0.5f * height, -0.5f * m_renderSize);
        
        float innerSquareSize = m_hasContour ? (1 - contourThicknessRatio) * m_renderSize : m_renderSize;
        
        m_vertices[8] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);
        m_vertices[9] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
        m_vertices[10] = new Vector3(0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
        m_vertices[11] = new Vector3(0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);

        if (m_hasContour)
        {
            //build actual contour
            m_vertices[12] = new Vector3(-0.5f * m_renderSize, 0.5f * height, 0.5f * m_renderSize);
            m_vertices[13] = new Vector3(-0.5f * m_renderSize, 0.5f * height, -0.5f * m_renderSize);
            m_vertices[14] = new Vector3(0.5f * m_renderSize, 0.5f * height, -0.5f * m_renderSize);
            m_vertices[15] = new Vector3(0.5f * m_renderSize, 0.5f * height, 0.5f * m_renderSize);
            m_vertices[16] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);
            m_vertices[17] = new Vector3(-0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
            m_vertices[18] = new Vector3(0.5f * innerSquareSize, 0.5f * height, -0.5f * innerSquareSize);
            m_vertices[19] = new Vector3(0.5f * innerSquareSize, 0.5f * height, 0.5f * innerSquareSize);
        }

        //TRIANGLES
        if (m_hasContour)
        {
            m_triangles = new int[] { 0, 2, 1, 0, 3, 2, //face 1
                                    4, 6, 5, 4, 7, 6, //face 2
                                    8, 10, 9, 8, 11, 10, //inner square                                    
                                    12, 17, 13, 12, 16, 17, //contour
                                    13, 18, 14, 13, 17, 18, //contour 
                                    14, 19, 15, 14, 18, 19, //contour
                                    15, 16, 12, 15, 19, 16, //contour
            };
        }
        else
        {
            m_triangles = new int[] { 0, 2, 1, 0, 3, 2, //face 1
                                    4, 6, 5, 4, 7, 6, //face 2
                                    8, 10, 9, 8, 11, 10, //top face
            };

        }

        //COLORS
        m_colors = new Color[m_hasContour ? 20 : 12];
        for (int i = 0; i != m_colors.Length; i++)
        {
            m_colors[i] = Color.white;
        }

        //UV
        UpdateUVMap();

        //NORMALS (in case of we change to vertex lit)
        Vector3[] normals;
        if (m_hasContour)
        {
            normals = new Vector3[20];
            for (int i = 0; i != normals.Length; i++)
            {
                if (i < 4)
                    normals[i] = Vector3.left;
                else if (i < 8)
                    normals[i] = Vector3.back;
                else
                    normals[i] = Vector3.up;
            }
        }
        else
        {
            normals = new Vector3[12];
            for (int i = 0; i != normals.Length; i++)
            {
                if (i < 4)
                    normals[i] = Vector3.left;
                else if (i < 8)
                    normals[i] = Vector3.back;
                else
                    normals[i] = Vector3.up;
            }
        }

        Mesh facesMesh = new Mesh();
        facesMesh.name = "TileMesh";
        facesMesh.vertices = m_vertices;
        facesMesh.triangles = m_triangles;
        facesMesh.colors = m_colors;
        facesMesh.uv = m_uv;
        facesMesh.normals = normals;

        this.GetComponent<MeshFilter>().sharedMesh = facesMesh;
    }

    //public void SetColors(TileColors colors)
    //{
    //    m_tileColors = colors;
    //    SetLeftFaceColor(colors.m_tileLeftFaceColor, false);
    //    SetRightFaceColor(colors.m_tileRightFaceColor, false);
    //    SetTopFaceColor(colors.m_tileTopFaceColor, false);
    //    SetContourColor(colors.m_tileContourColor, true);
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

    public void SetContourColor(Color color, bool bUpdateMeshDirectly = true)
    {
        if (!m_hasContour)
            return;

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
        float tileHeight = Tile.TILE_HEIGHT;
        if (m_tile.CurrentState == Tile.State.BLOCKED)
            tileHeight *= 2;


        float innerSquareSize = (1 - ratio) * m_renderSize;
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
        this.transform.localScale = new Vector3(1, GetTileHeight() / Tile.TILE_HEIGHT, 1);
    }

    public void UpdateTilePosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void UpdateTileColor()
    {
        Color color = GetColor();
        SetTileColor(color);
    }

    public void UpdateTileMaterial()
    {
        Material material = GetMaterial();
        GetComponent<MeshRenderer>().sharedMaterial = material;        
    }

    public void SetTileColor(Color color)
    {
        for (int i = 0; i != m_colors.Length; i++)
        {
            m_colors[i] = color;
        }

        SetContourColor(ColorUtils.LightenColor(color, 0.5f));

        m_colorsArrayDirty = true;
    }

    public void UpdateTileDecal()
    {
        if (m_tile.CurrentState == Tile.State.SWITCH || m_tile.CurrentState == Tile.State.TRIGGERED_BY_SWITCH)
            AddDecal(null);
        else
            RemoveDecal();        
    }

    /**
    * Build a bonus element over this tile
    **/
    public void BuildBonusObject()
    {
        Debug.Log("BuildBonusObject");

        m_bonus = Instantiate(m_bonusPfb);
        m_bonus.name = "Bonus";

        m_bonus.Build();

        Vector3 bonusPosition = m_tile.GetWorldPosition() + new Vector3(0, 0.5f * GetTileHeight() + 0.01f, 0);

        GameObject bonusContainer = GameController.GetInstance().m_bonuses;
        m_bonus.transform.parent = bonusContainer.transform;
        m_bonus.transform.localPosition = bonusPosition;
        m_bonus.transform.localScale = 0.33f * Vector3.one;

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
        float tileHeight = Tile.TILE_HEIGHT;
        if (m_tile.CurrentState == Tile.State.BLOCKED)
            tileHeight *= 2;
        else if (m_tile.CurrentState == Tile.State.TRIGGERED_BY_SWITCH)
        {
            TriggeredTile tile = (TriggeredTile)m_tile;
            if (tile.m_isLiftUp)
                tileHeight *= 2;
        }

        return tileHeight;
    }

    /**
    * Calculate the projections of each vertex of this tile onto the screen
    **/
    //private void CalculateVertexProjections()
    //{
    //    m_vertexProjections = new Vector2[m_vertices.Length];

    //    for (int i = 0; i != m_vertexProjections.Length; i++)
    //    {
    //        m_vertexProjections[i] = Camera.main.WorldToScreenPoint(m_vertices[i] + this.transform.position);
    //    }
    //}

    ///**
    //* Return the vertex projections with recalculating them if necessary
    //**/
    //public Vector2[] GetVertexProjections(bool bForceRecalculation = false)
    //{
    //    if (m_vertexProjections == null || bForceRecalculation)
    //         CalculateVertexProjections();

    //    return m_vertexProjections;
    //}

    ///**
    //* Tells if the projected mesh of this tile (when used as a button) contains the click location
    //**/
    //public bool ContainsClickAsButton(Vector2 clickLocation)
    //{
    //    GetVertexProjections();

    //    LevelSlot slot = (LevelSlot)this;

    //    for (int i = 0; i != m_triangles.Length; i+=3)
    //    {
    //        //form a triangle and test if it contains the point
    //        Geometry.Triangle triangle = new Geometry.Triangle(m_vertexProjections[m_triangles[i]],
    //                                                           m_vertexProjections[m_triangles[i + 1]],
    //                                                           m_vertexProjections[m_triangles[i + 2]]);

    //        if (triangle.ContainsPoint(clickLocation))
    //            return true;
    //    }

    //    return false;
    //}

    //public void ChangeColorsTo(TileColors toColors, float duration, float delay = 0.0f)
    //{
    //    m_colorVariating = true;
    //    m_fromColors = m_tileColors;
    //    m_toColors = toColors;
    //    m_duration = duration;
    //    m_delay = delay;
    //    m_elapsedTime = 0;
    //}

    /**
    * Add a decal texture on the top face of this tile
    **/
    public void AddDecal(Material quadTextureMaterial)
    {
        return; //TODO bypass decal
        if (m_decalObject != null)
            return;

        m_decalObject = new GameObject("Decal");
        //m_decalObject.transform.SetParent(this.transform, false);
        m_decalObject.transform.parent = this.transform;
        m_decalObject.transform.localPosition = new Vector3(0, 0.001f, 0); //set the decal right above the tile object
        m_decalObject.transform.localScale = Vector3.one;

        MeshFilter meshFilter = m_decalObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = m_decalObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = m_testDecalMaterial;

        //Build a quad mesh
        Mesh quadMesh = new Mesh();
        meshFilter.sharedMesh = quadMesh;

        //vertices are the same as tile's top face vertices
        Vector3[] vertices = new Vector3[4];
        vertices[0] = m_vertices[8];
        vertices[1] = m_vertices[9];
        vertices[2] = m_vertices[10];
        vertices[3] = m_vertices[11];

        int[] triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        
        Vector2[] uv = new Vector2[4];
        uv[0] = Vector2.zero;
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(0, 1);

        quadMesh.vertices = vertices;
        quadMesh.triangles = triangles;
        quadMesh.uv = uv;
    }

    public void RemoveDecal()
    {
        if (m_decalObject != null)
            Destroy(m_decalObject);
    }

    /**
    * Add a glow square and animate it
    **/
    public void DisplayGlowSquareOnBrickLanding()
    {
        GlowSquare glowSquare = Instantiate(m_glowSquarePfb);
        glowSquare.name = "GlowSquare";
        glowSquare.transform.parent = this.transform;
        glowSquare.Init();

        GameObjectAnimator glowSquareAnimator = glowSquare.GetComponent<GameObjectAnimator>();
        glowSquareAnimator.UpdatePivotPoint(Vector3.zero);
        glowSquareAnimator.SetPosition(new Vector3(0, 0.5f * Tile.TILE_HEIGHT + 0.0001f, 0));
        glowSquareAnimator.SetScale(new Vector3(1.33f, 1.33f, 1.33f));
        glowSquareAnimator.ScaleTo(new Vector3(1.4f, 1.4f, 1.4f), 0.5f);
        glowSquareAnimator.SetOpacity(1.0f);
        glowSquareAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, false);
    }

    /**
    * Generate a cycle of fading glow squares on the finish tile
    **/
    public void GenerateGlowSquaresOnFinishTile()
    {
        return;
        m_generatingGlowSquares = true;
        m_gsGenerationElapsedTime = 0;
        m_gsGenerationPeriod = 1.0f;
    }

    private void GenerateGlowSquareOnFinishTile()
    {
        GlowSquare glowSquare = Instantiate(m_glowSquarePfb);
        glowSquare.name = "GlowSquare";
        glowSquare.transform.parent = this.transform;
        glowSquare.Init();

        GlowSquareAnimator glowSquareAnimator = glowSquare.GetComponent<GlowSquareAnimator>();
        glowSquareAnimator.UpdatePivotPoint(Vector3.zero);
        glowSquareAnimator.SetPosition(new Vector3(0, 0.5f * Tile.TILE_HEIGHT + 0.0001f, 0));
        glowSquareAnimator.SetScale(new Vector3(1.4f, 1.4f, 1.4f));
        glowSquareAnimator.ScaleTo(Vector3.zero, 3.0f);
        glowSquareAnimator.SetOpacity(0);
        glowSquareAnimator.FadeTo(0.75f, 3.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    public void LiftUp()
    {
        //if (m_tile is IceTile)
        //{
        //    IceTile iceTile = (IceTile)m_tile;
        //    if (iceTile.m_blocked)
        //        return;
        //    else
        //        iceTile.m_blocked = true;
        //}
        //else if (m_tile is TriggeredTile)
        //{
        //    TriggeredTile triggeredTile = (TriggeredTile)m_tile;
        //    if (triggeredTile.m_isLiftUp)
        //        return;
        //    else
        //        triggeredTile.m_isLiftUp = true;
        //}

        GameObjectAnimator tileAnimator = this.gameObject.AddComponent<GameObjectAnimator>();
        tileAnimator.SetPosition(this.transform.localPosition);
        tileAnimator.TranslateBy(new Vector3(0, Tile.TILE_HEIGHT, 0), 0.3f, 0.5f * 90 / BrickRenderer.DEFAULT_ANGULAR_SPEED); 
    }

    public void LiftDown()
    {
        //if (m_tile is TriggeredTile)
        //{
        //    TriggeredTile triggeredTile = (TriggeredTile)m_tile;
        //    if (!triggeredTile.m_isLiftUp)
        //        return;
        //    else
        //        triggeredTile.m_isLiftUp = false;
        //}

        GameObjectAnimator tileAnimator = this.gameObject.AddComponent<GameObjectAnimator>();
        tileAnimator.SetPosition(this.transform.localPosition);
        tileAnimator.TranslateBy(new Vector3(0, -Tile.TILE_HEIGHT, 0), 0.3f, 0.5f * 90 / BrickRenderer.DEFAULT_ANGULAR_SPEED);
    }

    public Color GetColor()
    {
        ThemeManager.Theme currentTheme = GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme();
        return currentTheme.GetTileColorForTileState(m_tile.CurrentState);
    }

    public Material GetMaterial()
    {
        ThemeManager themeManager = GameController.GetInstance().GetComponent<ThemeManager>();
        ThemeManager.Theme currentTheme = themeManager.GetSelectedTheme();

        return currentTheme.GetTileMaterialForTileState(m_tile.CurrentState);
    }

    public void UpdateUVMap()
    {
        m_uv = new Vector2[12];

        if (m_tile.CurrentState == Tile.State.SWITCH || m_tile.CurrentState == Tile.State.TRIGGERED_BY_SWITCH)
        {
            m_uv[0] = new Vector2(0.162f, 0.347f);
            m_uv[1] = new Vector2(0.491f, 0.148f);
            m_uv[2] = new Vector2(0.497f, 0.438f);
            m_uv[3] = new Vector2(0.168f, 0.632f);
            m_uv[4] = new Vector2(0.491f, 0.148f);
            m_uv[5] = new Vector2(0.812f, 0.34f);
            m_uv[6] = new Vector2(0.816f, 0.629f);
            m_uv[7] = new Vector2(0.497f, 0.438f);
            m_uv[8] = new Vector2(0.168f, 0.632f);
            m_uv[9] = new Vector2(0.497f, 0.438f);
            m_uv[10] = new Vector2(0.816f, 0.629f);
            m_uv[11] = new Vector2(0.486f, 0.836f);
        }
        else
        {
            m_uv[0] = new Vector2(0, 1 - 0.752f);
            m_uv[1] = new Vector2(0.504f, 1 - 0.998f);
            m_uv[2] = new Vector2(0.497f, 0.5f);
            m_uv[3] = new Vector2(0.04f, 1 - 0.256f);
            m_uv[4] = new Vector2(0.504f, 1 - 0.998f);
            m_uv[5] = new Vector2(0.999f, 1 - 0.751f);
            m_uv[6] = new Vector2(0.994f, 1 - 0.255f);
            m_uv[7] = new Vector2(0.497f, 0.5f);
            m_uv[8] = new Vector2(0.04f, 1 - 0.256f);
            m_uv[9] = new Vector2(0.497f, 0.5f);
            m_uv[10] = new Vector2(0.994f, 1 - 0.255f);
            m_uv[11] = new Vector2(0.5f, 1);
        }
    }

    public void Invalidate()
    {
        UpdateTileHeight(GetTileHeight());
        UpdateTilePosition(m_tile.GetLocalPosition());
        UpdateTileColor();
        UpdateTileMaterial();
        UpdateTileDecal();
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        //if (m_colorVariating)
        //{      
        //    bool inDelay = (m_elapsedTime < m_delay);
        //    m_elapsedTime += dt;
        //    if (m_elapsedTime > m_delay)
        //    {
        //        if (inDelay) //we were in delay previously
        //            dt = m_elapsedTime - m_delay;
        //        float effectiveElapsedTime = m_elapsedTime - m_delay;
        //        float t1 = effectiveElapsedTime - dt;
        //        float t2 = effectiveElapsedTime;

        //        //Top color variation
        //        TileColors colorsVariation = m_toColors;
        //        colorsVariation.Substract(m_fromColors);
        //        TileColors deltaColors = colorsVariation;
        //        deltaColors.Multiply((t2 - t1) / m_duration);

        //        m_tileColors.Add(deltaColors);

        //        if (effectiveElapsedTime > m_duration)
        //        {
        //            m_tileColors = m_toColors;
        //            m_colorVariating = false;
        //        }

        //        SetColors(m_tileColors);
        //    }
        //}

        if (m_generatingGlowSquares)
        {
            m_gsGenerationElapsedTime -= dt;
            if (m_gsGenerationElapsedTime <= 0)
            {
                m_gsGenerationElapsedTime = m_gsGenerationPeriod;
                GenerateGlowSquareOnFinishTile();
            }
        }

        //do not update rendering of tile when searching for solutions inside a tree
        //bool bUpdateTileRenderingOnStateChange = false;

        //if (GameController.GetInstance().m_gameMode == GameController.GameMode.LEVEL_EDITOR)
        //{
        //    LevelEditor levelEditor = (LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
        //    if (levelEditor.m_isTestMenuShown && levelEditor.m_testMenu.m_testingLevel)
        //        bUpdateTileRenderingOnStateChange = true;
        //}
        //else if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME)
        //    bUpdateTileRenderingOnStateChange = true;


        if (m_tile.m_tileStateDirty)
        {
            bool bInvalidateTileRendering = true;
            if (GameController.GetInstance().m_gameMode == GameController.GameMode.LEVEL_EDITOR)
            {
                LevelEditor levelEditor = (LevelEditor)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
                if (levelEditor.m_computingSolution) //do not invalidate tile rendering when we are computing a solution
                    bInvalidateTileRendering = false;
            }

            if (bInvalidateTileRendering)
            {
                Invalidate();
                m_tile.m_tileStateDirty = false;
            }
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