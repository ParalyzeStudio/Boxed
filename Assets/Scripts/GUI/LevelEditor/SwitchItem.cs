using UnityEngine;
using UnityEngine.UI;

public class SwitchItem : MonoBehaviour
{
    public Switch m_switch { get; set; }

    public Color m_backgroundNormalColor;
    public Color m_backgroundSelectedColor;
    public Color m_numberTextNormalColor;
    public Color m_numberTextSelectedColor;

    public int m_number { get; set; }

    private SwitchesEditingPanel m_parentPanel;

    //public bool m_isUnderConstruction { get; set; }

    public void Init(SwitchesEditingPanel parentPanel, int number, Switch vSwitch)
    {
        m_parentPanel = parentPanel;
        m_number = number;

        this.GetComponentInChildren<Text>().text = number.ToString();

        //m_isUnderConstruction = true;

        m_switch = vSwitch;

        Deselect();
    }

    public void OnClick()
    {
        m_parentPanel.OnSwitchClick(this);
    }

    public void Select()
    {
        Image bg = this.GetComponent<Image>();
        Text numberText = this.GetComponentInChildren<Text>();

        //bg.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_backgroundSelectedColor, 0.5f) : m_backgroundSelectedColor;
        //numberText.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_numberTextSelectedColor, 0.5f) : m_numberTextSelectedColor;
        bg.color = m_backgroundSelectedColor;
        numberText.color = m_numberTextSelectedColor;

        m_switch.OnSelect();
    }

    public void Deselect()
    {
        Image bg = this.GetComponent<Image>();
        Text numberText = this.GetComponentInChildren<Text>();

        //bg.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_backgroundNormalColor, 0.5f) : m_backgroundNormalColor;
        //numberText.color = m_isUnderConstruction ? ColorUtils.FadeColor(m_numberTextNormalColor, 0.5f) : m_numberTextNormalColor;
        bg.color = m_backgroundNormalColor;
        numberText.color = m_numberTextNormalColor;

        m_switch.OnDeselect();
    }
}
