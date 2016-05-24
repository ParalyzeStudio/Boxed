using UnityEngine;

public class GameTouchHandler : TouchHandler
{
    private GameController m_gameController;

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;
    }
    
    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();
    }

    protected override void OnClick(Vector2 clickLocation)
    {
        Debug.Log(clickLocation);
        GameController gameController = GetGameController();
        if (gameController.m_levelEditorMode)
        {
            LevelEditor levelEditor = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();
            if (levelEditor.GUIProcessedClick())
                return;
            //Raycast the tiles to select them
            if (levelEditor.m_state == LevelEditor.State.SELECTING_TILES_BY_CLICKING)
            {                                
                Tile raycastTile = RayCastFloor();
                if (raycastTile.m_state == Tile.State.SELECTED)
                    raycastTile.SetState(Tile.State.NORMAL);
                else
                    raycastTile.SetState(Tile.State.SELECTED);
            }
        }
    }

    private Tile RayCastFloor()
    {
        //Build the plane containing tiles
        Plane tilesPlane = new Plane(Vector3.up, GetGameController().m_floor.transform.position);

        //Build a ray starting from camera near clip plane mouse world space position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (tilesPlane.Raycast(ray, out rayDistance))
        {
            Vector3 rayIntersectionPoint = ray.GetPoint(rayDistance);
            Debug.Log(rayIntersectionPoint);

            //Find the tile which contain the intersection point
            Floor floor = GetGameController().m_floor;
            for (int i = 0; i != floor.Tiles.Length; i++)
            {
                Tile tile = floor.Tiles[i];
                if (tile.ContainsXZPoint(rayIntersectionPoint))
                {
                    return tile;
                }
            }
        }

        return null;
    }

    private GameController GetGameController()
    {
        if (m_gameController == null)
            m_gameController = this.GetComponent<GameController>();

        return m_gameController;
    }
}