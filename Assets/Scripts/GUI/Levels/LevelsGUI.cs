using UnityEngine;
using UnityEngine.UI;

public class LevelsGUI : MonoBehaviour
{
    private const int NUM_LINES = 3;
    private const int LEVELS_PER_LINE = 5;

    public GameObject[] m_lines;
    public LevelSlot m_levelSlotPfb;

    public void Start()
    {
        for (int i = 0; i != NUM_LINES; i++)
        {
            PopulateLine(i);
        }
    }

    private void PopulateLine(int lineIdx)
    {
        for (int j = 0; j != 5; j++)
        {
            LevelSlot slot = Instantiate(m_levelSlotPfb);
            slot.Init(this, lineIdx * LEVELS_PER_LINE + j + 1);
            slot.transform.SetParent(m_lines[lineIdx].transform, false);
        }
    }

    public void OnSlotClick(LevelSlot slot)
    {
        Dismiss();
        GameController.GetInstance().StartGameForLevel(slot.m_number);
    }

    public void Dismiss()
    {
        Destroy(this.gameObject);
    }
}
