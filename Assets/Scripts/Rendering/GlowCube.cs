using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GlowCube : MonoBehaviour
{
    private struct CubeFace
    {
        public int[] m_indices; //array of 4 indices
        public Vector3 m_normal;

        public CubeFace(Vector3[] baseVertices, int a, int b, int c, int d)
        {
            m_indices = new int[4];
            m_indices[0] = a;
            m_indices[1] = b;
            m_indices[2] = c;
            m_indices[3] = d;

            Vector3 ab = baseVertices[b] - baseVertices[a];
            Vector3 bc = baseVertices[c] - baseVertices[b];
            m_normal = -Vector3.Cross(ab, bc);
            m_normal.Normalize();
        }
    }

    public void Start()
    {
        BuildMesh();
    }

    public void BuildMesh()
    {
        Vector3[] baseVertices = new Vector3[24];
        baseVertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
        baseVertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
        baseVertices[2] = new Vector3(0.5f, -0.5f, 0.5f);
        baseVertices[3] = new Vector3(-0.5f, -0.5f, 0.5f);
        baseVertices[4] = new Vector3(-0.5f, 0.5f, -0.5f);
        baseVertices[5] = new Vector3(0.5f, 0.5f, -0.5f);
        baseVertices[6] = new Vector3(0.5f, 0.5f, 0.5f);
        baseVertices[7] = new Vector3(-0.5f, 0.5f, 0.5f);

        //build top and bottom faces of the cube
        CubeFace[] faces = new CubeFace[6];
        faces[0] = new CubeFace(baseVertices, 3, 2, 1, 0);
        faces[1] = new CubeFace(baseVertices, 4, 5, 6, 7);

        //build other faces
        for (int i = 2; i != 6; i++)
        {
            int startIdx = i - 2;
            faces[i] = new CubeFace(baseVertices, startIdx, (i == 5) ? 0 : startIdx + 1, (i == 5) ? 4 : startIdx + 5, startIdx + 4);
        }


        //build mesh vertices
        Vector3[] vertices = new Vector3[24];

        for (int i = 0; i != faces.Length; i++)
        {
            CubeFace face = faces[i];
            for (int j = 0; j != face.m_indices.Length; j++)
            {
                vertices[i * 4 + j] = baseVertices[face.m_indices[j]];
            }
        }

        //Construct mesh triangles and normals
        int[] triangles = new int[36];
        Vector3[] normals = new Vector3[24];

        for (int i = 0; i != 6; i++)
        {
            triangles[i * 6] = i * 4;
            triangles[i * 6 + 1] = i * 4 + 2;
            triangles[i * 6 + 2] = i * 4 + 1;
            triangles[i * 6 + 3] = i * 4;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 2;

            for (int p = 0; p != 4; p++)
            {
                normals[i * 4 + p] = faces[i].m_normal;
            }
        }

        //same texture uv applied to each face
        Vector2[] uv = new Vector2[24];
        for (int i = 0; i != 6; i++)
        {
            uv[4 * i] = new Vector2(0, 0);
            uv[4 * i + 1] = new Vector2(1, 0);
            uv[4 * i + 2] = new Vector2(1, 1);
            uv[4 * i + 3] = new Vector2(0, 1);
        }

        Mesh cubeMesh = new Mesh();
        cubeMesh.name = "GlowCubeMesh";
        cubeMesh.vertices = vertices;
        cubeMesh.triangles = triangles;
        cubeMesh.normals = normals;
        cubeMesh.uv = uv;

        GetComponent<MeshFilter>().sharedMesh = cubeMesh;
    }
}
