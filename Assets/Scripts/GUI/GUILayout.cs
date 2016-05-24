using UnityEngine;
using UnityEngine.UI;

public class GUILayout : MonoBehaviour
{
    public GameObject m_canvas; //Unique instance of our canvas in the scene hierarchy

    public enum GUIButtonID
    {
        ID_LEVEL_EDITOR_SAVE_LEVEL = 1,
        ID_LEVEL_EDITOR_START_EDITING
    }

    public GameObject CreateGUIButton(GUIButtonID id, Vector2 pivot, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject guiBtnObject = (GameObject)Instantiate(GetPrefabForID(id));
        guiBtnObject.transform.SetParent(m_canvas.transform, false);

        RectTransform buttonTf = guiBtnObject.GetComponent<RectTransform>();

        //set anchor bot left hand corner
        buttonTf.anchorMin = new Vector2(0, 0);
        buttonTf.anchorMax = new Vector2(0, 0);

        //set pivot
        buttonTf.pivot = pivot;

        //set position
        buttonTf.position = new Vector3(position.x, position.y, 0);

        //add a listener for the onclick method
        Button button = guiBtnObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        return guiBtnObject;
    }

    public virtual GameObject GetPrefabForID(GUIButtonID id)
    {
        return null;
    }
}
