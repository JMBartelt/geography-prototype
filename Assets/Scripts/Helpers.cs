using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Helpers
{
    private static Camera _camera;
    public static Camera Camera
    {
        get{
            if(_camera == null)
            {
                _camera = Camera.main;

                if(_camera == null)
                {
                    Debug.LogError("No camera tagged as MainCamera found in scene, falling back to FindObjectOfType<Camera>()");
                    _camera = GameObject.FindObjectOfType<Camera>();
                }
            }
            return _camera;
        }
    }

    private static Canvas _canvas;
    public static Canvas Canvas
    {
        get{
            if(_canvas == null)
            {
                _canvas = GameObject.FindObjectOfType<Canvas>();
            }
            return _canvas;
        }
    }

    // is pointer over UI
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsOverUi()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }
}