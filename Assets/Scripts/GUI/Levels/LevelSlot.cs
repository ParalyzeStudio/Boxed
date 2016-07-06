using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour
{
    public int m_number { get; set; }

    public void Init(LevelsGUI parentGUI, int number)
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { parentGUI.OnSlotClick(this); });
        SetNumber(number);
    }

    public void SetNumber(int number)
    {
        m_number = number;
        this.GetComponentInChildren<Text>().text = number.ToString();
    }
}
