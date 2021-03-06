﻿using UnityEngine;

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
            LevelEditor levelEditor = (LevelEditor) GameController.GetInstance().GetGUIManager().m_currentGUI;
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
            LevelEditor levelEditor = (LevelEditor)GameController.GetInstance().GetGUIManager().m_currentGUI;
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.TILES_EDITING)
                EditTiles();
        }

        return true;
    }

    protected override void OnClick(Vector2 clickLocation)
    {
        GameController.GameMode gameMode = GameController.GetInstance().m_gameMode;

        if (gameMode == GameController.GameMode.LEVEL_EDITOR)
        {
            LevelEditor levelEditor = (LevelEditor)GameController.GetInstance().GetGUIManager().m_currentGUI;

            //Raycast the tiles to select them
            Tile raycastTile = RayCastFloorForTile();
            if (raycastTile == null)
                return;
            if (levelEditor.m_editingMode == LevelEditor.EditingMode.CHECKPOINTS_EDITING)
            {
                Tile levelStartTile = levelEditor.m_editedLevel.m_floor.GetStartTile();
                Tile levelFinishTile = levelEditor.m_editedLevel.m_floor.GetFinishTile();

                if (raycastTile.CurrentState == Tile.State.NORMAL || raycastTile.CurrentState == Tile.State.BLOCKED)
                {   
                    if (levelStartTile == null) //there is no other tile that has been marked as start tile
                    {
                        raycastTile.CurrentState = Tile.State.START;
                    }
                    else if (levelFinishTile == null) //there is no other tile that has been marked as finish tile
                    {
                        raycastTile.CurrentState = Tile.State.FINISH;
                    }
                }
                else if (raycastTile.CurrentState == Tile.State.START)
                {
                    if (levelFinishTile == null)
                    {
                        raycastTile.CurrentState = Tile.State.FINISH;
                    }
                    else
                    {
                        raycastTile.CurrentState = Tile.State.NORMAL;
                    }
                }
                else if (raycastTile.CurrentState == Tile.State.FINISH)
                {
                    raycastTile.CurrentState = Tile.State.NORMAL;
                }
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.SWITCHES_EDITING)
            {
                SwitchesEditingPanel switchEditingPanel = (SwitchesEditingPanel)levelEditor.m_activeEditingPanel;                

                if (switchEditingPanel.m_editingSwitch)
                {
                    SwitchItem editedSwitchItem = switchEditingPanel.m_selectedItem;

                    if (switchEditingPanel.m_editingSwitchTile)
                    {
                        if (raycastTile.CurrentState == Tile.State.NORMAL)
                        {
                            SwitchTile switchTile = new SwitchTile(raycastTile, null);
                            editedSwitchItem.SetSwitchTile(switchTile);


                            ////remove the previous switch tile and replace it with a normal tile
                            //if (editedSwitchItem.SwitchTile != null)
                            //{
                            //    Tile normalTile = new Tile(editedSwitchItem.SwitchTile);
                            //    normalTile.CurrentState = Tile.State.NORMAL;
                            //    levelEditor.m_editedLevel.m_floor.InsertTile(normalTile);

                            //    //invalidate the tile on the renderer
                            //    GameController.GetInstance().m_floor.ReplaceTileOnRenderer(editedSwitchItem.SwitchTile, normalTile);
                            //}

                            //Debug.Log("raycastTile:" + levelEditor.m_editedLevel.m_floor.GetTileIndex(raycastTile));

                            ////build a new tile and put it in the floor
                            //SwitchTile switchTile = new SwitchTile(raycastTile, null);
                            //levelEditor.m_editedLevel.m_floor.InsertTile(switchTile);
                            //editedSwitchItem.SetSwitchTile(switchTile);

                            ////invalidate the tile on the renderer
                            //GameController.GetInstance().m_floor.ReplaceTileOnRenderer(raycastTile, switchTile);
                        }
                        else if (raycastTile.CurrentState == Tile.State.SWITCH)
                        {
                            editedSwitchItem.SetSwitchTile(null);
                        }
                    }
                    else //editing tile that are triggered by the switch tile
                    {
                        if (raycastTile.CurrentState == Tile.State.NORMAL)
                        {
                            TriggeredTile triggeredTile = new TriggeredTile(raycastTile);
                            editedSwitchItem.AddTriggeredTile(triggeredTile);
                        }
                        else if (raycastTile.CurrentState == Tile.State.TRIGGERED_BY_SWITCH)
                        {
                            editedSwitchItem.RemoveTriggeredTile((TriggeredTile) raycastTile);
                        }
                    }
                }
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.BONUSES_EDITING)
            {
                if (raycastTile.AttachedBonus != null)
                {
                    FloorRenderer floorRenderer = GameController.GetInstance().m_floorRenderer;
                    floorRenderer.GetRendererForTile(raycastTile).DestroyBonusObject();
                    raycastTile.AttachedBonus = null;
                }
                else
                {
                    Bonus bonus = new Bonus(); //for the moment set an empty object as a bonus
                    FloorRenderer floorRenderer = GameController.GetInstance().m_floorRenderer;
                    floorRenderer.GetRendererForTile(raycastTile).BuildBonusObject();
                    raycastTile.AttachedBonus = bonus;
                }
            }
            else if (levelEditor.m_editingMode == LevelEditor.EditingMode.ICE_TILES_EDITING)
            {
                if (raycastTile.CurrentState == Tile.State.NORMAL)
                {
                    IceTile switchTile = new IceTile(raycastTile, 1);
                    GameController.GetInstance().m_floorRenderer.m_floorData.InsertTile(switchTile);
                    GameController.GetInstance().m_floorRenderer.ReplaceTileOnRenderer(switchTile);
                }
                else if (raycastTile.CurrentState == Tile.State.ICE)
                {
                    Tile normalTile = new Tile(raycastTile);
                    normalTile.CurrentState = Tile.State.NORMAL;
                    GameController.GetInstance().m_floorRenderer.m_floorData.InsertTile(normalTile);
                    GameController.GetInstance().m_floorRenderer.ReplaceTileOnRenderer(normalTile);
                }
            }
        }
    }

    /**
    * Apply the action of editing tiles to the currently raycast tile
    **/
    private bool EditTiles()
    {
        Tile raycastTile = RayCastFloorForTile();
        if (raycastTile == null || raycastTile == m_lastRaycastTile)
            return false;
        m_lastRaycastTile = raycastTile;

        LevelEditor levelEditor = (LevelEditor)GameController.GetInstance().GetGUIManager().m_currentGUI;
        TileEditingPanel tileEditingPanel = (TileEditingPanel) levelEditor.m_activeEditingPanel;
        TileEditingPanel.TileSelectionMode tileSelectionMode = tileEditingPanel.m_tileSelectionMode;

        if (tileSelectionMode == TileEditingPanel.TileSelectionMode.SELECT)
        {
            if (raycastTile.CurrentState == Tile.State.DISABLED || raycastTile.CurrentState == Tile.State.BLOCKED)
                raycastTile.CurrentState = Tile.State.NORMAL;
        }
        else if (tileSelectionMode == TileEditingPanel.TileSelectionMode.DESELECT)
        {
            if (raycastTile.CurrentState == Tile.State.NORMAL || raycastTile.CurrentState == Tile.State.BLOCKED)
            {
                raycastTile.CurrentState = Tile.State.DISABLED;

                //also destroy any attached bonus to this tile
                if (raycastTile.AttachedBonus != null)
                {
                    TileRenderer tileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(raycastTile);
                    tileRenderer.DestroyBonusObject();
                    raycastTile.AttachedBonus = null;
                }
            }
        }

        return true;
    }

    private Tile RayCastFloorForTile()
    {
        Vector3 raycastPoint;
        if (RaycastFloor(out raycastPoint))
        {
            Vector2 xzRaycastPoint = Geometry.RemoveYComponent(raycastPoint);

            //Find the tile which contain the intersection point
            Floor floor = GameController.GetInstance().m_floorRenderer.m_floorData;
            for (int i = 0; i != floor.Tiles.Length; i++)
            {
                Tile tile = floor.Tiles[i];
                if (tile.ContainsXZPoint(xzRaycastPoint))
                {
                    return tile;
                }
            }
        }

        return null;
    }

    private bool RaycastFloor(out Vector3 raycastPoint)
    {
        //Build a ray starting from camera near clip plane mouse world space position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        Vector3 planePosition = GameController.GetInstance().m_floorRenderer.transform.position + new Vector3(0, 0.5f * Tile.TILE_HEIGHT, 0);
        Plane floorPlane = new Plane(Vector3.up, planePosition);

        if (floorPlane.Raycast(ray, out rayDistance))
        {
            raycastPoint = ray.GetPoint(rayDistance);
            return true;
        }
        else
        {
            raycastPoint = Vector3.zero;
            return false;
        }
    }
}