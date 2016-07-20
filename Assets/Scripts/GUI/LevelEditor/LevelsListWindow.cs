using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsListWindow : MonoBehaviour
{
    protected LevelEditor m_parentEditor;

    protected List<LevelItem> m_items;
    protected LevelItem m_selectedItem;

    public Transform m_levelsListTf; //holder for level items
    public LevelItem m_levelItemPfb;

    public virtual void Init(LevelEditor parentEditor)
    {
        m_parentEditor = parentEditor;    
    }

    /**
    * Build a list of LevelItem objects corresponding to the list of Level object passed as parameter
    **/
    protected void BuildLevelItemsForLevels(List<Level> levels)
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
    protected LevelItem BuildListItemForLevel(Level level)
    {
        LevelItem levelItemObject = (LevelItem)Instantiate(m_levelItemPfb);

        LevelItem levelItem = levelItemObject.GetComponent<LevelItem>();
        levelItem.Init(level);

        levelItem.GetComponent<Button>().onClick.AddListener(delegate { OnLevelItemClick(levelItem); });

        return levelItem;
    }

    /**
    * Replace the current content of the levels list with a new one issued from the current m_items list
    **/
    protected void InvalidateItemList()
    {
        //detach old level items from their parent
        LevelItem[] children = m_levelsListTf.GetComponentsInChildren<LevelItem>();
        for (int i = 0; i != children.Length; i++)
        {
            children[i].transform.SetParent(null, false);
        }

        //replace them with the new list of items
        for (int i = 0; i != m_items.Count; i++)
        {
            m_items[i].transform.SetParent(m_levelsListTf, false);
        }
    }

    public virtual void OnLevelItemClick(LevelItem item)
    {
        if (m_selectedItem == null)
        {
            item.Select();
            m_selectedItem = item;
        }
        else
        {
            if (item != m_selectedItem) //we clicked on a different item
            {
                m_selectedItem.Deselect();
                item.Select();
                m_selectedItem = item;
            }
        }
    }
}
