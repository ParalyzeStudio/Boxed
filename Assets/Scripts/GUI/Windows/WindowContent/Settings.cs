using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settings : GameWindowContent
{
    private PersistentDataManager m_persistentDataManager;

    public Image m_musicSwitchOn;
    public Image m_musicSwitchOff;
    public Image m_soundSwitchOn;
    public Image m_soundSwitchOff;
    public GameWindowElement m_musicElement;
    public GameWindowElement m_soundElement;
    public GameWindowElement m_resetGameElement;
    //public GameWindowElement m_confirmResetWindow;
    public GameWindowContent m_confirmResetWindow;
    //private bool m_confirmResetWindowActive;

    public override IEnumerator Show(float timeSpacing = GameWindowContent.DEFAULT_TIME_SPACING)
    {   
        bool musicOn = GameController.GetInstance().GetPersistentDataManager().IsMusicOn();
        m_musicSwitchOn.gameObject.SetActive(musicOn);
        m_musicSwitchOff.gameObject.SetActive(!musicOn);

        bool soundOn = GameController.GetInstance().GetPersistentDataManager().IsSoundOn();
        m_soundSwitchOn.gameObject.SetActive(soundOn);
        m_soundSwitchOff.gameObject.SetActive(!soundOn);

        //remove the reset button when accessing the settings screen from game scene
        if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME)
        {
            m_resetGameElement.gameObject.SetActive(false);
        }

        //m_confirmResetWindowActive = false;

        yield return StartCoroutine(base.Show(timeSpacing));
    }

    public void OnClickMusicSwitch()
    {
        PersistentDataManager pDataManager = GameController.GetInstance().GetPersistentDataManager();
        bool musicOn = pDataManager.IsMusicOn();
        pDataManager.SetMusicOn(!musicOn);
        
        m_musicSwitchOn.gameObject.SetActive(!musicOn);
        m_musicSwitchOff.gameObject.SetActive(musicOn);
    }

    public void OnClickSoundSwitch()
    {
        PersistentDataManager pDataManager = GameController.GetInstance().GetPersistentDataManager();
        bool soundOn = pDataManager.IsSoundOn();
        pDataManager.SetSoundOn(!soundOn);

        m_soundSwitchOn.gameObject.SetActive(!soundOn);
        m_soundSwitchOff.gameObject.SetActive(soundOn);
    }

    public void OnClickResetGame()
    {
        MainPageMenu parentWindow = this.transform.parent.GetComponent<MainPageMenu>();
        parentWindow.StartCoroutine(parentWindow.ShowContentForID(MainPageMenu.ContentID.RESET_CONFIRM));
        parentWindow.DismissBackButton();
    }

    public void SaveSettings()
    {
        GameController.GetInstance().GetPersistentDataManager().SavePrefs();
    }
}
