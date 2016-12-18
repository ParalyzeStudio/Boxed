using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LightRays : MonoBehaviour
{
    public int m_numLayers = 1;
    public float m_height = 2;
    public float m_width = 1;

    public void Start()
    {
        BuildMesh();
    }

    private void BuildMesh()
    {
        Debug.Log("BuildMesh");
        //VERTICES
        Vector3[] vertices = new Vector3[m_numLayers * 8];
        for (int i = 0; i != m_numLayers; i++)
        {
            float width = (1 - 0.05f * m_numLayers) * m_width;
            vertices[i * 8] = new Vector3(-0.5f * width, 0, -0.5f * width);
            vertices[i * 8 + 1] = new Vector3(0.5f * width, 0, -0.5f * width);
            vertices[i * 8 + 2] = new Vector3(0.5f * width, 0, 0.5f * width);
            vertices[i * 8 + 3] = new Vector3(-0.5f * width, 0, 0.5f * width);
            vertices[i * 8 + 4] = new Vector3(-0.5f * width, m_height, -0.5f * width);
            vertices[i * 8 + 5] = new Vector3(0.5f * width, m_height, -0.5f * width);
            vertices[i * 8 + 6] = new Vector3(0.5f * width, m_height, 0.5f * width);
            vertices[i * 8 + 7] = new Vector3(-0.5f * width, m_height, 0.5f * width);
        }

        //TRIANGLES
        int[] baseTris = new int[] {
                                 0,4,5,
                                 0,5,1,
                                 1,5,2,
                                 2,5,6,
                                 2,6,7,
                                 2,7,3,
                                 3,7,0,
                                 0,7,4
                               };
        List<int> trianglesList = new List<int>(m_numLayers * 24);
        for (int i = 0; i != m_numLayers; i++)
        {
            for (int j = 0; j != 24; j++)
            {
                trianglesList.Add(baseTris[j] + i * 24);
            }
        }

        //uv
        Vector2[] uv = new Vector2[m_numLayers * 8];
        for (int i = 0; i != m_numLayers; i++)
        {
            Vector2 offset = new Vector2(i / (float)m_numLayers, i / (float)m_numLayers);
            uv[8 * i] = new Vector2(0, 0) + offset;
            uv[8 * i + 1] = new Vector2(0.25f, 0) + offset;
            uv[8 * i + 2] = new Vector2(0.5f, 0) + offset;
            uv[8 * i + 3] = new Vector2(0.75f, 0) + offset;
            uv[8 * i + 4] = new Vector2(0, 1) + offset;
            uv[8 * i + 5] = new Vector2(0.25f, 1) + offset;
            uv[8 * i + 6] = new Vector2(0.5f, 1) + offset;
            uv[8 * i + 7] = new Vector2(0.75f, 1) + offset;
        }

        Mesh lrMesh = new Mesh();
        lrMesh.name = "LightRaysMesh";
        lrMesh.vertices = vertices;
        lrMesh.triangles = trianglesList.ToArray();
        lrMesh.uv = uv;
        lrMesh.RecalculateBounds();
        lrMesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = lrMesh;
    }
}
