using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsGUI : BaseGUI
{
    private const int NUM_SLOTS = 15;

    private const float SLOTS_HORIZONTAL_SPACING = 50;
    private const float SLOTS_VERTICAL_SPACING = 100;
    private const float SLOT_ANIMATION_DURATION = 0.3f;
    private const float SLOT_ANIMATION_SPACING = 0.04f;

    public GameObject[] m_lines;
    public Text m_levelSlotNumberPfb;
    public LevelSlot m_levelSlotPfb;

    private LevelSlot[] m_slots;

    public GUIElementAnimator m_backBtn;
    public GUIElementAnimator m_chapterSelection;

    public Text m_chapterNumberText;
    public Button m_prevChapterBtn;
    public Button m_nextChapterBtn;

    //chapter lock
    public ChapterLock m_chapterLockPfb;
    private ChapterLock m_chapterLock;
    
    public override void Show()
    {
        SetChapterNumber(GameController.GetInstance().GetPersistentDataManager().GetCurrentChapterIndex() + 1);

        //Slots
        StartCoroutine(BuildSlots());

        //back button
        m_backBtn.SyncPositionFromTransform();
        m_backBtn.SetPosition(m_backBtn.GetPosition() - new Vector3(150, 0, 0));
        m_backBtn.TranslateBy(new Vector3(150, 0, 0), 0.4f);
        m_backBtn.SetOpacity(0);
        m_backBtn.FadeTo(1.0f, 0.5f);

        //animate also the chapter selection block
        m_chapterSelection.SyncPositionFromTransform();
        m_chapterSelection.SetPosition(m_chapterSelection.GetPosition() - new Vector3(0, 200, 0));
        m_chapterSelection.TranslateBy(new Vector3(0, 200, 0), 0.5f);
        m_chapterSelection.SetOpacity(0);
        m_chapterSelection.FadeTo(1.0f, 0.5f);

        //lock
        ShowOrHideLock();
    }

    private IEnumerator BuildSlots()
    {
        int numSlotsPerLine = 5;

        m_slots = new LevelSlot[NUM_SLOTS];
        for (int i = 0; i != NUM_SLOTS; i++)
        {
            m_slots[i] = Instantiate(m_levelSlotPfb);
            m_slots[i].Init(this, i);
            GUIElementAnimator slotAnimator = m_slots[i].GetComponent<GUIElementAnimator>();
            slotAnimator.SetScale(Vector3.zero);
            slotAnimator.SetOpacity(0);
            m_slots[i].transform.SetParent(m_lines[i / numSlotsPerLine].transform, false);
            yield return new WaitForEndOfFrame(); //build only one slot per frame to avoid small lagging
        }

        OnSlotsBuilt();
    }

    private IEnumerator ShowSlots()
    {
        for (int i = 0; i != NUM_SLOTS; i++)
        {
            //animate every slot by scaling them up with increasing delay
            GUIElementAnimator slotAnimator = m_slots[i].GetComponent<GUIElementAnimator>();
            slotAnimator.ScaleTo(Vector3.one, SLOT_ANIMATION_DURATION);
            yield return new WaitForSeconds(SLOT_ANIMATION_SPACING);
        }

        OnSlotsShown();
    }

    private IEnumerator DismissSlots()
    {
        for (int i = 0; i != NUM_SLOTS; i++)
        {
            //animate every slot by scaling them up with increasing delay
            GUIElementAnimator slotAnimator = m_slots[i].GetComponent<GUIElementAnimator>();
            slotAnimator.ScaleTo(Vector3.zero, SLOT_ANIMATION_DURATION);
            slotAnimator.FadeTo(0, SLOT_ANIMATION_DURATION);
            yield return new WaitForSeconds(i == NUM_SLOTS - 1 ? SLOT_ANIMATION_DURATION : SLOT_ANIMATION_SPACING); //on the last loop wait for the last slot animation to finish
        }

        OnSlotsDismissed();
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
        GameController.GetInstance().GetPersistentDataManager().SetCurrentChapterIndex(chapterNumber - 1);
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



    /**
    * Callback when slots have been created
    **/
    private void OnSlotsBuilt()
    {
        StartCoroutine(ShowSlots());
    }

    /**
    * Callback when slots entering animation has finished
    **/
    private void OnSlotsShown()
    {
        
    }

    /**
    * Callback when slots exiting animation has finished
    **/
    private void OnSlotsDismissed()
    {
        StartCoroutine(GameController.GetInstance().StartMainMenu());
    }

    public void OnClickBack()
    {
        GameController.GetInstance().GetPersistentDataManager().SavePrefs();

        StartCoroutine(DismissSlots());

        //back button
        m_backBtn.TranslateBy(new Vector3(-150, 0, 0), 0.4f);
        m_backBtn.FadeTo(0.0f, 0.5f);

        //chapter selection
        m_chapterSelection.TranslateBy(new Vector3(0, -200, 0), 0.5f);
        m_chapterSelection.FadeTo(0.0f, 0.5f);
    }

    public void OnClickPreviousChapter()
    {
        int chapterNumber = GameController.GetInstance().GetPersistentDataManager().GetCurrentChapterIndex() + 1;
        if (chapterNumber > 1)
        {
            SetChapterNumber(chapterNumber - 1);
            OnClickChapterButton();

        }
    }

    public void OnClickNextChapter()
    {
        int chapterNumber = GameController.GetInstance().GetPersistentDataManager().GetCurrentChapterIndex() + 1;
        if (chapterNumber < LevelManager.NUM_CHAPTERS)
        {
            SetChapterNumber(chapterNumber + 1);
            OnClickChapterButton();
        }
    }

    private void OnClickChapterButton()
    {
        //CanvasGroupFade fadeAnimator = m_slotNumbersHolder.GetComponent<CanvasGroupFade>();
        //fadeAnimator.FadeOut();

        ////Fade in new slots after a certain delay
        //CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();
        //callFuncHandler.AddCallFuncInstance(fadeAnimator.FadeIn, 0.5f);

        //Theme for new chapter
        ThemeManager themeManager = GameController.GetInstance().GetComponent<ThemeManager>();
        ThemeManager.Theme theme = themeManager.GetSelectedTheme();

        Color overlayTopColor = theme.m_backgroundGradientTopColor;
        Color overlayBottomColor = theme.m_backgroundGradientBottomColor;

        //background
        GUIManager guiManager = GameController.GetInstance().GetGUIManager();
        guiManager.m_background.ChangeColorsTo(overlayTopColor, overlayBottomColor, 0.5f);

        //overlay     
        if (guiManager.m_overlay != null)
        {
            overlayTopColor.a = 0;
            overlayBottomColor.a = 0;
            guiManager.m_overlay.m_topColor = overlayTopColor;
            guiManager.m_overlay.m_bottomColor = overlayBottomColor;
            guiManager.m_overlay.InvalidateColors();
        }

        //slots
        for (int i = 0; i != m_slots.Length; i++)
        {
            m_slots[i].InvalidateSprite();
        }

        //show lock window if necessary
        ShowOrHideLock();

        //Update levels on each slot
        //InvalidateLevelsOnSlots();
    }

    /**
    * Display a lock if the chapter currently displayed has not been purchased yet
    **/
    private void ShowOrHideLock()
    {
        PersistentDataManager pDataManager = GameController.GetInstance().GetPersistentDataManager();
        if (pDataManager.GetCurrentChapterIndex() > pDataManager.GetLastUnlockedChapterIndex())
        {
            if (m_chapterLock == null)
            {
                m_chapterLock = Instantiate(m_chapterLockPfb);
                m_chapterLock.transform.SetParent(GameController.GetInstance().GetGUIManager().m_canvas.transform, false);
            }

            m_chapterLock.InvalidateContent();
        }
        else
        {
            if (m_chapterLock != null)
                DismissChapterLock();
        }
    }

    private void DismissChapterLock()
    {
        Destroy(m_chapterLock.gameObject);
        m_chapterLock = null;
    }

    public void OnChapterUnlocked()
    {
        DismissChapterLock();
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
