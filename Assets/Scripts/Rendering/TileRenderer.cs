using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    public Tile m_tile; //the tile data used to render a cuboid tile

    //materials
    public Material m_defaultTileMaterial;
    public Material m_disabledTileMaterial;
    public Material m_selectedTileMaterial;
    public Material m_startTileMaterial;
    public Material m_finishTileMaterial;

    //Bonus
    public GameObject m_bonusPfb;
    private GameObject m_bonusObject;

    public void Init(Tile tile)
    {
        m_tile = tile;
        UpdateMaterial();
        if (tile.AttachedBonus != null)
            BuildBonusObject();
        this.transform.localScale = new Vector3(0.95f * tile.m_size, this.transform.localScale.y, 0.95f * tile.m_size);
    }

    public void UpdateMaterial()
    {
        Material material = null;
        if (m_tile.CurrentState == Tile.State.NORMAL)
            material = m_defaultTileMaterial;
        else if (m_tile.CurrentState == Tile.State.SELECTED)
            material = m_selectedTileMaterial;
        else if (m_tile.CurrentState == Tile.State.DISABLED)
            material = m_disabledTileMaterial;
        else if (m_tile.CurrentState == Tile.State.START)
            material = m_startTileMaterial;
        else if (m_tile.CurrentState == Tile.State.FINISH)
            material = m_finishTileMaterial;

        this.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /**
    * Build a bonus element over this tile
    **/
    public void BuildBonusObject()
    {
        m_bonusObject = (GameObject)Instantiate(m_bonusPfb);
        m_bonusObject.name = "Bonus";

        GameObject bonusContainer = GameController.GetInstance().m_bonuses;
        m_bonusObject.transform.parent = bonusContainer.transform;

        GameObjectAnimator bonusAnimator = m_bonusObject.GetComponent<GameObjectAnimator>();
        bonusAnimator.SetPosition(m_tile.GetWorldPosition() + new Vector3(0, 0.3f, 0));
    }

    public void DestroyBonusObject()
    {
        Destroy(m_bonusObject.gameObject);
    }

    public void Update()
    {
        if (m_tile.m_tileMaterialDirty)
        {
            UpdateMaterial();
            m_tile.m_tileMaterialDirty = false;
        }
    }
}