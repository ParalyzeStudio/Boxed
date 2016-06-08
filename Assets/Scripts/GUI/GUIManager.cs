using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public Canvas m_canvas;
    public LevelEditor m_levelEditorPfb;

    public LevelEditor m_levelEditor { get; set; }   

    /**
    * Tells if one of the GUI elements currently displayed on the screen intercepts the pointer event at location 'pointerLocation'
    **/
    public bool ProcessPointerEvent(Vector3 pointerLocation)
    {
        //Test with level editor if available
        if (m_levelEditor != null)
        {
            RectTransform[] childTransforms = m_levelEditor.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i != childTransforms.Length; i++)
            {
                if (m_levelEditor.transform != childTransforms[i] && RectTransformUtility.RectangleContainsScreenPoint(childTransforms[i], pointerLocation))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /**
    * Render the level editor GUI element that contains all menus/windows to easily create a level
    **/
    public void DisplayLevelEditor()
    {
        //build the level editor object
        m_levelEditor = Instantiate(m_levelEditorPfb);
        m_levelEditor.name = "LevelEditor";
        m_levelEditor.transform.SetParent(m_canvas.transform, false);
        m_levelEditor.Init();
    }
}

