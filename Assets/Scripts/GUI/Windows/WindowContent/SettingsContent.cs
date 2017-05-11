using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsContent : GameWindowContent
{
    private PersistentDataManager m_persistentDataManager;

    public Image m_musicSwitchOn;
    public Image m_musicSwitchOff;
    public Image m_soundSwitchOn;
    public Image m_soundSwitchOff;

    public override IEnumerator Show(float timeSpacing = 0.032F)
    {   
        bool musicOn = GetPersistentDataManager().IsMusicOn();
        m_musicSwitchOn.gameObject.SetActive(musicOn);
        m_musicSwitchOff.gameObject.SetActive(!musicOn);

        bool soundOn = GetPersistentDataManager().IsSoundOn();
        m_soundSwitchOn.gameObject.SetActive(soundOn);
        m_soundSwitchOff.gameObject.SetActive(!soundOn);

        Debug.Log("musicOn:" + musicOn);
        Debug.Log("soundOn:" + soundOn);

        return base.Show(timeSpacing);
    }

    public void OnClickMusicSwitch()
    {
        bool musicOn = GetPersistentDataManager().IsMusicOn();
        GetPersistentDataManager().SetMusicOn(!musicOn);
        
        m_musicSwitchOn.gameObject.SetActive(!musicOn);
        m_musicSwitchOff.gameObject.SetActive(musicOn);
    }

    public void OnClickSoundSwitch()
    {
        bool soundOn = GetPersistentDataManager().IsSoundOn();
        GetPersistentDataManager().SetSoundOn(!soundOn);

        m_soundSwitchOn.gameObject.SetActive(!soundOn);
        m_soundSwitchOff.gameObject.SetActive(soundOn);
    }

    public void SaveSettings()
    {
        GetPersistentDataManager().SavePrefs();
    }

    private PersistentDataManager GetPersistentDataManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameController.GetInstance().GetComponent<PersistentDataManager>();
        return m_persistentDataManager;
    }
}
