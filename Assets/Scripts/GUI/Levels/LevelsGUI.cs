using UnityEngine;
using UnityEngine.UI;

public class LevelsGUI : BaseGUI
{
    private const int NUM_LINES = 3;
    private const int LEVELS_PER_LINE = 5;

    public GameObject[] m_lines;
    public LevelSlot m_levelSlotPfb;

    private LevelSlot m_clickedSlot;

    public override void Show()
    {
        for (int i = 0; i != NUM_LINES; i++)
        {
            PopulateLine(i);
        }

        base.Show();
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
        m_clickedSlot = slot;

        Dismiss();

        CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();
        callFuncHandler.AddCallFuncInstance(StartLevel, 0.5f);
    }

    private void StartLevel()
    {
        Level level = GameController.GetInstance().GetComponent<LevelManager>().GetLevelForNumber(m_clickedSlot.m_number);

        if (level != null)
        {
            GameController.GetInstance().StartGameForLevel(level);
            GameController.GetInstance().GetComponent<GUIManager>().DisplayGameGUIForLevel(level);
        }
    }
}
