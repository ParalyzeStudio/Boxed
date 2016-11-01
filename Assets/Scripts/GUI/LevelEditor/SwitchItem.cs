using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchItem : MonoBehaviour
{
    private SwitchTile m_switchTile;
    public SwitchTile SwitchTile
    {
        get
        {
            return m_switchTile;
        }
    }

    private List<TriggeredTile> m_triggeredTiles;

    public Color m_backgroundNormalColor;
    public Color m_backgroundSelectedColor;
    public Color m_numberTextNormalColor;
    public Color m_numberTextSelectedColor;

    public int m_number { get; set; }

    private SwitchesEditingPanel m_parentPanel;

    public void Init(SwitchesEditingPanel parentPanel, int number)
    {
        m_parentPanel = parentPanel;
        m_number = number;
        if (m_triggeredTiles == null)
            m_triggeredTiles = new List<TriggeredTile>();

        this.GetComponentInChildren<Text>().text = number.ToString();

        Deselect();
    }

    public void OnClick()
    {
        m_parentPanel.OnSwitchItemClick(this);
    }

    public void Select()
    {
        Image bg = this.GetComponent<Image>();
        Text numberText = this.GetComponentInChildren<Text>();

        //bg.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_backgroundSelectedColor, 0.5f) : m_backgroundSelectedColor;
        //numberText.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_numberTextSelectedColor, 0.5f) : m_numberTextSelectedColor;
        bg.color = m_backgroundSelectedColor;
        numberText.color = m_numberTextSelectedColor;

        if (m_switchTile != null)
            m_switchTile.OnSelect();
    }

    public void Deselect()
    {
        Image bg = this.GetComponent<Image>();
        Text numberText = this.GetComponentInChildren<Text>();

        //bg.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_backgroundNormalColor, 0.5f) : m_backgroundNormalColor;
        //numberText.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_numberTextNormalColor, 0.5f) : m_numberTextNormalColor;
        bg.color = m_backgroundNormalColor;
        numberText.color = m_numberTextNormalColor;

        if (m_switchTile != null)
            m_switchTile.OnDeselect();
    }

    public void Remove()
    {

    }

    //public void AdaptTilesStatesOnToggle()
    //{
    //    if (m_switchTile != null)
    //    {
    //        m_switchTile.m_isOn = m_parentPanel.m_switchToggle.isOn;
    //    }

    //    for (int i = 0; i != m_switchTile.m_triggeredTiles.Count; i++)
    //    {
    //        m_switchTile.m_triggeredTiles[i].CurrentState = (m_parentPanel.m_switchToggle.isOn) ? Tile.State.TRIGGERED_BY_SWITCH : Tile.State.BLOCKED;
    //    }
    //}

    public void SetSwitchTile(SwitchTile tile)
    {
        if (m_switchTile == null)
        {
            if (tile != null)
            {
                m_parentPanel.m_parentMenu.m_parentEditor.m_editedLevel.m_floor.InsertTile(tile);

                //invalidate the tile on the renderer
                GameController.GetInstance().m_floor.ReplaceTileOnRenderer(tile);
            }
        }
        else
        {
            //remove the previous switch tile and replace it with a normal tile
            Tile normalTile = new Tile(m_switchTile);
            normalTile.CurrentState = Tile.State.NORMAL;
            m_parentPanel.m_parentMenu.m_parentEditor.m_editedLevel.m_floor.InsertTile(normalTile);

            //invalidate the tile on the renderer
            GameController.GetInstance().m_floor.ReplaceTileOnRenderer(normalTile);

            if (tile != null)
            {
                m_switchTile.m_isOn = m_parentPanel.m_switchToggle.isOn;

                m_parentPanel.m_parentMenu.m_parentEditor.m_editedLevel.m_floor.InsertTile(tile);

                GameController.GetInstance().m_floor.ReplaceTileOnRenderer(tile);
            }
        }

        m_switchTile = tile;
    }

    public void ToggleTiles()
    {
        m_switchTile.m_isOn = !m_switchTile.m_isOn;

        for (int i = 0; i != m_triggeredTiles.Count; i++)
        {
            m_triggeredTiles[i].m_isLiftUp = !m_triggeredTiles[i].m_isLiftUp;
            m_triggeredTiles[i].m_tileStateDirty = true;
        }
    }

    public void AddTriggeredTile(TriggeredTile tile)
    {
        if (m_switchTile != null && tile.m_columnIndex == m_switchTile.m_columnIndex && tile.m_lineIndex == m_switchTile.m_lineIndex) //not the same tile as switch tile
            return;

        m_parentPanel.m_parentMenu.m_parentEditor.m_editedLevel.m_floor.InsertTile(tile);
        GameController.GetInstance().m_floor.ReplaceTileOnRenderer(tile);

        m_triggeredTiles.Add(tile);
        Debug.Log("m_parentPanel.m_triggeredTileLiftUpState:" + m_parentPanel.m_triggeredTileLiftUpState);
        tile.m_isLiftUp = m_parentPanel.m_triggeredTileLiftUpState;
        tile.m_tileStateDirty = true;
    }

    public void RemoveTriggeredTile(TriggeredTile tile)
    {
        if (m_triggeredTiles.Contains(tile))
        {
            m_triggeredTiles.Remove(tile);

            Tile normalTile = new Tile(tile);
            normalTile.CurrentState = Tile.State.NORMAL;

            m_parentPanel.m_parentMenu.m_parentEditor.m_editedLevel.m_floor.InsertTile(normalTile);
            GameController.GetInstance().m_floor.ReplaceTileOnRenderer(normalTile);
        }
    }

    public void SaveSwitchTile()
    {
        if (m_switchTile != null)
        {
            m_switchTile.m_isOn = m_parentPanel.m_switchToggle.isOn;
            m_switchTile.m_triggeredTiles = m_triggeredTiles; //set item triggered tiles as switch triggered tiles
        }
    }
}
