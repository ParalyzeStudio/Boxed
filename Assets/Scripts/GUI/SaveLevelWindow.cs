using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLevelWindow : MonoBehaviour
{
    public InputField m_input; //the input where the player can enter a level filename
    public Transform m_levelsListTf; //holder for level items
    
    public GameObject m_levelItemPfb;

    private LevelItem m_selectedItem;

    public void Init(Level levelToSave)
    {
        //int levelsCount = PopulateLevelsList();
        int levelsCount = 5;
        PopulateWithDummyLevels(levelsCount);
        PrefillInput(levelsCount);
        m_selectedItem = null;
    }

    private void PopulateWithDummyLevels(int levelsCount)
    {
        for (int i = 0; i != levelsCount; i++)
        {
            GameObject levelItemObject = (GameObject)Instantiate(m_levelItemPfb);
            LevelItem levelItem = levelItemObject.GetComponent<LevelItem>();
            levelItem.m_title.text = "Level" + Level.GetNumberAsString(i+1);
            levelItem.m_index = i;

            levelItem.transform.SetParent(m_levelsListTf);
        }
    }

    /**
    * Populate the scroll list with levels saved inside persistent data path
    **/
    public int PopulateLevelsList()
    {
        string[] levelsFilenames = Directory.GetFiles(Application.persistentDataPath);

        for (int i = 0; i != levelsFilenames.Length; i++)
        {
            Debug.Log("filename:" + levelsFilenames[i]);

            GameObject levelItemObject = (GameObject)Instantiate(m_levelItemPfb);
            LevelItem levelItem = levelItemObject.GetComponent<LevelItem>();
            levelItem.m_title.text = levelsFilenames[i];
            levelItem.m_index = i;

            levelItem.transform.SetParent(m_levelsListTf);
        }

        return levelsFilenames.Length;
    }

    private void PrefillInput(int levelsCount)
    {
        //m_input.text = "Level_" + Level.GetNumberAsString(levelsCount + 1);
    }

    public void DoSave()
    {
        Debug.Log("SAVE");
    }

    public void DoCancel()
    {
        Destroy(this.gameObject);
    }    

    public void OnItemClick(LevelItem item)
    {
        if (m_selectedItem == item)
            return;

        if (m_selectedItem != null)
            m_selectedItem.Deselect();
        m_selectedItem = item;
        item.OnClick();

        //Fill the input field in case the player wants to overwrite the selected level file
        //m_input.text = item.m_title.text;
        Debug.Log("OnItemClick");
        m_input.text = "Test" + item.m_index;
        m_input.GetComponentInChildren<Text>().text = "Test" + item.m_index;
    }
}