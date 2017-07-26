using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour
{
    public int m_index { get; set; }
    public Level m_level { get; set; }
    public Text m_levelNumberText;

    private LevelsGUI m_parentGUI;

    private bool m_interactable;

    public void Init(LevelsGUI parentGUI, int index)
    {
        m_parentGUI = parentGUI;
        m_index = index;

        InvalidateLevel();
    }

    public void InvalidateLevel()
    {
        int localLevelNumber = m_index + 1;
        int chapterIndex = GameController.GetInstance().GetPersistentDataManager().GetCurrentChapterIndex();
        int absoluteLevelNumber = chapterIndex * LevelManager.NUM_LEVELS_PER_CHAPTER + localLevelNumber;
        m_level = GameController.GetInstance().GetLevelManager().GetLevelForNumber(absoluteLevelNumber);

        m_levelNumberText.text = localLevelNumber.ToString();
        Invalidate();
    }

    private void Disable(bool bNullLevel)
    {
        m_interactable = !bNullLevel;

        //TileColors colors = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme.m_defaultTileColors;

        //if (bNullLevel)
        //{
        //    colors.Darken(0.5f);
        //    colors.Fade(0.5f);
        //    ChangeColorsTo(colors, 0.5f);
        //    m_interactable = false;
        //}
        //else
        //{
        //    colors.Darken(0.35f);
        //    ChangeColorsTo(colors, 0.5f);
        //    m_interactable = true;
        //}
    }

    private void Enable()
    {
        //TileColors colors = GameController.GetInstance().GetComponent<GUIManager>().m_themes.m_currentTheme.m_defaultTileColors;
        //ChangeColorsTo(colors, 0.5f);
        m_interactable = true;
    }

    public void Invalidate()
    {
        if (m_level == null)
            Disable(true);
        else
        {
            LevelData levelData = GameController.GetInstance().GetLevelManager().GetLevelDataForLevel(m_level);
            if (levelData.m_movesCount == 0)
                Disable(false);
            else
                Enable();
        }
    }    

    public void OnClick()
    {
        if (m_interactable)
        {
            GameController.GetInstance().GetLevelManager().m_currentLevel = m_level;

            GameController.GetInstance().GetPersistentDataManager().SavePrefs();

            //LevelsGUI levelsGUI = (LevelsGUI)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
            //levelsGUI.OnSlotClick(this);
            
            StartCoroutine(GameController.GetInstance().ShowInterlevelWindowAfterDelay(0, GameController.GameStatus.IDLE));
            //GameController.GetInstance().StartGameForLevel(m_level);
        }
    }
}
