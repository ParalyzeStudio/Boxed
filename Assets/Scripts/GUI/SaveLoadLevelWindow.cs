using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadLevelWindow : MonoBehaviour
{
    public InputField m_input; //the input where the player can enter a level filename
    public Text m_saveErrorMessage1;
    public Text m_saveErrorMessage2;
    public Text m_saveSuccessMessage;
    public Button m_saveBtn;
    public Button m_loadBtn;
    public Transform m_levelsListTf; //holder for level items
    
    public LevelItem m_levelItemPfb;

    private LevelItem m_selectedItem;

    private LevelEditor m_parentLevelEditor;

    public OverwriteFilePopup m_overwriteFilePopupPfb;

    private enum ButtonID
    {
        ID_SAVE_LEVEL,
        ID_LOAD_LEVEL,
        ID_CANCEL
    }

    public void Init(LevelEditor parentLevelEditor)
    {
        m_parentLevelEditor = parentLevelEditor;
        
        int levelsCount = PopulateLevelsList();
        m_selectedItem = null;

        DisableButton(ButtonID.ID_SAVE_LEVEL);
        DisableButton(ButtonID.ID_LOAD_LEVEL);
    }

    /**
    * Populate the scroll list with levels saved inside persistent data path
    * Return the size of the list
    **/
    public int PopulateLevelsList()
    {
        List<Level> allLevels = GameController.GetInstance().GetComponent<LevelManager>().GetAllEditedLevelsFromDisk();

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

        //Build another empty level just after the last valid level
        BuildListItemAtIndexForLevel(listIndex, null);

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

        levelItem.GetComponent<Button>().onClick.AddListener(delegate { OnItemClick(levelItem); });

        levelItem.transform.SetParent(m_levelsListTf);
    }

    /**
    * In case we want to overwrite the currently selected level we need to nullify the related level so DoSave() actually performs the overwriting of the file
    **/
    public void ClearLevelOnSelectedItem()
    {
        m_selectedItem.m_level = null;
    }

    public void OnClickSave()
    {
        Debug.Log("DO SAVE");
        int levelNumber = m_selectedItem.m_index + 1;

        //Build a new floor and a new level that holds it
        //Floor clampedFloor = GameController.GetInstance().m_floor.m_floorData.Clamp();
        Level editedLevel = m_parentLevelEditor.m_editedLevel;
        editedLevel.m_number = levelNumber;
        editedLevel.m_title = "Level_" + Level.GetNumberAsString(levelNumber);

        //update the level item to display the new name
        if (m_selectedItem.m_level == null)
        {
            m_selectedItem.m_level = editedLevel;
            m_selectedItem.InvalidateContent();
            m_saveSuccessMessage.gameObject.SetActive(true);
            editedLevel.SaveToFile();
        }
        else
        {
            //Show popup to confirm choice of overwriting current selected level
            OverwriteFilePopup overwriteFilePopup = (OverwriteFilePopup)Instantiate(m_overwriteFilePopupPfb);
            overwriteFilePopup.Init(this);

            overwriteFilePopup.transform.SetParent(GameController.GetInstance().m_canvas.transform, false);
        }        
    }

    public void OnClickLoad()
    {
        GameController.GetInstance().ClearLevel();
        m_parentLevelEditor.BuildLevel(m_selectedItem.m_level);

        //Dismiss the window
        OnClickCancel();
    }

    public void OnClickCancel()
    {
        m_parentLevelEditor.OnDismissSaveLoadLevelWindow();
        Destroy(this.gameObject);
    }    

    public void OnItemClick(LevelItem item)
    {
        if (m_selectedItem == item)
            return;

        m_selectedItem = item;
        item.OnClick();

        if (item.m_level == null)
        {
            EnableButton(ButtonID.ID_SAVE_LEVEL);
            DisableButton(ButtonID.ID_LOAD_LEVEL);
        }
        else
        {
            EnableButton(ButtonID.ID_SAVE_LEVEL);
            EnableButton(ButtonID.ID_LOAD_LEVEL);
        }
    }

    /**
    * Enable and disable buttons by setting right opacity on their child text
    **/
    private Button GetButtonForID(ButtonID iID)
    {
        Button button = null;
        if (iID == ButtonID.ID_SAVE_LEVEL)
            button = m_saveBtn;
        else if (iID == ButtonID.ID_LOAD_LEVEL)
            button = m_loadBtn;

        return button;
    }

    private void EnableButton(ButtonID iID)
    {
        Button button = GetButtonForID(iID);
        button.interactable = true;

        Text childText = button.GetComponentInChildren<Text>();
        Color prevColor = childText.color;
        Color newColor = new Color(prevColor.r, prevColor.g, prevColor.b, 1.0f);
        childText.color = newColor;
    }

    private void DisableButton(ButtonID iID)
    {
        Button button = GetButtonForID(iID);
        button.interactable = false;

        Text childText = button.GetComponentInChildren<Text>();
        Color prevColor = childText.color;
        Color newColor = new Color(prevColor.r, prevColor.g, prevColor.b, 0.5f);
        childText.color = newColor;
    }
}