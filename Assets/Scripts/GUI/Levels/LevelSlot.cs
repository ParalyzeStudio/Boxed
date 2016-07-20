using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour
{
    public int m_number { get; set; }
    public Level m_level;

    public void Init(LevelsGUI parentGUI, int number)
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { parentGUI.OnSlotClick(this); });
        SetNumber(number);

        m_level = GameController.GetInstance().GetComponent<LevelManager>().GetLevelForNumber(number);
        Invalidate();
    }

    public void SetNumber(int number)
    {
        m_number = number;
    }

    private void Disable()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        GetComponent<Button>().interactable = false;
    }

    private void Enable()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        GetComponent<Button>().interactable = true;
    }

    public void Invalidate()
    {
        this.GetComponentInChildren<Text>().text = m_number.ToString();

        if (m_level == null)
            Disable();
        else
            Enable();
    }
}
