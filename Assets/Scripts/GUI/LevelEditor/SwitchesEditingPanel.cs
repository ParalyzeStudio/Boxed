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
    public Button m_editSwitchButton;
    public Button m_editTilesButton;

    public bool m_editingSwitch { get; set; }
    private bool m_editingSwitchTile;

    public void Start()
    {
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
            if (m_switches[i] == item)
                item.Select();
            else
                m_switches[i].Deselect();
        }
    }

    public void OnClickAdd()
    {
        if (m_switches == null)
            m_switches = new List<SwitchItem>();

        SwitchItem newItem = Instantiate(m_switchItemPfb);        
        newItem.Init(this, m_switches.Count + 1);
        m_switches.Add(newItem);

        newItem.transform.SetParent(m_switchList.transform, false);
    }

    public void OnClickEdit()
    {
        m_switchList.gameObject.SetActive(false);
        m_switchListButtons.gameObject.SetActive(false);
        m_switchEditButtons.gameObject.SetActive(true);

        m_editingSwitch = true;
        InvalidateSwitchEditingButtons();
    }

    public void OnClickRemove()
    {
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
        base.OnClickValidate();
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
            m_removeItemButton.gameObject.SetActive(false);
        else
            m_removeItemButton.gameObject.SetActive(true);
    }
}
