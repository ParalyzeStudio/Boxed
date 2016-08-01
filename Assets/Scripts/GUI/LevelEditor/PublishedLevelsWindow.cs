using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublishedLevelsWindow : LevelsListWindow
{
    public Button m_loadBtn;
    public Button m_moveUpBtn;
    public Button m_moveDownBtn;    

    public override void Init(LevelEditor parentEditor)
    {
        base.Init(parentEditor);

        DisableLoadButton();
        DisableUpDownButtons();

        List<Level> publishedLevels = GameController.GetInstance().GetComponent<LevelManager>().GetAllPublishedLevelsFromDisk();
        BuildLevelItemsForLevels(publishedLevels);
        InvalidateItemList();
    }

    protected override void BuildLevelItemsForLevels(List<Level> levels)
    {
        if (m_items == null)
            m_items = new List<LevelItem>();

        int listIndex = 0;
        for (int i = 0; i != levels.Count; i++)
        {
            Level level = levels[i];
            int levelNumber = level.m_number;

            //build empty levels until we reach the next valid level
            while (listIndex < levelNumber - 1)
            {
                m_items.Add(BuildListItemForLevel(null));
                listIndex++;
            }

            //Build the valid level
            m_items.Add(BuildListItemForLevel(level));
            listIndex++;
        }
    }

    /**
    * Build a level item object and add it to the list
    **/
    protected override LevelItem BuildListItemForLevel(Level level)
    {
        LevelItem levelItemObject = (LevelItem)Instantiate(m_levelItemPfb);

        LevelItem levelItem = levelItemObject.GetComponent<LevelItem>();
        if (level != null)
            level.m_title = "Level_" + Level.GetNumberAsString(level.m_number);
        levelItem.Init(level);

        levelItem.GetComponent<Button>().onClick.AddListener(delegate { OnLevelItemClick(levelItem); });

        return levelItem;
    }

    /** ONCLICK methods **/
    public void OnClickLoad()
    {
        GameController.GetInstance().ClearLevel();
        m_parentEditor.BuildLevel(m_selectedItem.m_level);
        if (m_selectedItem.m_level.m_validated)
            m_parentEditor.ShowTestMenu();

        //Dismiss the window
        OnClickQuit();
    }

    public void OnClickQuit()
    {
        Destroy(this.gameObject);
    }

    public void OnClickMoveLevelDown()
    {
        MoveSelectedItem(true);
    }

    public void OnClickMoveLevelUp()
    {
        MoveSelectedItem(false);
    }

    public void OnClickValidateReordering()
    {
        RepublishReorderedItems();
    }

    /**
    * Republish all levels after reordering has been done
    **/
    private void RepublishReorderedItems()
    {
        LevelManager levelManager = GameController.GetInstance().GetComponent<LevelManager>();

        List<Level> reorderedLevels = new List<Level>();
        //First find level items that have been reordered
        for (int i = 0; i != m_items.Count; i++)
        {
            if (m_items[i].m_level != null && i != m_items[i].m_level.m_number - 1)
            {
                levelManager.DeletePublishedLevelFileForLevel(m_items[i].m_level); //Destroy their associated level file
                m_items[i].m_level.m_number = i + 1;//assign their new level number
                reorderedLevels.Add(m_items[i].m_level);
            }
        }

        for (int i = 0; i != reorderedLevels.Count; i++)
        {
            reorderedLevels[i].Publish(); //republish the level
        }
    }

    /**
    * Move the selected item either up or down
    **/
    private void MoveSelectedItem(bool bDown)
    {
        if (m_selectedItem == null)
            return;

        for (int i = 0; i != m_items.Count; i++)
        {
            if (m_selectedItem == m_items[i])
            {
                if (bDown && i < (m_items.Count - 1))
                {
                    m_items.Remove(m_selectedItem);
                    m_items.Insert(i + 1, m_selectedItem);
                    break;
                }
                else if (!bDown && i > 0)
                {
                    m_items.Remove(m_selectedItem);
                    m_items.Insert(i - 1, m_selectedItem);
                    break;
                }
            }
        }

        InvalidateItemList();
    }

    /**
    * Enable buttons to move up or down a level that has been selected in the list
    **/
    private void EnableUpDownButtons()
    {
        Image moveDownIcon = m_moveDownBtn.GetComponent<Image>();
        Image moveUpIcon = m_moveUpBtn.GetComponent<Image>();

        Color oldColor = moveDownIcon.color;
        moveDownIcon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);

        oldColor = moveUpIcon.color;
        moveUpIcon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 1.0f);

        m_moveUpBtn.interactable = true;
        m_moveDownBtn.interactable = true;
    }

    /**
    * Disable buttons to move up or down a level that has been selected in the list
    **/
    private void DisableUpDownButtons()
    {
        Image moveDownIcon = m_moveDownBtn.GetComponent<Image>();
        Image moveUpIcon = m_moveUpBtn.GetComponent<Image>();

        Color oldColor = moveDownIcon.color;
        moveDownIcon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0);

        oldColor = moveUpIcon.color;
        moveUpIcon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0);

        m_moveUpBtn.interactable = true;
        m_moveDownBtn.interactable = true;
    }

    private void EnableLoadButton()
    {
        m_loadBtn.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
        m_loadBtn.interactable = true;
    }

    private void DisableLoadButton()
    {
        m_loadBtn.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.5f);
        m_loadBtn.interactable = false;
    }

    public void Update()
    {
        if (m_selectedItem == null)
        {
            DisableLoadButton();
            DisableUpDownButtons();
        }
        else
        {
            if (m_selectedItem.m_level != null)
            {
                EnableLoadButton();
                EnableUpDownButtons();
            }
            else
            {
                DisableLoadButton();
                DisableUpDownButtons();
            }
        }
    }
}
