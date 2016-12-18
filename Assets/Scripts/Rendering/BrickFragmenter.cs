using UnityEngine;

public class BrickFragmenter : MonoBehaviour
{
    private Vector3[] m_vertices;
    private int[] m_triangles;
    private Color[] m_colors;

    private bool m_verticesDirty;
    private bool m_trianglesDirty;
    private bool m_colorsDirty;

    private CubeFragment[] m_fragments;

    private class CubeFragment
    {
        public const int NUM_VERTICES = 12;
        public const int NUM_TRIANGLES = 6;

        private BrickFragmenter m_parentFragmenter;
        public int m_index;
        private Vector3 m_position;
        private float m_size;

        //translation
        private bool m_translating;
        private Vector3 m_translationFromPosition;
        private Vector3 m_translationToPosition;
        private float m_translationDuration;
        private float m_translationElapsedTime;
        private float m_translationDelay;

        public CubeFragment(BrickFragmenter parentFragmenter, int index, Color color, float size, Vector3 position)
        {
            m_parentFragmenter = parentFragmenter;
            m_index = index;
            m_position = position;
            m_size = size;
            SetPosition(position);
            SetColor(color);

            //build triangles
            int[] cubeLocalTriangles = new int[]
            {
                0,3,2,
                0,2,1,
                4,7,6,
                4,6,5,
                8,11,10,
                8,10,9
            };

            int firstTriangleIndex = 3 * NUM_TRIANGLES * index;
            int firstTriangleValue = NUM_VERTICES * index;
            for (int m = 0; m != 3 * NUM_TRIANGLES; m++)
            {
                m_parentFragmenter.m_triangles[firstTriangleIndex + m] = firstTriangleValue + cubeLocalTriangles[m];
            }
        }

        public void SetColor(Color color)
        {
            int firstColorIndex = NUM_VERTICES * m_index;
            for (int m = 0; m != NUM_VERTICES; m++)
            {
                m_parentFragmenter.m_colors[firstColorIndex + m] = color;
            }
        }

        public void SetPosition(Vector3 position)
        {
            m_position = position;

            Vector3[] cubeLocalVertices = new Vector3[NUM_VERTICES];
            cubeLocalVertices[0] = new Vector3(-0.5f * m_size, -0.5f * m_size, 0.5f * m_size);
            cubeLocalVertices[1] = new Vector3(-0.5f * m_size, -0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[2] = new Vector3(-0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[3] = new Vector3(-0.5f * m_size, 0.5f * m_size, 0.5f * m_size);
            cubeLocalVertices[4] = new Vector3(-0.5f * m_size, -0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[5] = new Vector3(0.5f * m_size, -0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[6] = new Vector3(0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[7] = new Vector3(-0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[8] = new Vector3(-0.5f * m_size, 0.5f * m_size, 0.5f * m_size);
            cubeLocalVertices[9] = new Vector3(-0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[10] = new Vector3(0.5f * m_size, 0.5f * m_size, -0.5f * m_size);
            cubeLocalVertices[11] = new Vector3(0.5f * m_size, 0.5f * m_size, 0.5f * m_size);

            int firstVertexIndex = NUM_VERTICES * m_index;
            
            for (int m = 0; m != NUM_VERTICES; m++)
            {
                m_parentFragmenter.m_vertices[firstVertexIndex + m] = position + cubeLocalVertices[m];
            }
        }

        public void SetSize(float size)
        {
            m_size = size;
            SetPosition(m_position);
        }


        public void TranslateTo(Vector3 toPosition, float duration, float delay = 0.0f)
        {
            m_translating = true;
            m_translationFromPosition = m_position;
            m_translationToPosition = toPosition;
            m_translationDuration = duration;
            m_translationDelay = delay;
            m_translationElapsedTime = 0;
        }

        public void Update(float dt)
        {
            if (m_translating)
            {
                bool inDelay = (m_translationElapsedTime < m_translationDelay);
                m_translationElapsedTime += dt;
                if (m_translationElapsedTime >= m_translationDelay)
                {
                    if (inDelay) //we were in delay previously
                        dt = m_translationElapsedTime - m_translationDelay;
                    float effectiveElapsedTime = m_translationElapsedTime - m_translationDelay;

                    Vector3 positionVariation = m_translationToPosition - m_translationFromPosition;
                    float deltaPositionX = Mathf.Lerp(m_translationFromPosition.x, m_translationToPosition.x, effectiveElapsedTime / m_translationDuration);
                    float deltaPositionY = Mathf.Lerp(m_translationFromPosition.y, m_translationToPosition.y, effectiveElapsedTime / m_translationDuration);
                    float deltaPositionZ = Mathf.Lerp(m_translationFromPosition.z, m_translationToPosition.z, effectiveElapsedTime / m_translationDuration);
                    Vector3 deltaPosition = new Vector3(deltaPositionX, deltaPositionY, deltaPositionZ);

                    if (effectiveElapsedTime > m_translationDuration)
                    {
                        SetPosition(m_translationToPosition);
                        m_translating = false;
                    }
                    else
                    {
                        SetPosition(m_position + deltaPosition);
                    }
                }
            }
        }
    }

    public void FragmentBrick()
    {
        int numCubesPerBrickDimension = 4;
        float cubeOriginalSize = Brick.BRICK_BASIS_DIMENSION / (float)numCubesPerBrickDimension;
        float cubeScale = 0;
        float cubeSize = cubeScale * Brick.BRICK_BASIS_DIMENSION / (float)numCubesPerBrickDimension;        

        int numCubes = 2 * numCubesPerBrickDimension * numCubesPerBrickDimension * numCubesPerBrickDimension;
        m_fragments = new CubeFragment[numCubes];
        m_vertices = new Vector3[numCubes * CubeFragment.NUM_VERTICES];
        m_triangles = new int[numCubes * 3 * CubeFragment.NUM_TRIANGLES];
        m_colors = new Color[m_vertices.Length];

        int cubeIndex = 0;
        for (int i = 0; i != numCubesPerBrickDimension; i++)
        {
            for (int j = 0; j != 2 * numCubesPerBrickDimension; j++)
            {
                for (int k = 0; k != numCubesPerBrickDimension; k++)
                {
                    Vector3 cubePosition;
                    if (numCubesPerBrickDimension % 2 == 0)
                    {
                        cubePosition = new Vector3((i - numCubesPerBrickDimension / 2 + 0.5f) * cubeOriginalSize,
                                                   (j - numCubesPerBrickDimension + 0.5f) * cubeOriginalSize,
                                                   (k - numCubesPerBrickDimension / 2 + 0.5f) * cubeOriginalSize);
                    }
                    else
                    {
                        cubePosition = new Vector3((i - numCubesPerBrickDimension / 2) * cubeOriginalSize,
                                                   (j - numCubesPerBrickDimension + 0.5f) * cubeOriginalSize,
                                                   (k - numCubesPerBrickDimension / 2) * cubeOriginalSize);
                    }

                    //build the actual cube
                    CubeFragment cube = new CubeFragment(this, cubeIndex, Color.white, cubeSize, cubePosition);
                    m_fragments[cubeIndex] = cube;

                    //increment the cube index
                    cubeIndex++;
                }
            }
        }

        //copy values to the actual mesh
        Mesh mesh = new Mesh();
        mesh.name = "BrickFragments";
        GetComponent<MeshFilter>().sharedMesh = mesh;
        m_verticesDirty = true;
        m_trianglesDirty = true;
        m_colorsDirty = true;

        Invalidate();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //build a child game object that will hold this mesh
        //GameObject fragments = new GameObject("Fragments");
        ////fragments.transform.parent = this.transform;
        //fragments.transform.localPosition = Vector3.zero;
        //fragments.AddComponent<MeshRenderer>();
        //MeshFilter fragmentsMeshFilter = fragments.AddComponent<MeshFilter>();
        //fragmentsMeshFilter.sharedMesh = mesh;
    }

    public void Invalidate()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        if (m_verticesDirty)
        {
            mesh.vertices = m_vertices;
            m_verticesDirty = false;
        }

        if (m_trianglesDirty)
        {
            mesh.triangles = m_triangles;
            m_trianglesDirty = false;
        }

        if (m_colorsDirty)
        {
            mesh.colors = m_colors;
            m_colorsDirty = false;
        }
    }
}
