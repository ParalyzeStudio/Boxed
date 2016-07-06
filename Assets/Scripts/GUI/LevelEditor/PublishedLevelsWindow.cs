using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublishedLevelsWindow : MonoBehaviour
{
    public Button m_swapLevelsBtn;
    private Level m_swapLevel1;
    private Level m_swapLevel2;

    private LevelEditor m_parentEditor;

    public Transform m_levelsListTf; //holder for level items
    public LevelItem m_levelItemPfb;

    public void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;

        PopulateLevelsList();
    }

    /**
   * Populate the scroll list with levels saved inside persistent data path
   * Return the size of the list
   **/
    public int PopulateLevelsList()
    {
        List<Level> allLevels = GameController.GetInstance().GetComponent<LevelManager>().GetAllPublishedLevelsFromDisk();

        int listIndex = 0;
        for (int i = 0; i != allLevels.Count; i++)
        {
            Level level = allLevels[i];
            int levelNumber = level.m_number;

            //build empty levels until we reach the next valid level
            while (listIndex < levelNumber - 1)
            {
                BuildListItemAtIndexForLevel(listIndex, null);
                listIndex++;
            }

            //Build the valid level
            BuildListItemAtIndexForLevel(listIndex, level);
            listIndex++;
        }

        return listIndex + 1;
    }

    /**
    * Build a level item object and add it to the list
    **/
    private void BuildListItemAtIndexForLevel(int index, Level level)
    {
        LevelItem levelItemObject = (LevelItem)Instantiate(m_levelItemPfb);

        LevelItem levelItem = levelItemObject.GetComponent<LevelItem>();
        levelItem.Init(index, level);

        levelItem.GetComponent<Button>().onClick.AddListener(delegate { OnLevelItemClick(levelItem); });

        levelItem.transform.SetParent(m_levelsListTf, false);
    }

    public void OnClickSwapLevels()
    {
        SwapSelectedLevels();
    }

    public void OnClickQuit()
    {
        Destroy(this.gameObject);
    }

    private void SwapSelectedLevels()
    {
        LevelManager levelManager = GameController.GetInstance().GetComponent<LevelManager>();

        //First destroy the two deprecated files
        levelManager.DeletePublishedLevelFileForLevel(m_swapLevel1);
        levelManager.DeletePublishedLevelFileForLevel(m_swapLevel2);

        //swap the numbers
        m_swapLevel1.m_number = m_swapLevel2.m_number;
        m_swapLevel2.m_number = m_swapLevel1.m_number;

        //republish the levels
        m_swapLevel1.Publish();
        m_swapLevel2.Publish();
    }

    private void EnableSwapButton()
    {
        Text buttonText = m_swapLevelsBtn.GetComponentInChildren<Text>();
        Color oldColor = buttonText.color;
        buttonText.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);

        m_swapLevelsBtn.interactable = true;
    }

    private void DisableSwapButton()
    {
        Text buttonText = m_swapLevelsBtn.GetComponentInChildren<Text>();
        Color oldColor = buttonText.color;
        buttonText.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);

        m_swapLevelsBtn.interactable = false;
    }

    public void OnLevelItemClick(LevelItem item)
    {
        if (item.m_level == m_swapLevel1)
        {
            m_swapLevel1 = null;
            item.Deselect();
        }
        else if (item.m_level == m_swapLevel2)
        {
            m_swapLevel2 = null;
            item.Deselect();
        }
        else
        {
            if (m_swapLevel1 == null)
            {
                m_swapLevel1 = item.m_level;
                item.Select();
            }
            else if (m_swapLevel2 == null)
            {
                m_swapLevel2 = item.m_level;
                item.Select();
            }
        }
    }

    public void Update()
    {
        if (m_swapLevel1 == null || m_swapLevel2 == null)
            DisableSwapButton();
        else
            EnableSwapButton();
    }
}
