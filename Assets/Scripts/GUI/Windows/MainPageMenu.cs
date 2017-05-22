using UnityEngine;

public class MainPageMenu : GameWindow
{
    public GameWindowContent m_mainContent;
    public Shop m_shopWindowContentPfb;
    private Shop m_shopWindowContent;
    public SettingsContent m_settingsContentPfb;
    private SettingsContent m_settingsContent;

    public bool Show()
    {
        return base.Show(m_mainContent, true);
    }

    public void OnClickShop()
    {
        StartCoroutine(DismissCurrentContent());
        m_shopWindowContent = Instantiate(m_shopWindowContentPfb);
        m_shopWindowContent.transform.SetParent(this.transform, false);
        m_shopWindowContent.gameObject.SetActive(false);
        StartCoroutine(ShowContentAfterDelay(m_shopWindowContent, GameWindowElement.ELEMENT_ANIMATION_DURATION));
    }

    public void OnClickSettings()
    {
        StartCoroutine(DismissCurrentContent());
        m_settingsContent = Instantiate(m_settingsContentPfb);
        m_settingsContent.transform.SetParent(this.transform, false);
        m_settingsContent.gameObject.SetActive(false);
        StartCoroutine(ShowContentAfterDelay(m_settingsContent, GameWindowElement.ELEMENT_ANIMATION_DURATION));
    }

    public void OnClickInfo()
    {
        Debug.Log(">>>>OnClickInfo");
    }

    public void OnClickGameCenter()
    {
        Debug.Log(">>>>OnClickGameCenter");
    }

    public override void OnClickBack()
    {
        if (m_content != null)
        {
            if (m_content == m_shopWindowContent || m_content == m_settingsContent)
            {
                if (m_content == m_settingsContent)
                    m_settingsContent.SaveSettings();

                StartCoroutine(ShowContentAfterDelay(m_mainContent, GameWindowElement.ELEMENT_ANIMATION_DURATION));
                StartCoroutine(DismissCurrentContent(true));
            }
            else
                base.OnClickBack();
        }
    }
}
