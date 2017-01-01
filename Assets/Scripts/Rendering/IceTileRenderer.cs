using UnityEngine;

public class IceTileRenderer : TileRenderer
{
    public void Init(IceTile tile)
    {
        base.Init(tile);
    }

    /**
    * Build a barrier that will prevent access on this tile
    **/
    public void GrowBarrier()
    {
        ((IceTile)m_tile).m_blocked = true;

        ///TODO make 3d models (crystals, mushrooms...) appear and grow on the tile
    }
}
