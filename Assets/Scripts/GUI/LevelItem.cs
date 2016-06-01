using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{    
    public Text m_title;
    public int m_index { get; set; } //the index of this item in the list
    public Level m_level { get; set; } //the level associated with this item

    public void Init(int index, Level level)
    {
        m_index = index;
        m_level = level;

        InvalidateContent();
    }

    public void InvalidateContent()
    {
        if (m_level == null)
        {
            //m_title.text = "Level_" + Level.GetNumberAsString(index + 1);
            m_title.text = "Click to add new level";
            m_title.color = Color.red;
        }
        else
        {
            if (m_level.m_title == null)
                m_title.text = "NoTitle";
            else
                m_title.text = m_level.m_title;
            m_title.color = Color.white;
        }
    }

    public void OnClick()
    {
        Debug.Log("Onclick list item:" + m_index);
    }
}
