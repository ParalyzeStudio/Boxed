using System.Collections;
using UnityEngine;

public class MainPageMenu : GameWindow
{
    //list all contents that can be instantiated and set as a child of this window
    public GameWindowContent m_mainContent;
   
    //4 main contents
    public Shop m_shopPfb;
    private Shop m_shop;
    public Settings m_settingsPfb;
    private Settings m_settings;
    public GameCenter m_gameCenterPfb;
    private GameCenter m_gameCenter;
    public Info m_info;

    //sub contents
    public ResetConfirm m_resetConfirm;
    public ResetDone m_resetDone;

    public enum ContentID
    {
        MAIN = 1,
        SETTINGS,
        GAMECENTER,
        SHOP,
        INFO,
        RESET_CONFIRM,
        RESET_DONE
    }

    public bool Show()
    {
        return base.Show(m_mainContent, true);
    }

    public void OnClickShop()
    {
        StartCoroutine(ShowContentForID(ContentID.SHOP));
    }

    public void OnClickSettings()
    {
        StartCoroutine(ShowContentForID(ContentID.SETTINGS));
    }

    public void OnClickInfo()
    {
        StartCoroutine(ShowContentForID(ContentID.INFO));
    }

    public void OnClickGameCenter()
    {
        StartCoroutine(ShowContentForID(ContentID.GAMECENTER));
    }

    public IEnumerator ShowContentForID(ContentID iID)
    {
        GameWindowContent content;
        switch (iID)
        {
            case ContentID.MAIN:
                content = m_mainContent;
                break;
            case ContentID.SETTINGS:
                if (m_settings == null)
                {
                    m_settings = Instantiate(m_settingsPfb);
                    m_settings.transform.SetParent(this.transform, false);
                }
                content = m_settings;
                break;
            case ContentID.GAMECENTER:
                if (m_gameCenter == null)
                {
                    m_gameCenter = Instantiate(m_gameCenterPfb);
                    m_gameCenter.transform.SetParent(this.transform, false);
                }
                content = m_gameCenter;
                break;
            case ContentID.SHOP:
                if (m_shop == null)
                {
                    m_shop = Instantiate(m_shopPfb);
                    m_shop.transform.SetParent(this.transform, false);
                }
                content = m_shop;
                break;
            case ContentID.INFO:
                content = m_info;
                break;
            case ContentID.RESET_CONFIRM:
                content = m_resetConfirm;
                break;
            case ContentID.RESET_DONE:
                content = m_resetDone;
                break;
            default:
                content = null;
                break;
        }
        
        if (content != null)
        {
            yield return StartCoroutine(DismissCurrentContent());
            yield return StartCoroutine(ShowContentAfterDelay(content, 0));
        }
    }

    public override void OnClickBack()
    {
        if (m_content != null)
        {
            if (m_content == m_mainContent)
                base.OnClickBack();
            else
            {
                if (m_content == m_settings)
                    m_settings.SaveSettings();
                StartCoroutine(ShowContentForID(ContentID.MAIN));
            }
        }
    }
}
