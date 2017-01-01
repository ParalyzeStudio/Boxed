using UnityEngine;
using UnityEngine.UI;

public class TutorialFrame : MonoBehaviour
{
    public Text m_title;
    public GameObject m_content;

    public void Render(Tutorial tutorial)
    {
        ////title
        //m_title.text = tutorial.m_title;

        ////content
        //for (int i = 0;i != tutorial.m_content.Length; i++)
        //{
        //    Tutorial.Content content = tutorial.m_content[i];

        //    if (content.m_type == Tutorial.Content.Type.TEXT)
        //    {
        //        Text textContent = (Text)Instantiate(m_textContentPfb);
        //        textContent.text = content.m_content;
        //    }
        //}
    }

    public void OnClickOk()
    {

    }
}