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
        GameController gameController = GetGameController();
        if (gameController.m_levelEditorMode)
        {
            LevelEditor levelEditor = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();
            if (levelEditor.GUIProcessedClick())
                return;

            //Raycast the tiles to select them
            Tile raycastTile = RayCastFloor();
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.TILES_EDITING)
            {
                if (raycastTile.m_state == Tile.State.SELECTED)
                    raycastTile.SetState(Tile.State.NORMAL);
                else
                    raycastTile.SetState(Tile.State.SELECTED);
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.CHECKPOINTS_EDITING)
            {
               
                if (raycastTile.m_state == Tile.State.SELECTED)
                {
                    if (levelEditor.m_startTile == null) //there is no other tile that has been marked as start tile
                    {
                        raycastTile.SetState(Tile.State.START);
                        levelEditor.m_startTile = raycastTile;
                    }
                    else if (levelEditor.m_finishTile == null) //there is no other tile that has been marked as finish tile
                    {
                        raycastTile.SetState(Tile.State.FINISH);
                        levelEditor.m_finishTile = raycastTile;
                    }
                }
                else if (raycastTile.m_state == Tile.State.START)
                {
                    if (levelEditor.m_finishTile == null)
                    {
                        raycastTile.SetState(Tile.State.FINISH);
                        levelEditor.m_finishTile = raycastTile;
                        levelEditor.m_startTile = null;
                    }
                    else
                    {
                        raycastTile.SetState(Tile.State.SELECTED);
                        levelEditor.m_startTile = null;
                    }
                }
                else if (raycastTile.m_state == Tile.State.FINISH)
                {
                    raycastTile.SetState(Tile.State.SELECTED);
                    levelEditor.m_finishTile = null;
                }
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.BONUSES_EDITING)
            {
                if (raycastTile.m_state == Tile.State.SELECTED)
                {
                    raycastTile.AddBonus();
                }
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