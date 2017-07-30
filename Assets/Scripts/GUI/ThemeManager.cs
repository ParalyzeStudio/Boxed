using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    //public enum ThemeID
    //{
    //    GREEN_THEME = 0,
    //    BLUE_THEME
    //}

    //public ThemeID m_themeID;
    //private ThemeID m_prevThemeID;

    public Theme[] m_themes;

    private PersistentDataManager m_persistentDataManager;

    public void Start()
    {
        for (int i = 0; i != m_themes.Length; i++)
        {
            m_themes[i].SyncPrivateParameters();
        }
    }

    //public void Init()
    //{
    //    //m_theme1 = new Theme();
    //    //m_theme2 = new Theme();
    //    //m_theme3 = new Theme();
    //    //m_theme4 = new Theme();

    //    //if (m_themes == null)
    //    //{
    //    //    m_themes = new Theme[LevelManager.NUM_CHAPTERS];
    //    //    for (int i = 0; i != m_themes.Length; i++)
    //    //    {
    //    //        m_themes[i] = new Theme();
    //    //    }
    //    //}

    //    //m_selectedTheme = m_themes[0];
    //}

    [System.Serializable]
    public class Theme
    {
        public Color m_backgroundGradientTopColor;
        public Color m_backgroundGradientBottomColor;
        public Color m_floorSupportColor;

        //materials for every type of tile
        public Material m_defaultTileMaterial;
        public Material m_switchTileMaterial;
        public Material m_triggeredTileMaterial;
        public Material m_iceTileMaterial;

        //private variables to notify if one value has been updated
        private Color m_prevBackgroundGradientTopColor;
        private Color m_prevBackgroundGradientBottomColor;
        private Color m_prevFloorSupportColor;
        private Color m_prevHighScoreColor;
        private Material m_prevDefaultTileMaterial;
        private Material m_prevSwitchTileMaterial;
        private Material m_prevTriggeredTileMaterial;
        private Material m_prevIceTileMaterial;

        public Theme() { }

        public Material GetTileMaterialForTileState(Tile.State state)
        {
            if (state == Tile.State.NORMAL)
                return m_defaultTileMaterial;
            else if (state == Tile.State.SWITCH)
                return m_switchTileMaterial;
            else if (state == Tile.State.TRIGGERED_BY_SWITCH)
                return m_triggeredTileMaterial;
            else if (state == Tile.State.ICE)
                return m_iceTileMaterial;
            else
                return m_defaultTileMaterial;
        }

        /**
        * Sync the public and private parameters of this theme, copying public ones into private ones
        **/
        public void SyncPrivateParameters()
        {
            m_prevBackgroundGradientBottomColor = m_backgroundGradientBottomColor;
            m_prevBackgroundGradientTopColor = m_backgroundGradientTopColor;
            m_prevFloorSupportColor = m_floorSupportColor;
            m_prevDefaultTileMaterial = m_defaultTileMaterial;
            m_prevSwitchTileMaterial = m_switchTileMaterial;
            m_prevTriggeredTileMaterial = m_triggeredTileMaterial;
            m_prevIceTileMaterial = m_iceTileMaterial;
        }

        public Color GetTileColorForTileState(Tile.State state)
        {
            if (state == Tile.State.DISABLED)
                return ColorUtils.DarkenColor(Color.white, 0.5f);
            else if (state == Tile.State.START)
                return Color.Lerp(Color.white, Color.green, 0.8f);
            else if (state == Tile.State.FINISH)
                return Color.Lerp(Color.white, Color.red, 0.8f);
            else if (state == Tile.State.BLOCKED)
                return ColorUtils.DarkenColor(Color.white, 0.8f);
            else
                return Color.white;
            //if (state == Tile.State.NORMAL)
            //    return m_defaultTileColor;
            //else if (state == Tile.State.DISABLED)
            //    return m_disabledTileColor;
            //else if (state == Tile.State.BLOCKED)
            //    return m_blockedTileColor;
            ////else if (state == Tile.State.SELECTED)
            ////    return m_selectedTileColors
            //else if (state == Tile.State.START)
            //    return m_startTileColor;
            //else if (state == Tile.State.TRAP)
            //    return m_trapTileColor;
            //else if (state == Tile.State.SWITCH)
            //    return m_switchTileColor;
            //else if (state == Tile.State.TRIGGERED_BY_SWITCH)
            //    return m_defaultTileColor;
            //else //FINISH
            //    return m_finishTileColor;
        }

        public bool ParametersAreDirty()
        {
            bool bValuesHaveChanged = false;
            if (m_backgroundGradientTopColor != m_prevBackgroundGradientTopColor)
            {
                bValuesHaveChanged = true;
                m_prevBackgroundGradientTopColor = m_backgroundGradientTopColor;
            }
            if (m_backgroundGradientBottomColor != m_prevBackgroundGradientBottomColor)
            {
                bValuesHaveChanged = true;
                m_prevBackgroundGradientBottomColor = m_backgroundGradientBottomColor;
            }
            if (m_floorSupportColor != m_prevFloorSupportColor)
            {
                bValuesHaveChanged = true;
                m_prevFloorSupportColor = m_floorSupportColor;
            }
            if (m_defaultTileMaterial != m_prevDefaultTileMaterial)
            {
                bValuesHaveChanged = true; ;
                m_prevDefaultTileMaterial = m_defaultTileMaterial;
            }
            if (m_switchTileMaterial != m_prevSwitchTileMaterial)
            {
                bValuesHaveChanged = true;
                m_prevSwitchTileMaterial = m_switchTileMaterial;
            }
            if (m_triggeredTileMaterial != m_prevTriggeredTileMaterial)
            {
                bValuesHaveChanged = true;
                m_prevTriggeredTileMaterial = m_triggeredTileMaterial;
            }
            if (m_iceTileMaterial != m_prevIceTileMaterial)
            {
                bValuesHaveChanged = true;
                m_prevIceTileMaterial = m_iceTileMaterial;
            }

            return bValuesHaveChanged;
        }
    }
    
    /**
    * Return the theme associated to this chapter
    **/
    public Theme GetSelectedTheme()
    {
        return m_themes[GameController.GetInstance().GetPersistentDataManager().GetCurrentChapterIndex()];
    }

    public Theme GetThemeForNumber(int number)
    {
        return m_themes[number - 1];
        //switch (number)
        //{
        //    case 1:
        //        return m_the;
        //    case 2:
        //        return m_theme2;
        //    case 3:
        //        return m_theme2;
        //    case 4:
        //        return m_theme2;
        //    default:
        //        return null;
        //}
    }

    /**
    * Update the scene elements so they match the selected theme properties
    **/
    public void InvalidateSelectedTheme()
    {
        //m_prevThemeID = m_themeID;

        //replace theme variables values inside inspector$
        //m_selectedTheme = m_themes[(int)m_themeID];

        Theme selectedTheme = GetSelectedTheme();

        //update the scene elements to match the currently selected theme
        //colors
        GUIManager guiManager = GameController.GetInstance().GetComponent<GUIManager>();
        guiManager.m_background.m_topColor = selectedTheme.m_backgroundGradientTopColor;
        guiManager.m_background.m_bottomColor = selectedTheme.m_backgroundGradientBottomColor;

        GameController.GameMode gameMode = GameController.GetInstance().m_gameMode;
        //if (gameMode == GameController.GameMode.GAME || gameMode == GameController.GameMode.LEVEL_EDITOR)
        //    GameController.GetInstance().m_floorRenderer.m_floorSupport.m_mainColor = selectedTheme.m_floorSupportColor;

        if (gameMode == GameController.GameMode.GAME || gameMode == GameController.GameMode.LEVEL_EDITOR)
            GameController.GetInstance().m_floorRenderer.m_floorSupport.SetColor(selectedTheme.m_floorSupportColor);

        if (gameMode == GameController.GameMode.GAME)
        {
            //materials
            Tile[] floorTiles = GameController.GetInstance().m_floorRenderer.m_floorData.Tiles;
            for (int i = 0; i != floorTiles.Length; i++)
            {
                floorTiles[i].m_tileStateDirty = true;
            }
        }
    }

    /**
    * Callback from the custom inspector save button
    **/
    //public void OnClickSaveTheme()
    //{
    //    SerializeTheme();
    //}

    //private bool SerializeTheme()
    //{
    //    BinaryFormatter bf = new BinaryFormatter();

    //    FileStream fs = null;
    //    try
    //    {
    //        string folderPath = Application.persistentDataPath + "/Themes";
    //        if (!Directory.Exists(folderPath))
    //            Directory.CreateDirectory(folderPath);
    //        string filePath = folderPath  + "/theme_" + ((int)m_themeID) + ".theme";            
    //        fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    //        Debug.Log("level saved to path:" + filePath);
    //    }
    //    catch (Exception)
    //    {
    //        if (fs != null)
    //            fs.Close();
    //        return false; //failed to open or create the file
    //    }

    //    bf.Serialize(fs, m_selectedTheme);
    //    fs.Close();

    //    return true;
    //}

    public void Update()
    {
        if (GetSelectedTheme().ParametersAreDirty())
            InvalidateSelectedTheme();
    }
}
