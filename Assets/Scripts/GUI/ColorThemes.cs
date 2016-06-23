using UnityEngine;

public class ColorThemes
{
    private ColorTheme[] m_themes;
    public ColorTheme[] Themes
    {
        get
        {
            return m_themes;
        }
    }

    public ColorTheme m_currentTheme;

    

    public void Init()
    {
        //define the colors of tiles that are common to every theme
       

        m_themes = new ColorTheme[1];

        //Theme 1
        ColorTheme theme1 = new ColorTheme();
        theme1.m_backgroundGradientTopColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 126, 121, 255));
        theme1.m_backgroundGradientBottomColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));

        theme1.m_startTileColors = new TileColors();
        theme1.m_startTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(21, 107, 18, 255));
        theme1.m_startTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(116, 240, 138, 255));
        theme1.m_startTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(90, 223, 85, 255));
        theme1.m_startTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));

        theme1.m_finishTileColors = new TileColors();
        theme1.m_finishTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(101, 33, 32, 255));
        theme1.m_finishTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(248, 95, 95, 255));
        theme1.m_finishTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 49, 49, 255));
        theme1.m_finishTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));

        theme1.m_selectedTileColors = new TileColors();
        theme1.m_selectedTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(69, 71, 43, 255));
        theme1.m_selectedTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(197, 195, 111, 255));
        theme1.m_selectedTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(227, 255, 98, 255));
        theme1.m_selectedTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));

        theme1.m_disabledTileColors = new TileColors();
        theme1.m_disabledTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(28, 28, 28, 255));
        theme1.m_disabledTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(75, 75, 75, 255));
        theme1.m_disabledTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(126, 126, 126, 255));
        theme1.m_disabledTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));

        theme1.m_defaultTileColors = new TileColors();
        theme1.m_defaultTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(78, 28, 50, 255));
        theme1.m_defaultTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(229, 150, 184, 255));
        theme1.m_defaultTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
        theme1.m_defaultTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(120, 38, 64, 255));
        theme1.m_floorSupportColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(77, 9, 30, 255));

        m_themes[0] = theme1;
        m_currentTheme = theme1;
    }
}

public struct TileColors
{
    public Color m_tileLeftFaceColor;
    public Color m_tileRightFaceColor;
    public Color m_tileTopFaceColor;
    public Color m_tileContourColor;
}

public class ColorTheme
{
    public Color m_backgroundGradientTopColor;
    public Color m_backgroundGradientBottomColor;
    public TileColors m_startTileColors; //colors used to render the start tile
    public TileColors m_finishTileColors; //colors used to render the finish tile
    public TileColors m_selectedTileColors; //colors used to render the tiles selected by the user inside level editor
    public TileColors m_disabledTileColors; //colors used to render the disabled tiles
    public TileColors m_defaultTileColors;
    public Color m_floorSupportColor;

    public TileColors GetTileColorsForTileState(Tile.State state)
    {
        if (state == Tile.State.NORMAL)
            return m_defaultTileColors;
        else if (state == Tile.State.DISABLED)
            return m_disabledTileColors;
        //else if (state == Tile.State.SELECTED)
        //    return m_selectedTileColors;
        else if (state == Tile.State.START)
            return m_startTileColors;
        else //FINISH
            return m_finishTileColors;
    }
}
