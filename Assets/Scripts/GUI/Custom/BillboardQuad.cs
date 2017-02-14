using UnityEngine;

public class BillboardQuad : Quad
{
    protected Camera m_camera;
    protected Vector2 m_size;

    public void Init(Camera camera, Material material, bool bTexturedQuad = true)
    {
        base.Init(material, bTexturedQuad);

        m_camera = camera;
    }

    public virtual void LateUpdate()
    {
        if (m_camera != null)
        {
            this.transform.rotation = m_camera.transform.rotation;
        }
    }
}
