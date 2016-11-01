using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchesEditingPanel : ActionPanel
{
    public SwitchItem m_switchItemPfb;
    public GameObject m_switchList;
    public GameObject m_switchListButtons;
    public GameObject m_switchEditButtons;

    private List<SwitchItem> m_switches;
    public SwitchItem m_selectedItem { get; set; }

    public Button m_removeItemButton;
    public Button m_editItemButton;
    public Toggle m_switchToggle;
    public Button m_editSwitchButton;
    public Toggle m_triggeredTileStateToggle;
    public Button m_editTilesButton;

    public bool m_editingSwitch { get; set; }
    public bool m_editingSwitchTile { get; set;}
    public bool m_triggeredTileLiftUpState { get; set; }

    public void Init()
    {
        ClearSwitchItems();
        
        List<SwitchTile> switches = m_parentMenu.m_parentEditor.m_editedLevel.m_floor.GetSwitchTiles();
        if (switches != null)
        {
            for (int i = 0; i != switches.Count; i++)
            {
                SwitchItem item = AddSwitchItem();
                item.SetSwitchTile(switches[i]);
            }
        }

        m_switchList.gameObject.SetActive(true);
        m_switchListButtons.gameObject.SetActive(true);
        m_switchEditButtons.gameObject.SetActive(false);

        m_switchToggle.isOn = true;
        m_triggeredTileStateToggle.isOn = false;
        m_triggeredTileLiftUpState = false;
    }

    public void OnSwitchItemClick(SwitchItem item)
    {
        if (m_selectedItem == item)
            return;

        m_selectedItem = item;

        for (int i = 0; i != m_switches.Count; i++)
        {
            if (m_switches[i] != item)
                m_switches[i].Deselect();
        }
        
        item.Select();
    }

    public void OnClickAdd()
    {
        AddSwitchItem();
    }

    private void ClearSwitchItems()
    {
        if (m_switches != null)
        {
            for (int i = 0; i != m_switches.Count; i++)
            {
                Destroy(m_switches[i].gameObject);
            }
            m_switches.Clear();
        }
    }

    private SwitchItem AddSwitchItem()
    {
        if (m_switches == null)
            m_switches = new List<SwitchItem>();

        SwitchItem newItem = Instantiate(m_switchItemPfb);
        newItem.Init(this, m_switches.Count + 1);
        m_switches.Add(newItem);

        newItem.transform.SetParent(m_switchList.transform, false);

        return newItem;
    }

    public void OnClickEdit()
    {
        m_switchList.gameObject.SetActive(false);
        m_switchListButtons.gameObject.SetActive(false);
        m_switchEditButtons.gameObject.SetActive(true);

        //buttons
        m_editingSwitch = true;
        m_editingSwitchTile = true;
        InvalidateSwitchEditingButtons();

        //toggle
        if (m_selectedItem.SwitchTile == null)
        {
            m_switchToggle.isOn = true;
        }
        else
        {
            m_switchToggle.isOn = m_selectedItem.SwitchTile.m_isOn;
        }
    }

    public void OnClickRemove()
    {
        if (m_selectedItem.SwitchTile != null)
            m_selectedItem.Remove();

        for (int i = 0; i != m_switches.Count; i++)
        {
            if (m_switches[i].m_number > m_selectedItem.m_number)
            {
                m_switches[i].m_number--;
            }
        }

        m_switches.Remove(m_selectedItem);
        Destroy(m_selectedItem.gameObject);
    }

    public override void OnClickValidate()
    {
        if (m_selectedItem != null)
        {
            m_selectedItem.Deselect();
            m_selectedItem = null;
        }

        base.OnClickValidate();
    }

    public void OnToggleTestSwitch(Toggle toggle)
    {
        m_selectedItem.ToggleTiles();
    }

    public void OnToggleTriggeredTileState(Toggle toggle)
    {
        m_triggeredTileLiftUpState = toggle.isOn;
    }

    public void OnClickEditSwitch()
    {
        m_editingSwitchTile = true;
        InvalidateSwitchEditingButtons();
    }

    public void OnClickEditTiles()
    {
        m_editingSwitchTile = false;
        InvalidateSwitchEditingButtons();
    }

    public void OnClickValidateSwitchEditing()
    {
        m_selectedItem.SaveSwitchTile();

        m_switchList.gameObject.SetActive(true);
        m_switchListButtons.gameObject.SetActive(true);
        m_switchEditButtons.gameObject.SetActive(false);
        m_editingSwitch = false;
    }
    
    private void InvalidateSwitchEditingButtons()
    {
        //buttons
        if (m_editingSwitchTile)
        {
            EnableButton(m_editSwitchButton);
            DisableButton(m_editTilesButton);
        }
        else
        {
            DisableButton(m_editSwitchButton);
            EnableButton(m_editTilesButton);
        }
    }

    private void DisableButton(Button button)
    {
        Image bg = button.GetComponent<Image>();
        Text text = button.GetComponentInChildren<Text>();

        bg.color = ColorUtils.FadeColor(bg.color, 0.5f);
        text.color = ColorUtils.FadeColor(text.color, 0.5f);
    }

    private void EnableButton(Button button)
    {
        Image bg = button.GetComponent<Image>();
        Text text = button.GetComponentInChildren<Text>();

        bg.color = ColorUtils.FadeColor(bg.color, 1f);
        text.color = ColorUtils.FadeColor(text.color, 1f);
    }

    public void Update()
    {
        if (m_selectedItem == null)
        {
            m_removeItemButton.gameObject.SetActive(false);
            m_editItemButton.gameObject.SetActive(false);
        }
        else
        {
            m_removeItemButton.gameObject.SetActive(true);
            m_editItemButton.gameObject.SetActive(true);
        }

        if (m_editingSwitch)
        {
            if (m_selectedItem.SwitchTile == null)
                m_switchToggle.interactable = false;
            else
                m_switchToggle.interactable = true;
        }
    }
}
