using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GlowSquare : MonoBehaviour
{
    private Vector3[] m_vertices;
    private Vector2[] m_uv;
    private int[] m_triangles;
    private Color[] m_colors;

    public void Init()
    {
        Mesh glowSquareMesh = new Mesh();
        glowSquareMesh.name = "GlowSquareMesh";

        GetComponent<MeshFilter>().mesh = glowSquareMesh;
                
        m_vertices = new Vector3[4];
        m_triangles = new int[4];
        m_colors = new Color[4];

        //vertices
        m_vertices[0] = new Vector3(-0.5f, 0, -0.5f);
        m_vertices[1] = new Vector3(0.5f, 0, -0.5f);
        m_vertices[2] = new Vector3(0.5f, 0, 0.5f);
        m_vertices[3] = new Vector3(-0.5f, 0, 0.5f);

        //triangles
        m_triangles = new int[] { 0, 3, 1, 1, 3 ,2};

        //uvs
        m_uv = new Vector2[4];
        m_uv[0] = new Vector2(0, 0);
        m_uv[1] = new Vector2(1, 0);
        m_uv[2] = new Vector2(1, 1);
        m_uv[3] = new Vector2(0, 1);

        //colors
        m_colors = new Color[4];
        for (int i = 0; i != m_colors.Length; i++)
        {
            m_colors[i] = Color.white;
        }

        Invalidate();
    }

    public void Invalidate()
    {
        float opacity = this.GetComponent<GlowSquareAnimator>().m_opacity;
        for (int i = 0; i != m_colors.Length; i++)
        {
            m_colors[i] = ColorUtils.FadeColor(m_colors[i], opacity);
        }

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        mesh.vertices = m_vertices;
        mesh.triangles = m_triangles;
        mesh.colors = m_colors;
        mesh.uv = m_uv;
    }
}
