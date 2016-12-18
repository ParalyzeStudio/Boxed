using UnityEngine;

/**
* Class used to modify the rendering of a bonus game object
**/
public class BonusRenderer : MonoBehaviour
{
    private const float MAIN_CUBE_ROTATION_SPEED = 120.0f; //120 degrees/sec
    private const float CUBE1_ORBITING_SPEED = 120.0f;
    private const float CUBE2_ORBITING_SPEED = 120.0f;

    private Vector3[] m_vertices;
    private int[] m_triangles;
    private Vector2[] m_uv;
    private Vector3[] m_normals;

    private OrbitCube[] m_cubes;

    private class OrbitCube
    {
        public int m_index; //the index of the cube in the parent combined mesh

        public Vector3 m_position;
        public Quaternion m_rotation;
        public float m_size;

        private Vector3[] m_baseVertices;
        public int[] m_triangles;
        public Vector2[] m_uv;
        public Vector3[] m_normals;

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

        public OrbitCube(int index, Vector3 position, Quaternion rotation, float size)
        {
            m_index = index;
            m_position = position;
            m_rotation = rotation;
            m_size = size;

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
            m_baseVertices = new Vector3[24];
            for (int i = 0; i != faces.Length; i++)
            {
                CubeFace face = faces[i];
                for (int j = 0; j != face.m_indices.Length; j++)
                {
                    m_baseVertices[i * 4 + j] = baseVertices[face.m_indices[j]];
                }
            }

            //Construct mesh triangles and normals
            m_triangles = new int[36];
            m_normals = new Vector3[24];

            for (int i = 0; i != 6; i++)
            {
                m_triangles[i * 6] = i * 4;
                m_triangles[i * 6 + 1] = i * 4 + 2;
                m_triangles[i * 6 + 2] = i * 4 + 1;
                m_triangles[i * 6 + 3] = i * 4;
                m_triangles[i * 6 + 4] = i * 4 + 3;
                m_triangles[i * 6 + 5] = i * 4 + 2;

                for (int p = 0; p != 4; p++)
                {
                    m_normals[i * 4 + p] = faces[i].m_normal;
                }
            }

            //same texture uv applied to each face
            m_uv = new Vector2[24];
            for (int i = 0; i != 6; i++)
            {
                m_uv[4 * i] = new Vector2(0, 0);
                m_uv[4 * i + 1] = new Vector2(1, 0);
                m_uv[4 * i + 2] = new Vector2(1, 1);
                m_uv[4 * i + 3] = new Vector2(0, 1);
            }
        }

        public Vector3[] GetVertices()
        {
            //Vector3[] vertices = new Vector3[24];
            //vertices[0] = new Vector3(-0.5f, -0.5f, -0.5f);
            //vertices[1] = new Vector3(0.5f, -0.5f, -0.5f);
            //vertices[2] = new Vector3(0.5f, -0.5f, 0.5f);
            //vertices[3] = new Vector3(-0.5f, -0.5f, 0.5f);
            //vertices[4] = new Vector3(-0.5f, 0.5f, -0.5f);
            //vertices[5] = new Vector3(0.5f, 0.5f, -0.5f);
            //vertices[6] = new Vector3(0.5f, 0.5f, 0.5f);
            //vertices[7] = new Vector3(-0.5f, 0.5f, 0.5f);

            Vector3[] transformedVertices = new Vector3[m_baseVertices.Length];
            for (int i = 0; i != 24; i++)
            {
                transformedVertices[i] = m_rotation * m_baseVertices[i];
                transformedVertices[i] *= m_size;
                transformedVertices[i] += m_position;
            }

            return transformedVertices;
        }

        public void TranslateBy(Vector3 dTranslation)
        {
            m_position += dTranslation;
        }

        public void RotateBy(float angle, Vector3 axis)
        {
            m_rotation = m_rotation * Quaternion.AngleAxis(angle, axis);
        }
    }

    /**
    * Build 3 cubes of ascendant size that will rotate and orbit
    **/
    public void Build()
    {
        //build actual mesh
        m_vertices = new Vector3[3 * 24];
        m_triangles = new int[3 * 36];
        m_uv = new Vector2[3 * 24];
        m_normals = new Vector3[3 * 24];

        Quaternion cube0Rotation = Quaternion.Euler(45, 0, 45);
        Quaternion cube1Rotation = Quaternion.Euler(15, 35, 60);
        Quaternion cube2Rotation = Quaternion.Euler(15, 35, 60);

        m_cubes = new OrbitCube[3];
        BuildCubeAtIndex(0, new OrbitCube(0, new Vector3(0, 0.75f, 0), cube0Rotation, 1.0f));
        BuildCubeAtIndex(1, new OrbitCube(1, new Vector3(1.0f, 1.5f, 0), cube1Rotation, 0.66f));
        BuildCubeAtIndex(2, new OrbitCube(2, new Vector3(-1.0f, 3.0f, 0), cube2Rotation, 0.5f));

        Mesh bonusMesh = new Mesh();
        bonusMesh.name = "BonusMesh";

        bonusMesh.vertices = m_vertices;
        bonusMesh.triangles = m_triangles;
        bonusMesh.uv = m_uv;
        bonusMesh.normals = m_normals;

        GetComponent<MeshFilter>().sharedMesh = bonusMesh;
    }

    private void BuildCubeAtIndex(int index, OrbitCube cube)
    {
        m_cubes[index] = cube;

        Vector3[] vertices = cube.GetVertices();
        for (int i = 0; i != 24; i++)
        {
            m_vertices[24 * index + i] = vertices[i];
            m_uv[24 * index + i] = cube.m_uv[i];
            m_normals[24 * index + i] = cube.m_normals[i];
        }

        for (int i = 0; i != 36; i++)
        {
            m_triangles[36 * index + i] = cube.m_triangles[i] + 24 * index;
        }
    }

    private void InvalidateCubeAtIndex(int index)
    {
        Vector3[] vertices = m_cubes[index].GetVertices();
        for (int i = 0; i != 24; i++)
        {
            m_vertices[24 * index + i] = vertices[i];
        }
    }

    /**
    * After invalidating the array of vertices, commit the changes to the mesh
    **/
    private void CommitChanges()
    {
        GetComponent<MeshFilter>().sharedMesh.vertices = m_vertices;
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        //Make the cube rotate along
        //GameObjectAnimator cubeAnimator = this.GetComponent<GameObjectAnimator>();
        //Vector3 rotationAxis = this.transform.rotation * Vector3.up;
        //cubeAnimator.SetRotationAxis(rotationAxis);
        //cubeAnimator.RotateBy(rotationSpeed * dt, dt);

        //rotate cubes
        float rotationAngle = MAIN_CUBE_ROTATION_SPEED * dt;
        m_cubes[0].RotateBy(MAIN_CUBE_ROTATION_SPEED * dt, m_cubes[0].m_rotation * Vector3.up);
        m_cubes[1].RotateBy(2 * MAIN_CUBE_ROTATION_SPEED * dt, Vector3.up);
        m_cubes[2].RotateBy(2.5f * MAIN_CUBE_ROTATION_SPEED * dt, Vector3.up);

        //make secondary cubes orbit around main cube
        Vector3 cube1Position = m_cubes[1].m_position;
        Vector3 cube2Position = m_cubes[2].m_position;
        m_cubes[1].m_position = Quaternion.AngleAxis(CUBE1_ORBITING_SPEED * dt, Vector3.up) * cube1Position;
        m_cubes[2].m_position = Quaternion.AngleAxis(CUBE1_ORBITING_SPEED * dt, Vector3.up) * cube2Position;

        InvalidateCubeAtIndex(0);
        InvalidateCubeAtIndex(1);
        InvalidateCubeAtIndex(2);

        CommitChanges();
    }
}
