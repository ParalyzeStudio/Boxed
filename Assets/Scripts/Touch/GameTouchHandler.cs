using UnityEngine;

public class GameTouchHandler : TouchHandler
{
    private Tile m_lastRaycastTile; //the last tile that has been raycast

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        if (GameController.GetInstance().m_gameMode == GameController.GameMode.LEVEL_EDITOR)
        {
            LevelEditor levelEditor = GameController.GetInstance().GetGUIManager().m_levelEditor;
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.TILES_EDITING)
                EditTiles();
        }

        base.OnPointerDown(pointerLocation);
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (GameController.GetInstance().m_gameMode == GameController.GameMode.LEVEL_EDITOR)
        {
            LevelEditor levelEditor = GameController.GetInstance().GetGUIManager().m_levelEditor;
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.TILES_EDITING)
                EditTiles();
        }

        return true;
    }

    protected override void OnClick(Vector2 clickLocation)
    {
        if (GameController.GetInstance().m_gameMode == GameController.GameMode.LEVEL_EDITOR)
        {
            LevelEditor levelEditor = GameController.GetInstance().GetGUIManager().m_levelEditor;

            //Raycast the tiles to select them
            Tile raycastTile = RayCastFloor();
            if (raycastTile == null)
                return;
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.CHECKPOINTS_EDITING)
            {
                Tile levelStartTile = levelEditor.m_editedLevel.m_floor.GetStartTile();
                Tile levelFinishTile = levelEditor.m_editedLevel.m_floor.GetFinishTile();

                if (raycastTile.CurrentState == Tile.State.NORMAL)
                {   
                    if (levelStartTile == null) //there is no other tile that has been marked as start tile
                    {
                        raycastTile.CurrentState = Tile.State.START;
                        levelEditor.m_editedLevel.m_floor.SetStartTile(raycastTile);
                    }
                    else if (levelFinishTile == null) //there is no other tile that has been marked as finish tile
                    {
                        raycastTile.CurrentState = Tile.State.FINISH;
                        levelEditor.m_editedLevel.m_floor.SetFinishTile(raycastTile);
                    }
                }
                else if (raycastTile.CurrentState == Tile.State.START)
                {
                    if (levelFinishTile == null)
                    {
                        raycastTile.CurrentState = Tile.State.FINISH;
                        levelEditor.m_editedLevel.m_floor.SetFinishTile(raycastTile);
                        levelEditor.m_editedLevel.m_floor.SetStartTile(null);
                    }
                    else
                    {
                        raycastTile.CurrentState = Tile.State.NORMAL;
                        levelEditor.m_editedLevel.m_floor.SetStartTile(null);
                    }
                }
                else if (raycastTile.CurrentState == Tile.State.FINISH)
                {
                    raycastTile.CurrentState = Tile.State.NORMAL;
                    levelEditor.m_editedLevel.m_floor.SetFinishTile(null);
                }
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.BONUSES_EDITING)
            {
                if (raycastTile.CurrentState == Tile.State.NORMAL)
                {
                    if (raycastTile.AttachedBonus != null)
                    {
                        FloorRenderer floorRenderer = GameController.GetInstance().m_floor;
                        floorRenderer.GetRendererForTile(raycastTile).DestroyBonusObject();
                        raycastTile.AttachedBonus = null;
                    }
                    else
                    {
                        Bonus bonus = new Bonus(); //for the moment set an empty object as a bonus
                        FloorRenderer floorRenderer = GameController.GetInstance().m_floor;
                        floorRenderer.GetRendererForTile(raycastTile).BuildBonusObject();
                        raycastTile.AttachedBonus = bonus;
                    }
                }
            }
        }
    }

    /**
    * Apply the action of editing tiles to the currently raycast tile
    **/
    private bool EditTiles()
    {
        Tile raycastTile = RayCastFloor();
        if (raycastTile == null || raycastTile == m_lastRaycastTile)
            return false;
        m_lastRaycastTile = raycastTile;

        LevelEditorMenuSwitcher menuSwitcher = GameController.GetInstance().GetGUIManager().m_levelEditor.m_menuSwitcher;
        EditTilesSubMenu.TileSelectionMode tileSelectionMode = ((EditTilesSubMenu)menuSwitcher.GetMenuForID(LevelEditorMenuSwitcher.MenuID.ID_EDIT_TILES)).m_tileSelectionMode;

        if (tileSelectionMode == EditTilesSubMenu.TileSelectionMode.SELECT)
        {
            if (raycastTile.CurrentState == Tile.State.DISABLED)
                raycastTile.CurrentState = Tile.State.NORMAL;
        }
        else if (tileSelectionMode == EditTilesSubMenu.TileSelectionMode.DESELECT)
        {
            if (raycastTile.CurrentState == Tile.State.NORMAL)
            {
                raycastTile.CurrentState = Tile.State.DISABLED;

                //also destroy any attached bonus to this tile
                if (raycastTile.AttachedBonus != null)
                {
                    TileRenderer tileRenderer = GameController.GetInstance().m_floor.GetRendererForTile(raycastTile);
                    tileRenderer.DestroyBonusObject();
                    raycastTile.AttachedBonus = null;
                }
            }
        }

        return true;
    }

    private Tile RayCastFloor()
    {
        //Build a ray starting from camera near clip plane mouse world space position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        Plane floorPlane = new Plane(Vector3.up, GameController.GetInstance().m_floor.transform.position);
        if (floorPlane.Raycast(ray, out rayDistance))
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