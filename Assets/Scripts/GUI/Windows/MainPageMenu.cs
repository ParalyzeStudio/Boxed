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
        m_shopWindowContent = Instantiate(m_shopWindowContentPfb);
        m_shopWindowContent.transform.SetParent(this.transform, false);
        StartCoroutine(DismissCurrentContent());
        StartCoroutine(ShowContentAfterDelay(m_shopWindowContent, 1.5f * GameWindowElement.ELEMENT_ANIMATION_DURATION));
    }

    public void OnClickSettings()
    {
        m_settingsContent = Instantiate(m_settingsContentPfb);
        m_settingsContent.transform.SetParent(this.transform, false);
        StartCoroutine(DismissCurrentContent());
        StartCoroutine(ShowContentAfterDelay(m_settingsContent, 1.5f * GameWindowElement.ELEMENT_ANIMATION_DURATION));
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
                StartCoroutine(DismissCurrentContent());
            }
            else
                base.OnClickBack();
        }
    }
}
