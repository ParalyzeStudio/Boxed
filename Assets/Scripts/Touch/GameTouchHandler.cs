using UnityEngine;

public class GameTouchHandler : TouchHandler
{
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
        if (GameController.GetInstance().m_levelEditorMode)
        {
            LevelEditor levelEditor = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();
            if (levelEditor.GUIProcessedClick() || levelEditor.IsSaveLevelWindowActive())
                return;

            //Raycast the tiles to select them
            Tile raycastTile = RayCastFloor();
            if (raycastTile == null)
                return;
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.TILES_EDITING)
            {
                if (raycastTile.CurrentState == Tile.State.SELECTED)
                    raycastTile.CurrentState = Tile.State.NORMAL;
                else
                    raycastTile.CurrentState = Tile.State.SELECTED;
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.CHECKPOINTS_EDITING)
            {
               
                if (raycastTile.CurrentState == Tile.State.SELECTED)
                {
                    if (levelEditor.m_startTile == null) //there is no other tile that has been marked as start tile
                    {
                        raycastTile.CurrentState = Tile.State.START;
                        levelEditor.m_startTile = raycastTile;
                    }
                    else if (levelEditor.m_finishTile == null) //there is no other tile that has been marked as finish tile
                    {
                        raycastTile.CurrentState = Tile.State.FINISH;
                        levelEditor.m_finishTile = raycastTile;
                    }
                }
                else if (raycastTile.CurrentState == Tile.State.START)
                {
                    if (levelEditor.m_finishTile == null)
                    {
                        raycastTile.CurrentState = Tile.State.FINISH;
                        levelEditor.m_finishTile = raycastTile;
                        levelEditor.m_startTile = null;
                    }
                    else
                    {
                        raycastTile.CurrentState = Tile.State.SELECTED;
                        levelEditor.m_startTile = null;
                    }
                }
                else if (raycastTile.CurrentState == Tile.State.FINISH)
                {
                    raycastTile.CurrentState = Tile.State.SELECTED;
                    levelEditor.m_finishTile = null;
                }
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.BONUSES_EDITING)
            {
                if (raycastTile.CurrentState == Tile.State.SELECTED)
                {
                    Bonus bonus = new Bonus(); //for the moment set an empty object as a bonus
                    FloorRenderer floorRenderer = GameController.GetInstance().m_floor;
                    floorRenderer.GetRendererForTile(raycastTile).BuildBonusObject();
                    raycastTile.AttachedBonus = bonus;
                }
            }
        }
    }

    private Tile RayCastFloor()
    {
        //Build the plane containing tiles
        Plane tilesPlane = new Plane(Vector3.up, GameController.GetInstance().m_floor.transform.position);

        //Build a ray starting from camera near clip plane mouse world space position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (tilesPlane.Raycast(ray, out rayDistance))
        {
            Vector3 rayIntersectionPoint = ray.GetPoint(rayDistance);

            //Find the tile which contain the intersection point
            Floor floor = GameController.GetInstance().m_floor.m_floorData;
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
}