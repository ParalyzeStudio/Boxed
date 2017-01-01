using UnityEngine;

public class Tutorial
{
    public class Content
    {
        public enum Type
        {
            TEXT,
            IMAGE
        }

        public Type m_type;
        public string m_content;

        public Content(Type type, string content)
        {
            m_type = type;
            m_content = content;
        }
    }

    public int m_number;
    public string m_title;
    public Content[] m_content;

    public Tutorial(int number, string title)
    {
        m_number = number;
        m_title = title;
    }

    public Tutorial(int number, string title, Content[] content) : this(number, title)
    {
        m_content = content;
    }
}
