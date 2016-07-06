using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{    
    public Text m_title;
    public int m_index { get; set; } //the index of this item in the list
    public Level m_level { get; set; } //the level associated with this item
    private bool m_selected;

    private Color m_selectedColor;
    private Color m_deselectedColor;

    public void Init(int index, Level level)
    {
        m_index = index;
        m_level = level;
        m_selected = false;

        m_deselectedColor = ColorUtils.GetColorFromRGBAVector4(new Color(25,25,25,255));
        m_selectedColor = ColorUtils.LightenColor(m_deselectedColor, 0.2f);

        InvalidateContent();
    }

    public void InvalidateContent()
    {
        Image background = this.GetComponent<Image>();
        if (m_level == null)
        {
            //m_title.text = "Level_" + Level.GetNumberAsString(index + 1);
            m_title.text = "Click to add new level";
            m_title.color = Color.red;
            background.color = m_deselectedColor;
        }
        else
        {
            if (m_level.m_title == null)
                m_title.text = m_level.GetFilename();
            else
                m_title.text = m_level.m_title;
            m_title.color = Color.white;
        }

        if (m_selected)
            background.color = m_selectedColor;
        else
            background.color = m_deselectedColor;
    }

    public void Select()
    {
        m_selected = true;
        InvalidateContent();
    }

    public void Deselect()
    {
        m_selected = false;
        InvalidateContent();
    }

    public void OnClick()
    {
        
    }
}
