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
        m_floor.FindVisibleContours(out frontLeftContour, out frontRightContour);

        Color faceColor = GetSupportColor(255);
        BuildFaces(frontLeftContour, faceColor);      
        BuildFaces(frontRightContour, ColorUtils.LightenColor(faceColor, 0.1f));

        //Build actual mesh
        Mesh supportMesh = new Mesh();
        supportMesh.name = "FloorSupportMesh";
        supportMesh.vertices = m_vertices.ToArray();
        supportMesh.triangles = m_triangles.ToArray();
        supportMesh.colors = m_colors.ToArray();

        GetComponent<MeshFilter>().sharedMesh = supportMesh;
    }

    private void BuildFaces(List<Geometry.Edge> contour, Color color)
    {
        int i = 0;
        while (i < contour.Count)
        {
            BuildFace(contour[i], color);
            i++;
        }       
    }

    /**
    * Build a face whose top edge is given by parameter 'topEdge' and add it at index 'index'
    **/
    private void BuildFace(Geometry.Edge topEdge, Color color)
    {
        int triangleFirstIndex = m_vertices.Count;

        m_vertices.Add(topEdge.m_pointA); //top-left
        m_vertices.Add(topEdge.m_pointB); //top-right
        Vector3 bottomEdgePointA = topEdge.m_pointA - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);
        Vector3 bottomEdgePointB = topEdge.m_pointB - new Vector3(0, 0.5f * SUPPORT_HEIGHT, 0);
        m_vertices.Add(bottomEdgePointA); //bottom-left
        m_vertices.Add(bottomEdgePointB); //bottom-right        

        int[] indices = new int[6] { triangleFirstIndex, triangleFirstIndex + 1 , triangleFirstIndex + 2, triangleFirstIndex + 3, triangleFirstIndex + 2, triangleFirstIndex + 1 };
        m_triangles.AddRange(indices);

        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GradientBackground background = GameController.GetInstance().GetComponent<GUIManager>().m_background;
        Color vertex3Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(bottomEdgePointA + this.transform.position));
        Color vertex4Color = background.GetColorAtViewportPosition(mainCamera.WorldToViewportPoint(bottomEdgePointB + this.transform.position));

        Color[] faceColors = new Color[4];
        faceColors[0] = color;
        faceColors[1] = color;
        faceColors[2] = vertex3Color;
        faceColors[3] = vertex4Color;
        m_colors.AddRange(faceColors);
    }

    private Color GetSupportColor(float opacity)
    {
        return ColorUtils.GetColorFromRGBAVector4(new Vector4(77, 9, 30, opacity));
    }
}