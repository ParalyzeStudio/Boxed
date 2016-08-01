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
    protected virtual void BuildLevelItemsForLevels(List<Level> levels)
    {

    }

    /**
    * Build a level item object and add it to the list
    **/
    protected virtual LevelItem BuildListItemForLevel(Level level)
    {
        return null;
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
