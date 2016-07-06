using UnityEngine;
using UnityEngine.UI;

public class PublishWindow : MonoBehaviour
{
    public InputField m_levelNumberInputField;
    public Button m_publishButton;

    public GameObject m_publishGroup;
    public GameObject m_confirmationGroup;

    private LevelEditor m_parentEditor;

    public void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;
    }

    public void OnClickClearNumberInputField()
    {
        m_levelNumberInputField.text = "";
    }

    public void OnClickPublish()
    {
        string levelNumber = m_levelNumberInputField.text;
        m_parentEditor.m_editedLevel.m_number = int.Parse(levelNumber);
        m_parentEditor.m_editedLevel.m_title = null;
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

    private void EnablePublishButton()
    {
        Text buttonText = m_publishButton.GetComponentInChildren<Text>();
        Color oldColor = buttonText.color;
        buttonText.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);
        m_publishButton.interactable = true;
    }

    private void DisablePublishButton()
    {
        Text buttonText = m_publishButton.GetComponentInChildren<Text>();
        Color oldColor = buttonText.color;
        buttonText.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);
        m_publishButton.interactable = false;
    }

    public void Update()
    {
        int levelNumber;
        try
        {
            levelNumber = int.Parse(m_levelNumberInputField.text);
        }
        catch (System.FormatException)
        {
            DisablePublishButton();
            return;
        }
        
        if (levelNumber > 0)
            EnablePublishButton();
        else
            DisablePublishButton();
    }
}
