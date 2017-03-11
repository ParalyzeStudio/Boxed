using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FloorSupportRenderer : MonoBehaviour
{
    //store vertices, triangles and colors in those lists before setting them to the mesh
    private Vector3[] m_vertices;
    private int[] m_triangles;
    private Color[] m_colors;
    private bool m_verticesDirty;
    private bool m_trianglesDirty;
    private bool m_colorsDirty;

    private Mesh m_mesh;
    private Column[] m_columns;

    private const int FLOOR_SUPPORT_HEIGHT = 3; //3 bricks under one tile

    /**
    * Build the floor support by assembling elementary cubes into one single mesh
    **/
    public void Render(Floor floor, bool bRenderWithAnimation = false)
    {
        int surfaceSize = floor.GetSurfaceSize();
        int cubeCount = FLOOR_SUPPORT_HEIGHT * surfaceSize;
        float cubeSize = 0.95f * floor.GetTileSize();

        m_vertices = new Vector3[8 * cubeCount];
        m_triangles = new int[12 * cubeCount];
        m_colors = new Color[8 * cubeCount];

        Color floorSupportColor = GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme().m_floorSupportColor;
        Color lightenedFloorSupportColor = ColorUtils.LightenColor(floorSupportColor, 0.1f);
        //Color columnBottomColor = ColorUtils.LightenColor(columnTopColor, 0.5f);

        //Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        //Color columnBottomColor = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(blVertex + this.transform.position));
        //Color vertex4Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(brVertex + this.transform.position));

        FloorRenderer floorRenderer = GameController.GetInstance().m_floorRenderer;
        int offset = 0;
        m_columns = new Column[surfaceSize];
        for (int i = 0; i != floor.Tiles.Length; i++)
        {
            Tile tile = floor.Tiles[i];

            if (tile.CurrentState != Tile.State.DISABLED)
            {
                TileRenderer tileRenderer = floorRenderer.GetRendererForTile(tile);
                Vector3 tilePosition = floorRenderer.GetRendererForTile(tile).transform.localPosition;
                if (bRenderWithAnimation) //offset the position of the tile vertically
                {
                    tilePosition -= new Vector3(0, 0.5f * Tile.TILE_HEIGHT, 0) + 3 * new Vector3(0, cubeSize, 0);
                    tileRenderer.transform.localPosition = tilePosition;
                }

                //Determine the 3 colors to apply gradient on cubes, the first and third are the same because the gradient background is vertical
                Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
                Vector3 columnBottomVertex1 = tilePosition + new Vector3(-0.5f * cubeSize, -FLOOR_SUPPORT_HEIGHT * cubeSize, 0.5f * cubeSize);
                Vector3 columnBottomVertex2 = tilePosition + new Vector3(-0.5f * cubeSize, -FLOOR_SUPPORT_HEIGHT * cubeSize, -0.5f * cubeSize);
                Color columnBottomColor1 = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(columnBottomVertex1 + this.transform.position));
                Color columnBottomColor2 = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(columnBottomVertex2 + this.transform.position));

                //Build FLOOR_SUPPORT_HEIGHT cubes under this tile
                Cube[] columnCubes = new Cube[FLOOR_SUPPORT_HEIGHT];
                for (int p = 0; p != FLOOR_SUPPORT_HEIGHT; p++)
                {
                    int firstVertexIndex = 8 * (FLOOR_SUPPORT_HEIGHT * (i - offset) + p);
                    int firstTriangleIndex = 12 * (FLOOR_SUPPORT_HEIGHT * (i - offset) + p);

                    Vector3 cubePosition = tilePosition - new Vector3(0, 0.5f * Tile.TILE_HEIGHT + (p + 0.5f) * cubeSize, 0);
                    Color[] cubeColors = new Color[8];
                    //top cube vertices color
                    if (p == 0)
                    {
                        cubeColors[2] = floorSupportColor;
                        cubeColors[3] = floorSupportColor;
                        cubeColors[6] = lightenedFloorSupportColor;
                        cubeColors[7] = lightenedFloorSupportColor;
                    }
                    else
                    {
                        cubeColors[2] = Color.Lerp(floorSupportColor, columnBottomColor2, p / (float) FLOOR_SUPPORT_HEIGHT);
                        cubeColors[3] = Color.Lerp(floorSupportColor, columnBottomColor1, p / (float)FLOOR_SUPPORT_HEIGHT);
                        cubeColors[6] = Color.Lerp(lightenedFloorSupportColor, columnBottomColor1, p / (float)FLOOR_SUPPORT_HEIGHT);
                        cubeColors[7] = Color.Lerp(lightenedFloorSupportColor, columnBottomColor2, p / (float)FLOOR_SUPPORT_HEIGHT);
                    }
                    //bottom cube vertices color
                    cubeColors[0] = Color.Lerp(floorSupportColor, columnBottomColor1, (p + 1) / (float)FLOOR_SUPPORT_HEIGHT);
                    cubeColors[1] = Color.Lerp(floorSupportColor, columnBottomColor2, (p + 1) / (float)FLOOR_SUPPORT_HEIGHT);
                    cubeColors[4] = Color.Lerp(lightenedFloorSupportColor, columnBottomColor2, (p + 1) / (float)FLOOR_SUPPORT_HEIGHT);
                    cubeColors[5] = Color.Lerp(lightenedFloorSupportColor, columnBottomColor1, (p + 1) / (float)FLOOR_SUPPORT_HEIGHT);

                    Cube cube = new Cube(this, cubeSize, cubeColors);
                    cube.BuildAtPosition(cubePosition, firstVertexIndex, firstTriangleIndex);

                    columnCubes[p] = cube;
                }

                //create the column structure
                m_columns[i - offset] = new Column(columnCubes, tileRenderer);
            }
            else
            {
                if (++offset == cubeCount)
                    break;
            }
        }
        
        m_mesh = new Mesh();
        m_mesh.name = "FloorSupportMesh";

        m_verticesDirty = true;
        m_trianglesDirty = true;
        m_colorsDirty = true;
        InvalidateMesh();

        //animate columns
        for (int i = 0; i != m_columns.Length; i++)
        {
            IEnumerator translationCoroutine = m_columns[i].TranslateColumn(1.0f + 0.05f * i);
            StartCoroutine(translationCoroutine);
        }
    }

    private void InvalidateMesh()
    {
        if (m_verticesDirty)
        {
            m_mesh.vertices = m_vertices;
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
        }
        if (m_trianglesDirty)
            m_mesh.triangles = m_triangles;
        if (m_colorsDirty)
            m_mesh.colors = m_colors;

        if (m_verticesDirty || m_trianglesDirty || m_colorsDirty)
            GetComponent<MeshFilter>().sharedMesh = m_mesh;
    }

    private class Cube
    {
        private float m_size;
        private Color[] m_colors;

        private FloorSupportRenderer m_parentRenderer;

        private int m_firstVertexIndex;
        private int m_firstTriangleIndex;

        public Cube(FloorSupportRenderer parentRenderer, float size, Color[] colors)
        {
            m_parentRenderer = parentRenderer;
            m_size = size;
            m_colors = colors;
        }

        public void BuildAtPosition(Vector3 position, int firstVertexIndex, int firstTriangleIndex)
        {
            m_firstVertexIndex = firstVertexIndex;
            m_firstTriangleIndex = firstTriangleIndex;

            //vertices
            m_parentRenderer.m_vertices[firstVertexIndex] = position + new Vector3(-0.5f * m_size, -0.5f * m_size, 0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 1] = position + new Vector3(-0.5f * m_size, -0.5f * m_size, -0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 2] = position + new Vector3(-0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 3] = position + new Vector3(-0.5f * m_size, 0.5f * m_size, 0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 4] = position + new Vector3(-0.5f * m_size, -0.5f * m_size, -0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 5] = position + new Vector3(0.5f * m_size, -0.5f * m_size, -0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 6] = position + new Vector3(0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            m_parentRenderer.m_vertices[firstVertexIndex + 7] = position + new Vector3(-0.5f * m_size, 0.5f * m_size, -0.5f * m_size);

            //triangles
            for (int i = 0; i != 2; i++)
            {
                int faceFirstTriangleIndex = firstTriangleIndex + 6 * i;
                int faceFirstTriangleValue = firstVertexIndex + 4 * i;
                m_parentRenderer.m_triangles[faceFirstTriangleIndex] = faceFirstTriangleValue;
                m_parentRenderer.m_triangles[faceFirstTriangleIndex + 1] = faceFirstTriangleValue + 2;
                m_parentRenderer.m_triangles[faceFirstTriangleIndex + 2] = faceFirstTriangleValue + 1;
                m_parentRenderer.m_triangles[faceFirstTriangleIndex + 3] = faceFirstTriangleValue;
                m_parentRenderer.m_triangles[faceFirstTriangleIndex + 4] = faceFirstTriangleValue + 3;
                m_parentRenderer.m_triangles[faceFirstTriangleIndex + 5] = faceFirstTriangleValue + 2;
            }

            //colors
            for (int i = 0; i != 8; i++)
            {
                m_parentRenderer.m_colors[firstVertexIndex + i] = m_colors[i];
            }
        }

        public void TranslateBy(float dy)
        {
            for (int i = 0; i != 8; i++)
            {
                m_parentRenderer.m_vertices[m_firstVertexIndex + i] += new Vector3(0, dy, 0);
            }

            m_parentRenderer.m_verticesDirty = true;
            m_parentRenderer.InvalidateMesh();
        }
    }

    /**
    * A data structure linking cubes and a tile together in one column
    **/
    private class Column
    {
        private Cube[] m_cubes;
        private TileRenderer m_tile;

        private const float TRANSLATION_SPEED = 9.0f;

        public Column(Cube[] cubes, TileRenderer tile)
        {
            m_cubes = cubes;
            m_tile = tile;
        }

        public IEnumerator TranslateColumn(float delay)
        {
            yield return new WaitForSeconds(delay);

            bool bTranslate = true;
            
            while (bTranslate)
            {
                float dy = Time.deltaTime * TRANSLATION_SPEED;
                Vector3 tileCurrentPosition = m_tile.gameObject.transform.localPosition;
                Vector3 tileNextPosition = tileCurrentPosition + new Vector3(0, dy, 0);
                if (tileNextPosition.y > 0)
                {
                    dy = -tileCurrentPosition.y;
                    tileNextPosition = tileCurrentPosition + new Vector3(0, dy, 0);
                    bTranslate = false;
                }

                for (int i = 0; i != m_cubes.Length; i++)
                {
                    m_cubes[i].TranslateBy(dy);
                }

                //Debug.Log("tileNextPositionY:" + tileNextPosition.y);
                m_tile.gameObject.transform.localPosition = tileNextPosition;
                yield return null;
            }

            yield return null;
        }
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i != m_colors.Length; i++)
        {
            m_colors[i] = color;
        }

        m_colorsDirty = true;
        InvalidateMesh();
    }

    //public const float SUPPORT_HEIGHT = 5.0f;
    //private Floor m_floor;

    ////store vertices, triangles and colors in those lists before setting them to the mesh
    //private List<Vector3> m_vertices;
    //private List<int> m_triangles;
    //private List<Color> m_colors;

    //public Color m_mainColor;
    //private Color m_prevMainColor;

    //public void Render(Floor floor)
    //{
    //    m_floor = floor;

    //    m_vertices = new List<Vector3>();
    //    m_triangles = new List<int>();
    //    m_colors = new List<Color>();

    //    Build();
    //}

    //public void Build()
    //{
    //    List<Geometry.Edge> frontLeftContour, frontRightContour;
    //    m_floor.FindVisibleContours(out frontLeftContour, out frontRightContour, 0.9f);

    //    Color faceColor = GetSupportColor();
    //    BuildFaces(frontLeftContour, faceColor);
    //    BuildFaces(frontRightContour, faceColor);
    //    //BuildFaces(frontRightContour, ColorUtils.LightenColor(faceColor, 0.1f));

    //    //Build actual mesh
    //    Mesh supportMesh = new Mesh();
    //    supportMesh.name = "FloorSupportMesh";
    //    supportMesh.vertices = m_vertices.ToArray();
    //    supportMesh.triangles = m_triangles.ToArray();
    //    supportMesh.colors = m_colors.ToArray();

    //    GetComponent<MeshFilter>().sharedMesh = supportMesh;
    //}

    //private void BuildFaces(List<Geometry.Edge> contour, Color color, float verticalOffset = 0)
    //{
    //    int i = 0;
    //    while (i < contour.Count)
    //    {
    //        BuildFace(contour[i], color, verticalOffset);
    //        i++;
    //    }       
    //}

    ///**
    //* Build a face whose top edge is given by parameter 'topEdge' and add it at index 'index'
    //**/
    //private void BuildFace(Geometry.Edge topEdge, Color color, float verticalOffset = 0)
    //{
    //    int triangleFirstIndex = m_vertices.Count;

    //    Vector3 tlVertex = topEdge.m_pointA + new Vector3(0, verticalOffset, 0);  //top-left
    //    Vector3 trVertex = topEdge.m_pointB + new Vector3(0, verticalOffset, 0);  //top-right
    //    Vector3 blVertex = tlVertex - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0); //bottom-left
    //    Vector3 brVertex = trVertex - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);  //bottom-right

    //    m_vertices.Add(tlVertex);
    //    m_vertices.Add(trVertex);
    //    m_vertices.Add(blVertex);
    //    m_vertices.Add(brVertex);

    //    //m_vertices.Add(topEdge.m_pointA + new Vector3(0, verticalOffset, 0));
    //    //m_vertices.Add(topEdge.m_pointB + new Vector3(0, verticalOffset, 0)); //top-right
    //    //Vector3 bottomEdgePointA = topEdge.m_pointA - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);
    //    //Vector3 bottomEdgePointB = topEdge.m_pointB - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);
    //    //m_vertices.Add(bottomEdgePointA + new Vector3(0, verticalOffset, 0)); //bottom-left
    //    //m_vertices.Add(bottomEdgePointB + new Vector3(0, verticalOffset, 0)); //bottom-right        

    //    int[] indices = new int[6] { triangleFirstIndex, triangleFirstIndex + 1 , triangleFirstIndex + 2, triangleFirstIndex + 3, triangleFirstIndex + 2, triangleFirstIndex + 1 };
    //    m_triangles.AddRange(indices);

    //    Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    //    FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
    //    Color vertex3Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(blVertex + this.transform.position));
    //    Color vertex4Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(brVertex + this.transform.position));

    //    m_colors.Add(color);
    //    m_colors.Add(color);
    //    m_colors.Add(vertex3Color);
    //    m_colors.Add(vertex4Color);
    //}

    //public void UpdateGradientTopColor(Color color)
    //{
    //    for (int i = 0; i != m_colors.Count; i+=4)
    //    {
    //        m_colors[i] = color;
    //        m_colors[i + 1] = color;
    //    }

    //    GetComponent<MeshFilter>().sharedMesh.colors = m_colors.ToArray();
    //}

    //public void InvalidateGradientBottomColor()
    //{
    //    Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    //    FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
    //    for (int i = 0; i != m_colors.Count; i += 4)
    //    {
    //        m_colors[i + 2] = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(m_vertices[i + 2] + this.transform.position)); ;
    //        m_colors[i + 3] = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(m_vertices[i + 3] + this.transform.position)); ;
    //    }

    //    GetComponent<MeshFilter>().sharedMesh.colors = m_colors.ToArray();
    //}

    //private Color GetSupportColor()
    //{
    //    return GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme().m_floorSupportColor;
    //}

    //public void Update()
    //{
    //    if (m_mainColor != m_prevMainColor)
    //    {
    //        UpdateGradientTopColor(m_mainColor);
    //        m_prevMainColor = m_mainColor;
    //    }
    //}
}