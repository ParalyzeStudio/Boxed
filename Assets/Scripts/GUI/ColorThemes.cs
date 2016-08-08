using System.Collections.Generic;
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
        m_themes = new ColorTheme[2];

        ColorTheme defaultTheme = BuildDefaultTheme();

        m_themes[0] = defaultTheme;
        m_themes[1] = new ColorTheme(defaultTheme, 25);

        m_currentTheme = m_themes[0];
    }

    private ColorTheme BuildDefaultTheme()
    {
        //Theme 1
        ColorTheme defaultTheme = new ColorTheme();
        defaultTheme.m_backgroundGradientTopColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 126, 121, 255));
        defaultTheme.m_backgroundGradientBottomColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));

        defaultTheme.m_startTileColors = new TileColors();
        defaultTheme.m_startTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(21, 107, 18, 255));
        defaultTheme.m_startTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(116, 240, 138, 255));
        defaultTheme.m_startTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(90, 223, 85, 255));
        defaultTheme.m_startTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));

        defaultTheme.m_finishTileColors = new TileColors();
        defaultTheme.m_finishTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(101, 33, 32, 255));
        defaultTheme.m_finishTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(248, 95, 95, 255));
        defaultTheme.m_finishTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 49, 49, 255));
        defaultTheme.m_finishTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));

        defaultTheme.m_selectedTileColors = new TileColors();
        defaultTheme.m_selectedTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(69, 71, 43, 255));
        defaultTheme.m_selectedTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(197, 195, 111, 255));
        defaultTheme.m_selectedTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(227, 255, 98, 255));
        defaultTheme.m_selectedTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));

        defaultTheme.m_blockedTileColors = new TileColors();
        defaultTheme.m_blockedTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(20, 20, 20, 255));
        defaultTheme.m_blockedTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(120, 120, 120, 255));
        defaultTheme.m_blockedTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));
        defaultTheme.m_blockedTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(120, 120, 120, 255));

        defaultTheme.m_disabledTileColors = new TileColors();
        defaultTheme.m_disabledTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(28, 28, 28, 255));
        defaultTheme.m_disabledTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(75, 75, 75, 255));
        defaultTheme.m_disabledTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(126, 126, 126, 255));
        defaultTheme.m_disabledTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(90, 90, 90, 255));

        defaultTheme.m_defaultTileColors = new TileColors();
        defaultTheme.m_defaultTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(78, 28, 50, 255));
        defaultTheme.m_defaultTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(229, 150, 184, 255));
        defaultTheme.m_defaultTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
        defaultTheme.m_defaultTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));
        defaultTheme.m_floorSupportColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(77, 9, 30, 255));

        defaultTheme.m_trapTileColors = defaultTheme.m_blockedTileColors;

        defaultTheme.ToHSV();

        return defaultTheme;
    }
}

public struct TileColors
{
    public Color m_tileLeftFaceColor;
    public Color m_tileRightFaceColor;
    public Color m_tileTopFaceColor;
    public Color m_tileContourColor;

    public HSVColor m_tileLeftFaceHSV;
    public HSVColor m_tileRightFaceHSV;
    public HSVColor m_tileTopFaceHSV;
    public HSVColor m_tileContourHSV;

    public void ToColor()
    {
        m_tileLeftFaceColor = m_tileLeftFaceHSV.ToRGBA(1);
        m_tileRightFaceColor = m_tileRightFaceHSV.ToRGBA(1);
        m_tileTopFaceColor = m_tileTopFaceHSV.ToRGBA(1);
        m_tileContourColor = m_tileContourHSV.ToRGBA(1);
    }

    public void ToHSV()
    {
        m_tileLeftFaceHSV = new HSVColor(m_tileLeftFaceColor);
        m_tileRightFaceHSV = new HSVColor(m_tileRightFaceColor);
        m_tileTopFaceHSV = new HSVColor(m_tileTopFaceColor);
        m_tileContourHSV = new HSVColor(m_tileContourColor);
    }

    public void TranslateHue(float deltaHue)
    {
        m_tileLeftFaceHSV.TranslateHue(deltaHue);
        m_tileRightFaceHSV.TranslateHue(deltaHue);
        m_tileTopFaceHSV.TranslateHue(deltaHue);
        m_tileContourHSV.TranslateHue(deltaHue);
    }
}

public class ColorTheme
{
    public Color m_backgroundGradientTopColor;
    public Color m_backgroundGradientBottomColor;
    public Color m_floorSupportColor;
    public TileColors m_startTileColors; //colors used to render the start tile
    public TileColors m_finishTileColors; //colors used to render the finish tile
    public TileColors m_selectedTileColors; //colors used to render the tiles selected by the user inside level editor
    public TileColors m_blockedTileColors; //colors used to render the tiles selected by the user inside level editor
    public TileColors m_disabledTileColors; //colors used to render the disabled tiles
    public TileColors m_trapTileColors;
    public TileColors m_defaultTileColors;

    public HSVColor m_backgroundGradientTopHSV;
    public HSVColor m_backgroundGradientBottomHSV;
    public HSVColor m_floorSupportHSV;

    public ColorTheme() { }

    public ColorTheme(ColorTheme other)
    {
        m_backgroundGradientTopColor = other.m_backgroundGradientTopColor;
        m_backgroundGradientBottomColor = other.m_backgroundGradientBottomColor;
        m_floorSupportColor = other.m_floorSupportColor;
        m_startTileColors = other.m_startTileColors;
        m_finishTileColors = other.m_finishTileColors;
        m_selectedTileColors = other.m_selectedTileColors;
        m_blockedTileColors = other.m_blockedTileColors;
        m_disabledTileColors = other.m_disabledTileColors;
        m_trapTileColors = other.m_trapTileColors;
        m_defaultTileColors = other.m_defaultTileColors;

        m_backgroundGradientTopHSV = other.m_backgroundGradientTopHSV;
        m_backgroundGradientBottomHSV = other.m_backgroundGradientBottomHSV;
        m_floorSupportHSV = other.m_floorSupportHSV;
    }

    public ColorTheme(ColorTheme other, float deltaHue) : this(other)
    {
        m_backgroundGradientTopHSV.TranslateHue(deltaHue);
        m_backgroundGradientBottomHSV.TranslateHue(deltaHue);
        m_floorSupportHSV.TranslateHue(deltaHue);

        m_startTileColors.TranslateHue(deltaHue);
        m_finishTileColors.TranslateHue(deltaHue);
        m_selectedTileColors.TranslateHue(deltaHue);
        m_blockedTileColors.TranslateHue(deltaHue);
        m_disabledTileColors.TranslateHue(deltaHue);
        m_trapTileColors.TranslateHue(deltaHue);
        m_defaultTileColors.TranslateHue(deltaHue);

        ToColor();
    }

    //public ColorTheme(Color colorReferent, Vector3[] hsvTranslations)
    //{
    //    Vector3 hsvReferent = ColorUtils.GetHSVFromRGBAColor(colorReferent);
    //    m_backgroundGradientTopHSV = hsvReferent;

    //    m_backgroundGradientBottomHSV = hsvReferent + hsvTranslations[0];
    //    m_floorSupportHSV = hsvReferent + hsvTranslations[1];

    //    List<Vector3> translations = new List<Vector3>(hsvTranslations);        
    //    Vector3[] startTileHSVTranslations = translations.GetRange(2, 4).ToArray();
    //    m_startTileColors.SetHSVs(hsvReferent, startTileHSVTranslations);

    //    Vector3[] finishTileHSVTranslations = translations.GetRange(6, 4).ToArray();
    //    m_finishTileColors.SetHSVs(hsvReferent, finishTileHSVTranslations);

    //    Vector3[] selectedTileHSVTranslations = translations.GetRange(10, 4).ToArray();
    //    m_selectedTileColors.SetHSVs(hsvReferent, selectedTileHSVTranslations);

    //    Vector3[] blockedTileHSVTranslations = translations.GetRange(14, 4).ToArray();
    //    m_blockedTileColors.SetHSVs(hsvReferent, blockedTileHSVTranslations);

    //    Vector3[] disabledTileHSVTranslations = translations.GetRange(18, 4).ToArray();
    //    m_disabledTileColors.SetHSVs(hsvReferent, disabledTileHSVTranslations);

    //    Vector3[] trapTileHSVTranslations = translations.GetRange(22, 4).ToArray();
    //    m_trapTileColors.SetHSVs(hsvReferent, trapTileHSVTranslations);

    //    Vector3[] defaultTileHSVTranslations = translations.GetRange(26, 4).ToArray();
    //    m_defaultTileColors.SetHSVs(hsvReferent, defaultTileHSVTranslations);

    //    ToColor();
    //}

    public TileColors GetTileColorsForTileState(Tile.State state)
    {
        if (state == Tile.State.NORMAL)
            return m_defaultTileColors;
        else if (state == Tile.State.DISABLED)
            return m_disabledTileColors;
        else if (state == Tile.State.BLOCKED)
            return m_blockedTileColors;
        //else if (state == Tile.State.SELECTED)
        //    return m_selectedTileColors;
        else if (state == Tile.State.START)
            return m_startTileColors;
        else if (state == Tile.State.TRAP)
            return m_trapTileColors;
        else //FINISH
            return m_finishTileColors;
    }

    public void ToHSV()
    {
        m_backgroundGradientTopHSV = new HSVColor(m_backgroundGradientTopColor);
        m_backgroundGradientBottomHSV = new HSVColor(m_backgroundGradientBottomColor);
        m_floorSupportHSV = new HSVColor(m_floorSupportColor);
        m_startTileColors.ToHSV();
        m_finishTileColors.ToHSV();
        m_selectedTileColors.ToHSV();
        m_blockedTileColors.ToHSV();
        m_disabledTileColors.ToHSV();
        m_trapTileColors.ToHSV();
        m_defaultTileColors.ToHSV();
    }

    public void ToColor()
    {
        m_backgroundGradientTopColor = m_backgroundGradientTopHSV.ToRGBA(1);
        m_backgroundGradientBottomColor = m_backgroundGradientBottomHSV.ToRGBA(1);
        m_floorSupportColor = m_floorSupportHSV.ToRGBA(1);

        m_startTileColors.ToColor();
        m_finishTileColors.ToColor();
        m_selectedTileColors.ToColor();
        m_blockedTileColors.ToColor();
        m_disabledTileColors.ToColor();
        m_trapTileColors.ToColor();
        m_defaultTileColors.ToColor();
    }
}

