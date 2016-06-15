using UnityEngine;
using UnityEngine.UI;

public class PublishWindow : MonoBehaviour
{   
    public InputField m_levelNameInputField;
    public Button m_publishButton;

    public GameObject m_publishGroup;
    public GameObject m_confirmationGroup;

    private LevelEditor m_parentEditor;

    public void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;
    }

    public void OnClickClearInputField()
    {
        m_levelNameInputField.text = "";
    }

    public void OnClickPublish()
    {
        string levelName = m_levelNameInputField.text;
        m_parentEditor.m_editedLevel.m_title = levelName;
        m_parentEditor.m_editedLevel.Publish();

        ShowConfirmation();
    }

    public void OnClickClose()
    {
        Destroy(this.gameObject);
    }

    private void ShowConfirmation()
    {
        m_publishGroup.SetActive(false);
        m_confirmationGroup.gameObject.SetActive(true);
    }

    public void Update()
    {
        m_publishButton.interactable = m_levelNameInputField.text.Length > 0;
    }
}
