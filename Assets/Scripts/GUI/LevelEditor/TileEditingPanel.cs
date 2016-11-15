using UnityEngine;
using UnityEngine.UI;

public class TileEditingPanel : ActionPanel
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

    public void OnClickSelect()
    {
        m_tileSelectionMode = TileSelectionMode.SELECT;
        EnableButton(m_selectTileButton);
        DisableButton(m_deselectTileButton);
    }

    public void OnClickDeselect()
    {
        m_tileSelectionMode = TileSelectionMode.DESELECT;
        DisableButton(m_selectTileButton);
        EnableButton(m_deselectTileButton);
    }

    public override void OnClickValidate()
    {
        GameController.GetInstance().m_floorRenderer.m_floorData.AssignBlockedTiles();
        base.OnClickValidate();
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
