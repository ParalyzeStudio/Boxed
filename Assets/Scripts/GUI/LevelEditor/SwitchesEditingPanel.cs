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
    public Button m_editTilesButton;

    public bool m_editingSwitch { get; set; }
    public bool m_editingSwitchTile { get; set;}

    public void Init()
    {
        ClearSwitchItems();

        Switch[] switches = m_parentMenu.m_parentEditor.m_editedLevel.m_switches;
        if (switches != null)
        {
            for (int i = 0; i != switches.Length; i++)
            {
                AddSwitchItem(switches[i]);
            }
        }

        m_switchList.gameObject.SetActive(true);
        m_switchListButtons.gameObject.SetActive(true);
        m_switchEditButtons.gameObject.SetActive(false);
    }

    public void OnSwitchClick(SwitchItem item)
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
        AddSwitchItem(new Switch());
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

    private void AddSwitchItem(Switch vSwitch)
    {
        if (m_switches == null)
            m_switches = new List<SwitchItem>();

        SwitchItem newItem = Instantiate(m_switchItemPfb);
        newItem.Init(this, m_switches.Count + 1, vSwitch);
        m_switches.Add(newItem);

        newItem.transform.SetParent(m_switchList.transform, false);
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
        m_switchToggle.isOn = m_selectedItem.m_switch.m_isOn;
    }

    public void OnClickRemove()
    {
        m_selectedItem.m_switch.OnRemove();

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
        if (m_switches != null)
        {
            //copy switches to the edited level
            Switch[] switches = new Switch[m_switches.Count];
            for (int i = 0; i != switches.Length; i++)
            {
                switches[i] = m_switches[i].m_switch;
            }

            m_parentMenu.m_parentEditor.m_editedLevel.m_switches = switches;
        }

        if (m_selectedItem != null)
        {
            m_selectedItem.Deselect();
            m_selectedItem = null;
        }

        base.OnClickValidate();
    }

    public void OnToggleSwitchState(Toggle toggle)
    {
        m_selectedItem.m_switch.SetOnOff(toggle.isOn);
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
    }
}
