using UnityEngine;

public class Button2Depth : MonoBehaviour
{
    public Button1Depth m_button1;
    public int guiDepth = 1;

    public void OnGUI()
    {
        GUI.depth = 0;
        //GUI.Button(new Rect(50, 50, 100, 100), "GoBack");

        //GUI.depth = guiDepth;
        //if (GUI.Button(new Rect(50, 50, 100, 100), "GoBack"))
        //{
        //    guiDepth = 1;
        //    m_button1.guiDepth = 0;
        //}
        //guiDepth = 0;
        //m_button1.guiDepth = 1;
    }
}