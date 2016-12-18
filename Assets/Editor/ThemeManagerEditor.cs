using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ThemeManager))]
public class ThemeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("SAVE THEME"))
        //{
        //    ThemeManager themeManager = (ThemeManager)target;
        //    themeManager.OnClickSaveTheme();
        //}

    //ThemeManager themeManager = (ThemeManager)target;
    //ThemeManager.Theme theme = themeManager.m_selectedTheme;
    //EditorGUILayout.IntField()
    //theme.m_backgroundGradientTopColor = EditorGUILayout.ColorField("BackgroundTopColor", theme.m_backgroundGradientTopColor);
    //theme.m_backgroundGradientBottomColor = EditorGUILayout.ColorField("BackgroundBottomColor", theme.m_backgroundGradientBottomColor);
    //theme.m_floorSupportColor = EditorGUILayout.ColorField("FloorSupportColor", theme.m_floorSupportColor);
    //theme.m_highScoreColor = EditorGUILayout.ColorField("FloorSupportColor", theme.m_highScoreColor);
    //theme.m_defaultTileMaterial = EditorGUILayout.ObjectField("DefaultTileMaterial", )

    //////materials for every type of tile
    ////public Material m_defaultTileMaterial;
    ////public Material m_switchTileMaterial;
    ////public Material m_triggeredTileMaterial;
    ////public Material m_iceTileMaterial;
}
}
