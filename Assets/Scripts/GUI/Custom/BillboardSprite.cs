using UnityEngine;

public class BillboardSprite : Quad
{
    protected Camera m_camera;

    public override void Init(Material material, bool bTexturedQuad = true)
    {
        base.Init(material, bTexturedQuad);

        m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public virtual void LateUpdate()
    {
        if (m_camera != null)
            this.transform.rotation = m_camera.transform.rotation;
    }
}
