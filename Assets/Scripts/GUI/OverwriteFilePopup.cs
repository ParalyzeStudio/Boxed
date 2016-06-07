using UnityEngine;
using UnityEngine.UI;

public class OverwriteFilePopup : MonoBehaviour
{
    public Button m_yesBtn;
    public Button m_noBtn;

    private SaveLoadLevelWindow m_parentWindow;

    public void Init(SaveLoadLevelWindow parentWindow)
    {
        m_parentWindow = parentWindow;

        m_yesBtn.onClick.AddListener(OnClickYes);
        m_noBtn.onClick.AddListener(OnClickNo);
    }

    public void OnClickYes()
    {
        m_parentWindow.ClearLevelOnSelectedItem();
        m_parentWindow.OnClickSave();
        Destroy(this.gameObject);
    }

    public void OnClickNo()
    {
        Destroy(this.gameObject);
    }
}
