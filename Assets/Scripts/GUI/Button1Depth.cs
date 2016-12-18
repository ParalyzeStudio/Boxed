using UnityEngine;

public class Button1Depth : MonoBehaviour
{
    public Button2Depth m_button2;
    public int guiDepth = 0;

    public void OnGUI()
    {
        GUI.depth = 1;
        //GUI.Button(new Rect(0, 0, 100, 100), "GoBack");

        //GUI.depth = guiDepth;
        //if (GUI.Button(new Rect(0, 0, 100, 100), "GoBack"))
        //{
        //    guiDepth = 1;
        //    m_button2.guiDepth = 0;
        //}
        //guiDepth = 1;
        //m_button2.guiDepth = 0;
    }
}