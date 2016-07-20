using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Quad : MonoBehaviour
{
    public Vector4 m_textureRange { get; set; }
    public TextureWrapMode m_textureWrapMode { get; set; }

    public Color[] m_colors { get; set; }

    //public void Start()
    //{
    //    //hide some components
    //    this.GetComponent<MeshFilter>().hideFlags = HideFlags.NotEditable;
    //    this.GetComponent<MeshRenderer>().hideFlags = HideFlags.NotEditable;
    //}

    /**
    * Init a quad object with a specified material.
    * Set bTexturedQuad to true if the material has a texture attached to it
    **/
    public virtual void Init(Material material, bool bTexturedQuad = false)
    {
        Mesh mesh = new Mesh();
        mesh.name = "BaseQuad";

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, 0.5f, 0.0f); //top-left
        vertices[1] = new Vector3(0.5f, 0.5f, 0.0f); //top-right
        vertices[2] = new Vector3(-0.5f, -0.5f, 0.0f); //bottom-left
        vertices[3] = new Vector3(0.5f, -0.5f, 0.0f); //bottom-right
        mesh.vertices = vertices;

        int[] indices = new int[6] { 0, 1, 2, 3, 2, 1 };
        mesh.triangles = indices;

        Vector3[] normals = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
        mesh.normals = normals;
        
        //Apply material to the quad
        if (material != null)
            GetComponent<MeshRenderer>().sharedMaterial = material;

        //In case the material has a texture attached to it, instantiate the uvs array
        if (bTexturedQuad)
        {
            //Set default texture range
            SetTextureRange(new Vector4(0, 0, 1, 1));

            //set default wrap mode to CLAMP
            SetTextureWrapMode(TextureWrapMode.Clamp);
        }        
        else
        {
            //Set default white value for colors array
            SetColors(new Color[4] { Color.white, Color.white, Color.white, Color.white });
        }
    }

    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    protected void UpdateUVs()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(m_textureRange.x, m_textureRange.y + m_textureRange.w); //top-left
        uvs[1] = new Vector2(m_textureRange.x + m_textureRange.z, m_textureRange.y + m_textureRange.w); //top-right
        uvs[2] = new Vector2(m_textureRange.x, m_textureRange.y); //bottom-left
        uvs[3] = new Vector2(m_textureRange.x + m_textureRange.z, m_textureRange.y); //bottom-right

        mesh.uv = uvs;
    }

    public void SetTextureRange(Vector4 textureRange)
    {
        m_textureRange = textureRange;
        UpdateUVs();
    }

    //public void SetSize(Vector2 size)
    //{
    //    Vector3[] vertices = new Vector3[4];
    //    vertices[0] = new Vector3(-0.5f * size.x, 0.5f * size.y, 0.0f); //top-left
    //    vertices[1] = new Vector3(0.5f * size.x, 0.5f * size.y, 0.0f); //top-right
    //    vertices[2] = new Vector3(-0.5f * size.x, -0.5f * size.y, 0.0f); //bottom-left
    //    vertices[3] = new Vector3(0.5f * size.x, -0.5f * size.y, 0.0f); //bottom-right
    //    mesh.vertices = vertices;
    //}

    /**
     * Switch between CLAMP and REPEAT wrap modes
     * **/
    public void SetTextureWrapMode(TextureWrapMode texWrapMode)
    {
        m_textureWrapMode = texWrapMode;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
            renderer.sharedMaterial.mainTexture.wrapMode = texWrapMode;
    }

    public void SetColors(Color[] colors)
    {
        m_colors = colors;
        GetComponent<MeshFilter>().sharedMesh.colors = colors;
    }
}
