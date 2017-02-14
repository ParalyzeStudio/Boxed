using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FloorSupportRenderer : MonoBehaviour
{
    public const float SUPPORT_HEIGHT = 5.0f;
    private Floor m_floor;

    //store vertices, triangles and colors in those lists before setting them to the mesh
    private List<Vector3> m_vertices;
    private List<int> m_triangles;
    private List<Color> m_colors;

    public Color m_mainColor;
    private Color m_prevMainColor;

    public void Render(Floor floor)
    {
        m_floor = floor;

        m_vertices = new List<Vector3>();
        m_triangles = new List<int>();
        m_colors = new List<Color>();

        Build();
    }

    public void Build()
    {
        List<Geometry.Edge> frontLeftContour, frontRightContour;
        m_floor.FindVisibleContours(out frontLeftContour, out frontRightContour, 0.9f);

        Color faceColor = GetSupportColor();
        BuildFaces(frontLeftContour, faceColor);
        BuildFaces(frontRightContour, faceColor);
        //BuildFaces(frontRightContour, ColorUtils.LightenColor(faceColor, 0.1f));

        //Build actual mesh
        Mesh supportMesh = new Mesh();
        supportMesh.name = "FloorSupportMesh";
        supportMesh.vertices = m_vertices.ToArray();
        supportMesh.triangles = m_triangles.ToArray();
        supportMesh.colors = m_colors.ToArray();

        GetComponent<MeshFilter>().sharedMesh = supportMesh;
    }

    private void BuildFaces(List<Geometry.Edge> contour, Color color, float verticalOffset = 0)
    {
        int i = 0;
        while (i < contour.Count)
        {
            BuildFace(contour[i], color, verticalOffset);
            i++;
        }       
    }

    /**
    * Build a face whose top edge is given by parameter 'topEdge' and add it at index 'index'
    **/
    private void BuildFace(Geometry.Edge topEdge, Color color, float verticalOffset = 0)
    {
        int triangleFirstIndex = m_vertices.Count;

        Vector3 tlVertex = topEdge.m_pointA + new Vector3(0, verticalOffset, 0);  //top-left
        Vector3 trVertex = topEdge.m_pointB + new Vector3(0, verticalOffset, 0);  //top-right
        Vector3 blVertex = tlVertex - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0); //bottom-left
        Vector3 brVertex = trVertex - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);  //bottom-right

        m_vertices.Add(tlVertex);
        m_vertices.Add(trVertex);
        m_vertices.Add(blVertex);
        m_vertices.Add(brVertex);

        //m_vertices.Add(topEdge.m_pointA + new Vector3(0, verticalOffset, 0));
        //m_vertices.Add(topEdge.m_pointB + new Vector3(0, verticalOffset, 0)); //top-right
        //Vector3 bottomEdgePointA = topEdge.m_pointA - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);
        //Vector3 bottomEdgePointB = topEdge.m_pointB - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);
        //m_vertices.Add(bottomEdgePointA + new Vector3(0, verticalOffset, 0)); //bottom-left
        //m_vertices.Add(bottomEdgePointB + new Vector3(0, verticalOffset, 0)); //bottom-right        

        int[] indices = new int[6] { triangleFirstIndex, triangleFirstIndex + 1 , triangleFirstIndex + 2, triangleFirstIndex + 3, triangleFirstIndex + 2, triangleFirstIndex + 1 };
        m_triangles.AddRange(indices);

        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        Color vertex3Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(blVertex + this.transform.position));
        Color vertex4Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(brVertex + this.transform.position));

        m_colors.Add(color);
        m_colors.Add(color);
        m_colors.Add(vertex3Color);
        m_colors.Add(vertex4Color);
    }

    public void UpdateGradientTopColor(Color color)
    {
        for (int i = 0; i != m_colors.Count; i+=4)
        {
            m_colors[i] = color;
            m_colors[i + 1] = color;
        }

        GetComponent<MeshFilter>().sharedMesh.colors = m_colors.ToArray();
    }

    public void InvalidateGradientBottomColor()
    {
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        FSGradientBillboardQuad background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        for (int i = 0; i != m_colors.Count; i += 4)
        {
            m_colors[i + 2] = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(m_vertices[i + 2] + this.transform.position)); ;
            m_colors[i + 3] = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(m_vertices[i + 3] + this.transform.position)); ;
        }

        GetComponent<MeshFilter>().sharedMesh.colors = m_colors.ToArray();
    }

    private Color GetSupportColor()
    {
        return GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme().m_floorSupportColor;
    }

    public void Update()
    {
        if (m_mainColor != m_prevMainColor)
        {
            UpdateGradientTopColor(m_mainColor);
            m_prevMainColor = m_mainColor;
        }
    }
}