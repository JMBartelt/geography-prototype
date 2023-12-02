using System.Collections;
using System.Collections.Generic;
using Microsoft.Maps.Unity;
using UnityEngine;

public class ZoomButtonController : MonoBehaviour
{
    [SerializeField] private MapRenderer _mapRenderer;
    [SerializeField] private float zoomSpeed = 0.01f;

    private bool _zoomingIn = false;
    private bool _zoomingOut = false;
    void Update()
    {
        if(_zoomingIn)
        {
            float zoomLevel = _mapRenderer.ZoomLevel + zoomSpeed;
            _mapRenderer.ZoomLevel = zoomLevel;
        }
        else if(_zoomingOut)
        {
            float zoomLevel = _mapRenderer.ZoomLevel - zoomSpeed;
            _mapRenderer.ZoomLevel = zoomLevel;
        }
    }

    public void ZoomIn()
    {
        _zoomingIn = true;
    }

    public void ZoomOut()
    {
        _zoomingOut = true;
    }

    public void StopZoom()
    {
        _zoomingIn = false;
        _zoomingOut = false;
    }
}
