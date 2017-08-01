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

    //two icons for fade animation
    public Image m_icon1;
    public Image m_icon2;
    private int m_currentIcon; //which one of the two icons is currently active (1 or 2 or 0 if no icon has been set yet as current)
    public Sprite[] m_textures;

    public void Init(LevelsGUI parentGUI, int index)
    {
        m_parentGUI = parentGUI;
        m_index = index;

        InvalidateLevel();

        //set correct texture for chapter index
        m_currentIcon = 0; //no icon at start
        InvalidateSprite();
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

    public void InvalidateSprite()
    {
        int chapterIndex = GameController.GetInstance().GetPersistentDataManager().GetCurrentChapterIndex();
        Sprite updatedSprite = m_textures[chapterIndex];
        
        //fade animation to new icon texture
        if (m_currentIcon == 0)
        {
            m_currentIcon = 1;
            m_icon1.sprite = updatedSprite;

            GUIElementAnimator currentIconAnimator = m_icon1.GetComponent<GUIElementAnimator>();
            currentIconAnimator.SetOpacity(0);
            currentIconAnimator.FadeTo(1.0f, 0.5f);

            m_icon2.GetComponent<GUIElementAnimator>().SetOpacity(0);
        }
        else if (m_currentIcon == 1)
        {
            m_icon2.sprite = updatedSprite;

            GUIElementAnimator currentIconAnimator = m_icon1.GetComponent<GUIElementAnimator>();
            currentIconAnimator.FadeTo(0.0f, 0.5f);

            GUIElementAnimator icon2Animator = m_icon2.GetComponent<GUIElementAnimator>();
            icon2Animator.SetOpacity(0);
            icon2Animator.FadeTo(1.0f, 0.5f);

            m_currentIcon = 2;
        }
        else
        {
            m_icon1.sprite = updatedSprite;

            GUIElementAnimator currentIconAnimator = m_icon2.GetComponent<GUIElementAnimator>();
            currentIconAnimator.FadeTo(0.0f, 0.5f);

            GUIElementAnimator icon1Animator = m_icon1.GetComponent<GUIElementAnimator>();
            icon1Animator.SetOpacity(0);
            icon1Animator.FadeTo(1.0f, 0.5f);

            m_currentIcon = 1;
        }
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
