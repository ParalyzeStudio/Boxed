using UnityEngine;
using UnityEngine.UI;

public class ChapterLock : MonoBehaviour
{
    public GameWindowElement m_mainContent;
    public GameWindowElement m_confirmPurchaseContent;
    public Text m_value;

    public void InvalidateContent()
    {
        PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        int chapterCost = (pDataManager.GetCurrentChapterIndex() + 1) * 300;
        m_value.text = chapterCost.ToString() + " CREDITS";
    }

    public void OnClickUnlock()
    {
        m_mainContent.gameObject.SetActive(false);
        m_confirmPurchaseContent.gameObject.SetActive(true);
    }

    public void OnClickConfirm()
    {
        PersistentDataManager pDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        pDataManager.SetLastUnlockedChapterIndex(pDataManager.GetCurrentChapterIndex());

        ((LevelsGUI)GameController.GetInstance().GetGUIManager().m_currentGUI).OnChapterUnlocked();

        Dismiss();
    }

    public void OnClickCancelConfirm()
    {
        m_mainContent.gameObject.SetActive(true);
        m_confirmPurchaseContent.gameObject.SetActive(false);
    }

    public void Dismiss()
    {
        Destroy(this.gameObject);
    }
}
