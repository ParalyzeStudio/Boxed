﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadLevelWindow : LevelsListWindow
{
    //public Text m_saveErrorMessage1;
    //public Text m_saveErrorMessage2;
    //public Text m_saveSuccessMessage;
    public Button m_saveBtn;
    public Button m_loadBtn;

    public OverwriteFilePopup m_overwriteFilePopupPfb;

    private enum ButtonID
    {
        ID_SAVE_LEVEL,
        ID_LOAD_LEVEL,
        ID_CANCEL
    }

    public override void Init(LevelEditor parentEditor)
    {
        base.Init(parentEditor);

        DisableButton(ButtonID.ID_SAVE_LEVEL);
        DisableButton(ButtonID.ID_LOAD_LEVEL);

        List<Level> editedLevels = GameController.GetInstance().GetComponent<LevelManager>().GetAllEditedLevelsFromDisk();
        BuildLevelItemsForLevels(editedLevels);
        InvalidateItemList();
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
        int levelNumber = m_selectedItem.m_level.m_number;

        //Build a new floor and a new level that holds it
        Level editedLevel = m_parentEditor.m_editedLevel;
        editedLevel.m_number = levelNumber;
        editedLevel.m_title = "Level_" + Level.GetNumberAsString(levelNumber);

        //update the level item to display the new name
        if (m_selectedItem.m_level == null)
        {
            m_selectedItem.m_level = editedLevel;
            m_selectedItem.InvalidateContent();
            //m_saveSuccessMessage.gameObject.SetActive(true);
            editedLevel.Save();
        }
        else
        {
            //Show popup to confirm choice of overwriting current selected level
            OverwriteFilePopup overwriteFilePopup = Instantiate(m_overwriteFilePopupPfb);
            overwriteFilePopup.Init(this);

            overwriteFilePopup.transform.SetParent(m_parentEditor.transform, false);
        }        
    }

    public void OnClickLoad()
    {
        GameController.GetInstance().ClearLevel();
        m_parentEditor.BuildLevel(m_selectedItem.m_level);
        if (m_selectedItem.m_level.m_validated)
            m_parentEditor.ShowTestMenu();

        //Dismiss the window
        OnClickCancel();
    }

    public void OnClickCancel()
    {
        m_parentEditor.OnDismissSaveLoadLevelWindow();
        Destroy(this.gameObject);
    }    

    public override void OnLevelItemClick(LevelItem item)
    {
        base.OnLevelItemClick(item);

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