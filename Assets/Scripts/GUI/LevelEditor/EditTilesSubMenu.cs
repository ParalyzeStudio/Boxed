﻿using UnityEngine;
using UnityEngine.UI;

public class EditTilesSubMenu : LevelEditorMenu
{
    public Button m_selectTileButton;
    public Button m_deselectTileButton;

    public enum TileSelectionMode
    {
        SELECT,
        DESELECT
    }

    public TileSelectionMode m_tileSelectionMode { get; set; }

    public void Start()
    {
        EnableButton(m_selectTileButton);
        DisableButton(m_deselectTileButton);
    }

    public void OnClickSelectTileButton()
    {
        Debug.Log("OnClickSelectTileButton");
        m_tileSelectionMode = TileSelectionMode.SELECT;
        EnableButton(m_selectTileButton);
        DisableButton(m_deselectTileButton);
    }

    public void OnClickDeselectTileButton()
    {
        Debug.Log("OnClickDeselectTileButton");
        m_tileSelectionMode = TileSelectionMode.DESELECT;
        DisableButton(m_selectTileButton);
        EnableButton(m_deselectTileButton);
    }

    private void EnableButton(Button button)
    {
        button.GetComponent<Image>().color = Color.white;
    }

    private void DisableButton(Button button)
    {
        button.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    }
}
