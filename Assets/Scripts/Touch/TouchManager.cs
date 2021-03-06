﻿using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
    public const float MOVE_EPSILON = 0.01f;

    public bool m_touchDeactivated { get; set; }

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
    protected bool m_mouseButtonPressed;
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WINRT || UNITY_BLACKBERRY //touch devices
    protected int m_touchCount;   
#endif

    public Vector2 m_prevPointerLocation { get; set; }
    public Vector2 m_pointerDeltaLocation { get; set; }

    public enum PointerEventType
    {
        NONE = 0,
        POINTER_DOWN,
        POINTER_MOVE,
        POINTER_UP
    }

    public void Awake()
    {
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        m_mouseButtonPressed = false;
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WINRT || UNITY_BLACKBERRY //touch devices
         m_touchCount = 0;
       
#endif
    }

    /**
     * Update function to handle touches/clicks
     * **/
    void Update()
    {
        if (m_touchDeactivated)
            return;

        PointerEventType eventType = PointerEventType.NONE;
        Vector2 pointerLocation = Vector2.zero;
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            pointerLocation = Input.mousePosition;

            if (!m_mouseButtonPressed) //first press = OnPointerDown
            {
                eventType = PointerEventType.POINTER_DOWN;
                m_prevPointerLocation = pointerLocation;
                m_mouseButtonPressed = true;
            }
            else //other presses = OnPointerMove
            {
                m_pointerDeltaLocation = pointerLocation - m_prevPointerLocation;
                if (m_pointerDeltaLocation.sqrMagnitude >= MOVE_EPSILON)
                {
                    eventType = PointerEventType.POINTER_MOVE;
                    m_prevPointerLocation = pointerLocation;
                }
            }
        }
        else
        {
            if (m_mouseButtonPressed) //we switched from a press state to a release state
            {
                eventType = PointerEventType.POINTER_UP;
                m_mouseButtonPressed = false;
            }
        }
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WINRT || UNITY_BLACKBERRY //touch devices
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            pointerLocation = touch.position;

            if (m_touchCount == 0) //first press = OnPointerDown
            {
                eventType = PointerEventType.POINTER_DOWN;
                m_prevPointerLocation = pointerLocation;
            }
            else //other presses = OnPointerMove
            {
                m_pointerDeltaLocation = pointerLocation - m_prevPointerLocation;
                if (m_pointerDeltaLocation.sqrMagnitude >= MOVE_EPSILON)
                {
                    eventType = PointerEventType.POINTER_MOVE;
                    m_prevPointerLocation = pointerLocation;
                }
            }
        }
        else if (Input.touchCount == 0)
        {
            if (m_touchCount == 1) //we switched from a press state to a release state
            {
                eventType = PointerEventType.POINTER_UP;
            }
        }
        m_touchCount = Input.touchCount;        
#endif

        if (eventType != PointerEventType.NONE)
        {
            //If GUI does not process the current mouse position, pass the pointer event to the GameTouchHandler
            if (!GameController.GetInstance().GetGUIManager().EventProcessedByGUI(pointerLocation, eventType))
            {
                this.GetComponent<GameTouchHandler>().ProcessPointerEvent(pointerLocation, eventType);
            }
        }
    }
}
