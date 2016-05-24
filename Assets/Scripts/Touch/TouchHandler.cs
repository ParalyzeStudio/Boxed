using UnityEngine;
using System.Collections;


/**
 * Base Class that handles touches or clicks on the scene
 * **/
public class TouchHandler : MonoBehaviour
{
    public const float ON_CLICK_MAX_DISPLACEMENT = 32.0f;
    
    protected bool m_selected;
    protected Vector2 m_pointerDownPointerLocation;

    
    private TouchManager m_touchManager;

    public virtual void Awake()
    {
        m_touchManager = null;
        m_selected = false;
    }

    public virtual void Start()
    {

    }

    /**
     * Process the pointer event and dispatch it to the relevant callback
     * **/
    public virtual bool ProcessPointerEvent(Vector2 pointerLocation, TouchManager.PointerEventType eventType)
    {
        if (eventType == TouchManager.PointerEventType.POINTER_DOWN)
        {
            if (IsPointerLocationContainedInObject(pointerLocation))
            {
                OnPointerDown(pointerLocation);
                return true;
            }
        }
        else if (eventType == TouchManager.PointerEventType.POINTER_MOVE)
        {
            return OnPointerMove(pointerLocation, GetTouchManager().m_pointerDeltaLocation);
        }
        else if (eventType == TouchManager.PointerEventType.POINTER_UP)
        {
            bool bSelected = m_selected;
            OnPointerUp();
            return bSelected;
        }

        return false;
    }

    /**
     * Virtual method that checks if a point is inside the touch area of the object
     * **/
    protected virtual bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return false;
    }

    /**
     * Player touched this object
     * **/
    protected virtual void OnPointerDown(Vector2 pointerLocation)
    {
        m_selected = true;
        m_pointerDownPointerLocation = pointerLocation;
    }

    /**
     * Player moved the pointer with object selected
     * **/
    protected virtual bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!m_selected)
            return false;

        return true;
    }

    /**
     * Player released the pointer
     * **/
    protected virtual void OnPointerUp()
    {
        Vector2 prevPointerLocation = GetTouchManager().m_prevPointerLocation;
        float pointerDisplacementSqrLength = (prevPointerLocation - m_pointerDownPointerLocation).sqrMagnitude;

        if (m_selected && IsPointerLocationContainedInObject(prevPointerLocation) && pointerDisplacementSqrLength <= ON_CLICK_MAX_DISPLACEMENT)
            OnClick(prevPointerLocation);

        m_selected = false;
    }

    /**
     * Player clicked on this object
     * **/
    protected virtual void OnClick(Vector2 clickLocation)
    {
        
    }

    /**
     * Getters for global instances
     * **/
    public TouchManager GetTouchManager()
    {
        if (m_touchManager == null)
            m_touchManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TouchManager>();

        return m_touchManager;
    }
}
