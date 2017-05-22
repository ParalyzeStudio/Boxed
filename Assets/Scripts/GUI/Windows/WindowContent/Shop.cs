using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shop : GameWindowContent
{
    private PersistentDataManager m_persistentDataManager;

    public override IEnumerator Show(float timeSpacing = DEFAULT_TIME_SPACING)
    {
        GameWindow parentWindow = this.transform.parent.GetComponent<GameWindow>();
        if (parentWindow is MainPageMenu)
        {
            parentWindow.InvalidateCreditsAmount(GetPersistentDataManager().GetCreditsAmount());
            parentWindow.ShowCreditsAmount();
        }
        return base.Show(timeSpacing);
    }

    public override IEnumerator Dismiss(bool bDestroy = false, float timeSpacing = 0.032F)
    {
        GameWindow parentWindow = this.transform.parent.GetComponent<GameWindow>();
        if (parentWindow is MainPageMenu)
            parentWindow.DismissCreditsAmount();
        return base.Dismiss(bDestroy, timeSpacing);
    }

    public void OnClickBuyOption1()
    {

    }

    public void OnClickBuyOption2()
    {

    }

    public void OnClickBuyOption3()
    {

    }

    private void IncrementCreditsAmount(int increment)
    {
        int previousCreditsAmount = GetPersistentDataManager().GetCreditsAmount();
        GetPersistentDataManager().SetCreditsAmount(previousCreditsAmount + increment);
    }

    private PersistentDataManager GetPersistentDataManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }
}
