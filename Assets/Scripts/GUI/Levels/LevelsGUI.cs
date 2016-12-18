using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsGUI : BaseGUI
{
    private const int NUM_SLOTS = 15;

    private const float SLOTS_HORIZONTAL_SPACING = 50;
    private const float SLOTS_VERTICAL_SPACING = 100;

    public GameObject[] m_lines;
    public Text m_levelSlotNumberPfb;
    public LevelSlot m_levelSlotPfb;

    //private LevelSlot[] m_slots;
    //private Text[] m_slotNumbers;
    //private LevelSlot m_clickedSlot;
    //private GameObject m_slotsHolder;
    //private bool m_slotsRendered;

    //public GameObject m_slotNumbersHolder;
    public Text m_chapterNumberText;
    public Button m_prevChapterBtn;
    public Button m_nextChapterBtn;

    public int m_chapterNumber { get; set; }
    
    public override void Show()
    {
        SetChapterNumber(1);

        //m_slotsRendered = false;
        //m_slotNumbers = GetSlotNumbers();

        BuildSlots();

        base.Show();
    }

    private void BuildSlots()
    {
        int numSlotsPerLine = 5;

        for (int i = 0; i != NUM_SLOTS; i++)
        {
            LevelSlot slot = Instantiate(m_levelSlotPfb);
            slot.Init(this, i);
            slot.transform.SetParent(m_lines[i / numSlotsPerLine].transform, false);
        }

        //m_slotsHolder = new GameObject("SlotsHolder");

        //ThemeManager.Theme currentTheme = GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme();

        //m_slots = new LevelSlot[NUM_SLOTS];
        //for (int i = 0; i != NUM_SLOTS; i++)
        //{
        //    LevelSlot slot = Instantiate(m_levelSlotPfb);
        //    slot.Init(this, i);

        //    Vector3 worldPosition = GetWorldPositionForSlot(m_slotNumbers[i]);
        //    slot.transform.parent = m_slotsHolder.transform;
        //    slot.transform.position = worldPosition;
        //    m_slots[i] = slot;
        //}

        //m_slotsRendered = true;
    }

    //private Text[] GetSlotNumbers()
    //{
    //    List<Text> slotNumbers = new List<Text>();
    //    int numLines = 3;
    //    for (int i = 0; i != numLines; i++)
    //    {
    //        Text[] slotNumbersPerLine = m_lines[i].GetComponentsInChildren<Text>();
    //        slotNumbers.AddRange(slotNumbersPerLine);
    //    }

    //    return slotNumbers.ToArray();
    //}

    //private void RenderSlots()
    //{
    //    m_slotsHolder = new GameObject("SlotsHolder");

    //    ThemeManager.Theme currentTheme = GameController.GetInstance().GetComponent<ThemeManager>().GetSelectedTheme();

    //    m_slots = new LevelSlot[NUM_SLOTS];
    //    for (int i = 0; i != NUM_SLOTS; i++)
    //    {
    //        LevelSlot slot = Instantiate(m_levelSlotPfb);
    //        slot.Init(this, i);

    //        Vector3 worldPosition = GetWorldPositionForSlot(m_slotNumbers[i]);
    //        slot.transform.parent = m_slotsHolder.transform;
    //        slot.transform.position = worldPosition;
    //        m_slots[i] = slot;
    //    }

    //    m_slotsRendered = true;
    //}

    public void SetChapterNumber(int chapterNumber)
    {
        m_chapterNumber = chapterNumber;
        m_chapterNumberText.text = "Chapter " + chapterNumber.ToString();

        if (chapterNumber == 1)
        {
            DisableChapterButton(false);
            EnableChapterButton(true);
        }
        else if (chapterNumber == LevelManager.NUM_CHAPTERS)
        {
            DisableChapterButton(true);
            EnableChapterButton(false);
        }
        else
        {
            EnableChapterButton(true);
            EnableChapterButton(false);
        }
    }
    
    //private void StartLevel()
    //{
    //    if (m_clickedSlot.m_level != null)
    //    {
    //        GameController.GetInstance().StartGameForLevel(m_clickedSlot.m_level);
    //        //GameController.GetInstance().GetComponent<GUIManager>().DisplayGameGUIForLevel(level);
    //    }
    //}

    //private void InvalidateLevelsOnSlots()
    //{
    //    for (int i = 0; i != m_slots.Length;i++)
    //    {
    //        m_slots[i].InvalidateLevel();
    //    }
    //}

    //private void DestroySlots()
    //{
    //    Destroy(m_slotsHolder.gameObject);
    //}

    //private Vector3 GetWorldPositionForSlot(Text slotNumber)
    //{
    //    Vector3 worldPosition;
    //    Vector3 slotNumberPosition = slotNumber.transform.position - new Vector3(0, 7.5f, 0); //need to offset the tile a bit so the number is centered inside the tile top face
    //    RectTransformUtility.ScreenPointToWorldPointInRectangle(slotNumber.rectTransform, slotNumberPosition, Camera.main, out worldPosition);
    //    return worldPosition;
    //}

    private void EnableChapterButton(bool bNextButton)
    {
        Image icon;
        Button button;
        if (bNextButton)
        {
            button = m_nextChapterBtn;
            icon = m_nextChapterBtn.GetComponent<Image>();
        }
        else
        {
            button = m_prevChapterBtn;
            icon = m_prevChapterBtn.GetComponent<Image>();
        }
        button.interactable = true;
        Color oldColor = icon.color;
        icon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1);
    }
    
    private void DisableChapterButton(bool bNextButton)
    {
        Image icon;
        Button button;
        if (bNextButton)
        {
            button = m_nextChapterBtn;
            icon = m_nextChapterBtn.GetComponent<Image>();
        }
        else
        {
            button = m_prevChapterBtn;
            icon = m_prevChapterBtn.GetComponent<Image>();
        }
        button.interactable = false;
        Color oldColor = icon.color;
        icon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.4f);
    }

    //public void OnSlotClick(LevelSlot slot)
    //{
    //    if (m_clickedSlot == null)
    //    {
    //        m_clickedSlot = slot;

    //        Dismiss(true);

    //        CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();
    //        callFuncHandler.AddCallFuncInstance(DestroySlots, 0.5f);
    //        callFuncHandler.AddCallFuncInstance(StartLevel, 0.5f);
    //    }
    //}

    public void OnClickBack()
    {
        Dismiss(true);
        //GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(DestroySlots, 0.5f);
        GameController.GetInstance().GetComponent<CallFuncHandler>().AddCallFuncInstance(GameController.GetInstance().StartMainMenu, 0.5f);
    }

    public void OnClickPreviousChapter()
    {
        if (m_chapterNumber > 1)
        {
            SetChapterNumber(m_chapterNumber - 1);
            OnClickChapterButton();

        }
    }

    public void OnClickNextChapter()
    {
        if (m_chapterNumber < LevelManager.NUM_CHAPTERS)
        {
            SetChapterNumber(m_chapterNumber + 1);
            OnClickChapterButton();
        }
    }

    private void OnClickChapterButton()
    {
        //CanvasGroupFade fadeAnimator = m_slotNumbersHolder.GetComponent<CanvasGroupFade>();
        //fadeAnimator.FadeOut();

        ////Fade in new slots after a certain delay
        CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();
        //callFuncHandler.AddCallFuncInstance(fadeAnimator.FadeIn, 0.5f);

        //Theme for new chapter
        ThemeManager themeManager = GameController.GetInstance().GetComponent<ThemeManager>();
        ThemeManager.Theme nextTheme = themeManager.m_themes[m_chapterNumber - 1];
        themeManager.m_selectedThemeIndex = m_chapterNumber - 1;

        Color overlayTopColor = nextTheme.m_backgroundGradientTopColor;
        Color overlayBottomColor = nextTheme.m_backgroundGradientBottomColor;

        //background
        GUIManager guiManager = GameController.GetInstance().GetComponent<GUIManager>();
        guiManager.m_background.ChangeColorsTo(overlayTopColor, overlayBottomColor, 0.5f);

        //overlay        
        overlayTopColor.a = 0;
        overlayBottomColor.a = 0;
        guiManager.m_overlay.m_topColor = overlayTopColor;
        guiManager.m_overlay.m_bottomColor = overlayBottomColor;
        guiManager.m_overlay.InvalidateColors();

        //Update levels on each slot
        //InvalidateLevelsOnSlots();
    }

    //public void ProcessClickOnSlots(Vector2 clickLocation)
    //{
    //    for (int i = 0; i != m_slots.Length; i++)
    //    {
    //        if (m_slots[i].ContainsClickAsButton(clickLocation))
    //        {
    //            m_slots[i].OnClick();
    //            return;
    //        }
    //    }
    //}

    //public void Update()
    //{
    //    if (!m_slotsRendered && m_slotNumbers != null)
    //    {
    //        Vector3 slotNumberPrefabPosition = m_levelSlotNumberPfb.transform.position;
    //        Vector3 firstSlotNumberPosition = m_slotNumbers[0].transform.position;

    //        if (slotNumberPrefabPosition != firstSlotNumberPosition) //slot positions have been updated
    //            RenderSlots();
    //    }
    //}
}
