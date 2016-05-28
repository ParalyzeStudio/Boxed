using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{    
    public Text m_title;
    public int m_index { get; set; } //the index of this item in the list

    private void Select()
    {
        Image background = this.GetComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f); //dark grey
    }

    public void Deselect()
    {
        Image background = this.GetComponent<Image>();
        background.color = Color.black;
    }

    public void OnClick()
    {
        Debug.Log("Onclick list item:" + m_index);
        Select();
    }
}
