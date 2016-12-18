//using System.Collections.Generic;
//using UnityEngine;

//public class ColorThemes
//{
//    private ColorTheme[] m_themes;
//    public ColorTheme[] Themes
//    {
//        get
//        {
//            return m_themes;
//        }
//    }

//    public ColorTheme m_currentTheme;

//    public void Init()
//    {
//        int numChapters = LevelManager.NUM_CHAPTERS;

//        //define the colors of tiles that are common to every theme
//        m_themes = new ColorTheme[numChapters];

//        m_themes[0] = BuildGreenTheme();

//        //ColorTheme defaultTheme = BuildDefaultTheme();        
//        //m_themes[0] = defaultTheme;
//        //for (int i = 1; i != numChapters; i++)
//        //{
//        //    m_themes[i] = new ColorTheme(defaultTheme, 25 * i);
//        //}

//        //m_currentTheme = m_themes[0];
//    }

//    //private ColorTheme BuildDefaultTheme()
//    //{
//    //    //Theme 1
//    //    ColorTheme defaultTheme = new ColorTheme();
//    //    defaultTheme.m_backgroundGradientTopColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 126, 121, 255));
//    //    defaultTheme.m_backgroundGradientBottomColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
//    //    defaultTheme.m_highScoreColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(192, 2, 127, 255));

//    //    defaultTheme.m_startTileColors = new TileColors();
//    //    defaultTheme.m_startTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(21, 107, 18, 255));
//    //    defaultTheme.m_startTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(116, 240, 138, 255));
//    //    defaultTheme.m_startTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(90, 223, 85, 255));
//    //    defaultTheme.m_startTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));

//    //    defaultTheme.m_finishTileColors = new TileColors();
//    //    //defaultTheme.m_finishTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(101, 33, 32, 255));
//    //    //defaultTheme.m_finishTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(248, 95, 95, 255));
//    //    //defaultTheme.m_finishTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 49, 49, 255));
//    //    //defaultTheme.m_finishTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));

//    //    defaultTheme.m_finishTileColors.m_tileLeftFaceColor = Color.white;
//    //    defaultTheme.m_finishTileColors.m_tileRightFaceColor = Color.white;
//    //    defaultTheme.m_finishTileColors.m_tileTopFaceColor = Color.white;
//    //    defaultTheme.m_finishTileColors.m_tileContourColor = Color.white;


//    //    defaultTheme.m_selectedTileColors = new TileColors();
//    //    defaultTheme.m_selectedTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(69, 71, 43, 255));
//    //    defaultTheme.m_selectedTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(197, 195, 111, 255));
//    //    defaultTheme.m_selectedTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(227, 255, 98, 255));
//    //    defaultTheme.m_selectedTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));

//    //    defaultTheme.m_blockedTileColors = new TileColors();
//    //    defaultTheme.m_blockedTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(20, 20, 20, 255));
//    //    defaultTheme.m_blockedTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(120, 120, 120, 255));
//    //    defaultTheme.m_blockedTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));
//    //    defaultTheme.m_blockedTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(120, 120, 120, 255));

//    //    defaultTheme.m_disabledTileColors = new TileColors();
//    //    defaultTheme.m_disabledTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(28, 28, 28, 255));
//    //    defaultTheme.m_disabledTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(75, 75, 75, 255));
//    //    defaultTheme.m_disabledTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(126, 126, 126, 255));
//    //    defaultTheme.m_disabledTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(90, 90, 90, 255));

//    //    defaultTheme.m_defaultTileColors = new TileColors();
//    //    defaultTheme.m_defaultTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(78, 28, 50, 255));
//    //    defaultTheme.m_defaultTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(229, 150, 184, 255));
//    //    defaultTheme.m_defaultTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
//    //    defaultTheme.m_defaultTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(240, 128, 180, 255));
//    //    defaultTheme.m_floorSupportColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(77, 9, 30, 255));

//    //    defaultTheme.m_trapTileColors = defaultTheme.m_blockedTileColors;

//    //    defaultTheme.m_switchTileColors = new TileColors();
//    //    defaultTheme.m_switchTileColors.m_tileLeftFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(176, 181, 88, 255));
//    //    defaultTheme.m_switchTileColors.m_tileRightFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(248, 255, 130, 255));
//    //    defaultTheme.m_switchTileColors.m_tileTopFaceColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 254, 35, 255));
//    //    defaultTheme.m_switchTileColors.m_tileContourColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(187, 189, 152, 255));
//    //    defaultTheme.m_switchTileColors.Darken(0.5f);

//    //    defaultTheme.m_triggeredTileColors = defaultTheme.m_switchTileColors;
//    //    defaultTheme.m_triggeredTileColors.Lighten(0.5f);

//    //    defaultTheme.ToHSV();

//    //    return defaultTheme;
//    //}

//    private ColorTheme BuildDefaultTheme()
//    {
//        //Theme 1
//        ColorTheme defaultTheme = new ColorTheme();
//        defaultTheme.m_backgroundGradientTopColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 126, 121, 255));
//        defaultTheme.m_backgroundGradientBottomColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
//        defaultTheme.m_highScoreColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(192, 2, 127, 255));

//        //defaultTheme.m_startTileColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(90, 223, 85, 255));
//        //defaultTheme.m_finishTileColor = Color.white;
//        //defaultTheme.m_selectedTileColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(227, 255, 98, 255));
//        //defaultTheme.m_blockedTileColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 60, 60, 255));
//        //defaultTheme.m_disabledTileColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(126, 126, 126, 255));
//        //defaultTheme.m_defaultTileColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
//        defaultTheme.m_floorSupportColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(77, 9, 30, 255));
//        //defaultTheme.m_trapTileColor = defaultTheme.m_blockedTileColor;

//        //defaultTheme.m_switchTileColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 254, 35, 255));
//        //defaultTheme.m_switchTileColor = ColorUtils.DarkenColor(defaultTheme.m_switchTileColor, 0.5f);

//        //defaultTheme.m_triggeredTileColor = defaultTheme.m_switchTileColor;
//        //defaultTheme.m_triggeredTileColor = ColorUtils.LightenColor(defaultTheme.m_triggeredTileColor, 0.5f);

//        //defaultTheme.ToHSV();

//        return defaultTheme;
//    }

//    private ColorTheme BuildGreenTheme()
//    {
//        ColorTheme greenTheme = new ColorTheme();
//        greenTheme.m_backgroundGradientTopColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(241, 126, 121, 255));
//        greenTheme.m_backgroundGradientBottomColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(216, 92, 146, 255));
//        greenTheme.m_highScoreColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(192, 2, 127, 255));
//        greenTheme.m_floorSupportColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(77, 9, 30, 255));

//        return greenTheme;
//    }

//}

////public struct TileColors
////{
////    public Color m_tileLeftFaceColor;
////    public Color m_tileRightFaceColor;
////    public Color m_tileTopFaceColor;
////    public Color m_tileContourColor;

////    public HSVColor m_tileLeftFaceHSV;
////    public HSVColor m_tileRightFaceHSV;
////    public HSVColor m_tileTopFaceHSV;
////    public HSVColor m_tileContourHSV;

////    public void ToColor()
////    {
////        m_tileLeftFaceColor = m_tileLeftFaceHSV.ToRGBA(1);
////        m_tileRightFaceColor = m_tileRightFaceHSV.ToRGBA(1);
////        m_tileTopFaceColor = m_tileTopFaceHSV.ToRGBA(1);
////        m_tileContourColor = m_tileContourHSV.ToRGBA(1);
////    }

////    public void ToHSV()
////    {
////        m_tileLeftFaceHSV = new HSVColor(m_tileLeftFaceColor);
////        m_tileRightFaceHSV = new HSVColor(m_tileRightFaceColor);
////        m_tileTopFaceHSV = new HSVColor(m_tileTopFaceColor);
////        m_tileContourHSV = new HSVColor(m_tileContourColor);
////    }

////    public void TranslateHue(float deltaHue)
////    {
////        m_tileLeftFaceHSV.TranslateHue(deltaHue);
////        m_tileRightFaceHSV.TranslateHue(deltaHue);
////        m_tileTopFaceHSV.TranslateHue(deltaHue);
////        m_tileContourHSV.TranslateHue(deltaHue);
////    }

////    public void Darken(float t)
////    {
////        m_tileLeftFaceColor = ColorUtils.DarkenColor(m_tileLeftFaceColor, t);
////        m_tileRightFaceColor = ColorUtils.DarkenColor(m_tileRightFaceColor, t);
////        m_tileTopFaceColor = ColorUtils.DarkenColor(m_tileTopFaceColor, t);
////        m_tileContourColor = ColorUtils.DarkenColor(m_tileContourColor, t);

////        ToHSV();
////    }
    
////    public void Lighten(float t)
////    {
////        m_tileLeftFaceColor = ColorUtils.LightenColor(m_tileLeftFaceColor, t);
////        m_tileRightFaceColor = ColorUtils.LightenColor(m_tileRightFaceColor, t);
////        m_tileTopFaceColor = ColorUtils.LightenColor(m_tileTopFaceColor, t);
////        m_tileContourColor = ColorUtils.LightenColor(m_tileContourColor, t);

////        ToHSV();
////    }

////    public void Darken2(float t)
////    {
////        m_tileLeftFaceHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 0, t));
////        m_tileRightFaceHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 0, t));
////        m_tileTopFaceHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 0, t));
////        m_tileContourHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 0, t));

////        ToColor();
////    }

////    public void Lighten2(float t)
////    {
////        m_tileLeftFaceHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 1, t));
////        m_tileRightFaceHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 1, t));
////        m_tileTopFaceHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 1, t));
////        m_tileContourHSV.SetValue(Mathf.Lerp(m_tileLeftFaceHSV.GetValue(), 1, t));

////        ToColor();
////    }

////    public void Add(TileColors colors)
////    {
////        m_tileLeftFaceColor += colors.m_tileLeftFaceColor;
////        m_tileRightFaceColor += colors.m_tileRightFaceColor;
////        m_tileTopFaceColor += colors.m_tileTopFaceColor;
////        m_tileContourColor += colors.m_tileContourColor;
////    }

////    public void Fade(float opacity)
////    {
////        m_tileLeftFaceColor = new Color(m_tileLeftFaceColor.r, m_tileLeftFaceColor.g, m_tileLeftFaceColor.b, opacity);
////        m_tileRightFaceColor = new Color(m_tileRightFaceColor.r, m_tileRightFaceColor.g, m_tileRightFaceColor.b, opacity);
////        m_tileTopFaceColor = new Color(m_tileTopFaceColor.r, m_tileTopFaceColor.g, m_tileTopFaceColor.b, opacity);
////        m_tileContourColor = new Color(m_tileContourColor.r, m_tileContourColor.g, m_tileContourColor.b, opacity);
////    }

////    public void Substract(TileColors colors)
////    {
////        m_tileLeftFaceColor -= colors.m_tileLeftFaceColor;
////        m_tileRightFaceColor -= colors.m_tileRightFaceColor;
////        m_tileTopFaceColor -= colors.m_tileTopFaceColor;
////        m_tileContourColor -= colors.m_tileContourColor;
////    }

////    public void Multiply(float scalar)
////    {
////        m_tileLeftFaceColor *= scalar;
////        m_tileRightFaceColor *= scalar;
////        m_tileTopFaceColor *= scalar;
////        m_tileContourColor *= scalar;
////    }
////}

//public class ColorTheme
//{
//    public Color m_backgroundGradientTopColor;
//    public Color m_backgroundGradientBottomColor;
//    public Color m_floorSupportColor;
//    public Color m_highScoreColor;
//    //public Color m_startTileColor;
//    //public Color m_finishTileColor;
//    //public Color m_selectedTileColor; //color used to render the tiles selected by the user inside level editor
//    //public Color m_blockedTileColor;
//    //public Color m_disabledTileColor;
//    //public Color m_trapTileColor;
//    //public Color m_switchTileColor;
//    //public Color m_triggeredTileColor;
//    //public Color m_defaultTileColor;

//    //public HSVColor m_backgroundGradientTopHSV;
//    //public HSVColor m_backgroundGradientBottomHSV;
//    //public HSVColor m_floorSupportHSV;
//    //public HSVColor m_highScoreHSV;
//    //public HSVColor m_startTileHSV;
//    //public HSVColor m_finishTileHSV;
//    //public HSVColor m_selectedTileHSV; //color used to render the tiles selected by the user inside level editor
//    //public HSVColor m_blockedTileHSV;
//    //public HSVColor m_disabledTileHSV;
//    //public HSVColor m_trapTileHSV;
//    //public HSVColor m_switchTileHSV;
//    //public HSVColor m_triggeredTileHSV;
//    //public HSVColor m_defaultTileHSV;

//    public ColorTheme() { }

//    public ColorTheme(ColorTheme other)
//    {
//        m_backgroundGradientTopColor = other.m_backgroundGradientTopColor;
//        m_backgroundGradientBottomColor = other.m_backgroundGradientBottomColor;
//        m_floorSupportColor = other.m_floorSupportColor;
//        m_highScoreColor = other.m_highScoreColor;

//        //m_startTileColor = other.m_startTileColor;
//        //m_finishTileColor = other.m_finishTileColor;
//        //m_selectedTileColor = other.m_selectedTileColor;
//        //m_blockedTileColor = other.m_blockedTileColor;
//        //m_disabledTileColor = other.m_disabledTileColor;
//        //m_trapTileColor = other.m_trapTileColor;
//        //m_switchTileColor = other.m_switchTileColor;
//        //m_triggeredTileColor = other.m_triggeredTileColor;
//        //m_defaultTileColor = other.m_defaultTileColor;
        
//        //m_backgroundGradientTopHSV = other.m_backgroundGradientTopHSV;
//        //m_backgroundGradientBottomHSV = other.m_backgroundGradientBottomHSV;
//        //m_floorSupportHSV = other.m_floorSupportHSV;
//        //m_highScoreHSV = other.m_highScoreHSV;
//        //m_startTileHSV = other.m_startTileHSV;
//        //m_finishTileHSV = other.m_finishTileHSV;
//        //m_selectedTileHSV = other.m_selectedTileHSV;
//        //m_blockedTileHSV = other.m_blockedTileHSV;
//        //m_disabledTileHSV = other.m_disabledTileHSV;
//        //m_trapTileHSV = other.m_trapTileHSV;
//        //m_switchTileHSV = other.m_switchTileHSV;
//        //m_triggeredTileHSV = other.m_triggeredTileHSV;
//        //m_defaultTileHSV = other.m_defaultTileHSV;
//    }

//    //public ColorTheme(ColorTheme other, float deltaHue) : this(other)
//    //{
//    //    m_backgroundGradientTopHSV.TranslateHue(deltaHue);
//    //    m_backgroundGradientBottomHSV.TranslateHue(deltaHue);
//    //    m_floorSupportHSV.TranslateHue(deltaHue);
//    //    m_highScoreHSV.TranslateHue(deltaHue);

//    //    m_startTileHSV.TranslateHue(deltaHue);
//    //    m_finishTileHSV.TranslateHue(deltaHue);
//    //    m_selectedTileHSV.TranslateHue(deltaHue);
//    //    m_blockedTileHSV.TranslateHue(deltaHue);
//    //    m_disabledTileHSV.TranslateHue(deltaHue);
//    //    m_trapTileHSV.TranslateHue(deltaHue);
//    //    m_switchTileHSV.TranslateHue(deltaHue);
//    //    m_triggeredTileHSV.TranslateHue(deltaHue);
//    //    m_defaultTileHSV.TranslateHue(deltaHue);

//    //    ToColor();
//    //}
    
//    //public TileColors GetTileColorsForTileState(Tile.State state)
//    //{
//    //    if (state == Tile.State.NORMAL)
//    //        return m_defaultTileColors;
//    //    else if (state == Tile.State.DISABLED)
//    //        return m_disabledTileColors;
//    //    else if (state == Tile.State.BLOCKED)
//    //        return m_blockedTileColors;
//    //    //else if (state == Tile.State.SELECTED)
//    //    //    return m_selectedTileColors;
//    //    else if (state == Tile.State.START)
//    //        return m_startTileColors;
//    //    else if (state == Tile.State.TRAP)
//    //        return m_trapTileColors;
//    //    else if (state == Tile.State.SWITCH)
//    //        return m_switchTileColors;
//    //    else if (state == Tile.State.TRIGGERED_BY_SWITCH)
//    //        return m_defaultTileColors;
//    //    else //FINISH
//    //        return m_finishTileColors;
//    //}

//    public Color GetTileColorForTileState(Tile.State state)
//    {
//        return Color.white;
//        //if (state == Tile.State.NORMAL)
//        //    return m_defaultTileColor;
//        //else if (state == Tile.State.DISABLED)
//        //    return m_disabledTileColor;
//        //else if (state == Tile.State.BLOCKED)
//        //    return m_blockedTileColor;
//        ////else if (state == Tile.State.SELECTED)
//        ////    return m_selectedTileColors
//        //else if (state == Tile.State.START)
//        //    return m_startTileColor;
//        //else if (state == Tile.State.TRAP)
//        //    return m_trapTileColor;
//        //else if (state == Tile.State.SWITCH)
//        //    return m_switchTileColor;
//        //else if (state == Tile.State.TRIGGERED_BY_SWITCH)
//        //    return m_defaultTileColor;
//        //else //FINISH
//        //    return m_finishTileColor;
//    }

//    //public void ToHSV()
//    //{
//    //    m_backgroundGradientTopHSV = new HSVColor(m_backgroundGradientTopColor);
//    //    m_backgroundGradientBottomHSV = new HSVColor(m_backgroundGradientBottomColor);
//    //    m_floorSupportHSV = new HSVColor(m_floorSupportColor);
//    //    m_highScoreHSV = new HSVColor(m_highScoreColor);

//    //    m_startTileHSV = new HSVColor(m_startTileColor);
//    //    m_finishTileHSV = new HSVColor(m_finishTileColor);
//    //    m_selectedTileHSV = new HSVColor(m_selectedTileColor);
//    //    m_blockedTileHSV = new HSVColor(m_blockedTileColor);
//    //    m_disabledTileHSV = new HSVColor(m_disabledTileColor);
//    //    m_trapTileHSV = new HSVColor(m_trapTileColor);
//    //    m_switchTileHSV = new HSVColor(m_switchTileColor);
//    //    m_triggeredTileHSV = new HSVColor(m_triggeredTileColor);
//    //    m_defaultTileHSV = new HSVColor(m_defaultTileColor);
//    //}

//    //public void ToColor()
//    //{
//    //    m_backgroundGradientTopColor = m_backgroundGradientTopHSV.ToRGBA(1);
//    //    m_backgroundGradientBottomColor = m_backgroundGradientBottomHSV.ToRGBA(1);
//    //    m_floorSupportColor = m_floorSupportHSV.ToRGBA(1);
//    //    m_highScoreColor = m_highScoreHSV.ToRGBA(1);

//    //    m_startTileColor = m_startTileHSV.ToRGBA();
//    //    m_finishTileColor = m_finishTileHSV.ToRGBA();
//    //    m_selectedTileColor = m_selectedTileHSV.ToRGBA();
//    //    m_blockedTileColor = m_blockedTileHSV.ToRGBA();
//    //    m_disabledTileColor = m_disabledTileHSV.ToRGBA();
//    //    m_trapTileColor = m_trapTileHSV.ToRGBA();
//    //    m_switchTileColor = m_switchTileHSV.ToRGBA();
//    //    m_triggeredTileColor = m_triggeredTileHSV.ToRGBA();
//    //    m_defaultTileColor = m_defaultTileHSV.ToRGBA();
//    //}
//}

