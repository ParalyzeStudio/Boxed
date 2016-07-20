using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublishedLevelsWindow : LevelsListWindow
{
    public Button m_moveUpBtn;
    public Button m_moveDownBtn;    

    public override void Init(LevelEditor parentEditor)
    {
        base.Init(parentEditor);

        List<Level> publishedLevels = GameController.GetInstance().GetComponent<LevelManager>().GetAllPublishedLevelsFromDisk();
        BuildLevelItemsForLevels(publishedLevels);
        InvalidateItemList();
    }    
    
    /** ONCLICK methods **/
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

    //private void SwapLevels(Level level1, Level level2)
    //{
    //    LevelManager levelManager = GameController.GetInstance().GetComponent<LevelManager>();

    //    //First destroy the two deprecated files
    //    levelManager.DeletePublishedLevelFileForLevel(level1);
    //    levelManager.DeletePublishedLevelFileForLevel(level2);

    //    //swap the numbers
    //    level1.m_number = level2.m_number;
    //    level2.m_number = level1.m_number;

    //    //republish the levels
    //    level1.Publish();
    //    level2.Publish();
    //}

    //private void MoveLevelToEmptySlot(Level level, int slotIndex)
    //{
    //    LevelManager levelManager = GameController.GetInstance().GetComponent<LevelManager>();
    //    levelManager.DeletePublishedLevelFileForLevel(level);
    //    level.m_number = slotIndex + 1;
    //    level.Publish();
    //}

    /**
    * Enable buttons to move up or down a level that has been selected in the list
    **/
    private void EnableUpDownButtons()
    {
        Image moveDownIcon = m_moveDownBtn.GetComponent<Image>();
        Image moveUpIcon = m_moveDownBtn.GetComponent<Image>();

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
        Image moveUpIcon = m_moveDownBtn.GetComponent<Image>();

        Color oldColor = moveDownIcon.color;
        moveDownIcon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);

        oldColor = moveUpIcon.color;
        moveUpIcon.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);

        m_moveUpBtn.interactable = true;
        m_moveDownBtn.interactable = true;
    }

    public void Update()
    {
        if (m_selectedItem == null)
            DisableUpDownButtons();
        else
            EnableUpDownButtons();
    }
}
