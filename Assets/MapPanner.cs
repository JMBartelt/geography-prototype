using System.Collections;
using System.Collections.Generic;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using UnityEngine;

public class MapPanner : MonoBehaviour
{
    [SerializeField] private MapRenderer _mapRenderer;
    [SerializeField] private float panSpeed = 0.001f;
    private Vector2 _panDirection = Vector2.zero;

    // use a curve instead for zoomMult
    [SerializeField] private AnimationCurve zoomMultCurve;

    public void SetPan(Vector2 direction)
    {
        _panDirection = direction;
    }

    public void StopPan()
    {
        _panDirection = Vector2.zero;
    }
    void Update()
    {
        if(_panDirection != Vector2.zero) Pan(_panDirection);
    }

    public void Pan(Vector2 direction)
    {
        float zoomMin = _mapRenderer.MinimumZoomLevel;
        float zoomMax = _mapRenderer.MaximumZoomLevel;
        float zoomMult = panSpeed*zoomMultCurve.Evaluate(_mapRenderer.ZoomLevel);
        Debug.Log("zoomMult = "+zoomMult+" because zoomLevel = "+_mapRenderer.ZoomLevel+" and zoomMultCurve.Evaluate(zoomLevel) = "+zoomMultCurve.Evaluate(_mapRenderer.ZoomLevel));
        LatLon newCenter = new LatLon(_mapRenderer.Center.LatitudeInDegrees + direction.y*zoomMult, _mapRenderer.Center.LongitudeInDegrees + direction.x*zoomMult);
        _mapRenderer.Center = newCenter;
    }


}
